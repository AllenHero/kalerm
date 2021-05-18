using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_model
{
    public class base_productionprocess
    {
        public string processid { get; set; }
        public string processno { get; set; }
        public string processname { get; set; }
        public string processtypeid { get; set; }
        public string productcode { get; set; }
        public string productmethod { get; set; }
        public double? changelinetime { get; set; }
        public string changeunit { get; set; }
        public double? wutaketime { get; set; }
        public double? wuuph { get; set; }
        public int? wuworkernumber { get; set; }
        public double? balancerate { get; set; }
        public int islock { get; set; }
        public string remark { get; set; }
        public string tenantid { get; set; }
        public DateTime? createtime { get; set; }
        public string createuser { get; set; }
        public DateTime? updatetime { get; set; }
        public string updateuser { get; set; }
        public int IsDeleted { get; set; }
    }
}
