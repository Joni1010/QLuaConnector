using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketObject;
using System.Threading;
using TradingLib;

namespace AppVEConector
{
	public partial class Form_GraphicDepth :Form
	{
		public Form_GraphicDepth(Connector.QuikConnector trader, TElement trElement, object parent)
		{
			InitializeComponent();

			this.Parent = (MainForm)parent;
			if (this.Parent.IsNull()) this.Close();

			this.Trader = trader;
			if (this.Trader.IsNull()) this.Close();

			this.TrElement = trElement;
			if (this.TrElement.IsNull()) this.Close();

			//Инициализация графика
			this.GraphicStock = new NSGraphic.Graphic(TrElement.Security.Params.MinPriceStep);
			//Загружаем список пользовательских инструментов
			this.LoadListTradeSec();
		}

		private void Form_GraphicDepth_Load(object sender, EventArgs e)
		{
			numericUpDownPrice.InitWheelDecimal();
			numericUpDownStopPrice.InitWheelDecimal();
			numericUpDownVolume.InitWheelDecimal();

			//Получаем коды клиента
			comboBoxCodeClient.Items.Insert(0, "");
			comboBoxCodeClient.Items.AddRange(this.Trader.Objects.Clients.ToArray());

			this.InitReset();

			//ЗАпуск таймера обновляющий стакана
			this.InitUpdater();


			comboBoxTimeFrame.SelectedItem = comboBoxTimeFrame.Items[0];

			//При наведении скрывать сообщение
			textBoxMessage.MouseMove += (s, ev) =>
			{
				if (!((TextBox)s).Parent.Empty())
					((TextBox)s).Parent.Visible = false;
			};

			GraphicStock.PanelCandels.OnMoveVerticalCandle += (candle) =>
			{
				labelInfoGraphic.Text =
					candle.Candle.Time.ToString() + " ; " +
					"High " + candle.Candle.High + " ; " +
					"Low " + candle.Candle.Low + " ; " +
					"Open " + candle.Candle.Open + " ; " +
					"Close " + candle.Candle.Close + " ; " +
					"Vol " + candle.Candle.Volume
					;
			};
		}

		private void dataGridViewDepth_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView dataGrid = (DataGridView)sender;
		}

		/// <summary> Кликеры по таблице стакана </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridViewDepth_Click(object sender, EventArgs e)
		{
			if (this.TrElement.Security.IsNull()) return;
			var MouseEvent = (MouseEventArgs)e;
			var dataGrid = (DataGridView)sender;
			dataGrid.ClearSelection();
			var hti = dataGrid.HitTest(MouseEvent.X, MouseEvent.Y);
			int indexRow = hti.RowIndex;
			int indexCol = hti.ColumnIndex;
			if (indexRow < 0 || indexCol < 0) return;

			DataGridViewCell cell = dataGrid.Rows[indexRow].Cells[indexCol];
			if (MouseEvent.Button == MouseButtons.Left)
			{
				if (cell.Tag != null)
				{
					int Volume = Convert.ToInt32(numericUpDownVolume.Value);
					if (Volume == 0)
					{
						UpdateTransReply("Не указан объем!");
					}
					if (cell.Tag.GetType().ToString().Contains("StructClickDepth"))
					{
						var data = (StructClickDepth)cell.Tag;

						OrderDirection? direction = null;
						if (data.Flag == "buy")
						{
							direction = OrderDirection.Buy;
						}
						if (data.Flag == "sell")
						{
							direction = OrderDirection.Sell;
						}
						var codeClient = comboBoxCodeClient.SelectedText;
						Common.Ext.NewThread(() =>
						{
							Trader.CreateOrder(new Order()
							{
								Sec = this.TrElement.Security,
								Direction = direction,
								Price = data.Price,
								Volume = Volume
							}, codeClient);
						});
					}
				}
			}
			if (MouseEvent.Button == MouseButtons.Right)
			{
				if (cell.Tag != null)
				{
					if (cell.Tag.GetType().ToString().Contains("StructClickDepth"))
					{
						var data = (StructClickDepth)cell.Tag;
						if (data.Flag == "buy" || data.Flag == "sell")
						{
							Common.Ext.NewThread(() =>
							{
								var ords = Trader.Objects.Orders.Where(o => o.Sec == this.TrElement.Security && o.Price == data.Price && o.Status == OrderStatus.ACTIVE);
								if (ords.NotIsNull())
								{
									foreach (var ord in ords)
										Trader.CancelOrder(ord.Sec, ord.OrderNumber);
								}
							});
						}
					}
				}
			}
		}

