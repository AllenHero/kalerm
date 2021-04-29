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
        public UserInfo UserInfo = new UserInfo();

        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
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

            var UserCode = textPersonID.Text;
            var Password = textPassword.Password;
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
            if (tokenObject!=null && Convert.ToString(tokenObject.Claims.FirstOrDefault(x => x.Type == "user_id").Value)!=null)
            {
                MainWindow.UserInfo = UserInfo;
                MainWindow aChild = new MainWindow();
                aChild.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                aChild.Show();
                // 关闭自己(父窗体) 
                this.Close();
            }
            else
            {
                ReMessageBox.Show("登录失败");
            }

        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
