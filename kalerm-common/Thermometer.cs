using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_common
{
    public class Thermometer
    {
        private SerialPort ComDevice = new SerialPort();
        public decimal CurTemp = 0;//温度仪当前温度
        string content = "";
        public Thermometer(string Com)
        {
            ComDevice.PortName = Com;
            ComDevice.BaudRate = 9600;
            ComDevice.Parity = Parity.None;
            ComDevice.DataBits = 8;
            ComDevice.StopBits = StopBits.One;
            ComDevice.DataReceived += ComDevice_DataReceived;
        }

        public bool Open()
        {
            bool result = true;
            try
            {
                ComDevice.Open();
                byte[] data = new byte[7];
                data[0] = 0x3C;
                data[1] = 0x03;
                data[2] = 0x01;
                data[3] = 0x66;
                data[4] = 0x01;
                data[5] = 0x59;
                data[6] = 0x0D;
                ComDevice.Write(data, 0, data.Length);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool ReOpen()
        {
            bool result = true;
            try
            {
                if (!ComDevice.IsOpen)
                    ComDevice.Open();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public void ReadTemp()
        {
            //CurTemp = 0;
            //try
            //{
            //    if (!ComDevice.IsOpen)
            //        ComDevice.Open();
            //    byte[] data = new byte[7];
            //    data[0] = 0x3C;
            //    data[1] = 0x03;
            //    data[2] = 0x01;
            //    data[3] = 0x66;
            //    data[4] = 0x01;
            //    data[5] = 0x59;
            //    data[6] = 0x0D;
            //    if (ComDevice.IsOpen)
            //        ComDevice.Write(data, 0, data.Length);
            //}
            //catch { }
        }

        public void ReadEnd()
        {
            CurTemp = 0;
            try
            {
                byte[] data = new byte[7];
                data[0] = 0x3C;
                data[1] = 0x03;
                data[2] = 0x01;
                data[3] = 0x66;
                data[4] = 0x00;
                data[5] = 0x58;
                data[6] = 0x0D;
                if (ComDevice.IsOpen)
                    ComDevice.Write(data, 0, data.Length);
                ComDevice.Close();
            }
            catch { }
        }

        private void ComDevice_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!ComDevice.IsOpen)
                    ComDevice.Open();
                byte[] ReDatas = new byte[ComDevice.BytesToRead];
                ComDevice.Read(ReDatas, 0, ReDatas.Length);//读取数据
                string str = new ASCIIEncoding().GetString(ReDatas);
                content += str;
                if (content.Contains("??"))
                {
                    string strReplaced = content.Replace("??", "");
                    int Count = (content.Length - strReplaced.Length) / 2;
                    if (Count > 1)
                    {
                        int first = content.IndexOf("??");
                        content = content.Substring(first + 2, content.Length - first - 2);
                        int second = content.IndexOf("??");
                        content = content = content.Substring(0, second);
                        CurTemp = Convert.ToDecimal(content);
                        content = "";
                    }
                }
            }
            catch
            {

            }
        }
    }
}
