using kalerm_bll.BaseData;
using kalerm_common;
using kalerm_model;
using kalerm_operation_desk.Control;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Dynamic;
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
    /// GrindBeanStandard.xaml 的交互逻辑
    /// </summary>
    public partial class GrindBeanStandard : Page
    {
        DateTime dt = new DateTime();

        //总测试数
        int TotalPass = 0;

        ObservableCollection<worksheet> worksheet = new ObservableCollection<worksheet>();

        //电子秤
        BalanceWeight BalanceWeight = new BalanceWeight(MainWindow.WeightCom);

        private BllBaseData bllBaseData = new BllBaseData();

        ObservableCollection<base_wu> base_wu = new ObservableCollection<base_wu>();

        List<dynamic> typeList = new List<dynamic>();

        //当前测试项目
        int TestCount = 0;

        //是否扫码
        bool isSCAN = true;

        //HAND测试
        bool HANDRUN = false;

        //主条码
        string SCAN_BARCODE = "";

        //组件编码
        string ComponentNumber = "";

        List<mes_grindbeandata> mes_grindbeandata = new List<mes_grindbeandata>();

        List<mes_grindbeandata> grindbeandataList = new List<mes_grindbeandata>();

        string TenantId = ConfigurationManager.AppSettings["TenantId"].ToString();

        //重量测试
        bool threadWeightRun = false;

        Thread thread;

        public GrindBeanStandard()
        {
            InitializeComponent();
            this.Loaded += GrindBeanStandard_Loaded;
            txtScan.KeyUp += TxtScan_KeyUp;
            btnSet.Click += BtnSet_Click;
            btnCom.Click += BtnCom_Click;
            btnClear.Click += BtnClear_Click;
        }

        private void GrindBeanStandard_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dt = DateTime.Now;
                if (BalanceWeight.Open())
                    lbWT.Content = "打开串口成功";
                else
                    lbWT.Content = "打开串口失败";
                string WorkSheetNo = MainWindow.WorkSheetNo + "";
                string WorkUnitId = MainWindow.WorkUnitId + "";
                TotalPass = Convert.ToInt32(MainWindow.TotalPass);
                lbWTCOM.Content = MainWindow.WeightCom + "";
                lbTotal.Content = TotalPass + "";
                this.Loaded -= GrindBeanStandard_Loaded;
                worksheet = new ObservableCollection<worksheet>(bllBaseData.GetWorkSheetList(TenantId));
                foreach (var row in worksheet)
                {
                    textWorkSheet.AddItem(new AutoCompleteEntry(row.WorkSheetNo, row.WorkSheetNo));
                }
                base_wu = new ObservableCollection<base_wu>(bllBaseData.GetBaseWuList(TenantId));
                cbbWorkUnit.ItemsSource = base_wu;
                cbbWorkUnit.SelectedValue = WorkUnitId;
                textWorkSheet.Text = WorkSheetNo;
                txtScan.Focus();

                txtSWZ_071.Text = ConfigurationManager.AppSettings["txtSWZ_071"].ToString();

                txtSWZ_03.Text = ConfigurationManager.AppSettings["txtSWZ_03"].ToString();

                //定义
                string[] arr = new string[] { "txtKMGL", "txtGL", "txtDW", "txtFirst", "txtSecond", "txtThird", "txtCSHZL_071", "txtCSHZL_03" };
                string[] arr1 = new string[] { "空磨功率", "功率<=135w", "档位", "粉重第一杯", "粉重第二杯", "粉重第三杯", "0.71测试后", "0.3测试后" };
                string[] arr2 = new string[] { "HAND", "HAND", "HAND", "WT", "WT", "WT", "WT", "WT" };
                for (int i = 0; i < arr.Length; i++)
                {
                    dynamic obj = new ExpandoObject();
                    obj.name = arr[i];
                    obj.text = arr1[i];
                    obj.type = arr2[i];
                    typeList.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, ex.Message);
            }           
        }

        private void textWorkSheet_MouseLeave(object sender, MouseEventArgs e)
        {
            //根据工单获取工作单元
            string WorkSheetNo = textWorkSheet.Text + "";
            string ProductCode = "";
            worksheet worksheet = null;
            if (!string.IsNullOrEmpty(WorkSheetNo))
            {
                worksheet = bllBaseData.GetWorkSheet(WorkSheetNo, TenantId);
            }
            if (worksheet != null)
            {
                ProductCode = worksheet.ProductCode;
                base_wu = new ObservableCollection<base_wu>(bllBaseData.GetBaseWuList(TenantId));
            }
            cbbWorkUnit.ItemsSource = base_wu;
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            ScanAndTestStandardTodayTask page = new ScanAndTestStandardTodayTask();
            page.ScanAndTestStandardTodayTaskEvent += new EventHandler(Page_ScanAndTestStandardTodayTaskEvent);
            page.Show();
        }

        private void Page_ScanAndTestStandardTodayTaskEvent(object sender, EventArgs e)
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
            page.ScanAndTestStandardComEvent += Page_ScanAndTestStandardComEvent;
            page.ShowDialog();
            txtScan.Focus();
        }

        private void Page_ScanAndTestStandardComEvent(object sender, EventArgs e)
        {
            lbWTCOM.Content = MainWindow.WeightCom + "";
            BalanceWeight = new BalanceWeight(MainWindow.WeightCom + "");
            if (BalanceWeight.Open())
                lbWT.Content = "打开串口成功";
            else
                lbWT.Content = "打开串口失败";
        }

        private void TxtScan_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (HANDRUN)
                {
                    //TODO:
                }
                //Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,txtScan.Text);
                if (e.Key == Key.Enter)
                {
                    if ((DateTime.Now - dt).TotalSeconds < 1)
                    {
                        return;
                    }
                    dt = DateTime.Now;
                    string str = (txtScan.Text + "").ToUpper();//转大写
                    txtScan.Text = str;
                    if (txtScan.Text + "" == "Y")//完成
                    {
                        SavaData();
                        txtScan.Text = "";
                        return;
                    }
                    string WorkSheetNo = textWorkSheet.Text + "";
                    //工单
                    worksheet worksheet = null;
                    //产品编号
                    string ProductCode = "";
                    lbWorkSheet_NO.Content = WorkSheetNo + "";
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
                        //ERP工单
                        lbERPORDER_NO.Content = worksheet.ERPOrderNo + "";
                        ProductCode = worksheet.ProductCode;
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
                        //校验条码
                        var dataInfo = ApiDataSource.GetCheckProcessScan(WorkSheetNo, ProductCode, Convert.ToString(cbbWorkUnit.SelectedValue), txtScan.Text);
                        if (dataInfo.status == false)
                        {
                            lbMessage.Content = dataInfo.message;
                            lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                            txtScan.IsEnabled = true;
                            return;
                        }
                        string WuId = Convert.ToString(cbbWorkUnit.SelectedValue);
                        if (string.IsNullOrEmpty(WuId))
                        {
                            lbMessage.Content = "请选择工作单元";
                            lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                            return;
                        }
                        //查询磨豆数据
                        if (!string.IsNullOrEmpty(WuId))
                        {
                            grindbeandataList = bllBaseData.GetGrindBeanDataList(WuId, WorkSheetNo, TenantId);
                        }
                        int index = 0;
                        foreach (var item in grindbeandataList)
                        {
                            index = index + 1;
                            item.Id = Convert.ToString(index);
                        }
                        txtScan.IsEnabled = true;
                        isSCAN = false;
                        TestCount = 0;
                        lbTest.Content = TestCount;
                        txtScan.Focus();
                        txtScan.Text = "";
                        dataGrid.ItemsSource = grindbeandataList;
                        lbReasult.Content = typeList[0].text + "";
                        TestStandard(0);//开始测试
                    }
                    else 
                    {
                        if (txtScan.Text == "000")//000测试回退
                        {
                            if (TestCount == 0)//测试没开始，不需要回退
                                return;
                            StopThread();
                            TestCount = TestCount - 1;
                            //回退清空值
                            string Type = typeList[TestCount].name;
                            ClearValue(Type);
                            txtScan.Text = "";
                            TestStandard(TestCount);//开始上一项测试
                            return;
                        }
                        if (TestCount < typeList.Count)//还有测试项目未完成
                        {
                            if (txtScan.Text + "" == "" && typeList[TestCount].type == "HAND")
                            {
                                lbMessage.Content = "输入不能为空";
                                lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                                txtScan.IsEnabled = true;
                                return;
                            }
                            if (MathHelper.IsNumeric(txtScan.Text) == false && typeList[TestCount].type == "HAND")
                            {
                                lbMessage.Content = "输入格式不正确";
                                lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                                txtScan.IsEnabled = true;
                                txtScan.Clear();
                                return;
                            }
                            if (typeList[TestCount].type == "HAND" && typeList[TestCount].name == "txtKMGL")
                            {
                                StopThread();
                                txtKMGL.Text = txtScan.Text;
                                txtScan.Text = "";
                            }
                            else if (typeList[TestCount].type == "HAND" && typeList[TestCount].name == "txtGL")
                            {
                                StopThread();
                                txtGL.Text = txtScan.Text;
                                txtScan.Text = "";
                            }
                            else if (typeList[TestCount].type == "HAND" && typeList[TestCount].name == "txtDW")
                            {
                                StopThread();
                                txtDW.Text = txtScan.Text;
                                txtScan.Text = "";
                            }
                            else if (typeList[TestCount].type == "WT" && typeList[TestCount].name == "txtFirst")
                            {
                                StopThread();
                                txtFirst.Text = Convert.ToString(lbData.Content);
                                txtScan.Text = "";
                            }
                            else if (typeList[TestCount].type == "WT" && typeList[TestCount].name == "txtSecond")
                            {
                                StopThread();
                                txtSecond.Text = Convert.ToString(lbData.Content);
                                txtScan.Text = "";
                            }
                            else if (typeList[TestCount].type == "WT" && typeList[TestCount].name == "txtThird")
                            {
                                StopThread();
                                txtThird.Text = Convert.ToString(lbData.Content);
                                txtScan.Text = "";
                                GetFZMin();
                            }
                            else if (typeList[TestCount].type == "WT" && typeList[TestCount].name == "txtCSHZL_071")
                            {
                                StopThread();
                                txtCSHZL_071.Text = Convert.ToString(lbData.Content);
                                txtScan.Text = "";
                                GetFZ_071AndRate_071();
                            }
                            else if (typeList[TestCount].type == "WT" && typeList[TestCount].name == "txtCSHZL_03")
                            {
                                StopThread();
                                txtCSHZL_03.Text = Convert.ToString(lbData.Content);
                                txtScan.Text = "";
                                GetFZ_03AndRate_03();
                            }
                            TestCount += 1;
                            lbTest.Content = TestCount;
                            if (TestCount < typeList.Count)//还有测试项目未完成
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
            threadWeightRun = false;
        }

        private void TestStandard(int count)
        {
            try
            {
                lbData.Content = "0";
                lbReasult.Content = typeList[count].text + "";
                switch (typeList[count].type)
                {
                    case "HAND":
                        HANDRUN = true;
                        break;
                    case "WT":
                        thread = new Thread(new ThreadStart(Weight));
                        thread.IsBackground = true;
                        thread.Start();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            SetTotalPass(0);
        }

        private void SetTotalPass(int TotalPass)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["TotalPass"].Value = TotalPass + "";
            MainWindow.TotalPass = TotalPass + "";
            cfa.Save();
            lbTotal.Content = TotalPass + "";
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <returns></returns>
        private bool CheckData()
        {
            bool isPass = true;
            mes_grindbeandata = new List<mes_grindbeandata>();
            mes_grindbeandata grindbeandata = new mes_grindbeandata();
            TextBox[] txt = new TextBox[] { txtKMGL, txtGL, txtDW, txtFirst, txtSecond, txtThird, txtFZMin, txtBZ, txtSWZ_071, txtCSHZL_071, txtFZ_071, txtRate_071, txtSWZ_03, txtCSHZL_03, txtFZ_03, txtRate_03, txtSumRate, };
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i].Text.Trim() == "")
                {
                    lbMessage.Content = "有数据为空,不能保存";
                    lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                    txt[i].Focus();
                    isPass = false;
                }
            }
            if (isPass)
            {
                grindbeandata.Id = Guid.NewGuid().ToString();
                grindbeandata.OrderNo = Convert.ToString(lbORDER_NO.Content);//制令单
                grindbeandata.WorkSheetNo = Convert.ToString(lbWorkSheet_NO.Content);//工单
                grindbeandata.WuId = Convert.ToString(cbbWorkUnit.SelectedValue);
                grindbeandata.BarCode = SCAN_BARCODE;
                grindbeandata.ComponentNumber = ComponentNumber;
                grindbeandata.KMGL = Convert.ToDecimal(txtKMGL.Text);
                grindbeandata.GL = Convert.ToDecimal(txtGL.Text);
                grindbeandata.DW = Convert.ToDecimal(txtDW.Text);
                grindbeandata.First = Convert.ToDecimal(txtFirst.Text);
                grindbeandata.Second = Convert.ToDecimal(txtSecond.Text);
                grindbeandata.Third = Convert.ToDecimal(txtThird.Text);
                grindbeandata.FZMin = Convert.ToDecimal(txtFZMin.Text);
                grindbeandata.BZ = Convert.ToDecimal(txtBZ.Text);
                grindbeandata.SWZ_071 = Convert.ToDecimal(txtSWZ_071.Text);
                grindbeandata.CSHZL_071 = Convert.ToDecimal(txtCSHZL_071.Text);
                grindbeandata.FZ_071 = Convert.ToDecimal(txtFZ_071.Text);
                grindbeandata.Rate_071 = Convert.ToDecimal(txtRate_071.Text);
                grindbeandata.SWZ_03 = Convert.ToDecimal(txtSWZ_03.Text);
                grindbeandata.CSHZL_03 = Convert.ToDecimal(txtCSHZL_03.Text);
                grindbeandata.FZ_03 = Convert.ToDecimal(txtFZ_03.Text);
                grindbeandata.Rate_03 = Convert.ToDecimal(txtRate_03.Text);
                grindbeandata.SumRate = Convert.ToDecimal(txtSumRate.Text);
                grindbeandata.CreateUser = MainWindow.UserInfo.userId;
                grindbeandata.CreateTime = DateTime.Now;
                grindbeandata.TenantId = ConfigurationManager.AppSettings["TenantId"].ToString();
                mes_grindbeandata.Add(grindbeandata);
            }
            return isPass;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void SavaData()
        {
            if (isSCAN)
                return;
            bool isPass = CheckData();//检测测试数据
            if (isPass)
            {
                //过站扫描
                dynamic data = new
                {
                    WorkSheetNo = mes_grindbeandata[0].WorkSheetNo,
                    Wuid = mes_grindbeandata[0].WuId,
                    WorkSheetBarcode = SCAN_BARCODE
                };
                var dataInfo = ApiDataSource.EditScanSave(data);
                if (dataInfo.status == true)
                {
                    //TODO:
                    int savecount = bllBaseData.SaveGrindBeanData(mes_grindbeandata);
                    if (savecount < 1)
                    {
                        lbMessage.Content = "保存失败";
                        lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                        return;
                    }
                    //测试数据保存
                    string WuId = Convert.ToString(cbbWorkUnit.SelectedValue);
                    string WorkSheetNo = Convert.ToString(lbWorkSheet_NO.Content);
                    if (!string.IsNullOrEmpty(WuId) && !string.IsNullOrEmpty(WorkSheetNo))
                    {
                        grindbeandataList = bllBaseData.GetGrindBeanDataList(WuId, WorkSheetNo, TenantId);
                    }
                    int index = 0;
                    foreach (var item in grindbeandataList)
                    {
                        index = index + 1;
                        item.Id = Convert.ToString(index);
                    }
                    dataGrid.ItemsSource = grindbeandataList;
                    ClearData();
                    isSCAN = true;
                    string MachineName = Environment.MachineName;
                    lbMessage.Content = dataInfo.message;
                    lbMessage.Foreground = new SolidColorBrush(Colors.Black);
                    //过站数量+1
                    TotalPass += 1;
                    SetTotalPass(TotalPass);
                }
                else
                {
                    lbMessage.Content = dataInfo.message;
                    lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                lbMessage.Content = "有数据为空,不能保存";
                lbMessage.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void ClearData()
        {
            TextBox[] txt = new TextBox[] { txtKMGL, txtGL, txtDW, txtFirst, txtSecond, txtThird, txtFZMin, txtBZ, txtCSHZL_071, txtFZ_071, txtRate_071, txtCSHZL_03, txtFZ_03, txtRate_03, txtSumRate, };
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i].Text.Trim() != null || txt[i].Text.Trim() != "")
                {
                    txt[i].Clear();
                }
            }
            lbReasult.Content = "测试结果";
            lbData.Content = "0";
        }

        /// <summary>
        /// 编辑071筛网重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSWZ_071_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConfigHelper.UpdateSettingString("txtSWZ_071", txtSWZ_071.Text);
        }

        /// <summary>
        /// 编辑03筛网重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSWZ_03_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConfigHelper.UpdateSettingString("txtSWZ_03", txtSWZ_03.Text);
        }

        /// <summary>
        /// 获取最小粉重
        /// </summary>
        public void GetFZMin() 
        {
            List<decimal> strList = new List<decimal>();
            decimal txtFirstValue = 0;
            decimal txtSecondValue = 0;
            decimal txtThirdValue = 0;
            if (!string.IsNullOrEmpty(txtFirst.Text))
            {
                txtFirstValue = Convert.ToDecimal(txtFirst.Text);
            }    
            if (!string.IsNullOrEmpty(txtSecond.Text))
            {
                txtSecondValue = Convert.ToDecimal(txtSecond.Text);
            }
            if (!string.IsNullOrEmpty(txtThird.Text))
            {
                txtThirdValue = Convert.ToDecimal(txtThird.Text);
            }
            strList.Add(txtFirstValue);
            strList.Add(txtSecondValue);
            strList.Add(txtThirdValue);
            decimal[] strArray = strList.ToArray();//list转数组
            FunctionHelper FunctionHelper = new FunctionHelper(strArray);
            decimal txtFZMinValue = Math.Round(FunctionHelper.Min, 2);
            txtFZMin.Text = Convert.ToString(txtFZMinValue);
        }

        /// <summary>
        /// 计算最终结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBZ_KeyUp(object sender, KeyEventArgs e) 
        {
            string result = "";
            decimal txtFZMinValue = 0;
            if (!string.IsNullOrEmpty(txtFZMin.Text))
            {
                txtFZMinValue = Convert.ToDecimal(txtFZMin.Text);
            }
            if (!string.IsNullOrEmpty(txtBZ.Text))
            {
                decimal txtBZValue = Convert.ToDecimal(txtBZ.Text);
                decimal resultValue = txtFZMinValue - txtBZValue;
                lbData.Content = resultValue;
                if (resultValue > 7 && resultValue <= 9)
                {
                    result = "C";
                }
                else if (resultValue > 9 && resultValue <= 11)
                {
                    result = "B";
                }
                else if (resultValue > 11 && resultValue <= 13)
                {
                    result = "C";
                }
                lbReasult.Content = result;
                threadWeightRun = false;
            }
            else
            {
                lbMessage.Content = "请输入杯重";
                lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                txtBZ.Focus();
            }
        }

        /// <summary>
        /// 获取0.71粉重和比率
        /// </summary>
        public void GetFZ_071AndRate_071()
        {
            decimal txtSWZ_071Value = 0;
            if (!string.IsNullOrEmpty(txtSWZ_071.Text))
            {
                txtSWZ_071Value = Convert.ToDecimal(txtSWZ_071.Text);
            }
            decimal txtCSHZL_071Value = 0;
            if (!string.IsNullOrEmpty(txtCSHZL_071.Text))
            {
                txtCSHZL_071Value = Convert.ToDecimal(txtCSHZL_071.Text);
            }
            decimal txtFZ_071Value = txtCSHZL_071Value - txtSWZ_071Value;
            txtFZ_071.Text = Convert.ToString(txtFZ_071Value);
            decimal txtFZMinValue = 0;
            if (!string.IsNullOrEmpty(txtFZMin.Text))
            {
                txtFZMinValue = Convert.ToDecimal(txtFZMin.Text);
            }
            decimal txtRate_071Value = 0;
            if (txtFZMinValue != 0)
            {
                txtRate_071Value = txtFZ_071Value / txtFZMinValue;
            }
            txtRate_071.Text = Convert.ToString(txtRate_071Value);
        }

        /// <summary>
        /// 获取0.3粉重和比率
        /// </summary>
        public void GetFZ_03AndRate_03()
        {
            decimal txtSWZ_03Value = 0;
            if (!string.IsNullOrEmpty(txtSWZ_03.Text))
            {
                txtSWZ_03Value = Convert.ToDecimal(txtSWZ_03.Text);
            }
            decimal txtCSHZL_03Value = 0;
            if (!string.IsNullOrEmpty(txtCSHZL_03.Text))
            {
                txtCSHZL_03Value = Convert.ToDecimal(txtCSHZL_03.Text);
            }
            decimal txtFZ_03Value = txtCSHZL_03Value - txtSWZ_03Value;
            txtFZ_03.Text = Convert.ToString(txtFZ_03Value);
            decimal txtFZMinValue = 0;
            if (!string.IsNullOrEmpty(txtFZMin.Text))
            {
                txtFZMinValue = Convert.ToDecimal(txtFZMin.Text);
            }
            decimal txtRate_03Value = 0;
            if (txtFZMinValue != 0)
            {
                txtRate_03Value = txtFZ_03Value / txtFZMinValue;
            }
            txtRate_03.Text = Convert.ToString(txtRate_03Value);
            GetSumRate();
        }

        /// <summary>
        /// 计算总比率
        /// </summary>
        public void GetSumRate() 
        {
            decimal txtRate_071Value = 0;
            if (!string.IsNullOrEmpty(txtRate_071.Text))
            {
                txtRate_071Value = Convert.ToDecimal(txtRate_071.Text);
            }
            decimal txtRate_03Value = 0;
            if (!string.IsNullOrEmpty(txtRate_03.Text))
            {
                txtRate_03Value = Convert.ToDecimal(txtRate_03.Text);
            }
            decimal txtSumRateValue = txtRate_071Value + txtRate_03Value;
            txtSumRate.Text = Convert.ToString(txtSumRateValue);
        }

        /// <summary>
        /// 称重
        /// </summary>
        private void Weight()
        {
            Thread.Sleep(200);//停顿0.2秒再开始
            threadWeightRun = true;
            BalanceWeight.ReadWeight();
            while (threadWeightRun == true)
            {
                decimal value = Math.Round(BalanceWeight.CurWeight, 1);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        lbData.Content = value + "";
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }));
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 回退清空值
        /// </summary>
        /// <param name="Type"></param>
        public void ClearValue(string Type) 
        {
            TextBox[] txt = new TextBox[] { txtKMGL, txtGL, txtDW, txtFirst, txtSecond, txtThird, txtFZMin, txtBZ, txtCSHZL_071, txtFZ_071, txtRate_071, txtCSHZL_03, txtFZ_03, txtRate_03, txtSumRate, };
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i].Name == Type)
                {
                    txt[i].Clear();
                }
                if (Type == "txtThird")
                {
                    txtFZMin.Clear();
                }
                if (Type == "txtCSHZL_071")
                {
                    txtFZ_071.Clear();
                    txtRate_071.Clear();
                }
                if (Type == "txtCSHZL_03")
                {
                    txtFZ_03.Clear();
                    txtRate_03.Clear();
                    txtSumRate.Clear();
                }
            }
        }
    }
}
