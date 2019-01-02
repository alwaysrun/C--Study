#pragma once
using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Collections::Generic;

#include "Crypt.h"

namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
			/// <summary>
			/// �Լ������ݶ�д�ӿڿ�(CreKData.dll)�ķ�װ,
			/// ��س�����CLen�ж���
			/// 
			/// ������--��������ʱ�����׳��쳣��
			/// ������ش����׳�SHParamException��
			/// �ӽ��ܲ�����ش����׳�SHCryptException��
			/// ���ݲ�����ش����׳�SHFileException��
			/// ����ͨ��SHException�����������׳������д���
			/// �����붨����Common�µ�SHErrorCode��
			/// 
			/// </summary>
			public ref class KData
			{
			public:
				/// <summary>
				/// ��������
				/// </summary>
				[Flags]
				enum class DataType
				{
					/// <summary>
					/// ��Ч�����ڳ�ʼ��
					/// </summary>
					None = 0,
					/// <summary>
					/// ���ܵ�����
					/// </summary>
					Encrypted = 0x01,
					/// <summary>
					/// ǩ�������ݣ�ֻ��Sign�������ݲŻ���Ӵ˱�ʶ
					/// </summary>
					Signed = 0x02,
					/// <summary>
					/// ͨ������������ݣ����Զ����Encrypted��ʶ
					/// </summary>
					ENByPsw = 0x04
				};

				/// <summary>
				/// ���������ڼ��ܻ�ǩ����¼
				/// </summary>
				/// <param name="euType_">��������</param>
				/// <param name="nRecordSize_">��¼�Ĵ�С</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <param name="strName_">�����ߣ������ߣ�����</param>
				/// <param name="strEmail_">������������</param>
				/// <returns>���ݾ��</returns>
				static IntPtr Create
					(
					DataType euType_,
					int nRecordSize_,
					String^ strCryptDll_,
					String^ strName_,
					String^ strEmail_
					);

				/// <summary>
				/// ʹ��Ĭ�ϼ��ܿⴴ�������ڼ��ܻ�ǩ����¼
				/// </summary>
				/// <param name="euType_">��������</param>
				/// <param name="nRecordSize_">��¼�Ĵ�С</param>
				/// <returns>���ݾ��</returns>
				static IntPtr Create(DataType euType_, int nRecordSize_)
				{
					return Create(euType_, nRecordSize_, nullptr, nullptr, nullptr);
				}

				/// <summary>
				/// �򿪣����ڽ��ܻ���֤��¼
				/// </summary>
				/// <param name="nRecordSize_">��¼�Ĵ�С</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				static IntPtr Open(int nRecordSize_, String^ strCryptDll_);

				/// <summary>
				/// ʹ��Ĭ�ϼ��ܿ�򿪣����ڽ��ܻ���֤��¼
				/// </summary>
				/// <param name="nRecordSize_">��¼�Ĵ�С</param>
				static IntPtr Open(int nRecordSize_)
				{
					return Open(nRecordSize_, nullptr);
				}

				/// <summary>
				/// �رմ򿪵ľ��
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				static void Close(IntPtr hData_);

				/// <summary>
				///  ��ȡ�����м�¼�Ĵ�С
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <returns>��¼��С</returns>
				static int GetRecordSize(IntPtr hData_);

				/// <summary>
				///  ��ȡ�����м��ܺ��¼�Ĵ�С
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <returns>���ܺ��¼��С������ͷ���������Ϣ��</returns>
				static int GetCryptSize(IntPtr hData_);

				/// <summary>
				///  ��ȡ������ǩ�����¼�Ĵ�С
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <returns>ǩ�����¼��С������ͷ��ǩ������Ϣ��</returns>
				static int GetSignSize(IntPtr hData_);

				/// <summary>
				/// �趨��Կ
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpId_">��˾ID</param>
				/// <param name="nCipherId_">�û�����Կ��ʶ</param>
				/// <param name="byKey_">��Կ</param>
				/// <param name="nKeyLen_">��Կ����</param>
				/// <param name="bPrivate_">�Ƿ���˽Կ</param>
				static void SetKey
					(
					IntPtr hData_,
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpId_,
					unsigned int nCipherId_,
					array<unsigned char>^ byKey_,
					int nKeyLen_,
					bool bPrivate_
					);

				/// <summary>
				/// �趨��Կ
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpId_">��˾ID</param>
				/// <param name="nCipherId_">�û�����Կ��ʶ</param>
				/// <param name="byKey_">��Կ</param>
				/// <param name="bPrivate_">�Ƿ���˽Կ</param>
				static void SetKey
					(
					IntPtr hData_,
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpId_,
					unsigned int nCipherId_,
					array<unsigned char>^ byKey_,
					bool bPrivate_
					)
				{
					SetKey(hData_, euAlg_, nCorpId_, nCipherId_, byKey_, byKey_->Length, bPrivate_);
				}

				/// <summary>
				/// �趨����
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <param name="strPsw_">����</param>
				static void SetPsw(IntPtr hData_, String^ strPsw_);

				/// <summary>
				/// �������ݣ���¼��
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <param name="byData_">Ҫ���ܵ�����</param>
				/// <returns>���ܺ�����ݣ�����ͷ���������Ϣ��</returns>
				static array<unsigned char>^ Encrypt
					(
					IntPtr hData_,
					array<unsigned char>^ byData_
					);

				/// <summary>
				/// ��������
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <param name="byCipher_">Ҫ���ܵ����ݣ�����ͷ���������Ϣ��</param>
				/// <returns>���ܺ������</returns>
				static array<unsigned char>^ Decrypt
					(
					IntPtr hData_,
					array<unsigned char>^ byCipher_
					);

				/// <summary>
				/// ǩ�����ݣ���¼��
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <param name="byData_">Ҫǩ��������</param>
				/// <returns>ǩ��������ݣ�����ͷ��ǩ����Ϣ��</returns>
				static array<unsigned char>^ Sign
					(
					IntPtr hData_,
					array<unsigned char>^ byData_
					);

				/// <summary>
				/// ��֤����
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <param name="hData_">Ҫ��֤�����ݣ�����ͷ��ǩ����Ϣ��</param>
				/// <returns>ԭ����</returns>
				static array<unsigned char>^ VerifyData
					(
					IntPtr hData_,
					array<unsigned char>^ bySign_
					);

				/// <summary>
				/// ��֤����
				/// </summary>
				/// <param name="hData_">���ݾ��</param>
				/// <param name="hData_">Ҫ��֤�����ݣ�����ͷ��ǩ����Ϣ��</param>
				/// <returns>��֤ͨ��������true�����򣬷���false</returns>
				static bool VerifyOnly
					(
					IntPtr hData_,
					array<unsigned char>^ bySign_
					);
			};
		} // CreAPI
	} // Xugd
} // SHCre