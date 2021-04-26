using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kalerm_common
{
    public class BalanceWeight
    {
        private SerialPort ComDevice = new SerialPort();
        public decimal CurWeight = 0;//电子秤当前重量
        Thread thread;
        bool threadRun = true;
        string content = "";

        public BalanceWeight(string Com)
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
                thread = new Thread(new ThreadStart(Read));
                thread.IsBackground = true;
                thread.Start();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public void ReadWeight()
        {
            threadRun = true;
            CurWeight = 0;
            try
            {
                threadRun = true;
                //thread = new Thread(new ThreadStart(Read));
                //thread.IsBackground = true;
                //thread.Start();



            }
            catch { }
        }

        private void Read()
        {
            Thread.Sleep(200);
            while (threadRun)
            {
                try
                {
                    if (!ComDevice.IsOpen)
                        ComDevice.Open();
                    byte[] data = new byte[1];
                    data[0] = 0x52;
                    if (ComDevice.IsOpen)
                        ComDevice.Write(data, 0, data.Length);
                }
                catch
                { }
                Thread.Sleep(400);
            }
        }



        public void ReadEnd()
        {
            CurWeight = 0;
            threadRun = false;
            try
            {
                ComDevice.Close();
            }
            catch { }
        }


        private void ComDevice_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(400);
            try
            {
                if (!ComDevice.IsOpen)
                    ComDevice.Open();

                byte[] ReDatas = new byte[ComDevice.BytesToRead];
                ComDevice.Read(ReDatas, 0, ReDatas.Length);//读取数据
                string str = new ASCIIEncoding().GetString(ReDatas);
                content += str;
                if (content.Contains("-"))
                    content = "";

                if (content.Contains("+") && content.Contains("g"))
                {
                    int first = content.IndexOf("+");
                    int last = content.IndexOf("g");
                    if (last > first)
                    {
                        content = content.Substring(first, last - first);
                        CurWeight = Convert.ToDecimal(content.Substring(1, content.Length - 1));
                        content = "";
                    }
                    else
                    {
                        content = content.Substring(first, content.Length - first);
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}
