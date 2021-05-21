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

        //主条码
        string SCAN_BARCODE = "";

        public GrindBeanStandard()
        {
            InitializeComponent();
            this.Loaded += GrindBeanStandard_Loaded;
            txtScan.KeyUp += TxtScan_KeyUp;
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
            worksheet = new ObservableCollection<worksheet>(bllBaseData.GetWorkSheetList());
            foreach (var row in worksheet)
            {
                textWorkSheet.AddItem(new AutoCompleteEntry(row.WorkSheetNo + '|' + row.ProductCode, row.WorkSheetNo + '|' + row.ProductCode));
            }
            base_wu = new ObservableCollection<base_wu>(bllBaseData.GetBaseWuList(""));
            cbbWorkUnit.ItemsSource = base_wu;
            cbbWorkUnit.SelectedValue = WorkUnitId;
            txtScan.Focus();

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
            base_wu = new ObservableCollection<base_wu>(bllBaseData.GetBaseWuList(ProductCode));
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
                        worksheet = bllBaseData.GetWorkSheet(WorkSheetNo);
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
                        productionprocess = bllBaseData.GetProductionProcess(ProcessId);
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
                        txtScan.IsEnabled = true;
                        isSCAN = false;
                        TestCount = 0;
                        lbTest.Content = TestCount;
                        txtScan.Focus();
                        txtScan.Text = "";
                    }

                    else//手动输入空磨功率，功率，档位，0.71筛网重，0.3筛网重
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
    }
}
