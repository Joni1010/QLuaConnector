using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketObject;
using System.Threading;
using System.Windows.Threading;
using TradingLib;

namespace AppVEConector
{
	public partial class Form_GraphicDepth :Form
	{
		public bool isClose = false;
		public TElement TrElement = null;
		public Portfolio Portfolio = null;
		public Connector.QuikConnector Trader = null;
		public Position Position = null;
		private MainForm Parent = null;

		public NSGraphic.Graphic GraphicStock = null;

		/// <summary> Последние данный по стакану </summary>
		public Common.LockObject<Quote> LastQuote = new Common.LockObject<Quote>();

		/// <summary> Кол-во свечей на графике (Масштаб) </summary>
		private int CountCandleInGraphic = 15;
		/// <summary> Текущий тайм-фрейм (в минутах) </summary>
		private int CurrentTimeFrame = 1;

		/// <summary> Наполнитель стакана ПРОДАЖА. Сбрасывается при новом стакане.</summary>
		private DataGridViewRow[] ArraySell = null;
		/// <summary> Наполнитель стакана ПОКУПКА. Сбрасывается при новом стакане.</summary>
		private DataGridViewRow[] ArrayBuy = null;

		class StructClickDepth
		{
			public string Flag = null;
			public decimal Price = -1;
			public decimal Volume = -1;
		}

		/// <summary> Инициализация нового инструмента в текущем окне. Сброс на новый инструмент.</summary>
		private void InitReset()
		{
			this.Portfolio = this.Trader.Objects.Portfolios.FirstOrDefault(p => p.Account.AccClasses.FirstOrDefault(c => c == this.TrElement.Security.Class) != null);
			this.Position = Trader.Objects.Positions.FirstOrDefault(s => s.Sec == this.TrElement.Security);

			Trader.RegisterDepth(this.TrElement.Security);
			Trader.RegisterSecurities(this.TrElement.Security);

			if (this.TrElement.OnNewCandle.IsNull())
			{
				this.TrElement.OnNewCandle += (tf, candle) =>
				{
					if (tf == this.CurrentTimeFrame)
					{
						this.GetHorVol();
					}
				};
			}

			//цена для заявки
			numericUpDownPrice.Increment = this.TrElement.Security.Params.MinPriceStep;
			numericUpDownPrice.DecimalPlaces = this.TrElement.Security.Scale;

			//Стоп цена
			numericUpDownStopPrice.Increment = this.TrElement.Security.Params.MinPriceStep;
			numericUpDownStopPrice.DecimalPlaces = this.TrElement.Security.Scale;

			this.Text = "(" + this.TrElement.Security.Code + ":" + this.TrElement.Security.Class.Code + ") " + this.TrElement.Security.Name;

			//Сброс цены на заявку
			numericUpDownPrice.Value = 0;
			//Сброс стоп цены
			numericUpDownStopPrice.Value = 0;

			//Настройки графика
			this.GraphicStock = new NSGraphic.Graphic(TrElement.Security.Params.MinPriceStep);
			//Задаем в графике точность цены
			GraphicStock.PanelPrices.CountFloat = this.TrElement.Security.Scale;
			//Скрол для графика
			hScrollGraphic.Value = hScrollGraphic.Maximum;

			//Clear depth
			dataGridViewDepth.Rows.Clear();
			this.ArraySell = null;
			this.ArrayBuy = null;
			this.SetDataDepth(null);

			//Обновим панель заявок
			this.UpdatePanelOrders();

			pictureBoxGraphic.Refresh();
		}


		/// <summary> Инициализация тафмерного-обновителя </summary>
		public void InitUpdater()
		{
			EventHandler eventLastPrice1s = (s, e) =>
			{
				if (this.isClose) return;
				this.UpdateDepth();
				this.UpdateInfoForm();
			};
			DispatcherTimer dispatcherTimer1s = new DispatcherTimer();
			dispatcherTimer1s.Tick += new EventHandler(eventLastPrice1s);
			dispatcherTimer1s.Interval = new TimeSpan(0, 0, 0, 0, 300);
			dispatcherTimer1s.Start();
		}

