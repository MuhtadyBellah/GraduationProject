using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminDashboard.Handler
{
    public static class ControlHandler
    {
        public static void SetDoubleBuffered(Control c)
        {
            typeof(Control)
              .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
              .SetValue(c, true, null);
        }
        public static void SetResizeRedraw(Control c)
        {
            typeof(Control)
                .GetProperty("ResizeRedraw", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(c, true, null);
        }
    }
}
