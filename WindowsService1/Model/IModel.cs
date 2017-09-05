using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1.Model
{
    public abstract class IModel
    {
        /// <summary>
        /// 数据库连接文件路径
        /// </summary>
        //public static string db_path = "";
        public abstract long save(Object obj);
        /// <summary>
        /// 释放数据库资源
        /// </summary>
        public abstract void release();
    }
}
