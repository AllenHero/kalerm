﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_Idal
{
    public abstract class BaseDataAdapter
    {
        /// <summary>
        /// 数据库上下文。
        /// </summary>
        public BaseDatabaseContext Context { get; private set; }

        public BaseDataAdapter(BaseDatabaseContext context)
        {
            this.Context = context;
        }
    }
}