		/// <summary> Загружаем торгуемые элементы из файла </summary>
		private void LoadListTradeSec()
		{
			System.IO.StreamReader openFile = new System.IO.StreamReader(@"market.list", true);
			ComboBox.ObjectCollection items = new ComboBox.ObjectCollection(comboBoxSelSec);
			items.Add("");
			while (!openFile.EndOfStream)
			{
				string line = openFile.ReadLine();
				if (!line.Empty())
				{
					string[] el = line.Split(':');
					if (el.Length > 0)
					{
						if (!el[0].Empty() && !el[1].Empty())
						{
							var sec = Trader.Objects.Securities.FirstOrDefault(s => s.Code == el[0] && s.Class.Code == el[1]);
							if (sec.NotIsNull()) items.Add(sec);
							
						}
					}
				}
			}
			openFile.Close();

			comboBoxSelSec.DataSource = items;
			comboBoxSelSec.SelectedIndex = 0;
		}

		/// <summary> Обновляем формы с иформацией </summary>
		public void UpdateInfoForm()
		{
			try
			{
				if (this.Position.NotIsNull())
				{
					buttonExitPos.Text = this.Position.Data.CurrentNet.ToString();
					labelOrdersBuy.Text = this.Position.Data.OrdersBuy.ToString();
					labelOrdersSell.Text = this.Position.Data.OrdersSell.ToString();
				}
				else
				{
					buttonExitPos.Text = "0";
					labelOrdersBuy.Text = "0";
					labelOrdersSell.Text = "0";
				}
				if (this.Portfolio.NotIsNull())
				{
					labelStatBalance.Text = this.Portfolio.CurrentBalance.ToString() + " / " + this.Portfolio.Balance.ToString();
					labelVarMargin.Text = this.Portfolio.VarMargin.ToString() +
					(this.Portfolio.RealMargin > 0 ? " (" + this.Portfolio.RealMargin.ToString() + ")" : "");
				}

				labelStatBidAsk.Text = this.TrElement.Security.Params.SumBidDepth.ToString() + " / " + this.TrElement.Security.Params.SumAskDepth.ToString();
				if (this.TrElement.Security.Params.SumBidDepth > this.TrElement.Security.Params.SumAskDepth)
					labelStatBidAsk.BackColor = Color.LightGreen;
				else labelStatBidAsk.BackColor = Color.LightCoral;


				labelDepthCount.Text = CountInDepth[0].ToString() + " / " + CountInDepth[1].ToString();
				if (CountInDepth[0] > CountInDepth[1]) labelDepthCount.BackColor = Color.LightGreen;
				else labelDepthCount.BackColor = Color.LightCoral;

				labelSumAllTrades.Text = this.TrElement.Security.SumAllTradesBuy.ToString() + " / " + this.TrElement.Security.SumAllTradesSell.ToString();
				if (this.TrElement.Security.SumAllTradesBuy > this.TrElement.Security.SumAllTradesSell) labelSumAllTrades.BackColor = Color.LightGreen;
				else labelSumAllTrades.BackColor = Color.LightCoral;

				labelStatOrdBid.Text = this.TrElement.Security.Params.NumBid.ToString() + " / " + this.TrElement.Security.Params.NumAsk.ToString();
				if (TrElement.Security.Params.NumBid > this.TrElement.Security.Params.NumAsk) labelStatOrdBid.BackColor = Color.LightGreen;
				else labelStatOrdBid.BackColor = Color.LightCoral;

				labelGaranty.Text = this.TrElement.Security.Params.BuyDepo.ToString();

				if (this.TrElement.Security.LastTrade.NotIsNull())
					labelLastPrice.Text = this.TrElement.Security.LastTrade.Price.ToString();
			}
			catch (Exception ee)
			{
				MessageBox.Show(ee.ToString());
			}
		}

		/// <summary> Установить последние данные по стакан </summary>
		/// <param name="quote"></param>
		public void SetDataDepth(Quote quote)
		{
			if (this.LastQuote.NotIsNull() && quote.NotIsNull())
				this.LastQuote.Object = quote;
		}

