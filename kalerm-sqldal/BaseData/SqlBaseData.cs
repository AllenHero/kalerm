﻿using kalerm_common.Extensions;
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

        public List<WorkSheet> GetWorkSheetList()
        {
            List<WorkSheet> list = new List<WorkSheet>();
            string sql = string.Format(@"select * from `kalerm-app-aps`.`worksheet`");
            try
            {
                DataTable dt = new DataTable();
                dt = GetDataTable(sql, ConnectionType.aps);
                if (dt != null && dt.Rows.Count > 0)
                    list = Common.DataTableConvertList<WorkSheet>(dt);
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<base_wu> GetBaseWuList(string ProductCode)
        {
            List<base_wu> list = new List<base_wu>();
            string sql = string.Format(@"select c.* from `kalerm-base-model`.`base_productionprocess` a left join `kalerm-base-model`.`mes_processwu` b on a.processid=b.processid left join `kalerm-base-model`.`base_wu` c on c.wuid=b.wuid where a.productcode='" + ProductCode + "'");
            try
            {
                DataTable dt = new DataTable();
                dt = GetDataTable(sql, ConnectionType.aps);
                if (dt != null && dt.Rows.Count > 0)
                    list = Common.DataTableConvertList<base_wu>(dt);
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<base_wutest> GetBaseWuTestList(string WuId, out bool isOK)
        {
            isOK = false;
            List<base_wutest> list = new List<base_wutest>();
            string sql = string.Format(@"select a.* from `kalerm-base-model`.`base_wutest` a  where a.wuid='" + WuId + "'");
            try
            {
                DataTable dt = new DataTable();
                dt = GetDataTable(sql, ConnectionType.model);
                if (dt != null && dt.Rows.Count > 0)
                    list = Common.DataTableConvertList<base_wutest>(dt);
                isOK = true;
                return list;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public WorkSheet GetWorkSheet(string WorkSheetNo)
        {
            WorkSheet model = new WorkSheet();
            string sql = string.Format(@"select a.* from `kalerm-app-aps`.`worksheet` a  where a.WorkSheetNo='" + WorkSheetNo + "'");
            try
            {
                DataTable dt = new DataTable();
                dt = GetDataTable(sql, ConnectionType.aps);
                if (dt != null && dt.Rows.Count > 0)
                    model = Common.DataTableToModel<WorkSheet>(dt);
                return model;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public base_productionprocess GetProductionProcess(string ProcessId)
        {
            base_productionprocess model = new base_productionprocess();
            string sql = string.Format(@"select a.* from `kalerm-base-model`.`base_productionprocess` a  where a.processid='" + ProcessId + "'");
            try
            {
                DataTable dt = new DataTable();
                dt = GetDataTable(sql, ConnectionType.model);
                if (dt != null && dt.Rows.Count > 0)
                    model = Common.DataTableToModel<base_productionprocess>(dt);
                return model;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region sql语句

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, ConnectionType type)
        {
            try
            {
                DataTable dt = new DataTable();
                using (DbConnection dbConnection = base.Context.CreateConnection(type))
                {
                    //dbConnection.Open();
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    sqlCommand.CommandText = sql;
                    MySqlDataAdapter da = new MySqlDataAdapter(sqlCommand);
                    da.Fill(dt);
                    sqlCommand.Dispose();
                    dbConnection.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, ConnectionType type, params MySqlParameter[] pms)
        {
            try
            {
                DataTable dt = new DataTable();
                using (DbConnection dbConnection = base.Context.CreateConnection(type))
                {
                    //dbConnection.Open();
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    sqlCommand.CommandText = sql;
                    if (pms != null)
                    {
                        foreach (MySqlParameter item in pms)
                        {
                            if (item != null)
                            {
                                sqlCommand.Parameters.Add(item);
                            }
                        }
                    }
                    MySqlDataAdapter da = new MySqlDataAdapter(sqlCommand);
                    da.Fill(dt);
                    sqlCommand.Dispose();
                    dbConnection.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, ConnectionType type)
        {
            try
            {
                using (DbConnection dbConnection = base.Context.CreateConnection(type))
                {
                    //dbConnection.Open();
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    int i = sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    dbConnection.Close();
                    return i;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }  
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, ConnectionType type, params MySqlParameter[] pms)
        {
            try
            {
                using (DbConnection dbConnection = base.Context.CreateConnection(type))
                {
                    //dbConnection.Open();
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    if (pms != null)
                    {
                        foreach (MySqlParameter item in pms)
                        {
                            if (item != null)
                            {
                                sqlCommand.Parameters.Add(item);
                            }
                        }
                    }
                    int i = sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    dbConnection.Close();
                    return i;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, ConnectionType type)
        {
            try
            {
                using (DbConnection dbConnection = base.Context.CreateConnection(type))
                {
                    //dbConnection.Open();
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    object i = sqlCommand.ExecuteScalar();
                    sqlCommand.Dispose();
                    dbConnection.Close();
                    return i;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, ConnectionType type, params MySqlParameter[] pms)
        {
            try
            {
                using (DbConnection dbConnection = base.Context.CreateConnection(type))
                {
                    //dbConnection.Open();
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    if (pms != null)
                    {
                        foreach (MySqlParameter item in pms)
                        {
                            if (item != null)
                            {
                                sqlCommand.Parameters.Add(item);
                            }
                        }
                    }
                    object i = sqlCommand.ExecuteScalar();
                    sqlCommand.Dispose();
                    dbConnection.Close();
                    return i;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 返回游标对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public MySqlDataReader ExecuteReader(string sql, ConnectionType type)
        {
            try
            {
                using (DbConnection dbConnection = base.Context.CreateConnection(type))
                {
                    //dbConnection.Open();
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    MySqlDataReader sdr = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    sdr.Close();
                    sqlCommand.Dispose();
                    dbConnection.Close();
                    return sdr;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 返回游标对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public MySqlDataReader ExecuteReader(string sql, ConnectionType type, params MySqlParameter[] pms)
        {
            try
            {
                using (DbConnection dbConnection = base.Context.CreateConnection(type))
                {
                    //dbConnection.Open();
                    MySqlCommand sqlCommand = (MySqlCommand)base.Context.CreateCommand(sql, dbConnection);
                    if (pms != null)
                    {
                        foreach (MySqlParameter item in pms)
                        {
                            if (item != null)
                            {
                                sqlCommand.Parameters.Add(item);
                            }
                        }
                    }
                    MySqlDataReader sdr = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    sdr.Close();
                    sqlCommand.Dispose();
                    dbConnection.Close();
                    return sdr;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
