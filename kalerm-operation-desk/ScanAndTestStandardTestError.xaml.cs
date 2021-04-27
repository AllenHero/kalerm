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
using System.Windows.Shapes;

namespace kalerm_operation_desk
{
    /// <summary>
    /// ScanAndTestStandardTestError.xaml 的交互逻辑
    /// </summary>
    public partial class ScanAndTestStandardTestError : Window
    {
        public event EventHandler UpdateTestErrorEvent = null;

        bool CanClose = false;
        public ScanAndTestStandardTestError()
        {
            InitializeComponent();
            this.Loaded += ScanAndTestStandardScanChange_Loaded;
            txtScan.KeyDown += TxtScan_KeyDown;
            btnCel.Click += BtnCel_Click;
            this.Closing += ScanAndTestStandardTestError_Closing;
        }

        private void ScanAndTestStandardTestError_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CanClose)
                e.Cancel = false;
            else
                e.Cancel = true;
        }

        public void Dispose()
        {
            txtScan.KeyDown -= TxtScan_KeyDown;
            btnCel.Click -= BtnCel_Click;
            this.Closing -= ScanAndTestStandardTestError_Closing;
            System.GC.Collect();
        }

        private void ScanAndTestStandardScanChange_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ScanAndTestStandardScanChange_Loaded;
            txtScan.Focus();
        }

        private void TxtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(txtScan.Text))
            {
                if (UpdateTestErrorEvent != null)
                    UpdateTestErrorEvent(txtScan.Text + "", new EventArgs());
                CanClose = true;
                this.Close();
            }
        }

        private void BtnCel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
