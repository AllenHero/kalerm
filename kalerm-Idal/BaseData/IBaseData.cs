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
        List<WorkSheet> GetWorkSheetList();

        List<base_wu> GetBaseWuList(string ProductCode);

        List<base_wutest> GetBaseWuTestList(string WuId, out bool isOK);

        WorkSheet GetWorkSheet(string WorkSheetNo);

        base_productionprocess GetProductionProcess(string ProcessId);
    }
}
