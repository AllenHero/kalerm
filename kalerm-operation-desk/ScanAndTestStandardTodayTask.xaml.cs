using kalerm_bll.BaseData;
using kalerm_model;
using kalerm_operation_desk.Base;
using kalerm_operation_desk.Control;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// ScanAndTestStandardTodayTask.xaml 的交互逻辑
    /// </summary>
    public partial class ScanAndTestStandardTodayTask :  IDisposable
    {
        public event EventHandler ScanAndTestStandardTodayTaskEvent = null;

        private BllBaseData bllBaseData = new BllBaseData();

        List<console_wuarrange> console_wuarrange = new List<console_wuarrange>();

        string TenantId = ConfigurationManager.AppSettings["TenantId"].ToString();

        public console_wuarrange console_wuarrange_model = new console_wuarrange();

        public ScanAndTestStandardTodayTask()
        {
            InitializeComponent();
            this.Loaded += ScanAndTestStandardTodayTask_Loaded;
            btnOk.Click += new RoutedEventHandler(btnOk_Click);
            btnCel.Click += new RoutedEventHandler(btnCel_Click);
        }

        private void ScanAndTestStandardTodayTask_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(ScanAndTestStandardTodayTask_Loaded);
            try
            {
                string UserId = MainWindow.UserInfo.userId;
                string NowDate = DateTime.Now.ToString("yyyy-MM-dd");
                console_wuarrange = bllBaseData.GetConsoleWuArrangeList(UserId, TenantId, NowDate);
                dataGrid.ItemsSource = console_wuarrange;
            }
            catch
            {
                ReMessageBox.Show("获取数据异常，请检查网络后重试");
                return;
            }
        }

        public void Dispose()
        {
            System.GC.Collect();
            btnOk.Click -= new RoutedEventHandler(btnOk_Click);
            btnCel.Click -= new RoutedEventHandler(btnCel_Click);
        }

        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                console_wuarrange mySelectedElement = (console_wuarrange)dataGrid.SelectedItem;
                string WorkSheetNo = mySelectedElement.WorkSheetNo;
                string wuid = mySelectedElement.wuid;
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["WorkSheetNo"].Value = WorkSheetNo + "";
                cfa.AppSettings.Settings["WorkUnitId"].Value = wuid + "";
                cfa.Save();
                MainWindow.WorkSheetNo = WorkSheetNo + "";
                MainWindow.WorkUnitId = wuid + "";
                if (ScanAndTestStandardTodayTaskEvent != null)
                    ScanAndTestStandardTodayTaskEvent(this, new EventArgs());
                this.Close();
            }
            catch(Exception ex)
            {
                ReMessageBox.Show("更新数据失败，请检查网络后重试"+ ex);
                return;
            }
            this.Close();
        }

        void btnCel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid.SelectedItem as console_wuarrange;
            CheckBox ckb = sender as CheckBox;
            if (ckb.IsChecked == true)
                item.isCheck = true;
            else
                item.isCheck = false;
        }
    }
}
