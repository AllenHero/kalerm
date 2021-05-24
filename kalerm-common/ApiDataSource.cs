using kalerm_model.BaseData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static kalerm_common.HttpHelper;

namespace kalerm_common
{
    public class ApiDataSource
    {
        static string leancAuthServer = ConfigurationManager.AppSettings["bladeAuthServer"].ToString();

        static string code = ConfigurationManager.AppSettings["bladeAuthCode"].ToString();

        public static dynamic GetBladeUserDetail(string tenantId, string id = "", string account = "", string phone = "", string userCode = "", string wechatCode = "", string realName = "", string email = "")
        {
            ParamApi api = new ParamApi();
            api.url = leancAuthServer + "blade-user/detail?tenantId=" + tenantId + "&id=" + id + "&account=" + account + "&phone=" + phone + "&userCode=" + userCode + "&realName=" + realName + "&wechatCode=" + wechatCode + "&email=" + email;
            api.method = "Get";
            api.tokenid = code;
            api.authorKey = "Blade-Auth";
            api.authorzation = "bearer " + GetToken();
            dynamic responseResult = WebApi(api);
            if (Convert.ToBoolean(responseResult.success))
            {
                return responseResult.data;
            }
            return null;
        }

        public static List<dynamic> GetBladeUserList(string tenantId, string id = "", string deptId = "", string name = "")
        {
            ParamApi api = new ParamApi();
            api.url = leancAuthServer + "blade-user/client/user-by-tenant?tenantId=" + tenantId + "&userIds=" + id + "&deptIds=" + deptId + "&userName=" + name;
            api.method = "Get";
            api.tokenid = code;
            api.authorKey = "Blade-Auth";
            api.authorzation = "bearer " + GetToken();
            dynamic responseResult = WebApi(api);
            if (Convert.ToBoolean(responseResult.success))
            {
                dynamic data = responseResult.data;
                return data.ToObject<List<dynamic>>();
            }
            return null;
        }

        public static dynamic WebApi(ParamApi paramapi)
        {
            var isCheck = HttpHelper.Check(paramapi);
            if (!isCheck.status)
            {
                return isCheck;
            }
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.ExpectContinue = false;
            string resultStr = "";
            HttpResponseMessage response;
            HttpContent content = new StringContent(JsonConvert.SerializeObject(paramapi.data));
            content.Headers.ContentType = new MediaTypeHeaderValue(paramapi.contentType);
            if (!string.IsNullOrWhiteSpace(paramapi.tenantid))
            {
                client.DefaultRequestHeaders.Add("Tenant-Id", paramapi.tenantid);
            }
            if (!string.IsNullOrWhiteSpace(paramapi.tokenid))
            {
                // 设置HTTP头Http Basic认证
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", paramapi.tokenid);
            }
            if (!string.IsNullOrWhiteSpace(paramapi.authorKey))
            {
                // 设置HTTP头Http Basic认证
                client.DefaultRequestHeaders.Add(paramapi.authorKey, paramapi.authorzation);
            }
            if (!string.IsNullOrWhiteSpace(paramapi.user) && !string.IsNullOrWhiteSpace(paramapi.password))
            {
                // 设置HTTP头Http Basic认证
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", paramapi.user, paramapi.password))));
            }
            try
            {
                switch (paramapi.method)
                {
                    case "Get": response = client.GetAsync(paramapi.url).Result; break;
                    case "Put": response = client.PutAsync(paramapi.url, content).Result; break;
                    case "Delete": response = client.DeleteAsync(paramapi.url).Result; break;
                    case "Post": response = client.PostAsync(paramapi.url, content).Result; break;
                    case "Send": response = client.SendAsync(new HttpRequestMessage()).Result; break;
                    case "Update": response = client.GetAsync(paramapi.url).Result; break;
                    default: response = client.GetAsync(paramapi.url).Result; break;
                }
            }
            catch (Exception ex)
            {

                return new ResponseObject(false, ex.Message);
            }
            if (response != null)
            {
                resultStr = response.Content.ReadAsStringAsync().Result;
            }
            return JsonConvert.DeserializeObject(resultStr);
        }

        public static dynamic GetToken()
        {
            string UserCode = ConfigHelper.GetSettingString("UserCode");
            string Password = ConfigHelper.GetSettingString("Password");
            string TenantId = ConfigHelper.GetSettingString("TenantId");
            dynamic token = Startup_Auth.getLoginToken(UserCode, Password, TenantId);
            return token;
        }
    }
}
