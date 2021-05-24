using kalerm_Idal;
using kalerm_sqldal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_bll
{
    public abstract class BaseBusiness
    {
        /// <summary>
        /// 数据库上下文。
        /// </summary>
        protected BaseDatabaseContext Context { get; private set; }

        public BaseBusiness()
        {
            ///如存在多种数据库，则需要在这里使用策略模式。
            Context = new SQLDatabaseContext();
        }
    }
}