		/// <summary> Сумма объема в стакане, 0 - bid, 1 - ask</summary>
		public long[] CountInDepth = new long[2];
		/// <summary> Обновление стакана </summary>
		public void UpdateDepth()
		{
			if (this.LastQuote.Object.NotIsNull() && this.TrElement.NotIsNull())
			{
				int countS = 20;
				int countB = 20;
				var QuoteBid = this.LastQuote.Object.Bid.ToArray();
				var QuoteAsk = this.LastQuote.Object.Ask.ToArray();
				int countInDepthSell = QuoteAsk.Length;
				int countInDepthBuy = QuoteBid.Length;

				if (ArraySell.IsNull() && countInDepthSell > 0)
				{
					ArraySell = new DataGridViewRow[countS];
					for (int i = countS - 1;i >= 0;i--)
					{            
						ArraySell[i] = (DataGridViewRow)dataGridViewDepth.Rows[0].Clone();
						ArraySell[i].Cells[0].Value = "";
						dataGridViewDepth.GuiAsync((param) =>
						{
							var r = (DataGridViewRow)param;
							dataGridViewDepth.Rows.Add(r);
						}, ArraySell[i]);
					}
				}
				if (ArrayBuy.IsNull() && countInDepthBuy > 0)
				{
					ArrayBuy = new DataGridViewRow[countB];
					for (int i = 0;i < countB;i++)
					{
						ArrayBuy[i] = (DataGridViewRow)dataGridViewDepth.Rows[0].Clone();
						ArrayBuy[i].Cells[0].Value = "";
						dataGridViewDepth.GuiAsync((param) =>
						{
							var r = (DataGridViewRow)param;
							dataGridViewDepth.Rows.Add(r);
						}, ArrayBuy[i]);
					}
				}
				CountInDepth[0] = 0;
				CountInDepth[1] = 0;
				//Наполняем Sell
				for (int i = 0;i < countS;i++)
				{
					if (countInDepthSell <= i)
					{
						ArraySell[i].Cells[0].Value = "";
						ArraySell[i].Cells[1].Value = "";
						ArraySell[i].Cells[2].Value = "";
						ArraySell[i].Cells[3].Value = "";
						ArraySell[i].Cells[1].Style.Font = new Font(DataGridView.DefaultFont, FontStyle.Regular);
					}
					else
					{
						decimal Price = QuoteAsk[i].Price;
						int Volume = QuoteAsk[i].Volume;
						CountInDepth[1] += Volume;
						decimal SumVol = Trader.Objects.Orders.ToArray().Where(o => o.Sec == this.TrElement.Security && o.Price == Price && o.Status == OrderStatus.ACTIVE).Sum(o => o.Volume);

						ArraySell[i].Cells[1].Value = Volume.ToString();
						ArraySell[i].Cells[2].Value = Price.ToString();
						if (ArraySell[i].Cells[2].Style.BackColor != Color.LightCoral)
							ArraySell[i].Cells[2].Style.BackColor = Color.LightCoral;
						if (SumVol > 0)
						{
							ArraySell[i].Cells[2].Style.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
							ArraySell[i].Cells[1].Value = Volume.ToString() + " (" + SumVol.ToString() + ")";
						}
						else ArraySell[i].Cells[2].Style.Font = new Font(DataGridView.DefaultFont, FontStyle.Regular);

						ArraySell[i].Cells[1].Tag = new StructClickDepth()
						{
							Flag = "sell",
							Price = Price,
							Volume = Volume,
						};

						ArraySell[i].Cells[3].Tag = new StructClickDepth()
						{
							Flag = "buy",
							Price = Price,
							Volume = Volume,
						};
					}
				}
				//Наполняем Buy
				for (int i = 0;i < countB;i++)
				{
					if (countInDepthBuy <= i)
					{
						ArrayBuy[i].Cells[0].Value = "";
						ArrayBuy[i].Cells[1].Value = "";
						ArrayBuy[i].Cells[2].Value = "";
						ArrayBuy[i].Cells[3].Value = "";
						ArrayBuy[i].Cells[3].Style.Font = new Font(DataGridView.DefaultFont, FontStyle.Regular);
					}
					else
					{
						decimal Price = QuoteBid[countInDepthBuy - i - 1].Price;
						int Volume = QuoteBid[countInDepthBuy - i - 1].Volume;
						CountInDepth[0] += Volume;
						decimal VolSum = Trader.Objects.Orders.ToArray().Where(o => o.Sec == this.TrElement.Security && o.Price == Price && o.Status == OrderStatus.ACTIVE).Sum(o => o.Volume);

						ArrayBuy[i].Cells[1].Tag = new StructClickDepth()
						{
							Flag = "sell",
							Price = Price,
							Volume = Volume,
						};
						ArrayBuy[i].Cells[2].Value = Price.ToString();
						if (ArrayBuy[i].Cells[2].Style.BackColor != Color.LightGreen)
							ArrayBuy[i].Cells[2].Style.BackColor = Color.LightGreen;
						ArrayBuy[i].Cells[3].Value = Volume.ToString();
						if (VolSum > 0)
						{
							ArrayBuy[i].Cells[2].Style.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
							ArrayBuy[i].Cells[3].Value = Volume.ToString() + " (" + VolSum.ToString() + ")";
						}
						else ArrayBuy[i].Cells[2].Style.Font = new Font(DataGridView.DefaultFont, FontStyle.Regular);

						ArrayBuy[i].Cells[1].Tag = new StructClickDepth()
						{
							Flag = "sell",
							Price = Price,
							Volume = Volume,
						};

						ArrayBuy[i].Cells[3].Tag = new StructClickDepth()
						{
							Flag = "buy",
							Price = Price,
							Volume = Volume,
						};
					}
				}
			}
		}
		/// <summary> Обновление сообщения </summary>
		private Thread threadMsg = null;

