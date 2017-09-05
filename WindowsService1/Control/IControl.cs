using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsService1.Model;

namespace WindowsService1
{
    public abstract class IControl
    {
        /// <summary>
        /// 
        /// </summary>
        public IModel model;
        /// <summary>
        /// 接收远程数据
        /// </summary>
        /// <param name="obj"></param>
        public abstract void receiveData(Object obj);
        /// <summary>
        /// 重新连接数据库
        /// </summary>
        public abstract void reInitMode();
    }
}
