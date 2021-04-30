using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static kalerm_common.HttpHelper;

namespace kalerm_common
{
    public class Startup_Auth
    {
        public static string getLoginToken(string username, string password, string tenantid)
        {
            string responseResult = String.Empty;
            try
            {
                string leancAuthServer = ConfigurationManager.AppSettings["bladeAuthServer"].ToString();
                string code = ConfigurationManager.AppSettings["bladeAuthCode"].ToString();
                ParamApi api = new ParamApi();
                api.url = leancAuthServer + "blade-auth/oauth/token";
                api.method = "Post";
                api.contentType = "application/x-www-form-urlencoded";
                var requestdata = new
                {
                    grant_type = "password",
                    username = username,
                    password = password,
                    scope = "all"
                };
                api.tenantid = tenantid;
                api.data = requestdata;
                api.tokenid = code;
                api.authorKey = "";
                var tokeninfo = HttpHelper.WebApiNoJson(api);
                if (tokeninfo.status)
                {
                    JObject jtoken = (JObject)tokeninfo.rows;
                    if (jtoken["access_token"] != null)
                    {
                        responseResult = jtoken["access_token"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                responseResult = "{\"code\": 401,\"success\": false}";
            }
            return responseResult;
        }

        public static dynamic getUserInfobyToken(string token)
        {
            string responseResult = String.Empty;
            try
            {
                string leancAuthServer = ConfigurationManager.AppSettings["bladeAuthServer"].ToString();
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(leancAuthServer + "blade-auth/oauth/user-info");
                req.Method = "Get";
                req.Headers.Add(HttpRequestHeader.Authorization, "bearer " + token);
                HttpWebResponse bladeRespone = (HttpWebResponse)req.GetResponse();
                if (bladeRespone != null && bladeRespone.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader sr;
                    using (sr = new StreamReader(bladeRespone.GetResponseStream()))
                    {
                        responseResult = sr.ReadToEnd();
                    }
                    sr.Close();
                }
                bladeRespone.Close();

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                responseResult = "{\"code\": 401,\"success\": false}";
            }
            return responseResult;
        }
    }
}
