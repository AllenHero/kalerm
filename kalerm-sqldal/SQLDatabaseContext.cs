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
            BaseData = new SQLBaseData(this);
        }
    }
}
