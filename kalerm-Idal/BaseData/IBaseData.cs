using kalerm_model.BaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_Idal.BaseData
{
    public interface IBaseData
    {
        List<ReportBaseModel> GetLineNo(bool IsQueryAll);

    }

    /// <summary>
    /// 关于美的设备信息的接口定义
    /// </summary>

    public interface IBase<T>
    {
        bool AddEntity(T entity);
        bool DelEntity(T entity);
        bool UpdateEntity(T entity);
        ICollection<T> FindEntity(T entity);
        ICollection<T> FindEntity(T entity, out int totalPage, out int totalRecord, int pageSize, int pageIndex, out string message);
        T GetByID(string ID);
    }
}
