using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_model
{
    public class console_wuarrange
    {
        public string WuArrangeId { get; set; }
        public string WorkSheetNo { get; set; }
        public string ProcessWuId { get; set; }
        public string wuid { get; set; }
        public string wuname { get; set; }
        public string ProductName { get; set; }
        public int? PlanCount { get; set; }
        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }
        public DateTime? Date { get; set; }
        public string ShiftId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string TenantId { get; set; }
        public string CreatePerson { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdatePerson { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool isCheck { get; set; }
    }
}
