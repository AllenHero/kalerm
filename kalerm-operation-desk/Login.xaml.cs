using kalerm_common;
using kalerm_model.BaseData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
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
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        //用户信息及权限
        public UserInfo UserInfo = new UserInfo();

        private System.ComponentModel.BackgroundWorker LoginWorker = null;

        public Login()
        {
            InitializeComponent();
            this.Loaded += Login_Loaded;
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            LoginWorker = new System.ComponentModel.BackgroundWorker();
            LoginWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(LoginWorker_DoWork);
            LoginWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(LoginWorker_RunWorkerCompleted);
        }

        //登录
        private void imageLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            if (textPersonID.Text + "" == "")
            {
                ReMessageBox.Show("登陆名不能为空");
                textPersonID.Focus();
                return;
            }
            if (textPassword.Password + "" == "")
            {
                ReMessageBox.Show("密码不能为空");
                textPassword.Focus();
                return;
            }
            UserInfo userInfo = new UserInfo()
            {
                userCode = textPersonID.Text,
                passWord = textPassword.Password
            };
            InputControlEnable(false);
            LoginWorker.RunWorkerAsync(userInfo);
            loading.Visibility = System.Windows.Visibility.Visible;
            loading.LableText = "登录中……";
        }

        void LoginWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                UserInfo userInfo = e.Argument as UserInfo;
                LoginSystem(userInfo);
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        private void LoginSystem(UserInfo userInfo)
        {
            try
            {
                var UserCode = userInfo.userCode;
                var Password = userInfo.passWord;
                var TenantId = ConfigurationManager.AppSettings["TenantId"].ToString();
                if (string.IsNullOrEmpty(TenantId))
                {
                    ReMessageBox.Show("租户不能为空");
                }
                var tokenId = "";
                if (tokenId == null || tokenId == "")
                {
                    tokenId = Startup_Auth.getLoginToken(UserCode, Password, TenantId);
                }
                string tokeninfo = Startup_Auth.getUserInfobyToken(tokenId);
                JObject jtoken = (JObject)JsonConvert.DeserializeObject(tokeninfo);
                //验证token是否有效
                if (jtoken["code"].ToString() != "200")
                {
                    ReMessageBox.Show("登录失败，请联系管理员");
                    textPassword.Focus();
                    return;
                }
                JwtSecurityToken tokenObject = new JwtSecurityTokenHandler().ReadJwtToken(tokenId);
                string tenantId = tokenObject.Claims.FirstOrDefault(x => x.Type == "tenant_id").Value;
                string userId = tokenObject.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
                string userCode = tokenObject.Claims.FirstOrDefault(x => x.Type == "user_code").Value;
                string roleName = tokenObject.Claims.FirstOrDefault(x => x.Type == "role_name").Value;
                string phone = tokenObject.Claims.FirstOrDefault(x => x.Type == "phone").Value;
                string avatar = tokenObject.Claims.FirstOrDefault(x => x.Type == "avatar").Value;
                string realName = tokenObject.Claims.FirstOrDefault(x => x.Type == "real_name").Value;
                string accessToken = tokenId;
                string roleId = tokenObject.Claims.FirstOrDefault(x => x.Type == "role_id").Value;
                string deptId = tokenObject.Claims.FirstOrDefault(x => x.Type == "dept_id").Value;
                UserInfo.tenantId = tenantId;
                UserInfo.userId = userId;
                UserInfo.userCode = userCode;
                UserInfo.roleName = roleName;
                UserInfo.phone = phone;
                UserInfo.avatar = avatar;
                UserInfo.realName = realName;
                UserInfo.accessToken = accessToken;
                UserInfo.roleId = roleId;
                UserInfo.deptId = deptId;
                UserInfo.passWord = textPassword.Password;
                ConfigHelper.UpdateSettingString("UserCode", userCode);
                ConfigHelper.UpdateSettingString("PassWord", UserInfo.passWord);
            }
            catch 
            {
                throw new Exception("获取数据异常，请检查网络后重试");
            }
        }

        void LoginWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    if (e.Result != null && e.Result is string)
                    {
                        ReMessageBox.Show(e.Result.ToString());
                    }
                    else
                    {
                        if (UserInfo != null && UserInfo.userCode != null)
                        {
                            MainWindow.UserInfo = UserInfo;
                            UserInfo.passWord = textPassword.Password + "";
                            MainWindow aChild = new MainWindow();
                            aChild.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            aChild.Show();
                            // 关闭自己(父窗体) 
                            this.Close();
                        }
                        else
                        {
                            ReMessageBox.Show("登录失败");
                            imageLogin.Source = new BitmapImage(new Uri("/Image/LandWindow_2.png", UriKind.Relative));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReMessageBox.Show("登录失败" + ex.Message);
                imageLogin.Source = new BitmapImage(new Uri("/Image/LandWindow_2.png", UriKind.Relative));
            }
            finally
            {
                InputControlEnable(true);
                loading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void imageLogin_MouseEnter(object sender, MouseEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            imageLogin.Source = new BitmapImage(new Uri("/Image/LandWindow_4.png", UriKind.Relative));
        }

        private void imageLogin_MouseLeave(object sender, MouseEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            imageLogin.Source = new BitmapImage(new Uri("/Image/LandWindow_2.png", UriKind.Relative));
        }

        #region imageClose

        private void imageClose_MouseEnter(object sender, MouseEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            imageClose.Source = new BitmapImage(new Uri("/Image/Exit_2.png", UriKind.Relative));
        }

        private void imageClose_MouseLeave(object sender, MouseEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            imageClose.Source = new BitmapImage(new Uri("/Image/Exit_1.png", UriKind.Relative));
        }

        private void imageClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            this.Close();
        }

        #endregion

        #region imageCancel

        private void imageCancel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            imageCancel.Source = new BitmapImage(new Uri("/Image/LandWindow_5.png", UriKind.Relative));
        }

        private void imageCancel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            imageCancel.Source = new BitmapImage(new Uri("/Image/LandWindow_3.png", UriKind.Relative));
        }

        private void imageCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (loading.Visibility == System.Windows.Visibility.Visible) return;
            this.Close();
        }

        #endregion

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (loading.Visibility == System.Windows.Visibility.Visible) return;
                if (e.Key == Key.Enter)
                {
                    imageLogin_MouseLeftButtonDown(null, null);
                }
                else if (e.Key == Key.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void InputControlEnable(bool IsEnable)
        {
            textPersonID.IsEnabled = IsEnable;
            textPassword.IsEnabled = IsEnable;
        }
    }
}
