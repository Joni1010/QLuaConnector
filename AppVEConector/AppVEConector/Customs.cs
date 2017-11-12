using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

public static class ControlExtension
{
    public static void GuiAsync(this Control form, Action action)
    {
        if (form != null)
        {
            MethodInvoker AsyncAction = delegate
            {
                if (action != null) action();
            };
            if (form.InvokeRequired)
                form.BeginInvoke(AsyncAction);
            else AsyncAction();
        }
    }
    public delegate void ActionWithParam(object param);
    public static void GuiAsync(this Control form, ActionWithParam action, object arg)
    {
        if (form != null)
        {
            MethodInvoker AsyncAction = delegate
            {
                if (action != null) action(arg);
            };
            if (form.InvokeRequired)
                form.BeginInvoke(AsyncAction);
            else AsyncAction();
        }
    }
}


public static class ToolStripStatusLabelExtension
{
    public static void GuiAsync(this ToolStripStatusLabel f, Action action)
    {
        /*Control form = (Control)f.;
        if (form != null)
        {
            MethodInvoker AsyncAction = delegate
            {
                if (action != null) action();
            };
            form.BeginInvoke(AsyncAction);
        }*/
    }
}


public static class NumericUpDownExtesion 
{
    public static void InitWheelDecimal(this NumericUpDown obj)
    {
        obj.MouseWheel += (s, e) => {
            var el = (NumericUpDown)s;
            if (e.Delta > 0)
            {
                obj.DownButton();
                obj.DownButton();
            } else
            {
                obj.UpButton();
                obj.UpButton();
            }
        };
    }
}
namespace AppVEConector
{
   
}
