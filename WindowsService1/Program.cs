using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WindowsService1.Control;
using WindowsService1.Utils;

namespace WindowsService1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
            //一下为测试
            //IControl control = new MainControl();
            //while (true) { }
            
        }

        private static void test()
        {
            byte[] data = { 90, 75, 67, 84, 1, 0, 9, 12, 0, 0, 50, 134, 86, 120, 86, 52, 18, 209, 0, 0, 0, 21, 34, 13, 10 };
            DataUtil.Reverse(data, 4, 2);
            //data[4] = 12;
            Console.WriteLine(DataUtil.ByteDataToStringSplit(data));
        }
    }
}
