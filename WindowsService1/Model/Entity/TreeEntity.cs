using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    class TreeEntity
    {
        /**
          *     10.树径测量数据包
        说明
        设备每隔12小时向服务器发送一次测量结果数据包；
        设备在5秒内收到服务器返回包则认为发送成功,否则认为发送失败；
        如果树径测量数据包发送失败，设备会继续向服务器发送该包，直至成功为止。
        设备发送测量结果包格式
        格式	长度
        (Byte)	说明
        
        起始位	4	固定值，统一为0x5A0x4B0x430x54
        序列号	2	从开机后，每次发送数据序列号都自动加 1
        数据包类型	1	固定值，0x09
        数据包长度	2	代表传输的数据包长度，uint16_t型，小端模式
        数据包	
            时间戳	4	从格林威治时间1970年1月1日0时0分0秒起至本次测量开始的总秒数，uint32_t型 ，小端模式
            编号	4	设备或目标编号，uint32_t型 ，小端模式(cjc-数据的低位放低地址)
            树径	4	本次测量的树径（单位：毫米），uint32_t型 ，小端模式
        
        检验和	2	数据包的CRC16校验值，接收方若收到的信息计算有CRC错误，则忽略该包（算法详见附表 1）
        停止位	2	固定值，统一为 0x0D 0x0A


        服务器回复测量包格式

        格式	长度
        (Byte)	说明
        起始位	4	固定值，统一为0x5A0x4B0x430x54
        序列号	2	从开机后，每次发送数据序列号都自动加 1
        数据包类型	1	固定值，0x09
        数据包长度	2	代表传输的数据包长度，uint16_t型，小端模式
        数据包	保留字节	4	保留字节，无需解析
        检验和	2	数据包的CRC16校验值，接收方若收到的信息计算有CRC错误，则忽略该包（算法详见附表 1）
        停止位	2	固定值，统一为 0x0D 0x0A
      * **/


        private byte[] startBit;
        private int order;
        private byte[] orders;
        private int type;
        private int packageLength;
        private DateTime timestamp;
        private int num;
        private int result;
        private byte[] crc;
        private byte[] endBit;
        private byte[] data;
        private string sourceData;//整个数据


        private int checkCode;
        private string checkMsg;
        private string ip;

        public byte[] StartBit { get => startBit; set => startBit = value; }
        public int Order { get => order; set => order = value; }
        public int Type { get => type; set => type = value; }
        public int PackageLength { get => packageLength; set => packageLength = value; }
        
       
        
        public byte[] Crc { get => crc; set => crc = value; }
        public byte[] EndBit { get => endBit; set => endBit = value; }
        
        
        public byte[] Data { get => data; set => data = value; }
        public string SourceData { get => sourceData; set => sourceData = value; }
        public byte[] Orders { get => orders; set => orders = value; }
        
        public int CheckCode { get => checkCode; set => checkCode = value; }
        public string CheckMsg { get => checkMsg; set => checkMsg = value; }
        
        
        public int Num { get => num; set => num = value; }
        public int Result { get => result; set => result = value; }
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        public string Ip { get => ip; set => ip = value; }
    }
}
