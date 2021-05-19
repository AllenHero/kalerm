﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_common.Extensions
{
    public static class ExtendConversion
    {
        #region 转换Int16,Int32,Int64

        /// <summary>
        /// 转换int16
        /// 出错返回默认值
        /// 默认值默认为0
        /// </summary>
        /// <param name="value">要转换值</param>
        /// <param name="defaultValue">默认值,出错返回默认值</param>
        /// <returns></returns>
        public static Int16 ToInt16(this object value, params Int16[] defaultValue)
        {
            Int16 revalue = 0;
            if (defaultValue.Length > 0)
            {
                try
                {
                    revalue = System.Convert.ToInt16(value);//如果为null肯定转换出错
                }
                catch (Exception ex)
                {
                    revalue = defaultValue[0];
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                try
                {
                    revalue = System.Convert.ToInt16(value);
                }
                catch (Exception ex)
                {
                    revalue = 0;
                    throw new Exception(ex.Message);
                }
            }
            return revalue;
        }

        /// <summary>
        /// 转换int32
        /// 出错返回默认值
        /// 默认值默认为0
        /// </summary>
        /// <param name="value">要转换值</param>
        /// <param name="defaultValue">默认值,出错返回默认值</param>
        /// <returns></returns>
        public static Int32 ToInt32(this object value, params Int32[] defaultValue)
        {
            Int32 revalue = 0;
            if (defaultValue.Length > 0)
            {
                try
                {
                    revalue = System.Convert.ToInt32(value);//如果为null肯定转换出错
                }
                catch (Exception ex)
                {
                    revalue = defaultValue[0];
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                try
                {
                    revalue = System.Convert.ToInt32(value);
                }
                catch (Exception ex)
                {
                    revalue = 0;
                    throw new Exception(ex.Message);
                }
            }
            return revalue;
        }

        /// <summary>
        /// 转换Int64
        /// 出错返回默认值
        /// 默认值默认为0
        /// </summary>
        /// <param name="value">要转换值</param>
        /// <param name="defaultValue">默认值,出错返回默认值</param>
        /// <returns></returns>
        public static Int64 ToInt64(this object value, params Int64[] defaultValue)
        {
            Int64 revalue = 0;
            if (defaultValue.Length > 0)
            {
                try
                {
                    revalue = System.Convert.ToInt64(value);//如果为null肯定转换出错
                }
                catch (Exception ex)
                {
                    revalue = defaultValue[0];
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                try
                {
                    revalue = System.Convert.ToInt64(value);
                }
                catch (Exception ex)
                {
                    revalue = 0;
                    throw new Exception(ex.Message);
                }
            }
            return revalue;
        }

        #endregion

        #region 转换Double

        /// <summary>
        /// 转换Double
        /// 出错返回默认值
        /// 默认值默认为0
        /// </summary>
        /// <param name="value">要转换值</param>
        /// <param name="defaultValue">默认值,出错返回默认值</param>
        /// <returns></returns>
        public static Double ToDouble(this object value, params Double[] defaultValue)
        {
            Double revalue = 0;
            try
            {
                revalue = System.Convert.ToDouble(value);
            }
            catch (Exception ex)
            {
                if (defaultValue.Length > 0)
                {
                    revalue = defaultValue[0];
                    throw new Exception(ex.Message);
                }
            }
            return revalue;
        }

        #endregion

        #region 转换Decimal

        /// <summary>
        /// 转换Decimal
        /// 出错返回默认值
        /// 默认值默认为0
        /// </summary>
        /// <param name="value">要转换值</param>
        /// <param name="defaultValue">默认值,出错返回默认值</param>
        /// <returns></returns>
        public static Decimal ToDecimal(this object value, params Decimal[] defaultValue)
        {
            Decimal revalue = 0;
            try
            {
                revalue = System.Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {
                if (defaultValue.Length > 0)
                {
                    revalue = defaultValue[0];
                    throw new Exception(ex.Message);
                }
            }
            return revalue;
        }

        #endregion

        #region 转换Boolean

        /// <summary>
        /// 转换Boolean
        /// 要转换值为1，结果为true ;  要转换值为0，结果为false ; 
        /// </summary>
        /// <param name="value">要转换值</param>
        /// <returns></returns>
        public static Boolean ToBoolean(this object value)
        {
            Boolean revalue = false;
            try
            {
                if (value.ToString() == "1")
                {
                    revalue = true;
                }
                else if (value.ToString() == "0")
                {
                    revalue = false;
                }
                else if (value.ToString().Trim().ToLower() == "true")
                {
                    revalue = true;
                }
                else if (value.ToString().Trim().ToLower() == "false")
                {
                    revalue = false;
                }
            }
            catch (Exception ex)
            {
                revalue = false;
                throw new Exception(ex.Message);
            }
            return revalue;
        }

        #endregion

        #region 转换Base64数字编码

        /// <summary>
        /// 转换Base64数字编码
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] value)
        {
            try
            {
                return System.Convert.ToBase64String(value);
            }
            catch (Exception ex)
            {
                return string.Empty;
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
