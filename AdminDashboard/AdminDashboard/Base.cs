using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard
{
    public static class Base
    {
        public static Func<int, string> url = x => x == 0 ? "https://rational-deep-dinosaur.ngrok-free.app/api/" : "https://concise-ant-sound.ngrok-free.app/api/";
    }
}
