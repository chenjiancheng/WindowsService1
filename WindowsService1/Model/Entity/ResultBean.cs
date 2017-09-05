namespace WindowsService1.Utils
{
    public class ResultBean
    {
        private bool result;
        private int type;
        private int code;
        private string msg = "";
        private int value1;
        private int value2;
        private object obj;

        public bool Result { get => result; set => result = value; }
        public int Type { get => type; set => type = value; }
        public int Code { get => code; set => code = value; }
        public string Msg { get => msg; set => msg = value; }
        public int Value1 { get => value1; set => value1 = value; }
        public int Value2 { get => value2; set => value2 = value; }
        public object Obj { get => obj; set => obj = value; }
    }
}