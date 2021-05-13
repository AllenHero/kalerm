using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_model
{
    public class mes_testdata
    {
        public string Id { get; set; }
        public string OrderNo { get; set; }
        public string WorkSheetNo { get; set; }
        public string WuId { get; set; }
        public string BarCode { get; set; }
        public string TesItemName { get; set; }
        public string Value { get; set; }
        public decimal MaxValue { get; set; }
        public decimal MinValue { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string TenantId { get; set; }
        public int IsPass { get; set; }
        public int IsEnd { get; set; }
    }
}
