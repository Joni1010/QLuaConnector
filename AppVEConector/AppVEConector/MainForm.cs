using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketObject;
using QuikControl;
using System.Windows.Threading;
using System.Threading;
using TradingLib;

namespace AppVEConector
{
    public partial class MainForm : Form
    {
        Connector.QuikConnector Trader = new Connector.QuikConnector();
        /// <summary> Торгуемы набор </summary>
        public TElementCollection DataTrading = new TElementCollection();

        /// <summary> 1 секундный таймер </summary>
        private event Action OnTimer1s = null;
        /// <summary> 2-х секундный таймер </summary>
        private event Action OnTimer2s = null;

        /// <summary> Список откртых форм со стаканом </summary>
        List<Form_GraphicDepth> ListFormsDepth = new List<Form_GraphicDepth>();
        Mutex MutexListFormsDEpth = new Mutex();

        /// <summary> Флаг закрытия приложения </summary>
        private bool isClose = false;

        //private TradeController ControlTrade = new TradeController();

        public MainForm()
        {
            InitializeComponent();
		}
        /// <summary>
        /// Поиск инструмента в обход демо и инфо инструментов.
        /// </summary>
        /// <param name="secCode"></param>
        /// <returns></returns>
        public Securities SearchSecurity(string secCode)
        {
            try
            {
                var sec = Trader.Objects.Securities.FirstOrDefault(s => s.Code.Contains(secCode)
                            && !s.Class.Code.Contains("INFO") && !s.Class.Code.Contains("EMU"));
                return sec;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }
        public Securities SearchSecurity(string secCode, string secClassCode)
        {
            try
            {
                var sec = Trader.Objects.Securities.FirstOrDefault(s => s.Code.Contains(secCode)
                            && s.Class.Code.Contains(secClassCode) && !s.Class.Code.Contains("EMU"));
                return sec;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            return null;
        }



        /// <summary> Получение формы со стаканом и графиком </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        private Form_GraphicDepth ShowGraphicDepth(Securities sec)
        {
            try
            {
                var form = ListFormsDepth.FirstOrDefault(f => !f.isClose && f.TrElement.Security == sec);
                if (form == null)
                {
                    var elTr = this.DataTrading.Collection.FirstOrDefault(e => e.Security == sec);
                    if (elTr == null)
                    {
                        elTr = new TElement(sec);
                        this.DataTrading.Add(elTr);
                        elTr.Create();
                    }
                    form = new Form_GraphicDepth(Trader, elTr, this);
                    ListFormsDepth.Add(form);
                }
                if (form != null) form.Show();
                return form;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            return null;
        }


        DataGridViewRow RowAllTradeClone = null;
        DataGridViewRow RowMyTradeClone = null;
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                dataGridPortfolios.Rows[0].Resizable = DataGridViewTriState.False;
                dataGridPositions.Rows[0].Resizable = DataGridViewTriState.False;
                dataGridViewAllTrade.Rows[0].Resizable = DataGridViewTriState.False;
                dataGridViewMyTrades.Rows[0].Resizable = DataGridViewTriState.False;
                dataGridViewStopOrders.Rows[0].Resizable = DataGridViewTriState.False;

                RowAllTradeClone = (DataGridViewRow)dataGridViewAllTrade.Rows[0].Clone();
                RowMyTradeClone = (DataGridViewRow)dataGridViewMyTrades.Rows[0].Clone();

                this.Trader.RegisterAllParamSec();

                InitFormCreateOrders();

                InitTimers();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }


        /// /////////////////////////////////////////////////////////////////////////
        List<DataGridViewRow> listRowsPortfolios = new List<DataGridViewRow>();
        void UpdateInfoPortfolios()
        {

            int i = 0;
            var listPortf = Trader.Objects.tPortfolios.AsArray;
            //int count = listPortf.Count();
            foreach (var p in listPortf)
            {
                dataGridPortfolios.GuiAsync(() =>
                {
                    var row = listRowsPortfolios.FirstOrDefault(r => r.Cells[0].Value.ToString() == p.Client.Code.ToString()
                        && r.Cells[1].Value.ToString() == p.LimitKind.ToString());
                    if (row == null)
                    {
                        var newRow = (DataGridViewRow)dataGridPortfolios.Rows[0].Clone();
                        newRow.Cells[0].Value = "";
                        listRowsPortfolios.Add(newRow);
                        dataGridPortfolios.Rows.Add(newRow);
                        row = newRow;
                    }

                    row.Cells[0].Value = p.Client.Code;
                    row.Cells[1].Value = p.LimitKind.ToString();
                    row.Cells[2].Value = p.Balance.ToString();
                    row.Cells[3].Value = p.CurrentBalance.ToString();
                    row.Cells[4].Value = p.PositionBalance.ToString();

                    row.Cells[5].Value = p.VarMargin.ToString();
                    if (p.VarMargin > 0) row.Cells[5].Style.BackColor = Color.LightGreen;
                    if (p.VarMargin == 0) row.Cells[5].Style.BackColor = Color.White;
                    if (p.VarMargin < 0) row.Cells[5].Style.BackColor = Color.LightCoral;

                    row.Cells[6].Value = p.Commission.ToString();
                });
                i++;
            }
        }
        /// /////////////////////////////////////////////////////////////////////////
        List<DataGridViewRow> listRowsPositions = new List<DataGridViewRow>();
        void UpdateInfoPositions()
        {
            int i = 0;
            var listPos = Trader.Objects.tPositions.AsArray;
            //int count = listPortf.Count();
            foreach (var p in listPos)
            {
                dataGridPositions.GuiAsync(() =>
                {
                    var row = listRowsPositions.FirstOrDefault(r => ((Securities)r.Tag).Code == p.Sec.Code.ToString());
                    if (row == null)
                    {
                        var newRow = (DataGridViewRow)dataGridPositions.Rows[0].Clone();
                        newRow.Cells[0].Value = "";
                        listRowsPositions.Add(newRow);
                        dataGridPositions.Rows.Add(newRow);
                        row = newRow;
                    }
                    row.Tag = p.Sec;
                    row.Cells[0].Value = p.Sec.Name;
                    row.Cells[1].Value = p.Sec.Code + ":" + p.Sec.Class.Code;
                    row.Cells[2].Value = p.Data.CurrentNet.ToString();
                    //Orders
                    row.Cells[3].Value = p.Data.OrdersBuy.ToString() + " / " + p.Data.OrdersSell.ToString();
                    //Var margin
                    row.Cells[4].Value = p.Data.VarMargin.ToString();
                    if (p.Data.VarMargin > 0) row.Cells[4].Style.BackColor = Color.LightGreen;
                    if (p.Data.VarMargin == 0) row.Cells[4].Style.BackColor = Color.White;
                    if (p.Data.VarMargin < 0) row.Cells[4].Style.BackColor = Color.LightCoral;
                });
                i++;
            }
        }

        /// /////////////////////////////////////////////////////////////////////////
        List<DataGridViewRow> listRowsOrders = new List<DataGridViewRow>();
        void UpdateInfoOrders(IEnumerable<Order> orders)
        {
            int i = 0;
            var list = orders.ToArray();
            foreach (var el in list)
            {
                dataGridViewOrders.GuiAsync(() =>
                {
                    var row = listRowsOrders.FirstOrDefault(r => r.Cells[2].Value.ToString() == el.OrderNumber.ToString());
                    if (row == null)
                    {
                        var newRow = (DataGridViewRow)dataGridViewOrders.Rows[0].Clone();
                        newRow.Cells[0].Value = listRowsOrders.Count.ToString();
                        newRow.Cells[2].Value = el.OrderNumber.ToString();
                        newRow.Cells[3].Value = el.Sec;
                        newRow.Cells[4].Value = el.Sec.Code;
                        newRow.Cells[5].Value = el.Price.ToString();
                        newRow.Cells[8].Value = el.Direction == OrderDirection.Buy ? "Buy" : "Sell";
                        newRow.Cells[9].Value = el.Sec.Name;
                        listRowsOrders.Add(newRow);
                        dataGridViewOrders.Rows.Add(newRow);
                        //Устанавливаем скрол вниз
                        dataGridViewOrders.FirstDisplayedCell = dataGridViewOrders.Rows[dataGridViewOrders.Rows.Count - 1].Cells[0];
                        row = newRow;
                    }
                    row.DefaultCellStyle.BackColor = dataGridViewOrders.Rows[0].DefaultCellStyle.BackColor;
                    //Status
                    if (el.Status == OrderStatus.ACTIVE)
                    {
                        row.Cells[1].Value = "Activ";
                        row.DefaultCellStyle.ForeColor = Color.Red;
                        if (el.Volume != el.Balance)
                        {
                            row.DefaultCellStyle.BackColor = Color.Gold;
                        }
                    }
                    if (el.Status == OrderStatus.CLOSED)
                    {
                        row.Cells[1].Value = "Close";
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                        if (el.Volume != el.Balance)
                        {
                            row.DefaultCellStyle.BackColor = Color.Gold;
                        }
                    }
                    if (el.Status == OrderStatus.EXECUTED)
                    {
                        row.Cells[1].Value = "Complete";
                        row.DefaultCellStyle.ForeColor = Color.Blue;
                    }

                    //Volume
                    row.Cells[6].Value = el.Volume.ToString();
                    //Balance
                    row.Cells[7].Value = el.Balance.ToString();
                });
                i++;
            }
        }

        /// /////////////////////////////////////////////////////////////////////////
        /// 
        List<DataGridViewRow> listRowsStopOrders = new List<DataGridViewRow>();
        void ResetTableStopOrders()
        {
            try
            {
                listRowsStopOrders.Clear();
                dataGridViewStopOrders.Rows.Clear();
                this.FilteringStopOrders(Trader.Objects.StopOrders);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        /// <summary> фильтр для стоп-заявок  </summary>
        /// <param name="stOrders"></param>
        void FilteringStopOrders(IEnumerable<StopOrder> stOrders)
        {
            IEnumerable<StopOrder> listFilter = new List<StopOrder>();
            bool changeFilter = false;
            if (checkBoxSOActive.Checked)
            {
                listFilter = listFilter.Concat(stOrders.Where(so => so.Status == OrderStatus.ACTIVE));
                changeFilter = true;
            }
            if (checkBoxSOClosed.Checked)
            {
                listFilter = listFilter.Concat(stOrders.Where(so => so.Status == OrderStatus.CLOSED));
                changeFilter = true;
            }
            if (checkBoxSOExec.Checked)
            {
                listFilter = listFilter.Concat(stOrders.Where(so => so.Status == OrderStatus.EXECUTED));
                changeFilter = true;
            }

            if (changeFilter)
            {
                this.UpdateInfoStopOrders(listFilter);
            }
            else
            {
                this.UpdateInfoStopOrders(stOrders);
            }
        }

        void UpdateInfoStopOrders(IEnumerable<StopOrder> stOrders)
        {
            int i = 0;
            var list = stOrders.ToArray();
            foreach (var el in list)
            {
                dataGridViewStopOrders.GuiAsync(() =>
                {
                    var row = listRowsStopOrders.FirstOrDefault(r => r.Cells[1].Value.ToString() == el.OrderNumber.ToString());
                    if (row == null)
                    {
                        var newRow = (DataGridViewRow)dataGridViewStopOrders.Rows[0].Clone();
                        newRow.Cells[0].Value = listRowsStopOrders.Count.ToString();
                        newRow.Cells[1].Value = el.OrderNumber.ToString();
                        newRow.Cells[2].Value = el.Sec.Code;
                        switch (el.TypeStopOrder)
                        {
                            case StopOrderType.StopLimit:
                                newRow.Cells[3].Value = "Stop лимит";
                                break;
                            case StopOrderType.LinkOrder:
                                newRow.Cells[3].Value = "Со связанной заявкой";
                                break;
                            case StopOrderType.TakeProfit:
                                newRow.Cells[3].Value = "Take профит";
                                break;
                        }
                        newRow.Cells[4].Value = el.Direction == OrderDirection.Buy ? "Buy" : "Sell";

                        newRow.Cells[6].Value = el.Price.ToString();
                        newRow.Cells[7].Value = el.Volume.ToString();
                        newRow.Cells[8].Value = el.ConditionPrice.ToString();
                        newRow.Cells[9].Value = el.ConditionPrice2.ToString();
                        newRow.Cells[10].Value = el.Spread.ToString();
                        newRow.Cells[11].Value = el.Offset.ToString();

                        listRowsStopOrders.Add(newRow);
                        dataGridViewStopOrders.Rows.Add(newRow);
                        //Устанавливаем скрол вниз
                        dataGridViewStopOrders.FirstDisplayedCell = dataGridViewStopOrders.Rows[dataGridViewStopOrders.Rows.Count - 1].Cells[0];
                        row = newRow;
                    }
                    row.DefaultCellStyle.BackColor = dataGridViewStopOrders.Rows[0].DefaultCellStyle.BackColor;
                    switch (el.Status)
                    {
                        case OrderStatus.ACTIVE:
                            row.Cells[5].Value = "Active";
                            break;
                        case OrderStatus.CLOSED:
                            row.Cells[5].Value = "Close";
                            break;
                        case OrderStatus.EXECUTED:
                            row.Cells[5].Value = "Complete";
                            break;
                    }
                    //Status
                    if (el.Status == OrderStatus.ACTIVE)
                    {
                        row.DefaultCellStyle.ForeColor = Color.Red;
                        if (el.Volume != el.Balance)
                        {
                            row.DefaultCellStyle.BackColor = Color.Gold;
                        }
                    }
                    if (el.Status == OrderStatus.CLOSED)
                    {
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                        if (el.Volume != el.Balance)
                        {
                            row.DefaultCellStyle.BackColor = Color.Gold;
                        }
                    }
                    if (el.Status == OrderStatus.EXECUTED)
                    {
                        row.DefaultCellStyle.ForeColor = Color.Blue;
                    }

                    //Volume
                    //row.Cells[6].Value = el.Volume.ToString();
                    //Balance
                    //row.Cells[7].Value = el.Balance.ToString();
                });
                i++;
            }
        }

        /// /////////////////////////////////////////////////////////////////////////
        List<DataGridViewRow> listRowsMyTrades = new List<DataGridViewRow>();
        Mutex mutexMyTrade = new Mutex();
        VScrollBar ScrollMyTrades = null;
        int countMyTrades = 1;
        void UpdateInfoMyTrades(IEnumerable<MyTrade> trades)
        {
            var list = Trader.Objects.tMyTrades.AsArray;
            foreach (var t in trades)
            {
                var row = (DataGridViewRow)RowMyTradeClone.Clone();
                mutexMyTrade.WaitOne();
                row.Cells[0].Value = "";
                listRowsMyTrades.Add(row);
                row.Cells[0].Value = countMyTrades.ToString();
                row.Cells[1].Value = t.Trade.Number.ToString();
                row.Cells[2].Value = t.OrderNum.ToString();

                row.Cells[3].Value = t.Trade.Sec.Code;
                row.Cells[4].Value = t.Trade.DateTrade.ToLongTimeString();
                row.Cells[5].Value = t.Trade.Price.ToString();
                row.Cells[6].Value = t.Trade.Volume.ToString();
                row.Cells[7].Value = t.Trade.Direction == OrderDirection.Buy ? "Buy" : "Sell";
                mutexMyTrade.ReleaseMutex();

                countMyTrades++;
            }

            dataGridViewMyTrades.GuiAsync(() =>
            {
                mutexMyTrade.WaitOne();
                if (listRowsMyTrades.Count > 0)
                {
                    dataGridViewMyTrades.Rows.AddRange(listRowsMyTrades.ToArray());
                    listRowsMyTrades.Clear();
                    if (ScrollMyTrades == null) ScrollMyTrades = (VScrollBar)dataGridViewAllTrade.Controls[1];
                    int v = ScrollMyTrades.Value;
                    int maxScroll = ScrollMyTrades.Maximum;
                    //if (v > maxScroll - 3000 && v != 0)
                    //ScrollMyTrades.Value = ScrollMyTrades.Maximum;
                    dataGridViewMyTrades.FirstDisplayedCell = dataGridViewMyTrades.Rows[dataGridViewMyTrades.Rows.Count - 1].Cells[0];
                }
                mutexMyTrade.ReleaseMutex();
            });
        }

        /// /////////////////////////////////////////////////////////////////////////
        private static Mutex mutexTrades = new Mutex();
        void UpdateInfoAllTrades(IEnumerable<Trade> trades)
        {
            //DataTrading.AddNewTrades(trades);
            //Common.Ext.NewThread(() =>
            //{
            mutexTrades.WaitOne();
                try
                {
                    foreach (var t in trades)
                    {
                        //ControlTrade.Add(t);
                        TElement lastElem = DataTrading.Collection.FirstOrDefault(e => e.Security == t.Sec);
                        if (lastElem == null)
                        {
                            lastElem = new TElement(t.Sec);
                            DataTrading.Add(lastElem);
                            lastElem.Create();
                        }
                        lastElem.NewTrade(t);
                        //Thread.Sleep(1);
                    }
                    ForEachWinDepth((f) =>
                    {
                        f.UpdateGraphic();
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            //});
            if (trades.Count() > 0)
            {
                Trade lastT = trades.Last();
                statusStrip1.GuiAsync(() =>
                {
                    toolStripStatusLabel1.Text = Trader.Objects.CountTrades + " " + lastT.DateTrade.ToLongTimeString() + " Trade " + lastT.Number + " " + lastT.SecCode + ": " +
                        lastT.Price + " (" + lastT.Volume + ") " + lastT.Direction;
                });
            }
            mutexTrades.ReleaseMutex();

            /*int count = trades.Count();
            Action ActionNewTrade = () =>
            {
                foreach (var t in trades)
                {
                    var el = DataTrading.Collection.FirstOrDefault(e => e.Security == t.Sec);
                    if (el == null)
                    {
                        el = new TradingElement(t.Sec);
                        DataTrading.Add(el);
                    }
                    el.NewTrade(t);

                    mutexAllTrade.WaitOne();
                    var row = (DataGridViewRow)RowAllTradeClone.Clone();
                    row.Cells[0].Value = "";
                    listRowsAllTrades.Add(row);
                    row.Cells[0].Value = countAllTrades.ToString();
                    row.Cells[1].Value = t.Number.ToString();
                    row.Cells[2].Value = t.Sec.Code;
                    row.Cells[3].Value = t.DateTrade.ToLongTimeString();
                    row.Cells[4].Value = t.Price.ToString();
                    row.Cells[5].Value = t.Volume.ToString();
                    row.Cells[6].Value = t.Direction == OrderDirection.Buy ? "Buy" : "Sell";
                    mutexAllTrade.ReleaseMutex();

                    countAllTrades++;
                }

                if (listRowsAllTrades.Count > 0)
                {
                    dataGridViewAllTrade.GuiAsync(() =>
                    {
                        mutexAllTrade.WaitOne();
                        dataGridViewAllTrade.Rows.AddRange(listRowsAllTrades.ToArray());
                        listRowsAllTrades.Clear();
                        mutexAllTrade.ReleaseMutex();
                    });

                    if (ScrollAllTrades == null) ScrollAllTrades = (VScrollBar)dataGridViewAllTrade.Controls[1];
                    int v = ScrollAllTrades.Value;
                    int maxScroll = ScrollAllTrades.Maximum;
                    if (v > maxScroll - 3000 && v != 0)
                    {
                            //ScrollAllTrades.Value = ScrollAllTrades.Maximum;
                            dataGridViewAllTrade.GuiAsync(() =>
                        {
                            dataGridViewAllTrade.FirstDisplayedCell = dataGridViewAllTrade.Rows[dataGridViewAllTrade.Rows.Count - 1].Cells[0];
                        });
                    }
                }
            };
            if (count > 1000)
            {
                Custome.NewThread(() =>
                {
                    ActionNewTrade();
                });
            }
            else
            {
                ActionNewTrade();
            }*/
        }
        void EventAllTrades(IEnumerable<Trade> trades)
        {
            try
            {
                if (this.isClose) return;
                UpdateInfoAllTrades(trades);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        void EventMyTrades(IEnumerable<MyTrade> myTrades)
        {
            try
            {
                if (this.isClose) return;
                UpdateInfoMyTrades(myTrades);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void UpdateOrderInDepth(IEnumerable<Order> orders)
        {
            foreach (var or in orders) {
                var formD = this.ListFormsDepth.FirstOrDefault(form => form.TrElement.Security.Code == or.Sec.Code);
                if(!formD.IsNull()) formD.UpdatePanelOrders();
            }
        }

        void EventOrders(IEnumerable<Order> orders)
        {
            try
            {
                if (this.isClose) return;
                UpdateInfoOrders(orders);
                this.UpdateOrderInDepth(orders);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        void EventStopOrders(IEnumerable<StopOrder> stOrders)
        {
            try
            {
                if (this.isClose) return;
                this.FilteringStopOrders(stOrders);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        void EventPositions(IEnumerable<Position> pos)
        {
            try
            {
                if (this.isClose) return;
                UpdateInfoPositions();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        void EventPortfolio(IEnumerable<Portfolio> portf)
        {
            try
            {
                if (this.isClose) return;
                UpdateInfoPortfolios();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        /// <summary>
        /// События в стакане
        /// </summary>
        /// <param name="listQuote"></param>
        void EventDepth(IEnumerable<Quote> listQuote)
        {
            try
            {
                int count = listQuote.Count();
                Action actionListQuote = () =>
                {
                    foreach (var q in listQuote)
                    {
                        MutexListFormsDEpth.WaitOne();
                        var form = ListFormsDepth.FirstOrDefault(f => f.TrElement.Security == q.Sec);
                        MutexListFormsDEpth.ReleaseMutex();
                        if (form != null) form.SetDataDepth(q);
                    }
                };
                if (count > 5)
                {
                    Common.Ext.NewThread(() => { actionListQuote(); });
                }
                else actionListQuote();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void EventNewSec(IEnumerable<Securities> listSec)
        {
            /*foreach (var s in listSec)
            {
                if (TradingElement.CheckHistory(s))
                {
                    var elTr = new TradingElement(s);
                    this.DataTrading.Add(elTr);
                    //Загрузка исторических котировок
                    elTr.LoadHistoryCandle(5, s);
                }
            }*/
        }
        /// <summary>
        /// Изменение текста в главном статус баре.
        /// </summary>
        /// <param name="text"></param>
        public void ChangeTextMainStatusBar(string text)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString() + ": " + text;
        }

        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).Enabled = false;

            ChangeTextMainStatusBar("Загрузка базовых данных...");

            Trader.Connect();
            Trader.Objects.tTrades.OnNew += new MarketEvents<Trade>.eventElement(EventAllTrades);

            Trader.Objects.tSecurities.OnNew += new MarketEvents<Securities>.eventElement(EventNewSec);

            Trader.Objects.tMyTrades.OnNew += new MarketEvents<MyTrade>.eventElement(EventMyTrades);
            Trader.Objects.tOrders.OnNew += new MarketEvents<Order>.eventElement(EventOrders);
            Trader.Objects.tOrders.OnChange += new MarketEvents<Order>.eventElement(EventOrders);

            Trader.Objects.tStopOrders.OnNew += new MarketEvents<StopOrder>.eventElement(EventStopOrders);
            Trader.Objects.tStopOrders.OnChange += new MarketEvents<StopOrder>.eventElement(EventStopOrders);

            Trader.Objects.tPortfolios.OnNew += new MarketEvents<Portfolio>.eventElement(EventPortfolio);
            Trader.Objects.tPortfolios.OnChange += new MarketEvents<Portfolio>.eventElement(EventPortfolio);

            Trader.Objects.tPositions.OnNew += new MarketEvents<Position>.eventElement(EventPositions);
            Trader.Objects.tPositions.OnChange += new MarketEvents<Position>.eventElement(EventPositions);

            Trader.Objects.tQuote.OnChange += new MarketEvents<Quote>.eventElement(EventDepth);//ToolsQuote.eventQuote

            Trader.Objects.tTransaction.OnTransReply += new ToolsTrans.eventTrans((listReply) =>
            {
                Qlog.CatchException(() => {
                    if (listReply.Count() > 0)
                    {
                        TransReply r = listReply.Last();
                        //foreach (TransReply r in listReply)
                        if (r != null)
                        {
                            statusStrip1.GuiAsync(() =>
                            {
                                toolStripStatusLabel1.Text = r.ResultMsg;
                            });

                            ForEachWinDepth((f) =>
                            {
                                f.UpdateTransReply(r.ResultMsg);
                            });
                        }
                    }
                });
            });
        }

        /// <summary>
        /// Выполняет какое либо действие для открытых окон
        /// </summary>
        /// <param name="action"></param>
        private void ForEachWinDepth(Action<Form_GraphicDepth> action)
        {
            try
            {
                var form = ListFormsDepth.Where(f => !f.isClose);
                if (form != null)
                {
                    foreach (var f in form)
                    {
                        if (f.TrElement != null) action(f);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        private void InitTimers()
        {
            try
            {
                EventHandler eventLastPrice1s = (s, e) =>
                {
                    this.ListFormsDepth.RemoveAll(f => f.isClose);
                    if (this.OnTimer1s != null)
                        this.OnTimer1s();
                };
                DispatcherTimer dispatcherTimer1s = new DispatcherTimer();
                dispatcherTimer1s.Tick += new EventHandler(eventLastPrice1s);
                dispatcherTimer1s.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer1s.Start();


                EventHandler eventLastPrice2s = (s, e) =>
                {
                    this.ListFormsDepth.RemoveAll(f => f.isClose);
                    if (this.OnTimer2s != null)
                        this.OnTimer2s();
                };
                DispatcherTimer dispatcherTimer2s = new DispatcherTimer();
                dispatcherTimer2s.Tick += new EventHandler(eventLastPrice2s);
                dispatcherTimer2s.Interval = new TimeSpan(0, 0, 2);
                dispatcherTimer2s.Start();

                //Сохранение свечек
                EventHandler eventSaveCharts = (s, e) =>
                {
                    this.ListFormsDepth.RemoveAll(f => f.isClose);
                    if (this.DataTrading != null)
                    {
                        Common.Ext.NewThread(() =>
                        {
                            foreach (var data in this.DataTrading.Collection)
                            {
                                if (this.isClose) return;
                                //data.SaveCharts(5);
                            }
                        });
                    }
                };
                DispatcherTimer TimerSaveCharts = new DispatcherTimer();
                TimerSaveCharts.Tick += new EventHandler(eventSaveCharts);
                TimerSaveCharts.Interval = new TimeSpan(0, 0, 20);
                TimerSaveCharts.Start();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.isClose = true;
            //DataTrading.Stop();
            Trader.Disconnect();
            int count = this.DataTrading.Collection.Count();
            int i = 0;
            this.DataTrading.Collection.ForEach<TElement>((tr) =>
            {
                tr.SaveCharts();
                this.Text = "Save " + ((int)(i * 100 / count)).ToString()  + "%";
                i++;
                Thread.Sleep(1);
            });
        }
        /*
        Control CopyAllControlInTab(Control parant, Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                Type r = c.GetType();
                //Control cNew = (Control)Activator.CreateInstance(c.GetType(), new object[] { parant });
                Control cNew = (Control)Activator.CreateInstance(c.GetType());
                //
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(c);

                foreach (PropertyDescriptor entry in pdc)
                {
                    object val = entry.GetValue(c);
                    entry.SetValue(cNew, val);
                }
                cNew.Visible = true;
                cNew.CreateControl();
                cNew.Parent = parant;
                parant.Controls.Add(this.CopyAllControlInTab(cNew, c.Controls));
                cNew.Update();
                cNew.ResumeLayout();
                cNew.PerformLayout();
                cNew.Refresh();
                cNew.Invalidate();
                // add control to new TabPage
                //tpNew.Controls.Add(cNew);
            }
            return parant;
        }*/

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (ListFormsDepth.Count > 0)
            {
                MessageBox.Show("Закройте все открытые окна!");
                e.Cancel = true;
                return;
            }
            var result = MessageBox.Show(this, "Закрыть окно?", "Закрыть окно?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK) e.Cancel = false;
            else
            {
                e.Cancel = true;
                return;
            }

            
        }

        private void dataGridPositions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var senderGrid = (DataGridView)sender;

                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                    e.RowIndex >= 0)
                {
                    var sec = senderGrid.Rows[e.RowIndex].Tag != null ? (Securities)senderGrid.Rows[e.RowIndex].Tag : null;
                    if (sec != null) ShowGraphicDepth(sec);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

		private void textBoxSearchSec_TextChanged(object sender, EventArgs e)
		{
			var obj = (TextBox)sender;
			if(obj.Text.Length > 1)
			{
				var list = this.FindSecurity(obj.Text);
				if(list.NotIsNull() && list.Count() > 0)
				{
					dataGridFoundSec.Rows.Clear();
					list.ToList().ForEach((el) =>
					{
						var row = (DataGridViewRow)dataGridFoundSec.Rows[0].Clone();
						row.Cells[0].Value = el.Code + ":" + el.Class.Code;
						row.Cells[1].Value = el.Name;
						row.Tag = el;
						dataGridFoundSec.Rows.Add(row);
					});
				} else dataGridFoundSec.Rows.Clear();
			} else
			{
				dataGridFoundSec.Rows.Clear();
			}
		}
		/// <summary> Поиск инструметов </summary>
		/// <param name="codeOrName"></param>
		/// <returns></returns>
		private IEnumerable<Securities> FindSecurity(string codeOrName)
		{
			if (this.Trader.IsNull()) return null;
			if(this.Trader.Objects.tSecurities.Count > 0)
			{
				var list = this.Trader.Objects.Securities.Where(s => s.Code.ToUpper().Contains(codeOrName.ToUpper()) || s.Name.ToUpper().Contains(codeOrName.ToUpper()));
				if (list.NotIsNull()) return list ;
			}
			return null;
		}

		private void buttonOpenFoundDepth_Click(object sender, EventArgs e)
		{
			foreach(DataGridViewRow row in dataGridFoundSec.SelectedRows)
			{
				if (row.Tag.NotIsNull())
				{
					var sec = (Securities)row.Tag;
					this.ShowGraphicDepth(sec);
				}
			}
		}
	}
}
