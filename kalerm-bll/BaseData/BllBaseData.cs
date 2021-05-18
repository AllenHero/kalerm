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
        public List<worksheet> GetWorkSheetList()
        {
            List<worksheet> result = new List<worksheet>();
            try
            {
                result = Context.BaseData.GetWorkSheetList();
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<base_wu> GetBaseWuList(string ProductCode)
        {
            List<base_wu> result = new List<base_wu>();
            try
            {
                result = Context.BaseData.GetBaseWuList(ProductCode);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<base_wutest> GetBaseWuTestList(string WuId, out bool isOK)
        {
            isOK = false;
            List<base_wutest> result = new List<base_wutest>();
            try
            {
                result = Context.BaseData.GetBaseWuTestList(WuId, out isOK);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public worksheet GetWorkSheet(string WorkSheetNo)
        {
            worksheet result = new worksheet();
            try
            {
                result = Context.BaseData.GetWorkSheet(WorkSheetNo);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public base_productionprocess GetProductionProcess(string ProcessId)
        {
            base_productionprocess result = new base_productionprocess();
            try
            {
                result = Context.BaseData.GetProductionProcess(ProcessId);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
