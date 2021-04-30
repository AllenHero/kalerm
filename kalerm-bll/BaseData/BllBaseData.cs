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
        public List<ReportBaseModel> GetLineNo(bool IsQueryAll)
        {
            List<ReportBaseModel> reslut = new List<ReportBaseModel>();
            try
            {
                reslut = Context.BaseData.GetLineNo(IsQueryAll);
            }
            catch (Exception ex)
            {
                
            }
            return reslut;
        }
    }
}
