using kalerm_common.Extensions;
using kalerm_Idal;
using kalerm_Idal.BaseData;
using kalerm_model.BaseData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_sqldal.BaseData
{
    public class SQLBaseData : BaseDataAdapter, IBaseData
    {
        public SQLBaseData(BaseDatabaseContext context) : base(context) { }

        public List<ReportBaseModel> GetLineNo(bool IsQueryAll)
        {
            List<ReportBaseModel> list = new List<ReportBaseModel>();
            string sql = string.Format(@"select CustomerCode as [Key],CustomerName as Value from sys_customer");
            try
            {
                DataSet ds = new DataSet();
                using (DbConnection dbConnection = base.Context.CreateConnection())
                {
                    SqlCommand sqlCommand = (SqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    sqlCommand.CommandText = sql;
                    SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                    da.Fill(ds);
                    sqlCommand.Dispose();
                    dbConnection.Close();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    list = Common.DataTableConvertList<ReportBaseModel>(ds.Tables[0]);
                if (IsQueryAll)
                    list.Insert(0, new ReportBaseModel { Key = "全部", Value = "" });
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
