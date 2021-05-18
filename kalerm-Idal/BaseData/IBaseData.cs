using kalerm_model;
using kalerm_model.BaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_Idal.BaseData
{
    public interface IBaseData
    {
        List<ReportBaseModel> GetLineNo();

        List<WorkSheet> GetWorkSheet();

        List<base_wu> GetBaseWu(string ProductCode);

        List<base_wutest> GetBaseWuTest(string WuId, out bool isOK);
    }
}
