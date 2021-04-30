using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_sqldal
{
    public class SqlHelper
    {
        //所有的方法都是静态的，不需要创建sqlhelper对象 ，可以直接用类名去调用
        //连接字符串
        static string connstr = "server=.;uid=sa;pwd=123;database=studb;";

        /// <summary>
        /// 用于绑定数据列表操作的，返回的是一个DataTable
        /// </summary>
        /// <param name="sql">传入的sql文本</param>
        /// <param name="type">传入的sql文本的类型</param>
        /// <param name="pms">传入的sql参数对象</param>
        /// <returns></returns>
        public static DataTable GetTable(string sql, CommandType type, params MySqlParameter[] pms)
        {
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.CommandType = type;//指定执行的sql文本是什么类型
            if (pms != null)
            {
                //添加之前判断SqlParameter是否为空，不是空的才能添加
                foreach (MySqlParameter item in pms)
                {
                    if (item != null)
                    {
                        //判断数组里面每个位置的元素是否为null，如果为null则不添加，不为null则照常添加,这样有任何的错误都会第一时间反馈给后台的code
                        cmd.Parameters.Add(item);
                    }
                }
            }
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            conn.Close();
            return dt;
        }

        //加一个重载 预先指定好传入的CommandType  是文本类型，以后如果写的是纯sql语句就调用这个重载就可以了
        public static DataTable GetTable(string sql, params MySqlParameter[] pms)
        {
            return GetTable(sql, CommandType.Text, pms);
        }


        /// <summary>
        /// 返回受影响行数，一般用于增删改操作
        /// </summary>
        /// <param name="sql">sql文本</param>
        /// <param name="type">文本类型</param>
        /// <param name="pms">参数数组</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, CommandType type, params MySqlParameter[] pms)
        {
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.CommandType = type;
            if (pms != null)
            {
                foreach (MySqlParameter item in pms)
                {
                    if (item != null)
                    {
                        cmd.Parameters.Add(item);
                    }
                }
            }
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            return i;
        }

        public static int ExecuteNonQuery(string sql, params MySqlParameter[] pms)
        {
            return ExecuteNonQuery(sql, CommandType.Text, pms);
        }


        /// <summary>
        /// 返回sql中查询出来的表的首行首列
        /// </summary>
        /// <param name="sql">sql文本</param>
        /// <param name="type">文本属性</param>
        /// <param name="pms">参数对象</param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, CommandType type, params MySqlParameter[] pms)
        {
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.CommandType = type;
            if (pms != null)
            {
                foreach (MySqlParameter item in pms)
                {
                    if (item != null)
                    {
                        cmd.Parameters.Add(item);

                    }
                }
            }
            object i = cmd.ExecuteScalar();
            conn.Close();
            return i;
        }

        public static object ExecuteScalar(string sql, params MySqlParameter[] pms)
        {
            return ExecuteScalar(sql, CommandType.Text, pms);
        }

        /// <summary>
        /// 返回一个游标对象
        /// </summary>
        /// <param name="sql">执行命令文本</param>
        /// <param name="type">文本类型</param>
        /// <param name="pms">参数数组对象</param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(string sql, CommandType type, params MySqlParameter[] pms)
        {
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.CommandType = type;
            if (pms != null)
            {
                foreach (MySqlParameter item in pms)
                {
                    if (item != null)
                    {
                        cmd.Parameters.Add(item);
                    }
                }
            }
            MySqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            // sdr.Close();//CommandBehavior.CloseConnection  关闭游标的同时 ，同时也关闭其对应的数据库连接对象    
            return sdr;
        }

        public static MySqlDataReader ExecuteReader(string sql, params MySqlParameter[] pms)
        {
            return ExecuteReader(sql, CommandType.Text, pms);
        }
    }
}
