using Ivi.Visa;
using kalerm_common;
using kalerm_common.Ulitity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace kalerm_operation_desk
{
    /// <summary>
    /// ScanAndTestStandardGPT.xaml 的交互逻辑
    /// </summary>
    public partial class ScanAndTestStandardGPT : Page, IDisposable
    {
        int Color = 0;
        Thread thread;
        //bool isTestEnd = false;
        //主条码
        string SCAN_BARCODE = "";
        //是否扫码
        bool isSCAN = true;
        //当前测试项目
        int TestCount = 0;
        //HAND测试
        bool HANDRUN = false;
        //测试开始时间
        DateTime StartTime = DateTime.Now;
        //时间测试
        bool threadTimeRun = false;
        bool TimeStart = false;
        //电子秤
        BalanceWeight BalanceWeight = new BalanceWeight(MainWindow.WeightCom);
        //重量测试
        bool threadWeightRun = false;
        //温度仪
        Thermometer Thermometer = new Thermometer(MainWindow.TemperatureCom);
        //重量测试
        bool threadTempRun = false;
        //总测试数
        int TotalPass = 0;
        DateTime dt = new DateTime();
        string CheckSCAN_BARCODE = "";
        PortOperatorBase portOperatorBase;
        string GPT9000 = "";

        public ScanAndTestStandardGPT()
        {
            InitializeComponent();
            this.Loaded += ScanAndTestStandard_Loaded;
            btnSet.Click += BtnSet_Click;
            btnCom.Click += BtnCom_Click;
            //btnError.Click += BtnError_Click;
            btnClear.Click += BtnClear_Click;
            btnAG.Click += BtnAG_Click;
        }

        //安规测试启动
        private void BtnAG_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!string.IsNullOrEmpty(GPT9000))
                {

                }
            }
            catch
            { }
            txtScan.Focus();
        }

        public void Dispose()
        {

            BalanceWeight.ReadEnd();
            Thermometer.ReadEnd();
            if (portOperatorBase != null)
                portOperatorBase.Close();
            btnSet.Click -= BtnSet_Click;
            btnCom.Click -= BtnCom_Click;
            //btnError.Click -= BtnError_Click;
            btnClear.Click -= BtnClear_Click;
            System.GC.Collect();
        }

        private void ScanAndTestStandard_Loaded(object sender, RoutedEventArgs e)
        {

            string[] content1 = PortUltility.FindAddresses(PortType.RS232);
            string[] content2 = PortUltility.FindRS232Type(content1);
            for (int i = 0; i < content2.Count(); i++)
            {
                if (content2[i] + "" == "COM8")
                {
                    GPT9000 = content1[i];
                }
                if (!string.IsNullOrEmpty(GPT9000))
                {
                    //lbAG.Content = "串口存在";
                    //打开安规测试仪
                    //portOperatorBase = new RS232PortOperator(GPT9000, 9600, SerialParity.None, SerialStopBitsMode.One, 8);
                    //portOperatorBase.Open();
                }
            }

            dt = DateTime.Now;
            if (BalanceWeight.Open())
                lbWT.Content = "打开串口成功";
            else
                lbWT.Content = "打开串口失败";

            if (Thermometer.Open())
                lbTP.Content = "打开串口成功";
            else
                lbTP.Content = "打开串口失败";

            string LineNO = MainWindow.LineNO + "";
            string PROCESS_NO = MainWindow.PROCESS_NO + "";
            TotalPass = Convert.ToInt32(MainWindow.TotalPass);
            lbWTCOM.Content = MainWindow.WeightCom + "";
            lbTPCOM.Content = MainWindow.TemperatureCom + "";
            lbTotal.Content = TotalPass + "";

            this.Loaded -= ScanAndTestStandard_Loaded;
            cbbLineNo.SelectedValue = LineNO;
            cbbPROCESS.SelectedValue = PROCESS_NO;

            txtScan.Focus();
        }


        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            ScanAndTestStandardSet page = new ScanAndTestStandardSet();
            page.LineNO = cbbLineNo.SelectedValue + "";
            page.PROCESS_NO = cbbPROCESS.SelectedValue + "";
            page.ScanAndTestStandardSetEvent += Page_ScanAndTestStandardSetEvent;
            page.ShowDialog();
            txtScan.Focus();
        }
        private void Page_ScanAndTestStandardSetEvent(object sender, EventArgs e)
        {
            string LineNO = MainWindow.LineNO + "";
            string PROCESS_NO = MainWindow.PROCESS_NO + "";
            cbbLineNo.SelectedValue = LineNO;
            cbbPROCESS.SelectedValue = PROCESS_NO;
        }


        private void BtnCom_Click(object sender, RoutedEventArgs e)
        {
            ScanAndTestStandardCom page = new ScanAndTestStandardCom();
            page.WeightCom = lbWTCOM.Content + "";
            page.TemperatureCom = lbTPCOM.Content + "";
            page.ScanAndTestStandardComEvent += Page_ScanAndTestStandardComEvent;
            page.ShowDialog();
            txtScan.Focus();
        }

        private void Page_ScanAndTestStandardComEvent(object sender, EventArgs e)
        {
            lbWTCOM.Content = MainWindow.WeightCom + "";
            lbTPCOM.Content = MainWindow.TemperatureCom + "";
            BalanceWeight = new BalanceWeight(MainWindow.WeightCom + "");
            Thermometer = new Thermometer(MainWindow.TemperatureCom + "");
            if (BalanceWeight.Open())
                lbWT.Content = "打开串口成功";
            else
                lbWT.Content = "打开串口失败";

            if (Thermometer.Open())
                lbTP.Content = "打开串口成功";
            else
                lbTP.Content = "打开串口失败";
        }

        private void StopThread()
        {
            HANDRUN = false;
            threadTimeRun = false;
            TimeStart = false;
            threadTempRun = false;
            threadWeightRun = false;
        }

        //开始测试
        private void TestStandard(int count)
        {
            try
            {
                dataGrid.ScrollIntoView(T_MES_TEST_STANDARD_ORDER[count]);
                //Thread.Sleep(100);//停顿100毫秒，进入下一个测试阶段。
                lbITEM_VALUE.Content = "0";
                lbITEM_NAME.Content = T_MES_TEST_STANDARD_ORDER[count].ITEM_NAME + "";
                switch (T_MES_TEST_STANDARD_ORDER[count].ITEM_TYPE)
                {
                    case "HAND":
                        HANDRUN = true;
                        break;
                    case "TIME":
                        TimeStart = true;
                        //thread = new Thread(new ThreadStart(Time));
                        //thread.IsBackground = true;
                        //thread.Start();
                        break;
                    case "TP":
                        thread = new Thread(new ThreadStart(Temp));
                        thread.IsBackground = true;
                        thread.Start();
                        break;
                    case "WT":
                        thread = new Thread(new ThreadStart(Weight));
                        thread.IsBackground = true;
                        thread.Start();
                        break;
                    case "MEASure1":
                        //MEASure("MEASure1?");
                        break;
                    case "MEASure2":
                        //MEASure("MEASure2?");
                        //thread = new Thread(new ThreadStart(Weight));
                        //thread.IsBackground = true;
                        //thread.Start();
                        break;
                    case "MEASure3":
                        //MEASure("MEASure3?");
                        //thread = new Thread(new ThreadStart(Weight));
                        //thread.IsBackground = true;
                        //thread.Start();
                        break;
                    case "MEASure11":
                        //MEASure("MEASure3?");
                        //thread = new Thread(new ThreadStart(Weight));
                        //thread.IsBackground = true;
                        //thread.Start();
                        break;
                    case "MEASure21":
                        //MEASure("MEASure3?");
                        //thread = new Thread(new ThreadStart(Weight));
                        //thread.IsBackground = true;
                        //thread.Start();
                        break;
                    case "MEASure31":
                        //MEASure("MEASure3?");
                        //thread = new Thread(new ThreadStart(Weight));
                        //thread.IsBackground = true;
                        //thread.Start();
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void TimeStartThread()
        {
            TimeStart = false;
            thread = new Thread(new ThreadStart(Time));
            thread.IsBackground = true;
            thread.Start();
        }

        //测试时间项目
        private void Time()
        {
            StartTime = DateTime.Now;
            threadTimeRun = true;

            while (threadTimeRun)
            {
                DateTime dt = DateTime.Now;
                decimal Value = Convert.ToDecimal((dt - StartTime).TotalSeconds);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        lbITEM_VALUE.Content = Math.Round(Value, 1) + "";
                        if (TestCount < T_MES_TEST_STANDARD_ORDER.Count)
                        {
                            if (T_MES_TEST_STANDARD_ORDER[TestCount].MIN_VALUE < Value && Value < T_MES_TEST_STANDARD_ORDER[TestCount].MAX_VALUE)
                                lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Green);
                            else
                                lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                    catch
                    {

                    }
                }));
                Thread.Sleep(100);
            }
        }

        //测试重量项目
        private void Weight()
        {
            Thread.Sleep(200);//停顿0.2秒再开始
            threadWeightRun = true;
            BalanceWeight.ReadWeight();
            while (threadWeightRun)
            {

                decimal value = Math.Round(BalanceWeight.CurWeight, 1);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        lbITEM_VALUE.Content = value + "";
                        if (TestCount < T_MES_TEST_STANDARD_ORDER.Count)
                        {
                            if (T_MES_TEST_STANDARD_ORDER[TestCount].MIN_VALUE < value && value < T_MES_TEST_STANDARD_ORDER[TestCount].MAX_VALUE)
                                lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Green);
                            else
                                lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                    catch
                    {

                    }
                }));
                Thread.Sleep(100);

            }
        }

        private void Temp()
        {
            Thread.Sleep(100);//停顿0.1秒再开始
            threadTempRun = true;
            while (threadTempRun)
            {
                Thermometer.ReOpen();
                decimal value = Math.Round(Thermometer.CurTemp, 1);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        lbITEM_VALUE.Content = value + "";
                        if (TestCount < T_MES_TEST_STANDARD_ORDER.Count)
                        {
                            if (T_MES_TEST_STANDARD_ORDER[TestCount].MIN_VALUE < value && value < T_MES_TEST_STANDARD_ORDER[TestCount].MAX_VALUE)
                                lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Green);
                            else
                                lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                    catch
                    { }
                }));
                Thread.Sleep(100);

            }
        }

        private void MEASure1(string str)
        {
            //获取耐压接口
            decimal value = 0;
            try
            {
                if (!string.IsNullOrEmpty(GPT9000))
                {
                    portOperatorBase = new RS232PortOperator(GPT9000, 9600, SerialParity.None, SerialStopBitsMode.One, 8);
                    portOperatorBase.Open();
                    string content = str;
                    portOperatorBase.WriteLine(content);
                    string result = portOperatorBase.ReadLine();

                    string[] strdata = result.Split(',');

                    if (strdata.Count() > 3)
                    {
                        string strdata3 = strdata[3].Substring(0, 5);
                        if (!strdata3.Contains('.'))
                            strdata3 = strdata3.Substring(0, 4);
                        if (decimal.TryParse(strdata3, out value))
                        {
                            value = Math.Round(value, 2);
                        }
                        else
                            value = 0;
                    }
                    else
                        value = 0;
                    portOperatorBase.Close();
                }
            }
            catch
            { }
            lbITEM_VALUE.Content = value + "";
            if (TestCount < T_MES_TEST_STANDARD_ORDER.Count)
            {
                if (T_MES_TEST_STANDARD_ORDER[TestCount].MIN_VALUE < value && value < T_MES_TEST_STANDARD_ORDER[TestCount].MAX_VALUE)
                    lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Green);
                else
                    lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void MEASure(string str)
        {
            //获取耐压接口
            decimal value = 0;
            try
            {

                if (!string.IsNullOrEmpty(GPT9000))
                {
                    //打开安规仪器
                    portOperatorBase = new RS232PortOperator(GPT9000, 9600, SerialParity.None, SerialStopBitsMode.One, 8);
                    portOperatorBase.Open();


                    string content = str;
                    portOperatorBase.WriteLine(content);
                    string result = portOperatorBase.ReadLine();

                    string[] strdata = result.Split(',');

                    if (strdata.Count() > 3)
                    {
                        if (decimal.TryParse(strdata[2].Substring(0, 5), out value))
                        {
                            if (strdata[2].Contains("kV"))
                                value = value * 1000;
                            value = Math.Round(value, 2);
                        }
                        else
                            value = 0;
                    }
                    else
                        value = 0;
                    portOperatorBase.Close();
                }
            }
            catch
            { }
            lbITEM_VALUE.Content = value + "";
            if (TestCount < T_MES_TEST_STANDARD_ORDER.Count)
            {
                if (T_MES_TEST_STANDARD_ORDER[TestCount].MIN_VALUE < value && value < T_MES_TEST_STANDARD_ORDER[TestCount].MAX_VALUE)
                    lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Green);
                else
                    lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void TestError()//测试失败
        {
            StopThread();//停止数据采集
            ScanAndTestStandardTestError page = new ScanAndTestStandardTestError();
            page.ShowDialog();
            txtScan.Focus();
        }

        //清除
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            SetTotalPass(0);
        }

        private void SetTotalPass(int TotalPass)
        {
            GridChange();
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["TotalPass"].Value = TotalPass + "";
            MainWindow.TotalPass = TotalPass + "";
            cfa.Save();
            lbTotal.Content = TotalPass + "";
        }

        private void GridChange()
        {
            if (Color == 0)
            {
                Color = 1;
                grid.Background = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                Color = 0;
                grid.Background = new SolidColorBrush(Colors.White);
            }
        }

    }
}
