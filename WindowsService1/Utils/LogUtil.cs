using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    class LogUtil
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static void Log(string msg)
        {
            Log(LogLevel.Info, msg);
        }
        public static void Log(LogLevel level, string msg)
        {
            logger.Log(level, msg);
        }
    }
}
