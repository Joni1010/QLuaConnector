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
	class ChangeUpDown :Control
	{
		public ChangeUpDown(decimal val) { this.NewValChange = val; }
		/// <summary> Новое значение при вращении колеса мыши </summary>
		public decimal NewValChange = 0;
		public DateTime lastChange = DateTime.Now;
	}
	public static void InitWheelDecimal(this NumericUpDown obj)
	{
		obj.Controls.Add(new ChangeUpDown(obj.Minimum));
		obj.ValueChanged += (s, e) =>
		{
			var el = (NumericUpDown)s;
			el.Controls.ForEach<Control>((child) =>
			{
				if (child is ChangeUpDown)
				{
					var con = (ChangeUpDown)child;
					if (DateTime.Now.Ticks - con.lastChange.Ticks > 50)
					{
						con.lastChange = DateTime.Now;
						con.NewValChange = el.Value;
					} else
					{
						el.Value = con.NewValChange;
					}
				}
			}); 
		};
	}
}
namespace AppVEConector
{

}
