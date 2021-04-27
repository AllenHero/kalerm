using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace kalerm_operation_desk
{
    /// <summary>
    /// ScanAndTestStandardSet.xaml 的交互逻辑
    /// </summary>
    public partial class ScanAndTestStandardSet : Window
    {
        public string LineNO = "";

        public string PROCESS_NO = "";

        public event EventHandler ScanAndTestStandardSetEvent = null;

        public ScanAndTestStandardSet()
        {
            InitializeComponent();
            this.Loaded += ScanAndTestStandardSet_Loaded;
            btnOk.Click += new RoutedEventHandler(btnOk_Click);
            btnCel.Click += new RoutedEventHandler(btnCel_Click);
        }

        public void Dispose()
        {
            System.GC.Collect();
            btnOk.Click -= new RoutedEventHandler(btnOk_Click);
            btnCel.Click -= new RoutedEventHandler(btnCel_Click);
        }

        private void ScanAndTestStandardSet_Loaded(object sender, RoutedEventArgs e)
        {
            //ReportBaseModel = new ObservableCollection<ReportBaseModel>(bllBaseData.GetLineNo(false));
            //cbbLINE_ON.ItemsSource = ReportBaseModel;
            if (string.IsNullOrEmpty(LineNO))
                cbbLINE_ON.SelectedIndex = 0;
            else
                cbbLINE_ON.SelectedValue = LineNO;


            //MES_PROCESSES = new ObservableCollection<MES_PROCESSES>(bllBaseData.GetMES_PROCESSES());
            //cbbPROCESS_NO.ItemsSource = MES_PROCESSES;
            if (string.IsNullOrEmpty(PROCESS_NO))
                cbbPROCESS_NO.SelectedIndex = 0;
            else
                cbbPROCESS_NO.SelectedValue = PROCESS_NO;
        }

        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["LineNO"].Value = cbbLINE_ON.SelectedValue + "";
            cfa.AppSettings.Settings["PROCESS_NO"].Value = cbbPROCESS_NO.SelectedValue + "";
            cfa.Save();

            MainWindow.LineNO = cbbLINE_ON.SelectedValue + "";
            MainWindow.PROCESS_NO = cbbPROCESS_NO.SelectedValue + "";
            if (ScanAndTestStandardSetEvent != null)
                ScanAndTestStandardSetEvent(this, new EventArgs());
            this.Close();
        }

        void btnCel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
