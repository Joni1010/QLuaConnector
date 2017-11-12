using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppVEConector
{
    public partial class MainForm : Form
    {
        private void checkBoxSOClosed_CheckedChanged(object sender, EventArgs e)
        {
            this.ResetTableStopOrders();
        }

        private void checkBoxSOActive_CheckedChanged(object sender, EventArgs e)
        {
            this.ResetTableStopOrders();
        }
        private void checkBoxSOExec_CheckedChanged(object sender, EventArgs e)
        {
            this.ResetTableStopOrders();
        }

        private void dataGridViewStopOrders_DoubleClick(object sender, EventArgs e)
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
                        decimal number = Convert.ToDecimal(row.Cells[1].Value.ToString());
                        var sec = this.SearchSecurity(row.Cells[2].Value.ToString());
                        if (sec != null) Trader.CancelStopOrder(sec, number);
                    });
                }
            }
        }
    }
}
