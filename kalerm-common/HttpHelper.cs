using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_common
{
    public class HttpHelper
    {
        public class ParamApi
        {
            public ParamApi()
            {

            }
            public ParamApi(string url)
            {
                this.url = url;
            }
            public ParamApi(string url, string user, string password)
            {
                this.url = url;
                this.user = user;
                this.password = password;
            }
            public ParamApi(string url, string method)
            {
                this.url = url;
                this.method = method;
            }
            public ParamApi(string url, dynamic data)
            {
                this.url = url;
                this.data = data;
            }
            public ParamApi(string url, string method, dynamic data)
            {
                this.url = url;
                this.method = method;
                this.data = data;
            }
            public ParamApi(string url, string method, dynamic data, string user, string password)
            {
                this.url = url;
                this.method = method;
                this.data = data;
                this.user = user;
                this.password = password;
            }
            public ParamApi(string url, string method, dynamic data, string contentType)
            {
                this.url = url;
                this.method = method;
                this.data = data;
                this.contentType = contentType;
            }
            public ParamApi(string url, string method, dynamic data, string contentType, string user, string password)
            {
                this.url = url;
                this.method = method;
                this.data = data;
                this.user = user;
                this.password = password;
                this.contentType = contentType;
            }
            public ParamApi(string url, string method, dynamic data, string contentType, int timeOut)
            {
                this.url = url;
                this.method = method;
                this.data = data;
                this.contentType = contentType;
                this.timeOut = timeOut;
            }
            public ParamApi(string url, string method, dynamic data, string contentType, string user, string password, int timeOut)
            {
                this.url = url;
                this.method = method;
                this.data = data;
                this.contentType = contentType;
                this.user = user;
                this.password = password;
                this.timeOut = timeOut;
            }
            



            public string url { get; set; }
            /// <summary>
            /// 请求方法（默认为GET）,首字母大写
            /// </summary>
            public string method { get; set; }
            /// <summary>
            /// json格式参数 Get、Delete、Update没有此参数
            /// </summary>
            public dynamic data { get; set; }
            /// <summary>
            /// 传送方式有 例如：application/json、 multipart/form-data  
            /// </summary>
            public string contentType { get; set; }
            public string user { get; set; }
            public string password { get; set; }
            public int timeOut { get; set; }

            public string tokenid { get; set; }
            public string authorzation { get; set; }
            public string authorKey { get; set; }
            public string tenantid { get; set; }
        }


        public static ResponseObject WebApiNoJson(ParamApi paramapi)
        {
            var isCheck = Check(paramapi);
            if (!isCheck.status)
            {
                return isCheck;
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.ExpectContinue = false;
            //client.Timeout = DateTime.Now.AddSeconds(paramapi.timeOut) - DateTime.Now;

            string resultStr = "";

            HttpResponseMessage response;
            string jsonString = JsonConvert.SerializeObject(paramapi.data);  //convert to JSON
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
            HttpContent content = new FormUrlEncodedContent(values);
            content.Headers.ContentType = new MediaTypeHeaderValue(paramapi.contentType);
            if (!string.IsNullOrWhiteSpace(paramapi.tokenid))
            {
                // 设置HTTP头Http Basic认证
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", paramapi.tokenid);
            }

            if (!string.IsNullOrWhiteSpace(paramapi.tenantid))
            {
                client.DefaultRequestHeaders.Add("Tenant-Id", paramapi.tenantid);
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
            if (response.IsSuccessStatusCode)
            {
                return new ResponseObject(true, JsonConvert.DeserializeObject(resultStr));
            }
            else
            {
                return new ResponseObject(false, JsonConvert.DeserializeObject(resultStr));
            }
        }

        public static ResponseObject Check(ParamApi param)
        {
            if (string.IsNullOrEmpty(param.url))
            {
                return new ResponseObject(false, "url不能为空");
            }
            if (string.IsNullOrEmpty(param.method))
            {
                param.method = "Get";
            }
            if (param.data == null)
            {
                param.data = new JObject();
            }
            if (string.IsNullOrWhiteSpace(param.contentType))
            {
                param.contentType = "application/json";
            }
            param.timeOut = param.timeOut == null ? 5 : param.timeOut;
            param.timeOut = param.timeOut <= 0 ? 5 : param.timeOut;

            return new ResponseObject(true);
        }
    }
}
