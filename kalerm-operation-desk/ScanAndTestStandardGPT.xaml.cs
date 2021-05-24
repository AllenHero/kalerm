using Ivi.Visa;
using kalerm_bll.BaseData;
using kalerm_common;
using kalerm_model;
using kalerm_model.BaseData;
using kalerm_operation_desk.Control;
using kalerm_operation_desk.Port;
using kalerm_operation_desk.Ulitity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        PortOperatorBase portOperatorBase;

        string GPT9000 = "";

        //测试项目
        List<base_wutest> base_wutest = new List<base_wutest>();

        ObservableCollection<ReportBaseModel> BaseModel = new ObservableCollection<ReportBaseModel>();

        ObservableCollection<worksheet> worksheet = new ObservableCollection<worksheet>();

        private BllBaseData bllBaseData = new BllBaseData();

        List<mes_testdata> mes_testdata = new List<mes_testdata>();

        ObservableCollection<base_wu> base_wu = new ObservableCollection<base_wu>();

        string TenantId = ConfigurationManager.AppSettings["TenantId"].ToString();

        public ScanAndTestStandardGPT()
        {
            InitializeComponent();
            this.Loaded += ScanAndTestStandard_Loaded;
            txtScan.KeyUp += TxtScan_KeyUp;
            btnSet.Click += BtnSet_Click;
            btnCom.Click += BtnCom_Click;
            btnClear.Click += BtnClear_Click;
            dataGrid.LoadingRow += DataGrid_LoadingRow;
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
            {

            }
            txtScan.Focus();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var row = e.Row.Item as base_wutest;
            decimal value = 0;
            try
            {
                value = Math.Round(Convert.ToDecimal(row.value), 2);
            }
            catch
            {
                e.Row.Background = new SolidColorBrush(Colors.Yellow);
            }
            if (string.IsNullOrEmpty(row.value))//无测试数据时灰色
            {
                e.Row.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                if (row.minvalue < value && value < row.maxvalue)//有测试数据且测试数据正确时白色
                    e.Row.Background = new SolidColorBrush(Colors.White);
                else//有测试数据且测试数据错误时白色
                    e.Row.Background = new SolidColorBrush(Colors.Red);
            }
        }

        public void Dispose()
        {
            BalanceWeight.ReadEnd();
            Thermometer.ReadEnd();
            if (portOperatorBase != null)
                portOperatorBase.Close();
            txtScan.KeyUp -= TxtScan_KeyUp;
            btnSet.Click -= BtnSet_Click;
            btnCom.Click -= BtnCom_Click;
            btnClear.Click -= BtnClear_Click;
            dataGrid.LoadingRow -= DataGrid_LoadingRow;
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
            string WorkSheetNo = MainWindow.WorkSheetNo + "";
            string WorkUnitId = MainWindow.WorkUnitId + "";
            TotalPass = Convert.ToInt32(MainWindow.TotalPass);
            lbWTCOM.Content = MainWindow.WeightCom + "";
            lbTPCOM.Content = MainWindow.TemperatureCom + "";
            lbTotal.Content = TotalPass + "";
            this.Loaded -= ScanAndTestStandard_Loaded;
            worksheet = new ObservableCollection<worksheet>(bllBaseData.GetWorkSheetList(TenantId));
            foreach (var row in worksheet)
            {
                textWorkSheet.AddItem(new AutoCompleteEntry(row.WorkSheetNo + '|' + row.ProductCode, row.WorkSheetNo + '|' + row.ProductCode));
            }
            base_wu = new ObservableCollection<base_wu>(bllBaseData.GetBaseWuList("", TenantId));
            cbbWorkUnit.ItemsSource = base_wu;
            cbbWorkUnit.SelectedValue = WorkUnitId;
            txtScan.Focus();
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            ScanAndTestStandardSet page = new ScanAndTestStandardSet();
            page.WorkSheetNo = textWorkSheet.Text + "";
            page.WorkUnitId = cbbWorkUnit.SelectedValue + "";
            page.ScanAndTestStandardSetEvent += Page_ScanAndTestStandardSetEvent;
            page.ShowDialog();
            txtScan.Focus();
        }

        private void Page_ScanAndTestStandardSetEvent(object sender, EventArgs e)
        {
            string WorkSheetNo = MainWindow.WorkSheetNo + "";
            string WorkUnitId = MainWindow.WorkUnitId + "";
            textWorkSheet.Text = WorkSheetNo;
            cbbWorkUnit.SelectedValue = WorkUnitId;
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

        //TODO:
        private void TxtScan_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //TODO:
                if (HANDRUN)
                {
                    decimal value = 0;
                    try
                    {
                        value = Math.Round(Convert.ToDecimal(txtScan.Text), 2);
                        lbITEM_VALUE.Content = value + "";
                        if (base_wutest[TestCount].minvalue <= value && value <= base_wutest[TestCount].maxvalue)
                            lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Green);
                        else
                            lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Black);
                        dataGrid.ItemsSource = base_wutest;
                    }
                    catch
                    {

                    }
                }
                if (e.Key == Key.Enter)
                {
                    if ((DateTime.Now - dt).TotalSeconds < 1)
                    {
                        return;
                    }
                    dt = DateTime.Now;
                    string str = (txtScan.Text + "").ToUpper();//转大写
                    txtScan.Text = str;
                    if (txtScan.Text + "" == "N")//扫不良条码
                    {
                        lbITEM_VALUE.Content = "NG";
                        TestError();
                        txtScan.Text = "";
                        return;
                    }
                    if (txtScan.Text + "" == "Y")//扫通过条码
                    {
                        lbITEM_VALUE.Content = "Y";
                        TestOK();
                        txtScan.Text = "";
                        return;
                    }
                    string str1 = textWorkSheet.Text + "";
                    string WorkSheetNo = "";
                    if (str1.Contains('|'))
                    {
                        string[] sArray = str1.Split('|');
                        WorkSheetNo = sArray[0];
                    }
                    //工单
                    lbWorkSheet_NO.Content = WorkSheetNo + "";
                    worksheet worksheet = null;
                    if (!string.IsNullOrEmpty(WorkSheetNo))
                    {
                        worksheet = bllBaseData.GetWorkSheet(WorkSheetNo, TenantId);
                    }    
                    string ProcessId = "";
                    if (worksheet != null)
                    {
                        //指令单
                        lbORDER_NO.Content = worksheet.OrderNo + "";
                        ProcessId = worksheet.ProcessId;
                    }
                    string processname = "";
                    base_productionprocess productionprocess = null;
                    if (!string.IsNullOrEmpty(ProcessId))
                    {
                        productionprocess = bllBaseData.GetProductionProcess(ProcessId, TenantId);
                    }                  
                    if (productionprocess != null)
                    {
                        processname = productionprocess.processname;
                    }
                    //工艺路线
                    lbROUTING_NO.Content = processname + "";
                    lbMessage.Content = "";
                    lbMessage.Foreground = new SolidColorBrush(Colors.Black);
                    if (isSCAN)//扫条码
                    {
                        //条码
                        lbSCAN_BARCODE.Content = txtScan.Text + "";
                        SCAN_BARCODE = txtScan.Text + "";
                        txtScan.IsEnabled = false;
                        if (txtScan.Text + "" == "")
                        {
                            lbMessage.Content = "条码号不能为空";
                            lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                            txtScan.IsEnabled = true;
                            return;
                        }
                        bool isOK = false;//获取测试项目
                        string WuId = Convert.ToString(cbbWorkUnit.SelectedValue);
                        if (string.IsNullOrEmpty(WuId))
                        {
                            lbMessage.Content = "请选择工作单元";
                            lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                            return;
                        }
                        base_wutest = bllBaseData.GetBaseWuTestList(WuId, TenantId, out isOK);
                        int index = 0;
                        foreach (var item in base_wutest)
                        {
                            index = index + 1;
                            item.testno = Convert.ToString(index);
                        }
                        if (!isOK)
                        {
                            lbMessage.Content = "获取测试项目失败，请重新刷卡";
                            lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                            txtScan.IsEnabled = true;
                            return;
                        }
                        txtScan.IsEnabled = true;
                        isSCAN = false;
                        TestCount = 0;
                        lbTest.Content = TestCount;
                        txtScan.Focus();
                        dataGrid.ItemsSource = base_wutest;
                        if (base_wutest.Count == 0)//没有测试项目直接过站
                        {
                            isSCAN = true;
                            string MachineName = Environment.MachineName;
                            lbMessage.Content = "成功过站";
                            lbMessage.Foreground = new SolidColorBrush(Colors.Black);
                            //过站数量+1
                            TotalPass += 1;
                            SetTotalPass(TotalPass);
                        }
                        else//有测试项目，开始测试
                        {
                            lbITEM_NAME.Content = base_wutest[0].testitemname + "";
                            dataGrid.ScrollIntoView(base_wutest[0]);
                            TestStandard(0);//开始测试
                        }
                        txtScan.Text = "";
                    }
                    else//测试项目
                    {
                        if (txtScan.Text == "000")//000测试回退
                        {
                            if (TestCount == 0)//测试没开始，不需要回退
                                return;
                            StopThread();
                            TestCount = TestCount - 1;
                            base_wutest[TestCount].value = "";
                            txtScan.Text = "";
                            TestStandard(TestCount);//开始上一项测试
                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = base_wutest;
                            return;
                        }
                        if (TimeStart)
                        {
                            TimeStartThread();
                            txtScan.Text = "";
                            return;
                        }
                        if (TestCount < base_wutest.Count)//还有测试项目未完成
                        {
                            switch (base_wutest[TestCount].testtype)//当前测试项目
                            {
                                case "HAND":
                                    StopThread();
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    txtScan.Text = "";
                                    break;
                                case "TIME":
                                    StopThread();
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                                case "TP":
                                    StopThread();
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                                case "WT":
                                    StopThread();
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                                case "MEASure1":
                                    StopThread();
                                    MEASure("MEASure1?");
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                                case "MEASure2":
                                    StopThread();
                                    MEASure("MEASure2?");
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                                case "MEASure3":
                                    StopThread();
                                    MEASure("MEASure3?");
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                                case "MEASure11":
                                    StopThread();
                                    MEASure1("MEASure1?");
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                                case "MEASure21":
                                    StopThread();
                                    MEASure1("MEASure2?");
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                                case "MEASure31":
                                    StopThread();
                                    MEASure1("MEASure3?");
                                    base_wutest[TestCount].value = lbITEM_VALUE.Content + "";
                                    break;
                            }
                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = base_wutest;
                            TestCount += 1;
                            lbTest.Content = TestCount;
                            if (TestCount < base_wutest.Count)//还有测试项目未完成
                            {
                                TestStandard(TestCount);//开始下一项测试
                            }
                            else//全部测试完成
                            {
                                StopThread();
                            }
                        }
                        else
                        {
                            TestCount += 1;
                            lbTest.Content = TestCount;
                            StopThread();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                dataGrid.ScrollIntoView(base_wutest[count]);
                //Thread.Sleep(100);//停顿100毫秒，进入下一个测试阶段。
                lbITEM_VALUE.Content = "0";
                lbITEM_NAME.Content = base_wutest[count].testitemname + "";
                switch (base_wutest[count].testtype)
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
                throw new Exception(ex.Message);
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
                        if (TestCount < base_wutest.Count)
                        {
                            if (base_wutest[TestCount].minvalue < Value && Value < base_wutest[TestCount].maxvalue)
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
                        if (TestCount < base_wutest.Count)
                        {
                            if (base_wutest[TestCount].minvalue < value && value < base_wutest[TestCount].maxvalue)
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
                        if (TestCount < base_wutest.Count)
                        {
                            if (base_wutest[TestCount].minvalue < value && value < base_wutest[TestCount].maxvalue)
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
            { 

            }
            lbITEM_VALUE.Content = value + "";
            if (TestCount < base_wutest.Count)
            {
                if (base_wutest[TestCount].minvalue < value && value < base_wutest[TestCount].maxvalue)
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
            { 

            }
            lbITEM_VALUE.Content = value + "";
            if (TestCount < base_wutest.Count)
            {
                if (base_wutest[TestCount].minvalue < value && value < base_wutest[TestCount].maxvalue)
                    lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Green);
                else
                    lbITEM_VALUE.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        //TODO:
        private void Page_ClickEvent(object sender, EventArgs e)
        {
            bool result = Convert.ToBoolean(sender);
            if (result)//测试成功
            {
                //TODO:
                isSCAN = true;
                string MachineName = Environment.MachineName;
                //保存过站
                lbMessage.Content = "成功过站";
                lbMessage.Foreground = new SolidColorBrush(Colors.Black);
            }
            else//测试失败
            {
                ScanAndTestStandardTestError page = new ScanAndTestStandardTestError();
                page.UpdateTestErrorEvent += Page_UpdateTestErrorEvent;
                page.ShowDialog();
                txtScan.Focus();
            }
            //过站数量+1
            TotalPass += 1;
            SetTotalPass(TotalPass);
        }

        private void TestError()//测试失败
        {
            StopThread();//停止数据采集
            SavaAndCheck();//保存测试数据
            ScanAndTestStandardTestError page = new ScanAndTestStandardTestError();
            page.UpdateTestErrorEvent += Page_UpdateTestErrorEvent;
            page.ShowDialog();
            txtScan.Focus();
        }

        //TODO:
        private void TestOK()
        {
            StopThread();
            if (isSCAN)
                return;
            bool isPass = SavaAndCheck();//检测测试数据
            if (isPass)
            {
                //TODO:
                int savecount = bllBaseData.SaveTestData(mes_testdata, 1);
                if (savecount < 1)
                {
                    lbMessage.Content = mes_testdata.Count + "|" + mes_testdata.Count + "|" + savecount + "保存测试数据失败，请重新扫Y保存过站";
                    lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                    return;
                }//测试数据保存
                lbITEM_VALUE.Content = "OK";
                //isTestEnd = false;
                isSCAN = true;
                string MachineName = Environment.MachineName;
                lbMessage.Content = "成功过站";
                lbMessage.Foreground = new SolidColorBrush(Colors.Black);
                //过站数量+1
                TotalPass += 1;
                SetTotalPass(TotalPass);
            }
            else
            {
                lbMessage.Content = "测试数据有NG";
                lbMessage.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private bool SavaAndCheck()
        {
            bool isPass = true;
            mes_testdata = new List<mes_testdata>();
            try
            {
                foreach (var row in base_wutest)
                {
                    mes_testdata item = new mes_testdata();
                    item.Id = Guid.NewGuid().ToString();
                    item.OrderNo = Convert.ToString(lbORDER_NO.Content);//制令单
                    item.WorkSheetNo = Convert.ToString(lbWorkSheet_NO.Content);//工单
                    item.WuId = row.wuid;
                    item.BarCode = SCAN_BARCODE;
                    item.TesItemName = row.testitemname;
                    item.Value = row.value;
                    item.MinValue = row.minvalue;
                    item.MaxValue = row.maxvalue;
                    item.CreateUser = MainWindow.UserInfo.userId;
                    item.TenantId = ConfigurationManager.AppSettings["TenantId"].ToString();
                    if (string.IsNullOrEmpty(row.value))//
                    {
                        row.value = "0";
                    }
                    if (Convert.ToDecimal(row.value) > row.maxvalue || Convert.ToDecimal(row.value) < row.minvalue)
                    {
                        item.IsPass = 0;
                        isPass = false;
                    }
                    else
                    {
                        item.IsPass = 1;
                    }
                    mes_testdata.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("第" + mes_testdata.Count + "条测试数据出错。" + ex.Message);
            }
            return isPass;
        }

        //TODO:
        private void Page_UpdateTestErrorEvent(object sender, EventArgs e)
        {
            //TODO:
            bllBaseData.SaveTestData(mes_testdata, 0);//测试数据保存
            lbMessage.Content = "进入测试站";
            lbMessage.Foreground = new SolidColorBrush(Colors.Black);
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

        private void textWorkSheet_MouseLeave(object sender, MouseEventArgs e)
        {
            //根据工单获取工作单元
            string str1 = textWorkSheet.Text + "";
            string ProductCode = "";
            if (str1.Contains('|'))
            {
                string[] sArray = str1.Split('|');
                ProductCode = sArray[1];
            }
            base_wu = new ObservableCollection<base_wu>(bllBaseData.GetBaseWuList(ProductCode, TenantId));
            cbbWorkUnit.ItemsSource = base_wu;
        }
    }
}
