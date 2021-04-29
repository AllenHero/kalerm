using kalerm_Idal;
using kalerm_sqldal.BaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_sqldal
{
    public partial class SQLDatabaseContext : BaseDatabaseContext
    {

        public SQLDatabaseContext()
        {

            #region 系统设置部分

            #endregion 系统设置部分

            #region 基础数据部分
            BaseData = new SQLBaseData(this);
            #endregion

        }

    }
}
