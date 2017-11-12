using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MarketObject;
using System.Windows.Threading;

namespace AppVEConector
{
    public partial class MainForm : Form
    {
        Securities CreateOrderSec = null;
        private void InitFormCreateOrders()
        {
            try
            {
                this.OnTimer2s += () =>
                {
                    if (CreateOrderSec != null)
                    {
                        var pos = Trader.Objects.Positions.FirstOrDefault(p => p.Sec == CreateOrderSec);
                        OrdersLastPrice.GuiAsync(() =>
                        {
                            if (CreateOrderSec != null)
                                if (CreateOrderSec.LastTrade != null)
                                    OrdersLastPrice.Text = CreateOrderSec.LastTrade.Price.ToString();
                            if (pos != null)
                            {
                                labelPosSec.Text = pos.Data.CurrentNet.ToString();
                                labelOrdersCountOrder.Text = pos.Data.OrdersBuy.ToString() + " / " + pos.Data.OrdersSell.ToString();
                            }
                            else
                            {
                                labelPosSec.Text = "0";
                                labelOrdersCountOrder.Text = "0";
                            }
                        });
                    }
                };

                OrdersSetPrice.InitWheelDecimal();
                OrdersSetVolume.InitWheelDecimal();

                textBoxOrderFindSec.TextChanged += (sen, e) =>
                {
                    var textBox_fs = (TextBox)sen;
                    string secCode = textBox_fs.Text;
                    this.SelectSecurity(secCode);
                };
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        /// <summary>
        /// Переключатель на нужный инструмент
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelOrdersClass_Click(object sender, EventArgs e)
        {
            try
            {
                if (CreateOrderSec != null)
                {

                    if (labelOrdersClass.Tag == null) labelOrdersClass.Tag = 0;
                    int index = (int)labelOrdersClass.Tag;
                    var listSec = Trader.Objects.Securities.Where(s => s.Code == CreateOrderSec.Code).ToArray();
                    if (listSec.Length > 0)
                    {
                        if (index >= listSec.Length) index = 0;
                        var classSec = listSec.ElementAt(index);
                        this.SelectSecurity(CreateOrderSec.Code, classSec.Class.Code);
                        index++;
                        labelOrdersClass.Tag = index;
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        /// <summary>
        /// Осуществляет поиск и подстановку значений в формы нужно инструмента.
        /// </summary>
        /// <param name="secCode"></param>
        private void SelectSecurity(string secCode, string secClass = "")
        {
            try
            {
                Securities sec = null;
                if (secClass != "")
                {
                    sec = this.SearchSecurity(secCode, secClass);
                }
                else
                {
                    sec = this.SearchSecurity(secCode);
                }
                if (sec != null)
                {
                    CreateOrderSec = sec;

                    labelOrdersSec.Text = CreateOrderSec.Code;
                    labelOrdersClass.Text = CreateOrderSec.Class.Code;
                    if (CreateOrderSec.LastTrade != null)
                    {
                        OrdersLastPrice.Text = CreateOrderSec.LastTrade.Price.ToString();
                        OrdersSetPrice.Value = CreateOrderSec.LastTrade.Price;
                    }
                }
                else
                {
                    labelOrdersSec.Text = "---";
                    labelOrdersClass.Text = secClass;
                    OrdersLastPrice.Text = "0";
                    OrdersSetPrice.Value = 0;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        private void dataGridViewOrders_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var dataGrid = (DataGridView)sender;
                DataGridViewRow selectRow = null;
                foreach (var r in dataGrid.SelectedRows)
                    selectRow = (DataGridViewRow)r;
                if (selectRow != null)
                {
                    CreateOrderSec = (Securities)selectRow.Cells[3].Value;
                    OrdersSetPrice.DecimalPlaces = CreateOrderSec.Scale;
                    OrdersSetPrice.Increment = CreateOrderSec.Params.MinPriceStep;
                    OrdersSetPrice.Value = Convert.ToDecimal(selectRow.Cells[5].Value);
                    OrdersSetVolume.Value = Convert.ToDecimal(selectRow.Cells[6].Value);

                    textBoxOrderFindSec.Text = CreateOrderSec.Code;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void buttonOrderCreateBuy_Click(object sender, EventArgs e)
        {
            try
            {
                if (CreateOrderSec != null && OrdersSetPrice.Value > 0 && OrdersSetVolume.Value > 0)
                {
                    Common.Ext.NewThread(() =>
                    {
                        Trader.CreateOrder(new Order()
                        {
                            Price = OrdersSetPrice.Value,
                            Volume = Convert.ToInt32(OrdersSetVolume.Value),
                            Direction = OrderDirection.Buy,
                            Sec = CreateOrderSec
                        });
                    });
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void buttonOrderCreateSell_Click(object sender, EventArgs e)
        {
            try
            {
                if (CreateOrderSec != null && OrdersSetPrice.Value > 0 && OrdersSetVolume.Value > 0)
                {
                    Common.Ext.NewThread(() =>
                    {
                        Trader.CreateOrder(new Order()
                        {
                            Price = OrdersSetPrice.Value,
                            Volume = Convert.ToInt32(OrdersSetVolume.Value),
                            Direction = OrderDirection.Sell,
                            Sec = CreateOrderSec
                        });
                    });
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void OrdersLastPrice_Click(object sender, EventArgs e)
        {
            OrdersSetPrice.Value = Convert.ToDecimal(OrdersLastPrice.Text);
        }

        private void buttonOrdersCancelAll_Click(object sender, EventArgs e)
        {
            if (CreateOrderSec != null)
            {
                Common.Ext.NewThread(() =>
                {
                    Trader.CancelAllOrder(CreateOrderSec);
                });
            }
        }

        //Снятие заявки
        private void dataGridViewOrders_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var MouseEvent = (MouseEventArgs)e;
                var dataGrid = (DataGridView)sender;
                dataGrid.ClearSelection();
                if (MouseEvent.Button == MouseButtons.Right)
                {
                    var hti = dataGrid.HitTest(MouseEvent.X, MouseEvent.Y);
                    int index = hti.RowIndex;
                    if (index >= 0)
                    {
                        DataGridViewRow row = dataGrid.Rows[index];
                        row.Selected = true;
                        Common.Ext.NewThread(() =>
                        {
                            decimal number = Convert.ToDecimal(row.Cells[2].Value.ToString());
                            var sec = Trader.Objects.Securities.FirstOrDefault(s => s == row.Cells[3].Value);
                            if (sec != null) Trader.CancelOrder(sec, number);
                        });
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void buttonOrdersShowDepth_Click(object sender, EventArgs e)
        {
            if (CreateOrderSec != null)
            {
                ShowGraphicDepth(CreateOrderSec);
            }
        }
    }
}
