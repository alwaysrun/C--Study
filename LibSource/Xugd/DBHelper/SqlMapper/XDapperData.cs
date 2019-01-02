using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// 所有通过Dapper操作的数据都要继承此接口
    /// </summary>
    public interface IXDapperData
    {
    }

    /// <summary>
    /// 用于插入的接口约束
    /// </summary>
    public interface IXDapperInsert : IXDapperData
    {
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        string GetTableName();
    }

    /// <summary>
    /// 用于更新的接口约束
    /// </summary>
    public interface IXDapperUpdate:IXDapperInsert
    {
        /// <summary>
        /// 判断条件
        /// </summary>
        /// <returns></returns>
        string GetWhereClause();
    }
}
