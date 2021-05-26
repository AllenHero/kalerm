using kalerm_model.BaseData;
using kalerm_operation_desk.Control;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
    public partial class MainWindow: Window
    {
        Dictionary<int, string> TabItemControl = new Dictionary<int, string>();

        public static UserInfo UserInfo;

        public static string WorkSheetNo;

        public static string WorkUnitId;

        public static string WeightCom;

        public static string TemperatureCom;

        public static string TotalPass;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            ReMessageBox.Dispatcher = this.Dispatcher;
            imageUp.MouseEnter += new MouseEventHandler(ImageButton_MouseEnter);
            imageDown.MouseEnter += new MouseEventHandler(ImageButton_MouseEnter);
            imageUp.MouseLeave += new MouseEventHandler(ImageButton_MouseLeave);
            imageDown.MouseLeave += new MouseEventHandler(ImageButton_MouseLeave);
            imageUp.MouseLeftButtonDown += new MouseButtonEventHandler(ImageUp_MouseLeftButtonDown);
            imageDown.MouseLeftButtonDown += new MouseButtonEventHandler(ImageDown_MouseLeftButtonDown);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WorkSheetNo = ConfigurationManager.AppSettings["WorkSheetNo"] + "";
            WorkUnitId = ConfigurationManager.AppSettings["WorkUnitId"] + "";
            WeightCom = ConfigurationManager.AppSettings["WeightCom"] + "";
            TemperatureCom = ConfigurationManager.AppSettings["TemperatureCom"] + "";
            TabItemControl.Add(0, "首页");
            if (UserInfo != null && UserInfo.realName != null)
            {
                tbUserName.Text = "当前用户：【" + UserInfo.realName + "】";
            }
        }

        #region 主菜单无法显示完全时，滚动按钮

        private void ImageDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                menuStack.LineUp();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ImageUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                menuStack.LineDown();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ImageButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            if (image != null)
            {
                image.Opacity = 1;
            }
        }

        private void ImageButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            if (image != null)
            {
                image.Opacity = 0.5;
            }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            CheckShowUpDownButton();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            CheckShowUpDownButton();
        }

        /// <summary>
        /// Expander展开或折叠事件触发后，并未立即改变Expander.ActualHeight的值，所以需要计算Expander的高度
        /// </summary>
        private void CheckShowUpDownButton()
        {
            double allHeight = 0;
            foreach (UIElement control in menuStack.Children)
            {
                Expander exp = control as Expander;
                if (exp != null)
                {
                    allHeight += 30;
                    if (exp.IsExpanded)
                    {
                        Grid grid = exp.Content as Grid;
                        if (grid != null)
                        {
                            int count = grid.RowDefinitions.Count;
                            allHeight += (count + 1) / 2 * 30;
                            allHeight += (count - 1) / 2 * 2;
                        }
                    }
                }
            }
            double aheight = menuStack.ActualHeight;
            if (allHeight > aheight)
            {
                gridUpDownButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                gridUpDownButton.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #endregion

        public void Dispose()
        {
            this.Loaded -= new RoutedEventHandler(MainWindow_Loaded);
            imageUp.MouseEnter -= new MouseEventHandler(ImageButton_MouseEnter);
            imageDown.MouseEnter -= new MouseEventHandler(ImageButton_MouseEnter);
            imageUp.MouseLeave -= new MouseEventHandler(ImageButton_MouseLeave);
            imageDown.MouseLeave -= new MouseEventHandler(ImageButton_MouseLeave);
            imageUp.MouseLeftButtonDown -= new MouseButtonEventHandler(ImageUp_MouseLeftButtonDown);
            imageDown.MouseLeftButtonDown -= new MouseButtonEventHandler(ImageDown_MouseLeftButtonDown);
            GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }

        //关闭窗体
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string header = btn.Tag.ToString();
            foreach (ControlTabItem item in MainTab.Items)
            {
                if (item.Header.ToString() == header && item.Header.ToString() != "首页")
                {
                    MainTabItemControl MainTabItemControl = item.Content as MainTabItemControl;
                    IDisposable dispose = MainTabItemControl.frame.Content as IDisposable;
                    if (dispose != null)
                        dispose.Dispose();
                    MainTab.Items.Remove(item);
                    List<string> list = new List<string>();
                    foreach (var row in TabItemControl)
                    {
                        if (row.Value != item.Header.ToString())
                        {
                            list.Add(row.Value);
                        }
                    }
                    TabItemControl = new Dictionary<int, string>();
                    int i = 0;
                    foreach (var row in list)
                    {
                        TabItemControl.Add(i, row);
                        i = i + 1;
                    }
                    break;
                }
            }
        }

        private void mainHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
                if (e.ClickCount >= 2)
                {
                    if (this.WindowState == System.Windows.WindowState.Maximized)
                        this.WindowState = System.Windows.WindowState.Normal;
                    else this.WindowState = System.Windows.WindowState.Maximized;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //显示和隐藏菜单
        private void dockShowMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is Image)
                {
                    Image dock = sender as Image;
                    if (gcMenu.Width.Value >= 10)
                    {
                        gcMenu.Width = new GridLength(6);
                        rotatetransform.Angle = 180;
                    }
                    else
                    {
                        gcMenu.Width = new GridLength(200);
                        rotatetransform.Angle = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void dockShowMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            dockShowMenu.Opacity = 0.7;
        }

        private void dockShowMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            dockShowMenu.Opacity = 1;
        }

        #region 菜单的鼠标效果和点击事件

        private void ExpanderItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ExpanderItem expItem = sender as ExpanderItem;
                if (expItem != null)
                {
                    if (expItem.Tag != null && expItem.Tag.ToString() == "ScanAndTestStandard")
                    {
                        foreach (var row in TabItemControl)
                        {
                            if (row.Value + "" == "流程扫描")
                            {
                                MainTab.SelectedIndex = Convert.ToInt32(row.Key);
                                return;
                            }
                        }
                        MainTabItemControl MainTabItemControl = new MainTabItemControl();
                        MainTabItemControl.UICode = "/ScanAndTestStandard.xaml";
                        MainTabItemControl.HeaderText = "流程扫描";
                        ControlTabItem TabItem = new ControlTabItem();
                        TabItem.Margin = new Thickness(3, 0, 0, 0);
                        TabItem.Header = "流程扫描";
                        MainTabItemControl.Margin = new Thickness(-5, -5, -3, -3);
                        TabItem.Content = MainTabItemControl;
                        MainTab.Items.Add(TabItem);
                        TabItemControl.Add(TabItemControl.Count, "流程扫描");
                        MainTab.SelectedIndex = TabItemControl.Count - 1;
                    }
                    else if (expItem.Tag != null && expItem.Tag.ToString() == "ScanAndTestStandardGPT")
                    {
                        foreach (var row in TabItemControl)
                        {
                            if (row.Value + "" == "安规测试扫码")
                            {
                                MainTab.SelectedIndex = Convert.ToInt32(row.Key);
                                return;
                            }
                        }
                        MainTabItemControl MainTabItemControl = new MainTabItemControl();
                        MainTabItemControl.UICode = "/ScanAndTestStandardGPT.xaml";
                        MainTabItemControl.HeaderText = "安规测试扫码";
                        ControlTabItem TabItem = new ControlTabItem();
                        TabItem.Margin = new Thickness(3, 0, 0, 0);
                        TabItem.Header = "安规测试扫码";
                        MainTabItemControl.Margin = new Thickness(-5, -5, -3, -3);
                        TabItem.Content = MainTabItemControl;
                        MainTab.Items.Add(TabItem);
                        TabItemControl.Add(TabItemControl.Count, "安规测试扫码");
                        MainTab.SelectedIndex = TabItemControl.Count - 1;
                    }
                    else if (expItem.Tag != null && expItem.Tag.ToString() == "GrindBeanStandard")
                    {
                        foreach (var row in TabItemControl)
                        {
                            if (row.Value + "" == "磨豆数据采集")
                            {
                                MainTab.SelectedIndex = Convert.ToInt32(row.Key);
                                return;
                            }
                        }
                        MainTabItemControl MainTabItemControl = new MainTabItemControl();
                        MainTabItemControl.UICode = "/GrindBeanStandard.xaml";
                        MainTabItemControl.HeaderText = "磨豆数据采集";
                        ControlTabItem TabItem = new ControlTabItem();
                        TabItem.Margin = new Thickness(3, 0, 0, 0);
                        TabItem.Header = "磨豆数据采集";
                        MainTabItemControl.Margin = new Thickness(-5, -5, -3, -3);
                        TabItem.Content = MainTabItemControl;
                        MainTab.Items.Add(TabItem);
                        TabItemControl.Add(TabItemControl.Count, "磨豆数据采集");
                        MainTab.SelectedIndex = TabItemControl.Count - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ExpanderItem_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                ExpanderItem expItem = sender as ExpanderItem;
                if (expItem != null)
                {
                    expItem.HeaderForeground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    expItem.BackgroundImage = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ExpanderItem_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                ExpanderItem expItem = sender as ExpanderItem;
                if (expItem != null)
                {
                    expItem.HeaderForeground = new SolidColorBrush(Color.FromArgb(255, 1, 62, 127));
                    expItem.BackgroundImage = new BitmapImage(new Uri("/Image/ExpanderMouse.jpg", UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region 最大化、最小化、关闭按钮、主菜单按钮

        private void WindowButton_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Image button = sender as Image;
                if (button != null)
                {
                    button.Source = new BitmapImage(new Uri(string.Format("/Image/{0}_2.png", button.Tag), UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void WindowButton_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Image button = sender as Image;
                if (button != null)
                {
                    button.Source = new BitmapImage(new Uri(string.Format("/Image/{0}_1.png", button.Tag), UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void WindowButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image button = sender as Image;
                if (button != null)
                {
                    if (button.Tag != null)
                    {
                        string cmd = button.Tag.ToString();
                        if (cmd == "Exit")
                        {
                            if (ReMessageBox.Show("是否退出系统？", "提示", MessageWindowButtons.YesNo, MessageWindowIcons.Question) == MessageWindowResult.Yes)
                            {
                                Dispose();
                                this.Close();
                            }
                        }
                        else if (cmd == "Maximized")
                        {
                            imageMax.Tag = "Recovery";
                            this.WindowState = System.Windows.WindowState.Maximized;
                        }
                        else if (cmd == "Recovery")
                        {
                            imageMax.Tag = "Maximized";
                            this.WindowState = System.Windows.WindowState.Normal;
                        }
                        else if (cmd == "Minimized")
                        {
                            this.WindowState = System.Windows.WindowState.Minimized;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            #region 判断是否显示按钮

            CheckShowUpDownButton();

            #endregion

            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                imageMax.Tag = "Recovery";
                imageMax.Source = new BitmapImage(new Uri(string.Format("/Image/{0}_1.png", imageMax.Tag), UriKind.Relative));
            }
            else if (this.WindowState == System.Windows.WindowState.Normal)
            {
                imageMax.Tag = "Maximized";
                imageMax.Source = new BitmapImage(new Uri(string.Format("/Image/{0}_1.png", imageMax.Tag), UriKind.Relative));
            }
        }

        #endregion
    }
}
