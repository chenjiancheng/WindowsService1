using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsService1.Utils;

namespace WindowsService1
{
    class SocketService : IServer
    {
        private IControl control;
        public SocketService(IControl control)
        {
            this.control = control;
        }
        public void send(object ob)
        {
            control.receiveData(ob);
        }

        public void start()
        {
            Thread serverThread = new Thread(new ThreadStart(createServer));
            //将窗体线程设置为与后台同步，随着主线程结束而结束  
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        public void createServer()
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            const int port = IPUtil.PORT_TREE;//端口号
            string host = IPUtil.GetLocalIpv4()[0];//获取本机IP
            try
            {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);//IPEndPoint类是对ip端口做了一层封装的
                server.Bind(ipe);//向操作系统申请一个可用的ip地址和端口号用于通信
                server.Listen(500);//设置最大的连接数
                string msg = "创建端口:" + port + " 等待客户端连接中...";
                LogUtil.Log(msg);

            }
            catch (Exception e)
            {
                LogUtil.Log("-------创建Socket失败-------");
                throw e;
            }

            while (true)
            {
                createClienAcceptThread(server.Accept());//阻塞状态，等待连接,创建线程接收信息
            }

        }


        private void createClienAcceptThread(Socket client)
        {
            //创建一个通信线程      
            ParameterizedThreadStart pts = new ParameterizedThreadStart(clientAccept);
            Thread thread = new Thread(pts);
            thread.IsBackground = true;//设置为后台线程，随着主线程退出而退出          
            thread.Start(client);//启动线程  
        }

        static int testNum = 1;
        private void clientAccept(Object socketServer)
        {
            Socket client = socketServer as Socket;
            //获取客户端的IP和端口号  
            string clientIP = (client.RemoteEndPoint as IPEndPoint).Address.ToString();//IPAddress
            int clientPort = (client.RemoteEndPoint as IPEndPoint).Port;
            LogUtil.Log("客户端连入 " + clientIP + " : " + clientPort);//CJC应提示或列出报警器是否可用正常
            int order = 1;//序号
            while (true)
            {
                byte[] clientData = new byte[1024];
                //try
                //{
                int revLen = client.Receive(clientData, SocketFlags.None);
                if (revLen <= 0)
                {
                    LogUtil.Log("客户端:" + clientIP + "数据长度为0 break");
                    break;
                }
                else if (revLen == 25)
                {
                    TreeEntity bean = DataUtil.ParseData(clientData, revLen, clientIP);
                    ResultBean result = DataUtil.CheckEntity(bean);
                    LogUtil.Log(LogLevel.Info, "接收数据 " + (bean.SourceData));
                    if (result.Result)
                    {
                        try
                        {
                            client.Send(DataUtil.Respond(bean, order));
                            order++;
                            LogUtil.Log(LogLevel.Info, "回复数据 " + (DataUtil.ByteDataToStringSplit(DataUtil.Respond(bean, order))));
                        }
                        catch
                        {
                            LogUtil.Log(LogLevel.Error, "发送数据 " + (DataUtil.ByteDataToStringSplit(DataUtil.Respond(bean, order))) + " 失败");
                        }

                    }
                    send(bean);//保存数据库
                }
                else
                {
                    string data = DataUtil.ByteDataToStringSplit(clientData, 0, revLen);
                    LogUtil.Log(LogLevel.Error, clientIP+"|接收到长度为" +revLen+"的数据|" + data);
                }
                /*
            }
            catch (Exception e)
            {
                LogUtil.Log("远程主机关闭了一个现有的连接" + e.Message);
                break;
           }
           */
            }
        }
    }
}
