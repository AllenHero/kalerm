using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_model
{
    public class base_wutest
    {
        public string wutestid { get; set; }
        public string wuid { get; set; }
        public string testno { get; set; }
        public string testitemname { get; set; }
        public string testtype { get; set; }
        public string testresulttype { get; set; }
        public string remark { get; set; }
        public string tenantid { get; set; }
        public DateTime createtime { get; set; }
        public string createuser { get; set; }
        public DateTime updatetime { get; set; }
        public string updateuser { get; set; }
        public decimal maxvalue { get; set; }
        public decimal minvalue { get; set; }

    }
}
