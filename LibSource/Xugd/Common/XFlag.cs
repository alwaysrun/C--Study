using System;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 标志位处理类型
    /// </summary>
    public static class XFlag
    {
        private const int BitsOfHex = 4;
        private const int BitsOfUint = 32;
        /// <summary>
        /// 获取一个数字指定位数，如：
        /// GetSubDigits(0xABCDEF, 2, 0)则结果为0xEF；
        /// GetSubDigits(0xABCDEF, 1, 2)则结果为0xD
        /// </summary>
        /// <param name="nNumber_">操作的数字</param>
        /// <param name="nGetHexDigits_">要取多少个十六进制位（即多少个F）</param>
        /// <param name="nShiftHexDigits_">右移多少个十六进制位（即从第几个十六进制位开始取）</param>
        /// <returns>结果值</returns>
        public static uint GetSubDigits(uint nNumber_, int nGetHexDigits_, int nShiftHexDigits_=0)
        {
            if (nGetHexDigits_ <= 0)
                return 0;

            uint nToAnd = uint.Parse(new string('F', nGetHexDigits_), System.Globalization.NumberStyles.HexNumber);
            return (nNumber_ >> (nShiftHexDigits_ * BitsOfHex)) & nToAnd;
        }

        /// <summary>
        /// 判断标志位是否设定(如果是多位，则需要所有为都设定了，才为设定)
        /// </summary>
        /// <param name="nFlag_">标志字段</param>
        /// <param name="nToCheck_">要判断的标志</param>
        /// <returns>已设定为true；否则false</returns>
        public static bool Check(uint nFlag_, uint nToCheck_)
        {
            return (nToCheck_ != 0) && ((nFlag_ & nToCheck_) == nToCheck_);
        }

        /// <summary>
        /// 判断标志位是否设定(如果是多位，则需要所有位都设定了，才为设定)
        /// </summary>
        /// <param name="euFlag_">标志字段</param>
        /// <param name="euToCheck_">要判断的标志</param>
        /// <returns>已设定为true；否则false</returns>
        public static bool Check(Enum euFlag_, Enum euToCheck_)
        {
            // Under Net4.0 Can use: (Convert.ToUInt32(euFlag_) != 0) && euFlag_.HasFlag(euToCheck_);
            return Check(Convert.ToUInt32(euFlag_), Convert.ToUInt32(euToCheck_));
        }

        /// <summary>
        /// 判断标志位是否设定(如果是多位，则任何一位设定了，则为设定)
        /// </summary>
        /// <param name="nFlag_">标志字段</param>
        /// <param name="nToCheck_">要判断的标志</param>
        /// <returns>已设定为true；否则false</returns>
        public static bool CheckAny(uint nFlag_, uint nToCheck_)
        {
            return (0 != (nFlag_ & nToCheck_));
        }

        /// <summary>
        /// 判断标志位是否设定(如果是多位，则任何一位设定了，则为设定)
        /// </summary>
        /// <param name="euFlag_">标志字段</param>
        /// <param name="euToCheck_">要判断的标志</param>
        /// <returns>已设定为true；否则false</returns>
        public static bool CheckAny(Enum euFlag_, Enum euToCheck_)
        {
            return CheckAny(Convert.ToUInt32(euFlag_), Convert.ToUInt32(euToCheck_));
        }

        /// <summary>
        /// 设定标志位（直接修改标志字段）
        /// </summary>
        /// <param name="nFlag_">标志字段</param>
        /// <param name="nToSet_">要设定的标志位</param>
        public static void Set(ref uint nFlag_, uint nToSet_)
        {
            nFlag_ |= nToSet_;
        }

        /// <summary>
        /// 设定标志位（直接修改标志字段）
        /// </summary>
        /// <param name="euFlag_">标志字段</param>
        /// <param name="euToSet_">要设定的标志位</param>
        public static void Set<U>(ref U euFlag_, U euToSet_)
        {
            if (!typeof(U).IsEnum)
                throw new ArgumentException("Flag must be Enum type");

            uint nFlag = Convert.ToUInt32(euFlag_);
            Set(ref nFlag, Convert.ToUInt32(euToSet_));

            euFlag_ = (U)Enum.ToObject(typeof(U), nFlag);
        }

        /// <summary>
        /// 清除标志位（直接修改标志字段）
        /// </summary>
        /// <param name="nFlag_">标志字段</param>
        /// <param name="nToClear_">要清除的标志位</param>
        public static void Clear(ref uint nFlag_, uint nToClear_)
        {
            nFlag_ &= ~(nToClear_);
        }

        /// <summary>
        /// 清除标志位（直接修改标志字段）
        /// </summary>
        /// <param name="euFlag_">标志字段</param>
        /// <param name="euToClear_">要添加的标志位</param>
        public static void Clear<U>(ref U euFlag_, U euToClear_)
        {
            if (!typeof(U).IsEnum)
                throw new ArgumentException("Flag must be Enum type");

            uint nFlag = Convert.ToUInt32(euFlag_);
            Clear(ref nFlag, Convert.ToUInt32(euToClear_));

            euFlag_ = (U)Enum.ToObject(typeof(U), nFlag);
        }

        /// <summary>
        /// 增加标志位（修改结果通过返回值返回）
        /// </summary>
        /// <param name="nFlag_">标志字段</param>
        /// <param name="nToAdd_">要添加的标志位</param>
        /// <returns>添加后的标志字段</returns>
        public static uint Add(uint nFlag_, uint nToAdd_)
        {
            return (nFlag_ | nToAdd_);
        }

        /// <summary>
        /// 增加标志位（修改结果通过返回值返回）
        /// </summary>
        /// <param name="euFlag_">标志字段</param>
        /// <param name="euToAdd_">要添加的标志位</param>
        /// <returns>添加后的标志字段</returns>
        public static U Add<U>(U euFlag_, U euToAdd_)
        {
            if (!typeof(U).IsEnum)
                throw new ArgumentException("Flag must be Enum type");

            return (U)Enum.ToObject(typeof(U), Convert.ToUInt32(euFlag_) | Convert.ToUInt32(euToAdd_));
        }

        /// <summary>
        /// 移除标志位（修改结果通过返回值返回）
        /// </summary>
        /// <param name="nFlag_">标志字段</param>
        /// <param name="nToRemove_">要移除的标志位</param>
        /// <returns>移除后的标志字段</returns>
        public static uint Remove(uint nFlag_, uint nToRemove_)
        {
            return (nFlag_ & ~(nToRemove_));
        }

        /// <summary>
        /// 清除标志位（修改结果通过返回值返回）
        /// </summary>
        /// <param name="euFlag_">标志字段</param>
        /// <param name="euToClear_">要添加的标志位</param>
        /// <returns>添加后的标志字段</returns>
        public static U Remove<U>(U euFlag_, U euToClear_)
        {
            if (!typeof(U).IsEnum)
                throw new ArgumentException("Flag must be Enum type");

            return (U)Enum.ToObject(typeof(U), Remove(Convert.ToUInt32(euFlag_), Convert.ToUInt32(euToClear_)));
        }

        /// <summary>
        /// 求两个标志位的异或值（修改结果通过返回值返回）
        /// </summary>
        /// <param name="nOld_">旧的标志位</param>
        /// <param name="nNew_">新的标志位</param>
        /// <returns>异或值：可用于判断标志位是否作为修改，无修改返回0，否则返回修改的项。</returns>
        public static uint DiffAll(uint nOld_, uint nNew_)
        {
            return (nOld_ ^ nNew_);
        }

        /// <summary>
        /// 获取新增的标志位（nNew中有，而nOld中没有的位）（修改结果通过返回值返回）
        /// </summary>
        /// <param name="nOld_">旧的标志位</param>
        /// <param name="nNew_">新的标志位</param>
        /// <returns>没有新增的位返回0；否则返回新增的位</returns>
        public static uint DiffAdd(uint nOld_, uint nNew_)
        {
            return (DiffAll(nOld_, nNew_) & ~nOld_);
        }

        /// <summary>
        /// 获取减少的标志位（nOld中有，而nNew中没有的位）（修改结果通过返回值返回）
        /// </summary>
        /// <param name="nOld_">旧的标志位</param>
        /// <param name="nNew_">新的标志位</param>
        /// <returns>没有减少（移除）的位返回0；否则返回减少的位</returns>
        public static uint DiffRemove(uint nOld_, uint nNew_)
        {
            return (DiffAll(nOld_, nNew_) & ~nNew_);
        }
    }
}
