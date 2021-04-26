using kalerm_common;
using System;
using System.Collections.Generic;
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
    /// ScanAndTestStandard.xaml 的交互逻辑
    /// </summary>
    public partial class ScanAndTestStandard : Page
    {
        DateTime dt = new DateTime();

        //电子秤
        BalanceWeight BalanceWeight = new BalanceWeight(Login.WeightCom);

        //温度仪
        Thermometer Thermometer = new Thermometer(Login.TemperatureCom);

        //总测试数
        int TotalPass = 0;

        public ScanAndTestStandard()
        {
            InitializeComponent();
            this.Loaded += ScanAndTestStandard_Loaded;
        }

        private void ScanAndTestStandard_Loaded(object sender, RoutedEventArgs e)
        {

            //string[] content1 = PortUltility.FindAddresses(PortType.RS232);
            //string[] content2 = PortUltility.FindRS232Type(content1);
            //for (int i = 0; i < content2.Count(); i++)
            //{
            //    if (content2[i] + "" == "COM8")
            //    {
            //        GPT9000 = content1[i];
            //    }
            //    if (!string.IsNullOrEmpty(GPT9000))
            //        lbAG.Content = "串口存在";
            //}

            dt = DateTime.Now;
            if (BalanceWeight.Open())
                lbWT.Content = "打开串口成功";
            else
                lbWT.Content = "打开串口失败";

            if (Thermometer.Open())
                lbTP.Content = "打开串口成功";
            else
                lbTP.Content = "打开串口失败";

            string LineNO = Login.LineNO + "";
            string PROCESS_NO = Login.PROCESS_NO + "";
            TotalPass = Convert.ToInt32(Login.TotalPass);
            lbWTCOM.Content = Login.WeightCom + "";
            lbTPCOM.Content = Login.TemperatureCom + "";
            lbTotal.Content = TotalPass + "";

            this.Loaded -= ScanAndTestStandard_Loaded;
            //BaseModel = new ObservableCollection<ReportBaseModel>(bllBaseData.GetLineNo(false));
           //cbbLineNo.ItemsSource = BaseModel;
            cbbLineNo.SelectedValue = LineNO;
            //MES_PROCESSES = new ObservableCollection<MES_PROCESSES>(bllBaseData.GetMES_PROCESSES());
           //cbbPROCESS.ItemsSource = MES_PROCESSES;
            cbbPROCESS.SelectedValue = PROCESS_NO;

            txtScan.Focus();
        }
    }
}
