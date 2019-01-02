using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// 数据库缓冲插入（如果当前数据库连接不正常，则先存入xml文件中：
    /// 插入成功后从xml文件中删除；如果插入失败且数据库连接正常，则把记录
    /// 转移到错误表中，文件名为XmlFile+yyyyMMdd）：
    /// 如果数据库中已存在对应的记录（根据类中的GetKeyColName函数确定），则更新，否则插入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XBufferInsert<T> where T : class, IDbDataInsert
    {
        private bool _bStart = false;
        /// <summary>
        /// 对应的DapperORM
        /// </summary>
        public XDapperORM DapperORM {get; private set;}
        /// <summary>
        /// 存放缓冲数据的XML
        /// </summary>
        public string StoreFile {get; private set;}

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="dapper_"></param>
        /// <param name="strXmlFile_"></param>
        public XBufferInsert(XDapperORM dapper_, string strXmlFile_)
        {
            DapperORM = dapper_;
            StoreFile = strXmlFile_;
        }

        /// <summary>
        /// 启动：如果要测试数据库连接，则先进行连接测试，如果测试失败会抛出连接异常
        /// </summary>
        /// <param name="bConTest_">是否要测试数据库连接</param>
        public void Start(bool bConTest_=true)
        {
            if (_bStart)
                return;

            if (bConTest_)
                DapperORM.ConnectTest();

            _bStart = true;
            InitData();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (!_bStart)
                return;

            _bStart = false;
            _evtInsert.Set();
        }

        /// <summary>
        /// 增加要写入数据库中的数据
        /// </summary>
        /// <param name="tData_"></param>
        public void AddData(T tData_)
        {
            if(_bStart) 
                BufferAddData(tData_);
        }
    }
}
