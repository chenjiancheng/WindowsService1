using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1.Utils
{
    class DataUtil
    {
        private readonly static byte[] startBit = { 0x5A, 0x4B, 0x43, 0x54 };
        private readonly static byte[] endBit = { 0x0D, 0x0A };
        /**
         *5A 4B 43 54 01 00 09 0C 00 00 32 86 56 78 56 34 12 D1 00 00 00 15 22 0D 0A
         * 
         *  5A 4B 43 54
            01 00 序列号，小端
            09 
            0C 00 数据包长度，小端
            00 32 86 56 时间戳，小端
            78 56 34 12 设备编号，小端
            D1 00 00 00 树径（毫米) 小端
            15 22 CRC
            0D 0A
         *
         **/

        public static TreeEntity ParseData(byte[] data, int revLen, string ip)
        {
            /*
            byte[] byteOrder = Reverse(data, 4, 2);
            byte[] byteLength = Reverse(data, 7, 2);
            byte[] byteTime = Reverse(data, 9, 4);
            byte[] byteNum = Reverse(data, 13, 4);
            byte[] byteValue = Reverse(data, 17, 4);
            */
            byte[] byteStart = Copy(data, 0, 4);
            byte[] byteOrder = Copy(data, 4, 2);
            byte[] byteLength = Copy(data, 7, 2);
            byte[] byteTime = Copy(data, 9, 4);
            byte[] byteNum = Copy(data, 13, 4);
            byte[] byteValue = Copy(data, 17, 4);
            byte[] byteData = Copy(data, 9, 12);
            byte[] byteCrc = Copy(data, 21, 2);
            byte[] byteEnd = Copy(data, 23, 2);

            TreeEntity bean = new TreeEntity();
            bean.SourceData = ByteDataToStringSplit(data, 0, revLen);//整个数据

            bean.StartBit = byteStart;

            bean.Order = BitConverter.ToInt16(byteOrder, 0);// 序列号，小端  Convert.ToInt32(byteOrder) BitConverter
            bean.Orders = byteOrder;
            bean.Type = data[6];//09
            bean.PackageLength = BitConverter.ToInt16(byteLength, 0);//数据包长度，小端
            long t = BitConverter.ToInt32(byteTime, 0);
            Console.WriteLine("时间戳"+t);
            bean.Timestamp = TimeUtil.ConvertLongToDate(t);//时间戳，小端
            
            bean.Num = BitConverter.ToInt32(byteNum, 0);//设备编号，小端
            bean.Result = BitConverter.ToInt32(byteValue, 0);//树径（毫米) 小端

            bean.Crc = byteCrc;
            bean.EndBit = byteEnd;

            bean.Data = byteData;
            bean.Ip = ip;

            return bean;
        }
        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        public static ResultBean CheckEntity(TreeEntity bean)
        {
            ResultBean result = new ResultBean();

            for (int i = 0; i < 4; i++)
            {
                if (bean.StartBit[i] != startBit[i])
                {
                    result.Code++;
                    result.Msg += "起始位" + i + "不正确|";
                }
            }

            ushort crc = CRCUtil.CalculateCrc(bean.Data, bean.Data.Length);
            //src[6] = (byte)(crc & 0xff);
            //src[7] = (byte) (crc >> 8);
            byte crcL = ((byte)(crc & 0xff));
            byte crcH = ((byte)(crc >> 8));
            if (bean.Crc[0] != crcL || bean.Crc[1] != crcH)
            {
                result.Code++;
                result.Msg += "接收的CRC为" + bean.Crc[0] + "" + bean.Crc[1] + "计算的CRC为" + crcL + "" + crcH + " 对比不正确|";
            }
            result.Result = result.Code == 0;
            if (result.Result)
            {
                result.Msg = "正确";
            }
            result.Obj = bean;
            bean.CheckCode = result.Code;
            bean.CheckMsg = result.Msg;

            return result;
        }
        /// <summary>
        /// 回复
        /// </summary>
        /// <param name="bean"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static byte[] Respond(TreeEntity bean, int order)
        {
            int length = 17;
            byte[] respond = new byte[length];
            for (int i = 0; i < 4; i++)//起始位和结尾
            {
                respond[i] = startBit[i];
                if (i < 2)
                {
                    respond[length - i - 1] = endBit[1 - i];
                }
            }
            respond[4] = (byte)(order & 0xFF);
            respond[5] = (byte)(order >> 8 & 0xFF);//序列号

            respond[6] = 0x09;//数据类型

            respond[8] = 0x00;
            respond[7] = 0x04;//数据包长度

            respond[9] = respond[4];
            respond[10] = respond[5];//数据包，CJC  =序列号

            ushort crc = CRCUtil.CalculateCrc(Copy(respond, 9, 4), 4);
            //src[6] = (byte)(crc & 0xff);
            //src[7] = (byte) (crc >> 8);
            byte crcL = ((byte)(crc & 0xff));
            byte crcH = ((byte)(crc >> 8));

            respond[13] = crcL;
             respond[14] = crcH;//CRC

            return respond;
        }
        /// <summary>
        /// 反转数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Reverse(byte[] data, int index, int length)
        {
            byte[] b = new byte[length];
            byte temp;
            int k = 0;
            for (int i = index; i < data.Length && i < index + length && k < length / 2; i++)
            {
                k++;
                temp = data[i];
                data[i] = data[index + length - k];
                data[index + length - k] = temp;
            }
            int j = 0;
            for (int i = index; i < data.Length && i < index + length; i++)
            {
                b[j++] = data[i];
            }
            return b;
        }
        /// <summary>
        /// 复制数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Copy(byte[] data, int index, int length)
        {
            byte[] b = new byte[length];

            int k = 0;
            for (int i = index; i < data.Length && i < index + length; i++)
            {
                b[k++] = data[i];
            }

            return b;
        }
        public static string ByteDataToStringSplit(byte[] data)
        {
            return ByteDataToStringSplit(data, ",");
        }

        public static string ByteDataToStringSplit(byte[] data, string split)
        {
            return ByteDataToStringSplit(data, split, 0, data.Length);
        }
        public static string ByteDataToStringSplit(byte[] data, int index, int length)
        {
            return ByteDataToStringSplit(data, ",", index, length);
        }
        public static string ByteDataToStringSplit(byte[] data, string split, int index, int length)
        {
            StringBuilder builder = new StringBuilder();
            if (length > data.Length)
            {
                length = data.Length;
            }
            for (int i = index; i < length; i++)
            {
                builder.Append(data[i]);
                if (i != length - 1)
                {
                    builder.Append(split);
                }
            }
            return builder.ToString();
        }


    }
}
