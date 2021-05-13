using kalerm_bll.BaseData;
using kalerm_model;
using kalerm_operation_desk.Control;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private BllBaseData bllBaseData = new BllBaseData();

        ObservableCollection<base_wu> base_wu = new ObservableCollection<base_wu>();

        public event EventHandler ScanAndTestStandardSetEvent = null;

        ObservableCollection<WorkSheet> WorkSheet = new ObservableCollection<WorkSheet>();



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

            WorkSheet = new ObservableCollection<WorkSheet>(bllBaseData.GetWorkSheet());
            foreach (var row in WorkSheet)
            {
                textWrokSheet.AddItem(new AutoCompleteEntry(row.WorkSheetNo + '|' + row.ProductCode, row.WorkSheetNo + '|' + row.ProductCode));
            }

        }

        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["LineNO"].Value = textWrokSheet.Text + "";
            cfa.AppSettings.Settings["PROCESS_NO"].Value = cbbWorkUnit.SelectedValue + "";
            cfa.Save();

            MainWindow.LineNO = textWrokSheet.Text + "";
            MainWindow.PROCESS_NO = cbbWorkUnit.SelectedValue + "";
            if (ScanAndTestStandardSetEvent != null)
                ScanAndTestStandardSetEvent(this, new EventArgs());

            this.Close();
        }

        void btnCel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void textWrokSheet_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //根据工单获取工作单元
            string str1 = textWrokSheet.Text + "";
            string ProductCode = "";

            if (str1.Contains('|'))
            {
                string[] sArray = str1.Split('|');
                ProductCode = sArray[1];
            }

            base_wu = new ObservableCollection<base_wu>(bllBaseData.GetBaseWu(ProductCode));
            cbbWorkUnit.ItemsSource = base_wu;
            cbbWorkUnit.DisplayMemberPath = "wuname";
            cbbWorkUnit.SelectedValuePath = "wuid";
        }
    }
}
