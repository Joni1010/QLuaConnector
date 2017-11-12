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
    public partial class Form_GraphicDepth : Form
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

        DataGridViewRow CloneRowDepth = null;

        /// <summary> Наполнитель стакана ПРОДАЖА. Сбрасывается при новом стакане.</summary>
        DataGridViewRow[] ArraySell = null;
        /// <summary> Наполнитель стакана ПОКУПКА. Сбрасывается при новом стакане.</summary>
        DataGridViewRow[] ArrayBuy = null;

        class StructClickDepth
        {
            public string Flag = null;
            public decimal Price = -1;
            public decimal Volume = -1;
        }
        public Form_GraphicDepth(Connector.QuikConnector trader, TElement trElement, object parent)
        {
            try
            {
                InitializeComponent();

                this.Parent = (MainForm)parent;

                if (trElement == null) this.Close();
                this.Trader = trader;
                this.TrElement = trElement;

                //Настройки графика
                this.GraphicStock = new NSGraphic.Graphic(TrElement.Security.Params.MinPriceStep);

                this.LoadListTradeSec();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Form_GraphicDepth_Load(object sender, EventArgs e)
        {
            //Установки для таблицы стакана
            dataGridViewDepth.Rows[0].Resizable = DataGridViewTriState.False;
            dataGridViewDepth.Rows[0].Height = 18;
            //Клонируем ячейку default
            CloneRowDepth = (DataGridViewRow)dataGridViewDepth.Rows[0].Clone();

            numericUpDownPrice.InitWheelDecimal();
            numericUpDownStopPrice.InitWheelDecimal();

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

            GraphicStock.PanelCandels.OnMoveVerticalCandle += (candle) => {
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

        /// <summary> Инициализация нового инструмента в текущем окне. Сброс на новый инструмент.</summary>
        private void InitReset()
        {
            this.Portfolio = this.Trader.Objects.Portfolios.FirstOrDefault(p => p.Account.AccClasses.FirstOrDefault(c => c == this.TrElement.Security.Class) != null);
            this.Position = Trader.Objects.Positions.FirstOrDefault(s => s.Sec == this.TrElement.Security);

            Trader.RegisterDepth(this.TrElement.Security);
            Trader.RegisterSecurities(this.TrElement.Security);

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

            //Поток загрузки исторических котирововок
            Common.Ext.NewThread(() =>
            {
                if (!this.TrElement.CheckLoadHistory())
                {
                    this.TrElement.HistoryComplete();
                    if (TradingLib.TElement.CheckDirHistory(this.TrElement.Security))
                    {
                        //Загрузка исторических котировок
                        for (int i = 1; i < 40; i++)
                        {
                            var date = this.Trader.Objects.Terminal.TradeDate.AddDays(i * -1);
                            this.TrElement.LoadHistoryCandle(5, date);
                            Thread.Sleep(500);
                        }
                        //this.TrElement.SaveAllCollection();
                    }
                }
            });

            pictureBoxGraphic.Refresh();
        }

        /// <summary>
        /// Загружаем торгуемые элементы из файла
        /// </summary>
        private void LoadListTradeSec()
        {
            if (this.Trader.Empty()) return;
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
                            if (!sec.Empty())
                            {
                                items.Add(sec);
                            }
                        }
                    }
                }
            }
            openFile.Close();

            comboBoxSelSec.DataSource = items;
            comboBoxSelSec.SelectedIndex = 0;
        }


        public void InitUpdater()
        {
            EventHandler eventLastPrice1s = (s, e) =>
            {
                if (this.isClose) return;
                this.UpdateDepth();
                //Если регистрация стакана не прошла, запрашиваем еще раз
                //if (this.LastQuote.Object.Empty())
                //    this.Trader.RegisterDepth(this.TrElement.Security);
                // this.UpdateHorVol();
                this.UpdateInfoForm();
            };
            DispatcherTimer dispatcherTimer1s = new DispatcherTimer();
            dispatcherTimer1s.Tick += new EventHandler(eventLastPrice1s);
            dispatcherTimer1s.Interval = new TimeSpan(0, 0, 0, 0, 300);
            dispatcherTimer1s.Start();
        }

        /// <summary>
        /// Обновляем формы с иформацией
        /// </summary>
        public void UpdateInfoForm()
        {
            try
            {
                if (this.Position != null)
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
                if (this.Portfolio != null)
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

                if (this.TrElement.Security.LastTrade != null)
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
            try
            {
                this.LastQuote.Object = quote;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        /// <summary> Сумма объема в стакане, 0 - bid, 1 - ask</summary>
        public long[] CountInDepth = new long[2];
        public void UpdateDepth()
        {
            try
            {
                if (!this.LastQuote.Object.Empty() && !this.TrElement.Empty())
                {
                    int countS = 20;
                    int countB = 20;
                    var QuoteBid = this.LastQuote.Object.Bid.ToArray();
                    var QuoteAsk = this.LastQuote.Object.Ask.ToArray();
                    int countInDepthSell = QuoteAsk.Length;
                    int countInDepthBuy = QuoteBid.Length;

                    if (ArraySell == null && countInDepthSell > 0)
                    {
                        ArraySell = new DataGridViewRow[countS];
                        for (int i = countS - 1; i >= 0; i--)
                        {
                            ArraySell[i] = (DataGridViewRow)CloneRowDepth.Clone();
                            ArraySell[i].Cells[0].Value = "";
                            dataGridViewDepth.GuiAsync((param) =>
                            {
                                var r = (DataGridViewRow)param;
                                dataGridViewDepth.Rows.Add(r);
                            }, ArraySell[i]);
                        }
                    }
                    if (ArrayBuy == null && countInDepthBuy > 0)
                    {
                        ArrayBuy = new DataGridViewRow[countB];
                        for (int i = 0; i < countB; i++)
                        {
                            ArrayBuy[i] = (DataGridViewRow)CloneRowDepth.Clone(); ;
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
                    //Custome.NewThread(() =>
                    //{
                    for (int i = 0; i < countS; i++)
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

                    for (int i = 0; i < countB; i++)
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
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }



        private void dataGridViewDepth_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGrid = (DataGridView)sender;
        }

        private void dataGridViewDepth_Click(object sender, EventArgs e)
        {
            if (this.TrElement.Security == null) return;
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
                        Common.Ext.NewThread(() =>
                        {
                            Trader.CreateOrder(new Order()
                            {
                                Sec = this.TrElement.Security,
                                Direction = direction,
                                Price = data.Price,
                                Volume = Volume
                            });
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
                                if (ords != null)
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
            if (this.TrElement.Security != null)
            {
                Trader.CancelAllOrder(this.TrElement.Security);
            }
        }

        private void buttonExitPos_Click(object sender, EventArgs e)
        {
            if (this.TrElement.Security != null)
            {
                var pos = Trader.Objects.Positions.FirstOrDefault(p => p.Sec == this.TrElement.Security);
                if (pos != null)
                {
                    if (pos.Data.CurrentNet != 0)
                    {
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
                            });
                            //statusStrip1.GuiAsync(() =>
                            //{
                            //    toolStripStatusLabel1.Text = "Создать заявку " + Price.ToString() + "(" + Volume.ToString() + ")" + direction.ToString();
                            //});
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
                    });
                    //toolStripStatusLabel1.Text = "Создать заявку " + numericUpDownPrice.Value.ToString() +
                    //    "(" + numericUpDownVolume.Value.ToString() + ") Buy";
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
                    });
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
            if (WidthPanelDepth < 0) WidthPanelDepth = panelDepth.Width;
            if (checkBoxShowHideDepth.Checked) panelDepth.Width = 0;
            else panelDepth.Width = WidthPanelDepth;
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

        /// <summary>
        /// Обновление сообщения
        /// </summary>
        private Thread threadMsg = null;
        public void UpdateTransReply(string Msg)
        {
            if (this.TrElement != null)
            {
                if (threadMsg != null)
                {
                    threadMsg.Abort();
                    threadMsg = null;
                }
                panelMessage.GuiAsync(() =>
                {
                    panelMessage.Visible = true;
                    textBoxMessage.Text = Msg;
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

        Thread threadPaint = null;
        public void UpdateGraphic()
        {
            //try
            var timeFrame = this.TrElement.CollectionTimeFrames.FirstOrDefault(tf => tf.TimeFrame == this.CurrentTimeFrame);
            if (timeFrame != null)
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
                //if (this.CountCandleInGraphic < index) index = timeFrame.Count - this.CountCandleInGraphic;

                try
                {
                    timeFrame.LockCollection();
                    GraphicStock.PanelCandels.CollectionCandle = timeFrame.MainCollection.ToArray().Skip(index).Take(this.CountCandleInGraphic);
                    timeFrame.UnlockCollection();
                }
                catch (Exception ee)
                {
                    var t = ee.ToString();
                }

                GraphicStock.PanelCandels.CountPaintCandle = this.CountCandleInGraphic;

                //Orders
                List<MarketObject.Chart> orders = new List<MarketObject.Chart>();
                /*var lBuy = this.Trader.Objects.Orders.Where(o => o.Sec.Code == this.TrElement.Security.Code && o.Direction == OrderDirection.Buy && o.Status == OrderStatus.ACTIVE);
                foreach (var o in lBuy)
                {
                    var ch = orders.FirstOrDefault(c => c.Price == o.Price);
                    if (ch != null) ch.Volume += o.Volume;
                    else orders.Add(new NSGraphic.Graphic.Chart() { Price = o.Price, Volume = o.Volume });
                }
                var lSell = this.Trader.Objects.Orders.Where(o => o.Sec.Code == this.TrElement.Security.Code && o.Direction == OrderDirection.Sell && o.Status == OrderStatus.ACTIVE);
                foreach (var o in lSell)
                {
                    var ch = orders.FirstOrDefault(c => c.Price == o.Price);
                    if (ch != null) ch.Volume += o.Volume * -1;
                    else orders.Add(new NSGraphic.Graphic.Chart() { Price = o.Price, Volume = o.Volume * -1 });
                }*/

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
                    pictureBoxGraphic.Refresh();
                });
            }

            /*catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }*/
        }
        private void GetHorVol()
        {
            var timeFrame = this.TrElement.CollectionTimeFrames.FirstOrDefault(tf => tf.TimeFrame == this.CurrentTimeFrame);
            if (!timeFrame.IsNull())
            {
                int index = hScrollGraphic.Maximum - hScrollGraphic.Value;
                if (this.CountCandleInGraphic < index) index = timeFrame.Count - this.CountCandleInGraphic;

                timeFrame.LockCollection();
                var col = timeFrame.MainCollection.ToArray().Skip(index).Take(Convert.ToInt32(this.numericUpDownFilterHorVol.Value));
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
            }
            this.GetHorVol();
            this.UpdateGraphic();
        }

        private void buttonDec_Click(object sender, EventArgs e)
        {

            if (this.CountCandleInGraphic < 300)
            {
                this.CountCandleInGraphic += 3;
            }
            this.GetHorVol();
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
    }
}
