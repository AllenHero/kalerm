using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_model
{
    public class worksheet 
    {
        public string WorkSheetId { get; set; }
        public string ERPOrderNo { get; set; }
        public string OrderNo { get; set; }
        public int Sort { get; set; }
        public string WorkSheetNo { get; set; }
        public string ParentWorkSheetNo { get; set; }
        public int PlanCount { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductSpec { get; set; }
        public int? WorkSheetType { get; set; }
        public string CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public int? LowerLimit { get; set; }
        public int? UpperLimit { get; set; }
        public int InBatches { get; set; }
        public string WorkShopId { get; set; }
        public string WorkShopCode { get; set; }
        public string WorkShopName { get; set; }
        public string LineId { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public decimal? StandardUph { get; set; }
        public DateTime SchedulingDate { get; set; }
        public DateTime SubmitDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime PlanDate { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? ActualUph { get; set; }
        public int CompletedCount { get; set; }
        public string CreatePerson { get; set; }
        public string PlanPerson { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatePerson { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string TenantId { get; set; }
        public int? PassQty { get; set; }
        public int? NgQty { get; set; }
        public int RestCount { get; set; }
        public string Remark { get; set; }
        public string ProcessId { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string PreWorkSheetNo { get; set; }
        public int QualifiedCount { get; set; }
        public int UnQualifiedCount { get; set; }
        public int InCount { get; set; }
        public string MainNo { get; set; }
        public string StartNo { get; set; }
        public string EndNo { get; set; }
        public string MaterialCode { get; set; }
        public int Dispatching { get; set; }
        public string Customer { get; set; }
        public int Seq { get; set; }

        public string OrganizeId { get; set; }
        public string OrganizeCode { get; set; }
        public string OrganizeName { get; set; }

        public string ParentOrganizeId { get; set; }
        public string ParentOrganizeCode { get; set; }
        public string ParentOrganizeName { get; set; }

        //public int TotalHoursCount { get; set; }
        public string ShiftName { get; set; }
    }
}
