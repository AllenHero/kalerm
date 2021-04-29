using kalerm_Idal;
using kalerm_model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_sqldal
{
    public partial class SQLDatabaseContext : BaseDatabaseContext
    {
        #region General Define
        /// <summary>
        /// 获取数据源连接字符串。
        /// </summary>
        /// <returns></returns>
        public override string ConnectionString()
        {
            return string.Empty;
        }
        /// <summary>
        /// 获取数据源连接字符串。
        /// </summary>
        public override string ConnectionString(ConnectionType connectionType)
        {
            switch (connectionType)
            {
                default:
                case ConnectionType.aps:
                    return ConfigurationManager.ConnectionStrings["aps"].ToString();
                case ConnectionType.mes:
                    return ConfigurationManager.ConnectionStrings["mes"].ToString();
                case ConnectionType.model:
                    return ConfigurationManager.ConnectionStrings["model"].ToString();
                case ConnectionType.business:
                    return ConfigurationManager.ConnectionStrings["business"].ToString();
            }
        }

        /// <summary>
        /// 获取一个新的数据库连接。
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection(ConnectionType connectionType)
        {
            SqlConnection conn = new SqlConnection(ConnectionString(connectionType));
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 获取一个新的数据库连接。
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString(ConnectionType.aps));
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 获取指定的数据源字符串。
        /// </summary>
        /// <param name="connectionConfigurationName">数据源配置节名称。</param>
        /// <returns>数据源连接字符串</returns>
        public override string GetConnectionString(string connectionConfigurationName)
        {
            return ConfigurationManager.ConnectionStrings[connectionConfigurationName].ToString();
        }

        /// <summary>
        /// 创建 DbCommand 对象。
        /// </summary>
        /// <param name="cmdText">SQL 命令。</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public override DbCommand CreateCommand(string cmdText, DbConnection conn)
        {
            return new SqlCommand(cmdText, (SqlConnection)conn);
        }

        /// <summary>
        /// 创建 DbCommand 对象。
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public override DbCommand CreateCommand(DbConnection conn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = (SqlConnection)conn;
            return cmd;
        }

        #endregion
    }
}
