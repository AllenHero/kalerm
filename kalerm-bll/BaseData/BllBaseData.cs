using kalerm_model;
using kalerm_model.BaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_bll.BaseData
{
    public class BllBaseData : BaseBusiness
    {
        public List<ReportBaseModel> GetLineNo()
        {
            List<ReportBaseModel> reslut = new List<ReportBaseModel>();
            try
            {
                reslut = Context.BaseData.GetLineNo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reslut;
        }

        public List<WorkSheet> GetWorkSheet()
        {
            List<WorkSheet> result = new List<WorkSheet>();
            try
            {
                result = Context.BaseData.GetWorkSheet();

            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
