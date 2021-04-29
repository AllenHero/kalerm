﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_common.Extensions
{
    public static class DataRowHelper
    {
        public static string ToString(this DataRow row, string columnName)
        {
            return row.Field<string>(columnName);
        }

        public static decimal? ToDecimal(this DataRow row, string columnName)
        {
            return row.Field<decimal?>(columnName);
        }

        public static DateTime? ToDateTime(this DataRow row, string columnName)
        {
            return row.Field<DateTime?>(columnName);
        }

        public static int? ToInt32(this DataRow row, string columnName)
        {
            return row.Field<int?>(columnName);
        }
    }
}
