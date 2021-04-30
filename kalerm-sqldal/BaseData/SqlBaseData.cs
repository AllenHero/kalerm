using kalerm_common.Extensions;
using kalerm_Idal;
using kalerm_Idal.BaseData;
using kalerm_model;
using kalerm_model.BaseData;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_sqldal.BaseData
{
    public class SQLBaseData : BaseDataAdapter, IBaseData
    {
        public SQLBaseData(BaseDatabaseContext context) : base(context) { }

        public List<ReportBaseModel> GetLineNo()
        {
            List<ReportBaseModel> list = new List<ReportBaseModel>();
            string sql = string.Format(@"select CustomerCode as Code,CustomerName as Name from sys_customer");
            try
            {
                DataSet ds = new DataSet();
                using (DbConnection dbConnection = base.Context.CreateConnection(ConnectionType.model))
                {
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    sqlCommand.CommandText = sql;
                    MySqlDataAdapter da = new MySqlDataAdapter(sqlCommand);
                    da.Fill(ds);
                    sqlCommand.Dispose();
                    dbConnection.Close();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    list = Common.DataTableConvertList<ReportBaseModel>(ds.Tables[0]);
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
