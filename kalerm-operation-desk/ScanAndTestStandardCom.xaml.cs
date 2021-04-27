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
    /// ScanAndTestStandardCom.xaml 的交互逻辑
    /// </summary>
    public partial class ScanAndTestStandardCom : Window
    {
        public event EventHandler ScanAndTestStandardComEvent = null;

        public string WeightCom = "";

        public string TemperatureCom = "";

        public ScanAndTestStandardCom()
        {
            InitializeComponent();
            this.Loaded += ScanAndTestStandardCom_Loaded;
            btnOk.Click += new RoutedEventHandler(btnOk_Click);
            btnCel.Click += new RoutedEventHandler(btnCel_Click);
        }

        public void Dispose()
        {
            System.GC.Collect();
            btnOk.Click -= new RoutedEventHandler(btnOk_Click);
            btnCel.Click -= new RoutedEventHandler(btnCel_Click);
        }

        private void ScanAndTestStandardCom_Loaded(object sender, RoutedEventArgs e)
        {
            txtWT.Text = WeightCom;
            txtTP.Text = TemperatureCom;
        }


        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["WeightCom"].Value = txtWT.Text + "";
            cfa.AppSettings.Settings["TemperatureCom"].Value = txtTP.Text + "";
            cfa.Save();

            MainWindow.WeightCom = txtWT.Text + "";
            MainWindow.TemperatureCom = txtTP.Text + "";
            if (ScanAndTestStandardComEvent != null)
                ScanAndTestStandardComEvent(this, new EventArgs());
            this.Close();
        }

        void btnCel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
