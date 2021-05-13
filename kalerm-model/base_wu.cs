using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_model
{
    public class base_wu 
    {
        public string wuid { get; set; }
        public string wuno { get; set; }
        public string wuname { get; set; }
        public string wutype { get; set; }
        public double? wutaketime { get; set; }
        public string wutaketimeunit { get; set; }
        public string wuneedcache { get; set; }
        public int? wuworkernumber { get; set; }
        public double? wuuph { get; set; }
        public int? isEnable { get; set; }
        public string remark { get; set; }
        public string tenantid { get; set; }
        public DateTime? createtime { get; set; }
        public string createuser { get; set; }
        public DateTime? updatetime { get; set; }
        public string updateuser { get; set; }
    }
}
