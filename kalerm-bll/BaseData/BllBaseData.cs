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
        public List<worksheet> GetWorkSheetList(string TenantId)
        {
            List<worksheet> result = new List<worksheet>();
            try
            {
                result = Context.BaseData.GetWorkSheetList(TenantId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<base_wu> GetBaseWuList(string TenantId)
        {
            List<base_wu> result = new List<base_wu>();
            try
            {
                result = Context.BaseData.GetBaseWuList(TenantId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<base_wutest> GetBaseWuTestList(string WuId,string TenantId, out bool isOK)
        {
            isOK = false;
            List<base_wutest> result = new List<base_wutest>();
            try
            {
                result = Context.BaseData.GetBaseWuTestList(WuId, TenantId, out isOK);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public worksheet GetWorkSheet(string WorkSheetNo,string TenantId)
        {
            worksheet result = new worksheet();
            try
            {
                result = Context.BaseData.GetWorkSheet(WorkSheetNo, TenantId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public base_productionprocess GetProductionProcess(string ProcessId,string TenantId)
        {
            base_productionprocess result = new base_productionprocess();
            try
            {
                result = Context.BaseData.GetProductionProcess(ProcessId, TenantId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int SaveTestData(List<mes_testdata> mes_testdata, int ISPASS)
        {
            int result = -2;
            if (mes_testdata.Count < 1)//没有测试数据
                return 0;
            try
            {
                result = Context.BaseData.SaveTestData(mes_testdata, ISPASS);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int SaveGrindBeanData(List<mes_grindbeandata> mes_grindbeandata)
        {
            int result = 0;
            try
            {
                result = Context.BaseData.SaveGrindBeanData(mes_grindbeandata);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<mes_grindbeandata> GetGrindBeanDataList(string WuId,string WorkSheetNo,string TenantId)
        {
            List<mes_grindbeandata> result = new List<mes_grindbeandata>();
            try
            {
                result = Context.BaseData.GetGrindBeanDataList(WuId, WorkSheetNo, TenantId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<console_wuarrange> GetConsoleWuArrangeList(string UserId, string TenantId, string NowDate)
        {
            List<console_wuarrange> result = new List<console_wuarrange>();
            try
            {
                result = Context.BaseData.GetConsoleWuArrangeList(UserId, TenantId, NowDate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
