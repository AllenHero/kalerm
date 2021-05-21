using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_model
{
    public class mes_grindbeandata
    {
        public string Id { get; set; }
        public string OrderNo { get; set; }
        public string WorkSheetNo { get; set; }
        public string WuId { get; set; }
        public string BarCode { get; set; }
        public string ComponentNumber { get; set; }
        public decimal KMGL { get; set; }
        public decimal GL { get; set; }
        public decimal DW { get; set; }
        public decimal First { get; set; }
        public decimal Second { get; set; }
        public decimal Third { get; set; }
        public decimal FZMin { get; set; }
        public decimal BZ { get; set; }
        public decimal SWZ_071 { get; set; }
        public decimal CSHZL_071 { get; set; }
        public decimal FZ_071 { get; set; }
        public decimal Rate_071 { get; set; }
        public decimal SWZ_03 { get; set; }
        public decimal CSHZL_03 { get; set; }
        public decimal FZ_03 { get; set; }
        public decimal Rate_03 { get; set; }
        public decimal SumRate { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string TenantId { get; set; }
    }
}
