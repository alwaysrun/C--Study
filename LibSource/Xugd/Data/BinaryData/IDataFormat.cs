using System;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 类序列化与反序列化的接口
    /// </summary>
    public interface IXDataFormat
    {
        /// <summary>
        /// 数据已序列化完成
        /// </summary>
        Action<object> DataDeserialized { get; set; }

        /// <summary>
        /// 反序列化失败
        /// </summary>
        Action<Exception> DeserializeFailed {get; set;}

        /// <summary>
        /// 添加要反序列化的数据
        /// </summary>
        /// <param name="byBuffer_"></param>
        void Add(byte[] byBuffer_);

        /// <summary>
        /// 添加要反序列化的数据
        /// </summary>
        /// <param name="byBuffer_"></param>
        /// <param name="nStart_"></param>
        /// <param name="nCount_"></param>
        void Add(byte[] byBuffer_, int nStart_, int nCount_);

        /// <summary>
        /// 清空要反序列化的数据
        /// </summary>
        void Clear();

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        object Deserialize();

        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="objData_"></param>
        /// <returns></returns>
        byte[] Serialize(object objData_);
    }
}
