﻿using kalerm_bll.BaseData;
using kalerm_common;
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

        public GrindBeanStandard()
        {
            InitializeComponent();
            this.Loaded += GrindBeanStandard_Loaded;
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