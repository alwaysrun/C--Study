using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SHCre.Xugd.Common;
using System.Diagnostics;
using System.IO;

namespace SHCre.Xugd.DbHelper
{
    partial class XBufferInsert<T>
    {
        XDbInsertData<T> _insertData;
        private object _objDataLocker = new object();
        private object _objErrLocker = new object();
        private Thread _thrInsertData = null;
        private AutoResetEvent _evtInsert = new AutoResetEvent(false);

        private void InitData()
        {
            lock (_objDataLocker)
            {
                if (_insertData == null)
                    _insertData = XDbInsertData<T>.Read(StoreFile);

                if (_insertData.Count > 0)
                    StartInsertThread();
            }
        }

        private void BufferAddData(T tData_)
        {
            lock(_objDataLocker)
            {
                _insertData.Data.Add(tData_);
                _insertData.Write(StoreFile);
                StartInsertThread();
            }
        }

        private void StartInsertThread()
        {
            _evtInsert.Set();

            XThread.TryStartThread(ref _thrInsertData, InsertDataThread);
        }

        private void RemoveInsertData(T tData_)
        {
            lock(_objDataLocker)
            {
                _insertData.Data.Remove(tData_);
                _insertData.Write(StoreFile);
            }
        }

        private void AddErrorData(T tData_, string strCmd_, Exception ex_)
        {
            lock(_objErrLocker)
            {
                var errData = XDbErrorData<T>.Read(StoreFile);
                errData.Data.Add(new XDbErrorItem<T>()
                    {
                        Item = tData_,
                        Sql = strCmd_,
                        Error = ex_.Message,
                    });
                errData.Write(StoreFile);
            }
        }

        private void InsertToDatabase(T tData_)
        {
            string strCmd = string.Empty;
            try 
            {
                if (string.IsNullOrEmpty(tData_.GetKeyColName()))
                    DapperORM.Insert(tData_.GetTableName(), tData_, out strCmd);
                else
                    DapperORM.InsertOrUpdate(tData_.GetTableName(), tData_.GetKeyColName(), tData_, out strCmd);

                RemoveInsertData(tData_);
            }
            catch (System.Data.Common.DbException ex)
            {
                if(DapperORM.CanConnect())
                {
                    RemoveInsertData(tData_);
                    AddErrorData(tData_, strCmd, ex);
                }
                else
                {
                    throw;
                }
            }
        }

        private void InsertDataThread()
        {
            while (_bStart)
            {
                try 
                {
                    _evtInsert.WaitOne(60000);

                    while (true)
                    {
                        T tGet;
                        lock (_objDataLocker)
                        {
                            tGet = _insertData.Data.FirstOrDefault();
                        }
                        if (tGet == null)
                            break;

                        InsertToDatabase(tGet);
                    }
                }
                catch(Exception ex)
                {
#if DEBUG
                    File.AppendAllText(@"E:\_Test\Error.txt", ex.ToString());
#endif
                    Trace.WriteLine(ex.Message, "SHCre.Xugd.DbHelper.XBufferInsert.InsertDataThread");
                }
            }

            _thrInsertData = null;
        }

    }
}
