using MySql.Data.MySqlClient;
using System;

namespace WindowsService1.Model
{
    class MainModelImpl : IModel
    {
        private MySqlConnection conn;
        private MySqlCommand cmd;
        public MainModelImpl()
        {
            /*
             * 1、概述

                ado.net提供了丰富的数据库操作，这些操作可以分为三个步骤：

                第一，使用SqlConnection对象连接数据库；
                第二，建立SqlCommand对象，负责SQL语句的执行和存储过程的调用；
                第三，对SQL或存储过程执行后返回的“结果”进行操作。
                对返回“结果”的操作可以分为两类：

                一是用SqlDataReader直接一行一行的读取数据集；
                二是DataSet联合SqlDataAdapter来操作数据库。
                两者比较：

                SqlDataReader时刻与远程数据库服务器保持连接，将远程的数据通过“流”的形式单向传输给客户端，它是“只读”的。由于是直接访问数据库，所以效率较高，但使用起来不方便。
                DataSet一次性从数据源获取数据到本地，并在本地建立一个微型数据库（包含表、行、列、规则、表之间的关系等），期间可以断开与服务器的连接，使用SqlDataAdapter对象操作“本地微型数据库”，
                结束后通过SqlDataAdapter一次性更新到远程数据库服务器。这种方式使用起来更方，便简单。但性能较第一种稍微差一点。（在一般的情况下两者的性能可以忽略不计。）


                 http://www.cnblogs.com/youuuu/archive/2011/06/16/2082730.html
             *
             */
            string mysqlcon = "";
            //创建连接
            try
            {
                //对需要输出到bin的文件，右键属性，复制到输出目录 http://blog.csdn.net/dqs78833488/article/details/52684146
                string ip = IPUtil.GetLocalIpv4()[0];

                mysqlcon = "database=tree;Password=root;User ID=root;server=" + ip;
                conn = new MySqlConnection(mysqlcon);
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    LogUtil.Log("-------数据库连接成功-------");
                }

                cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;//使其只执行SQL语句文本形式
                try
                {
                    //启动定时任务
                    iniTimerTask();
                }
                catch
                {
                    LogUtil.Log("定时任务启动异常");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                LogUtil.Log("数据库连接异常 "+ mysqlcon);
            }

        }
        public override long save(Object ob)
        {
            TreeEntity b = ob as TreeEntity;
            if (b == null)
            {
                return -1;
            }
            string sql = "insert into work(ip,orders,type,length,send_time,num,result,data,check_code,check_msg) values('"+b.Ip+"'," + b.Order + "," + b.Type + "," + b.PackageLength + ",'" + (b.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")) + "'," + b.Num + "," + b.Result + ",'" + b.SourceData + "',"+b.CheckCode+",'"+b.CheckMsg+"')";
            LogUtil.Log(sql);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();// ExecuteScalar();
            long id = cmd.LastInsertedId;
            //LogUtil.Log("数据库保存后的ID为:" + id);
            return id;
        }


        private void iniTimerTask()
        {

            System.Timers.Timer t = new System.Timers.Timer(4 * 60 * 60 * 1000); //设置时间间隔为5000=5秒 5*60
            t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_TimesUp);
            t.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            t.Enabled = true; //是否触发Elapsed事件
            t.Start();
        }

        private void Timer_TimesUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                string sql = "select id from work where id = 1";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();// ExecuteScalar();
                LogUtil.Log("执行定时查询数据库任务，防止长时间不操作时连接中断");
            }
            catch(Exception et)
            {
                LogUtil.Log(NLog.LogLevel.Error,"执行定时查询数据库任务时异常|"+et.ToString());
            }
            
        }

        public override void release()
        {
            try
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            catch
            {
            }
        }


    }
}
