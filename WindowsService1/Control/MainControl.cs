using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsService1.Model;

namespace WindowsService1.Control
{
    class MainControl : IControl
    {
        private IServer server;

        public MainControl()
        {
            server = new SocketService(this);
            server.start();
            model = new MainModelImpl();
        }
        public override void receiveData(object obj)
        {
            ParameterizedThreadStart start = new ParameterizedThreadStart(backWorkToSave);
            Thread serverThread = new Thread(start);
            //将窗体线程设置为与后台同步，随着主线程结束而结束  
            serverThread.IsBackground = true;
            serverThread.Start(obj);
        }

        private void backWorkToSave(Object obj)
        {
            try
            {
                //处理异常
                model.save(obj);
            }
            catch (Exception e)
            {
                LogUtil.Log(NLog.LogLevel.Error, "数据保存时发生异常:"+e.ToString());
            }
            
        }

        public override void reInitMode()
        {
        }
    }
}