		public MainForm Parent1 { get => this.Parent; set => this.Parent = value; }

		public void UpdateTransReply(string Msg)
		{
			if (this.TrElement.NotIsNull())
			{
				if (threadMsg.NotIsNull())
				{
					threadMsg.Abort();
					threadMsg = null;
				}
				panelMessage.GuiAsync(() =>
				{
					panelMessage.Visible = true;
					textBoxMessage.Text = Msg;
					this.SetBottomMessage(Msg);
				});
				Common.Ext.NewThread(() =>
				{
					Thread.Sleep(3000);
					panelMessage.GuiAsync(() =>
					{
						panelMessage.Visible = false;
					});
				});
			}
		}

		public void UpdateGraphic()
		{
			//try
			var timeFrame = this.TrElement.CollectionTimeFrames.FirstOrDefault(tf => tf.TimeFrame == this.CurrentTimeFrame);
			if (!timeFrame.IsNull())
			{
				int index = GetIndexFirstCandle(timeFrame);

				GraphicStock.SetCountCandles(this.CountCandleInGraphic);

				//Orders
				List<MarketObject.Chart> orders = new List<MarketObject.Chart>();
				var allOrd = this.Trader.Objects.Orders.Where(o => o.Sec.Code == this.TrElement.Security.Code && o.Status == OrderStatus.ACTIVE);
				foreach (var o in allOrd)
				{
					var ch = orders.FirstOrDefault(c => c.Price == o.Price);
					var vol = o.Direction == OrderDirection.Sell ? o.Volume * -1 : o.Volume;
					if (ch != null) ch.Volume += vol;
					else orders.Add(new MarketObject.Chart() { Price = o.Price, Volume = vol });
				}

				var allStOrd = this.Trader.Objects.StopOrders.Where(o => o.Sec.Code == this.TrElement.Security.Code && o.Status == OrderStatus.ACTIVE);
				foreach (var o in allStOrd)
				{
					var ch = orders.FirstOrDefault(c => c.Price == o.Price);
					var vol = o.Direction == OrderDirection.Sell ? o.Volume * -1 : o.Volume;
					if (ch != null) ch.Volume += vol;
					else orders.Add(new MarketObject.Chart() { Price = o.Price, Volume = vol });
				}
				GraphicStock.SetOrders(orders);

				pictureBoxGraphic.GuiAsync(() =>
				{
					GraphicStock.CountCandleShowHVol = Convert.ToInt32(numericUpDownFilterHorVol.Value);

					timeFrame.LockCollection();
					GraphicStock.PanelCandels.CollectionCandle = timeFrame.MainCollection.Skip(index).Take(this.CountCandleInGraphic).ToArray();
					timeFrame.UnlockCollection();

					pictureBoxGraphic.Refresh();
				});

			}

			/*catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }*/
		}
	}// end class
}
