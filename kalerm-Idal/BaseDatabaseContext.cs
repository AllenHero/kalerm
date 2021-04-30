using kalerm_Idal.BaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_Idal
{
    public abstract partial class BaseDatabaseContext
    {
        public IBaseData BaseData { get; set; }
    }
}