		private void Form_GraphicDepth_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.Trader.UnregisterDepth(this.TrElement.Security);
			this.isClose = true;
		}

		private void buttonCancelAll_Click(object sender, EventArgs e)
		{
			if (this.TrElement.Security.NotIsNull())
			{
				Trader.CancelAllOrder(this.TrElement.Security);
			}
		}

		private void buttonExitPos_Click(object sender, EventArgs e)
		{
			if (this.TrElement.Security.NotIsNull())
			{
				var pos = Trader.Objects.Positions.FirstOrDefault(p => p.Sec == this.TrElement.Security);
				if (pos != null)
				{
					if (pos.Data.CurrentNet != 0)
					{
						var codeClient = comboBoxCodeClient.SelectedText;
						Common.Ext.NewThread(() =>
						{
							OrderDirection? direction = pos.Data.CurrentNet > 0 ? OrderDirection.Sell : OrderDirection.Buy;
							decimal Price = 0;

							decimal LastPrice = this.TrElement.Security.Params.LastPrice == 0 ? this.TrElement.Security.LastTrade.Price : this.TrElement.Security.Params.LastPrice;

							if (direction == OrderDirection.Sell) Price = LastPrice - this.TrElement.Security.Params.MinPriceStep * 5;
							else Price = LastPrice + this.TrElement.Security.Params.MinPriceStep * 5;

							int Volume = pos.Data.CurrentNet < 0 ? pos.Data.CurrentNet * -1 : pos.Data.CurrentNet;

							Trader.CreateOrder(new Order()
							{
								Sec = this.TrElement.Security,
								Direction = direction,
								Price = Price,
								Volume = Volume
							}, codeClient);
						});
					}
					else
					{
						UpdateTransReply("По данному инструменту нет позиций.");
					}
				}
			}
		}

		private void labelLastPrice_Click(object sender, EventArgs e)
		{
			MouseEventArgs ev = (MouseEventArgs)e;
			if (ev.Button == MouseButtons.Left)
			{
				numericUpDownPrice.Value = Convert.ToDecimal(labelLastPrice.Text);
			}
			if (ev.Button == MouseButtons.Right)
			{
				numericUpDownStopPrice.Value = Convert.ToDecimal(labelLastPrice.Text);
			}
		}

		private void buttonBuy_Click(object sender, EventArgs e)
		{
			if (this.TrElement.Security != null)
			{
				if (numericUpDownVolume.Value > 0)
				{
					Trader.CreateOrder(new Order()
					{
						Sec = this.TrElement.Security,
						Direction = OrderDirection.Buy,
						Price = numericUpDownPrice.Value,
						Volume = Convert.ToInt32(numericUpDownVolume.Value),
					}, comboBoxCodeClient.SelectedText);
				}
				else
				{
					UpdateTransReply("Объем не может быть 0 или отрицательным значением.");
				}
			}
		}

		private void buttonSell_Click(object sender, EventArgs e)
		{
			if (this.TrElement.Security != null)
			{
				if (numericUpDownVolume.Value > 0)
				{
					Trader.CreateOrder(new Order()
					{
						Sec = this.TrElement.Security,
						Direction = OrderDirection.Sell,
						Price = numericUpDownPrice.Value,
						Volume = Convert.ToInt32(numericUpDownVolume.Value),
					}, comboBoxCodeClient.SelectedText);
				}
				else
				{
					UpdateTransReply("Объем не может быть 0 или отрицательным значением.");
				}
			}
		}

		private void Form_GraphicDepth_FormClosing(object sender, FormClosingEventArgs e)
		{
			var result = MessageBox.Show(this, "Закрыть окно " + this.TrElement.Security.Code + "?", "Закрыть окно " + this.TrElement.Security.Code + "?",
				MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

			if (result == DialogResult.OK) e.Cancel = false;
			else e.Cancel = true;
		}

		private void checkBoxLockDepth_CheckedChanged(object sender, EventArgs e)
		{
			var check = (CheckBox)sender;
			if (check.Checked)
			{
				if (!this.TrElement.Empty())
					this.Trader.RegisterDepth(this.TrElement.Security);
				dataGridViewDepth.Enabled = false;
				panelLock.Visible = true;
			}
			else
			{
				dataGridViewDepth.Enabled = true;
				panelLock.Visible = false;
			}
		}

		private void buttonCancelStopOrders_Click(object sender, EventArgs e)
		{
			if (this.TrElement.Security != null)
			{
				Trader.CancelAllStopOrder(this.TrElement.Security);
			}
		}

		int WidthPanelDepth = -1;
		private void checkBoxShowHideDepth_CheckedChanged(object sender, EventArgs e)
		{
			if (WidthPanelDepth < 0) WidthPanelDepth = panelRight.Width;
			if (checkBoxShowHideDepth.Checked) panelRight.Width = 0;
			else panelRight.Width = WidthPanelDepth;
		}

		private void numericUpDownPrice_ValueChanged(object sender, EventArgs e)
		{

		}

		private void labelOrdersSell_Click(object sender, EventArgs e)
		{
			try
			{
				var MouseEvent = (MouseEventArgs)e;
				if (MouseEvent.Button == MouseButtons.Right)
				{
					var listOrders = Trader.Objects.Orders.Where(o => o.Status == OrderStatus.ACTIVE && o.Direction == OrderDirection.Sell);
					foreach (var ord in listOrders)
					{
						Trader.CancelOrder(this.TrElement.Security, ord.OrderNumber);
					}
				}
			}
			catch (Exception ee)
			{
				MessageBox.Show(ee.ToString());
			}
		}

		private void labelOrdersBuy_Click(object sender, EventArgs e)
		{
			try
			{
				var MouseEvent = (MouseEventArgs)e;
				if (MouseEvent.Button == MouseButtons.Right)
				{
					var listOrders = Trader.Objects.Orders.Where(o => o.Status == OrderStatus.ACTIVE && o.Direction == OrderDirection.Buy);
					foreach (var ord in listOrders)
					{
						Trader.CancelOrder(this.TrElement.Security, ord.OrderNumber);
					}
				}
			}
			catch (Exception ee)
			{
				MessageBox.Show(ee.ToString());
			}
		}

		private void buttonCreateStopOrder_Click(object sender, EventArgs e)
		{
			try
			{
				decimal Price = numericUpDownStopPrice.Value;
				if (this.Position.Data.CurrentNet == 0)
				{
					UpdateTransReply("Позиций не открыто!");
					return;
				}
				if (this.Position.Data.CurrentNet > 0 && Price > 0)
				{
					var stopOrder = new StopOrder()
					{
						Direction = OrderDirection.Sell,
						Sec = this.TrElement.Security,
						Price = Price - this.TrElement.Security.Params.MinPriceStep * 20,
						Volume = this.Position.Data.CurrentNet,
						ConditionPrice = Price,
						DateExpiry = dateTimePickerStopOrder.Value
					};
					if (Price > this.TrElement.Security.LastTrade.Price)
					{
						UpdateTransReply("Не корректная цена стоп заявки! Необходимо указать цену ниже текущей.");
					}
					else
					{
						this.Trader.CancelAllStopOrder(this.TrElement.Security);
						this.Trader.CreateStopOrder(stopOrder, StopOrderType.StopLimit);
						Common.Ext.NewThread(() =>
						{
							Thread.Sleep(300);
							this.UpdateDepth();
						});
					}
				}
				if (this.Position.Data.CurrentNet < 0 && Price > 0)
				{
					var stopOrder = new StopOrder()
					{
						Direction = OrderDirection.Buy,
						Sec = this.TrElement.Security,
						Price = Price + this.TrElement.Security.Params.MinPriceStep * 20,
						Volume = this.Position.Data.CurrentNet * -1,
						ConditionPrice = Price,
						DateExpiry = dateTimePickerStopOrder.Value
					};
					if (Price < this.TrElement.Security.LastTrade.Price)
					{
						UpdateTransReply("Не корректная цена стоп заявки! Необходимо указать цену выше текущей.");
					}
					else
					{
						this.Trader.CancelAllStopOrder(this.TrElement.Security);
						this.Trader.CreateStopOrder(stopOrder, StopOrderType.StopLimit);
					}
				}
			}
			catch (Exception ee)
			{
				MessageBox.Show(ee.ToString());
			}
		}

		private void numericUpDownFilterHorVol_ValueChanged(object sender, EventArgs e)
		{
			//this.UpdateHorVol();
		}


		/// <summary> Обработчик нажатия на сообщение </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBoxMessage_Click(object sender, EventArgs e)
		{
			var MouseEvent = (MouseEventArgs)e;
			if (MouseEvent.Button == MouseButtons.Right)
			{
				panelMessage.Visible = false;
			}
		}

		private void pictureBoxGraphic_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				var g = e.Graphics;
				GraphicStock.Paint(g, pictureBoxGraphic.ClientRectangle);
			}
			catch (Exception ee)
			{
				MessageBox.Show(ee.ToString());
			}
		}



		/// <summary>
		/// Получает индекс первой свечи с учетом скрола.
		/// </summary>
		/// <param name="timeFrame">Тайм-фрейм</param>
		/// <returns></returns>
		private int GetIndexFirstCandle(CandleLib.CandleCollection timeFrame)
		{
			hScrollGraphic.GuiAsync(() =>
			{
				hScrollGraphic.Minimum = 0;
				if (hScrollGraphic.Maximum == hScrollGraphic.Value)
				{
					hScrollGraphic.Maximum = timeFrame.Count - this.CountCandleInGraphic;
					hScrollGraphic.Value = hScrollGraphic.Maximum;
				}
				else
					hScrollGraphic.Maximum = timeFrame.Count - this.CountCandleInGraphic;
			});
			int index = hScrollGraphic.Maximum - hScrollGraphic.Value;
			return index;
		}


		/// <summary> Расчет горизонтальных объемов </summary>
		private void GetHorVol()
		{
			var timeFrame = this.TrElement.CollectionTimeFrames.FirstOrDefault(tf => tf.TimeFrame == this.CurrentTimeFrame);
			if (!timeFrame.IsNull())
			{
				int index = GetIndexFirstCandle(timeFrame);

				timeFrame.LockCollection();
				var col = timeFrame.MainCollection.Skip(index).Take(Convert.ToInt32(this.numericUpDownFilterHorVol.Value));
				timeFrame.UnlockCollection();

				GraphicStock.CalculationHVol(col);
			}
		}

		private void pictureBoxGraphic_MouseMove(object sender, MouseEventArgs e)
		{
			GraphicStock.CrossLine = new Point(e.X, e.Y);
			this.UpdateGraphic();
		}

		private void Form_GraphicDepth_Resize(object sender, EventArgs e)
		{
			this.UpdateGraphic();
		}

		private void buttonInc_Click(object sender, EventArgs e)
		{
			if (this.CountCandleInGraphic > 5)
			{
				this.CountCandleInGraphic -= 3;
				this.GetHorVol();
			}
			this.UpdateGraphic();
		}

		private void buttonDec_Click(object sender, EventArgs e)
		{
			if (this.CountCandleInGraphic < 300)
			{
				this.CountCandleInGraphic += 3;
				this.GetHorVol();
			}
			this.UpdateGraphic();
		}

		private void comboBoxTimeFrame_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox sen = (ComboBox)sender;

			if (sen.SelectedItem != null)
			{
				switch (sen.SelectedItem.ToString())
				{
					case "M1":
						this.CurrentTimeFrame = 1;
						break;
					case "M2":
						this.CurrentTimeFrame = 2;
						break;
					case "M3":
						this.CurrentTimeFrame = 3;
						break;
					case "M5":
						this.CurrentTimeFrame = 5;
						break;
					case "M15":
						this.CurrentTimeFrame = 15;
						break;
					case "M30":
						this.CurrentTimeFrame = 30;
						break;
					case "M60":
						this.CurrentTimeFrame = 60;
						break;
					case "H4":
						this.CurrentTimeFrame = 240;
						break;
					case "D1":
						this.CurrentTimeFrame = 1440;
						break;
				}
				this.GetHorVol();
				UpdateGraphic();
			}
		}

		private void hScrollGraphic_Scroll(object sender, ScrollEventArgs e)
		{
			UpdateGraphic();
		}

		private void comboBoxSelSec_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (sender is ComboBox)
			{
				if (((ComboBox)sender).SelectedItem is Securities)
				{
					var objSec = (Securities)((ComboBox)sender).SelectedItem;
					if (!objSec.Empty())
					{
						this.TrElement.OnNewCandle = null;

						this.Trader.UnregisterDepth(this.TrElement.Security);
						var trEl = this.Parent.DataTrading.Collection.FirstOrDefault(tr => tr.Security.Code == objSec.Code);
						if (!trEl.Empty())
						{
							this.TrElement = trEl;
							this.InitReset();
						}
					}
				}
			}
		}

		private void dateTimePickerStopOrder_ValueChanged(object sender, EventArgs e)
		{
			if (sender is DateTimePicker)
			{
				var pikerDT = (DateTimePicker)sender;
			}
		}

		private void numericUpDownFilterHorVol_ValueChanged_1(object sender, EventArgs e)
		{
			this.GetHorVol();
		}
		/// <summary> Установка сообщения в нижней панели</summary>
		/// <param name="text"></param>
		private void SetBottomMessage(string text)
		{
			labelLastMsg.Text = text;
		}
		/// <summary>
		/// Обновляет список заявок
		/// </summary>
		public void UpdatePanelOrders()
		{
			var listOrders = this.Trader.Objects.Orders.Where(o => o.Sec.Code == this.TrElement.Security.Code &&
				o.Status == OrderStatus.ACTIVE);
			if (listOrders.IsNull()) return;
			this.GuiAsync(() =>
			{
				dataGridOrders.Rows.Clear();
				listOrders.ForEach<Order>((o) =>
				{
					var newRow = (DataGridViewRow)dataGridOrders.Rows[0].Clone();
					newRow.Cells[0].Value = o.OrderNumber;
					newRow.Cells[1].Value = o.Price.ToString();
					newRow.Cells[2].Value = o.Volume.ToString();
					newRow.Cells[3].Value = o.Balance.ToString();
					newRow.Cells[4].Value = o.Direction == OrderDirection.Buy ? 'B' : 'S';
					newRow.Tag = o;
					dataGridOrders.Rows.Add(newRow);
				});
			});
		}

		private void buttonDelOrder_Click(object sender, EventArgs e)
		{
			dataGridOrders.SelectedRows.ForEach<DataGridViewRow>((row) =>
			{
				if (!row.Tag.IsNull())
				{
					var ord = (Order)row.Tag;
					this.Trader.CancelOrder(ord.Sec, ord.OrderNumber);
				}
			});
		}

		private void buttonCopyOrder_Click(object sender, EventArgs e)
		{
			dataGridOrders.SelectedRows.ForEach<DataGridViewRow>((row) =>
			{
				if (!row.Tag.IsNull())
				{
					var ord = (Order)row.Tag;
					this.Trader.CreateOrder(ord, comboBoxCodeClient.SelectedText);
				}
			});
		}
	}
}
