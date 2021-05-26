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
        BalanceWeight BalanceWeight = new BalanceWeight(MainWindowNew.WeightCom);

        private BllBaseData bllBaseData = new BllBaseData();

        ObservableCollection<base_wu> base_wu = new ObservableCollection<base_wu>();

        List<dynamic> typeList = new List<dynamic>();

        //当前测试项目
        int TestCount = 0;

        //是否扫码
        bool isSCAN = true;

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
                textWorkSheet.AddItem(new AutoCompleteEntry(row.WorkSheetNo + '|' + row.ProductCode, row.WorkSheetNo + '|' + row.ProductCode));
            }
            base_wu = new ObservableCollection<base_wu>(bllBaseData.GetBaseWuList("", TenantId));
            cbbWorkUnit.ItemsSource = base_wu;
            cbbWorkUnit.SelectedValue = WorkUnitId;
            txtScan.Focus();

            txtSWZ_071.Text = ConfigurationManager.AppSettings["txtSWZ_071"].ToString();

            txtSWZ_03.Text = ConfigurationManager.AppSettings["txtSWZ_03"].ToString();

            string[] arr = new string[] { "txtKMGL", "txtGL", "txtDW", "txtSWZ_071", "txtSWZ_03" };
            foreach (var item in arr)
            {
                dynamic obj = new ExpandoObject();
                obj.type = item;
                typeList.Add(obj);
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
                    if (txtScan.Text + "" == "OK")//完成
                    {
                        CheckData();
                        SavaData();
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
                    worksheet worksheet = null;
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
                    }

                    else if (isSCAN == false)//手动输入空磨功率，功率，档位，0.71筛网重，0.3筛网重
                    {
                        if (TestCount < typeList.Count)//还有测试项目未完成
                        {
                            switch (typeList[TestCount].type)//当前测试项目
                            {
                                case "txtKMGL":
                                    txtKMGL.Text = txtScan.Text;
                                    txtScan.Clear();
                                    break;
                                case "txtGL":
                                    txtGL.Text = txtScan.Text;
                                    txtScan.Clear();
                                    break;
                                case "txtDW":
                                    txtDW.Text = txtScan.Text;
                                    txtScan.Clear();
                                    break;
                                case "txtSWZ_071":
                                    txtSWZ_071.Text = txtScan.Text;
                                    txtScan.Clear();
                                    break;
                                case "txtSWZ_03":
                                    txtSWZ_03.Text = txtScan.Text;
                                    txtScan.Clear();
                                    break;
                            }
                            TestCount += 1;
                        }
                        if (typeList.Count == TestCount)
                        {
                            txtFirst.Focus();
                        }
                    }
                    //自动称重
                    else if (txtFirst.IsFocused == true)
                    {
                        txtFirst.KeyUp += txtFirst_KeyUp;
                    }
                    else if (txtSecond.IsFocused==true) 
                    {
                        txtSecond.KeyUp += txtSecond_KeyUp; 
                    }
                    else if (txtThird.IsFocused==true)
                    {
                        txtThird.KeyUp += txtThird_KeyUp;    
                    }
                    else if (txtCSHZL_071.IsFocused==true)
                    {
                        txtCSHZL_071.KeyUp += txtCSHZL_071_KeyUp;                      
                    }
                    else if (txtCSHZL_03.IsFocused==true)
                    {
                        txtCSHZL_03.KeyUp += txtCSHZL_03_KeyUp;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                //TODO:
                int savecount = bllBaseData.SaveGrindBeanData(mes_grindbeandata);
                if (savecount < 1)
                {
                    lbMessage.Content = "保存失败";
                    lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                    return;
                }//测试数据保存
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
                lbMessage.Content = "成功过站";
                lbMessage.Foreground = new SolidColorBrush(Colors.Black);
                //过站数量+1
                TotalPass += 1;
                SetTotalPass(TotalPass);
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
        /// 第一杯粉重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFirst_KeyUp(object sender, KeyEventArgs e) 
        {
            //thread = new Thread(new ThreadStart(Weight));
            //thread.IsBackground = true;
            //thread.Start();
            Thread.Sleep(200);//停顿0.2秒再开始
            threadWeightRun = true;
            BalanceWeight.ReadWeight();
            decimal value = Math.Round(BalanceWeight.CurWeight, 1);
            lbData.Content = value + "";
            Thread.Sleep(100);
            dynamic txtFirstValue = lbData.Content;
            txtFirst.Text = txtFirstValue;
            txtSecond.Focus();
        }

        /// <summary>
        /// 第二杯粉重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSecond_KeyUp(object sender, KeyEventArgs e)
        {
            //thread = new Thread(new ThreadStart(Weight));
            //thread.IsBackground = true;
            //thread.Start();
            Thread.Sleep(200);//停顿0.2秒再开始
            threadWeightRun = true;
            BalanceWeight.ReadWeight();
            decimal value = Math.Round(BalanceWeight.CurWeight, 1);
            lbData.Content = value + "";
            Thread.Sleep(100);
            dynamic txtSecondValue = lbData.Content;
            txtSecond.Text = txtSecondValue;
            txtThird.Focus();
        }

        /// <summary>
        /// 第三杯粉重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtThird_KeyUp(object sender, KeyEventArgs e)
        {
            //thread = new Thread(new ThreadStart(Weight));
            //thread.IsBackground = true;
            //thread.Start();
            Thread.Sleep(200);//停顿0.2秒再开始
            threadWeightRun = true;
            BalanceWeight.ReadWeight();
            decimal value = Math.Round(BalanceWeight.CurWeight, 1);
            lbData.Content = value + "";
            Thread.Sleep(100);
            dynamic txtThirdValue = lbData.Content;
            txtThird.Text = txtThirdValue;
            GetFZMin();
            txtCSHZL_071.Focus();
        }

        /// <summary>
        /// 获取最小粉重
        /// </summary>
        public void GetFZMin() 
        {
            List<decimal> strList = new List<decimal>();
            strList.Add(Convert.ToDecimal(txtFirst.Text));
            strList.Add(Convert.ToDecimal(txtFirst.Text));
            strList.Add(Convert.ToDecimal(txtThird.Text));
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
            decimal txtFZMinValue = Convert.ToDecimal(txtFZMin.Text);
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
            }
            else
            {
                lbMessage.Content = "请输入杯重";
                lbMessage.Foreground = new SolidColorBrush(Colors.Red);
                txtBZ.Focus();
            }
        }

        /// <summary>
        /// 0.71测试后重量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCSHZL_071_KeyUp(object sender, KeyEventArgs e) 
        {
            Thread.Sleep(200);//停顿0.2秒再开始
            threadWeightRun = true;
            BalanceWeight.ReadWeight();
            decimal value = Math.Round(BalanceWeight.CurWeight, 1);
            lbData.Content = value + "";
            Thread.Sleep(100);
            dynamic txtCSHZL_071Value = lbData.Content;
            txtCSHZL_071.Text = txtCSHZL_071Value;
            GetFZ_071AndRate_071();
            txtCSHZL_03.Focus();
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
        /// 0.3测试后重量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCSHZL_03_KeyUp(object sender, KeyEventArgs e)
        {
            Thread.Sleep(200);//停顿0.2秒再开始
            threadWeightRun = true;
            BalanceWeight.ReadWeight();
            decimal value = Math.Round(BalanceWeight.CurWeight, 1);
            lbData.Content = value + "";
            Thread.Sleep(100);
            dynamic txtCSHZL_03Value = lbData.Content;
            txtCSHZL_03.Text = txtCSHZL_03Value;
            GetFZ_03AndRate_03();
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
    }
}
