using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard.Handler
{
    public static class Base
    {
        public static Func<string> NET => () => "https://rational-deep-dinosaur.ngrok-free.app/api/";
        public static Func<string> Laravel = () => "https://concise-ant-sound.ngrok-free.app/api/";
    }
}
