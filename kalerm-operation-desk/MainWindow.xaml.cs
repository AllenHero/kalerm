﻿using kalerm_model.BaseData;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace kalerm_operation_desk
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string LineNO;
        public static string PROCESS_NO;
        public static string WeightCom;
        public static string TemperatureCom;
        public static string TotalPass;

        public static UserInfo UserInfo;

        public MainWindow()
        {
            InitializeComponent();
            LineNO = ConfigurationManager.AppSettings["LineNO"] + "";
            PROCESS_NO = ConfigurationManager.AppSettings["PROCESS_NO"] + "";
            WeightCom = ConfigurationManager.AppSettings["WeightCom"] + "";
            TemperatureCom = ConfigurationManager.AppSettings["TemperatureCom"] + "";
        }

        private void btnScanAndTestStandard_Click(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("ScanAndTestStandard.xaml", UriKind.Relative);
            window.Show();
        }

        private void btnScanAndTestStandardGPT_Click(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("ScanAndTestStandardGPT.xaml", UriKind.Relative);
            window.Show();
        }
    }
}
