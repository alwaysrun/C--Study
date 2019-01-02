using System;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// ��־λ��������
    /// </summary>
    public static class XFlag
    {
        private const int BitsOfHex = 4;
        private const int BitsOfUint = 32;
        /// <summary>
        /// ��ȡһ������ָ��λ�����磺
        /// GetSubDigits(0xABCDEF, 2, 0)����Ϊ0xEF��
        /// GetSubDigits(0xABCDEF, 1, 2)����Ϊ0xD
        /// </summary>
        /// <param name="nNumber_">����������</param>
        /// <param name="nGetHexDigits_">Ҫȡ���ٸ�ʮ������λ�������ٸ�F��</param>
        /// <param name="nShiftHexDigits_">���ƶ��ٸ�ʮ������λ�����ӵڼ���ʮ������λ��ʼȡ��</param>
        /// <returns>���ֵ</returns>
        public static uint GetSubDigits(uint nNumber_, int nGetHexDigits_, int nShiftHexDigits_=0)
        {
            if (nGetHexDigits_ <= 0)
                return 0;

            uint nToAnd = uint.Parse(new string('F', nGetHexDigits_), System.Globalization.NumberStyles.HexNumber);
            return (nNumber_ >> (nShiftHexDigits_ * BitsOfHex)) & nToAnd;
        }

        /// <summary>
        /// �жϱ�־λ�Ƿ��趨(����Ƕ�λ������Ҫ����Ϊ���趨�ˣ���Ϊ�趨)
        /// </summary>
        /// <param name="nFlag_">��־�ֶ�</param>
        /// <param name="nToCheck_">Ҫ�жϵı�־</param>
        /// <returns>���趨Ϊtrue������false</returns>
        public static bool Check(uint nFlag_, uint nToCheck_)
        {
            return (nToCheck_ != 0) && ((nFlag_ & nToCheck_) == nToCheck_);
        }

        /// <summary>
        /// �жϱ�־λ�Ƿ��趨(����Ƕ�λ������Ҫ����λ���趨�ˣ���Ϊ�趨)
        /// </summary>
        /// <param name="euFlag_">��־�ֶ�</param>
        /// <param name="euToCheck_">Ҫ�жϵı�־</param>
        /// <returns>���趨Ϊtrue������false</returns>
        public static bool Check(Enum euFlag_, Enum euToCheck_)
        {
            // Under Net4.0 Can use: (Convert.ToUInt32(euFlag_) != 0) && euFlag_.HasFlag(euToCheck_);
            return Check(Convert.ToUInt32(euFlag_), Convert.ToUInt32(euToCheck_));
        }

        /// <summary>
        /// �жϱ�־λ�Ƿ��趨(����Ƕ�λ�����κ�һλ�趨�ˣ���Ϊ�趨)
        /// </summary>
        /// <param name="nFlag_">��־�ֶ�</param>
        /// <param name="nToCheck_">Ҫ�жϵı�־</param>
        /// <returns>���趨Ϊtrue������false</returns>
        public static bool CheckAny(uint nFlag_, uint nToCheck_)
        {
            return (0 != (nFlag_ & nToCheck_));
        }

        /// <summary>
        /// �жϱ�־λ�Ƿ��趨(����Ƕ�λ�����κ�һλ�趨�ˣ���Ϊ�趨)
        /// </summary>
        /// <param name="euFlag_">��־�ֶ�</param>
        /// <param name="euToCheck_">Ҫ�жϵı�־</param>
        /// <returns>���趨Ϊtrue������false</returns>
        public static bool CheckAny(Enum euFlag_, Enum euToCheck_)
        {
            return CheckAny(Convert.ToUInt32(euFlag_), Convert.ToUInt32(euToCheck_));
        }

        /// <summary>
        /// �趨��־λ��ֱ���޸ı�־�ֶΣ�
        /// </summary>
        /// <param name="nFlag_">��־�ֶ�</param>
        /// <param name="nToSet_">Ҫ�趨�ı�־λ</param>
        public static void Set(ref uint nFlag_, uint nToSet_)
        {
            nFlag_ |= nToSet_;
        }

        /// <summary>
        /// �趨��־λ��ֱ���޸ı�־�ֶΣ�
        /// </summary>
        /// <param name="euFlag_">��־�ֶ�</param>
        /// <param name="euToSet_">Ҫ�趨�ı�־λ</param>
        public static void Set<U>(ref U euFlag_, U euToSet_)
        {
            if (!typeof(U).IsEnum)
                throw new ArgumentException("Flag must be Enum type");

            uint nFlag = Convert.ToUInt32(euFlag_);
            Set(ref nFlag, Convert.ToUInt32(euToSet_));

            euFlag_ = (U)Enum.ToObject(typeof(U), nFlag);
        }

        /// <summary>
        /// �����־λ��ֱ���޸ı�־�ֶΣ�
        /// </summary>
        /// <param name="nFlag_">��־�ֶ�</param>
        /// <param name="nToClear_">Ҫ����ı�־λ</param>
        public static void Clear(ref uint nFlag_, uint nToClear_)
        {
            nFlag_ &= ~(nToClear_);
        }

        /// <summary>
        /// �����־λ��ֱ���޸ı�־�ֶΣ�
        /// </summary>
        /// <param name="euFlag_">��־�ֶ�</param>
        /// <param name="euToClear_">Ҫ��ӵı�־λ</param>
        public static void Clear<U>(ref U euFlag_, U euToClear_)
        {
            if (!typeof(U).IsEnum)
                throw new ArgumentException("Flag must be Enum type");

            uint nFlag = Convert.ToUInt32(euFlag_);
            Clear(ref nFlag, Convert.ToUInt32(euToClear_));

            euFlag_ = (U)Enum.ToObject(typeof(U), nFlag);
        }

        /// <summary>
        /// ���ӱ�־λ���޸Ľ��ͨ������ֵ���أ�
        /// </summary>
        /// <param name="nFlag_">��־�ֶ�</param>
        /// <param name="nToAdd_">Ҫ��ӵı�־λ</param>
        /// <returns>��Ӻ�ı�־�ֶ�</returns>
        public static uint Add(uint nFlag_, uint nToAdd_)
        {
            return (nFlag_ | nToAdd_);
        }

        /// <summary>
        /// ���ӱ�־λ���޸Ľ��ͨ������ֵ���أ�
        /// </summary>
        /// <param name="euFlag_">��־�ֶ�</param>
        /// <param name="euToAdd_">Ҫ��ӵı�־λ</param>
        /// <returns>��Ӻ�ı�־�ֶ�</returns>
        public static U Add<U>(U euFlag_, U euToAdd_)
        {
            if (!typeof(U).IsEnum)
                throw new ArgumentException("Flag must be Enum type");

            return (U)Enum.ToObject(typeof(U), Convert.ToUInt32(euFlag_) | Convert.ToUInt32(euToAdd_));
        }

        /// <summary>
        /// �Ƴ���־λ���޸Ľ��ͨ������ֵ���أ�
        /// </summary>
        /// <param name="nFlag_">��־�ֶ�</param>
        /// <param name="nToRemove_">Ҫ�Ƴ��ı�־λ</param>
        /// <returns>�Ƴ���ı�־�ֶ�</returns>
        public static uint Remove(uint nFlag_, uint nToRemove_)
        {
            return (nFlag_ & ~(nToRemove_));
        }

        /// <summary>
        /// �����־λ���޸Ľ��ͨ������ֵ���أ�
        /// </summary>
        /// <param name="euFlag_">��־�ֶ�</param>
        /// <param name="euToClear_">Ҫ��ӵı�־λ</param>
        /// <returns>��Ӻ�ı�־�ֶ�</returns>
        public static U Remove<U>(U euFlag_, U euToClear_)
        {
            if (!typeof(U).IsEnum)
                throw new ArgumentException("Flag must be Enum type");

            return (U)Enum.ToObject(typeof(U), Remove(Convert.ToUInt32(euFlag_), Convert.ToUInt32(euToClear_)));
        }

        /// <summary>
        /// ��������־λ�����ֵ���޸Ľ��ͨ������ֵ���أ�
        /// </summary>
        /// <param name="nOld_">�ɵı�־λ</param>
        /// <param name="nNew_">�µı�־λ</param>
        /// <returns>���ֵ���������жϱ�־λ�Ƿ���Ϊ�޸ģ����޸ķ���0�����򷵻��޸ĵ��</returns>
        public static uint DiffAll(uint nOld_, uint nNew_)
        {
            return (nOld_ ^ nNew_);
        }

        /// <summary>
        /// ��ȡ�����ı�־λ��nNew���У���nOld��û�е�λ�����޸Ľ��ͨ������ֵ���أ�
        /// </summary>
        /// <param name="nOld_">�ɵı�־λ</param>
        /// <param name="nNew_">�µı�־λ</param>
        /// <returns>û��������λ����0�����򷵻�������λ</returns>
        public static uint DiffAdd(uint nOld_, uint nNew_)
        {
            return (DiffAll(nOld_, nNew_) & ~nOld_);
        }

        /// <summary>
        /// ��ȡ���ٵı�־λ��nOld���У���nNew��û�е�λ�����޸Ľ��ͨ������ֵ���أ�
        /// </summary>
        /// <param name="nOld_">�ɵı�־λ</param>
        /// <param name="nNew_">�µı�־λ</param>
        /// <returns>û�м��٣��Ƴ�����λ����0�����򷵻ؼ��ٵ�λ</returns>
        public static uint DiffRemove(uint nOld_, uint nNew_)
        {
            return (DiffAll(nOld_, nNew_) & ~nNew_);
        }
    }
}
