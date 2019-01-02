#pragma once
using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Collections::Generic;

#include "KFile.h"

namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
			/// <summary>
			/// ��Կ�׶�д�ӿڿ�(CreCMail.dll)�ķ�װ,
			/// ��س�����CLen�ж���
			/// 
			/// ������--��������ʱ�����׳��쳣��
			/// ������ش����׳�SHParamException��
			/// �ӽ��ܲ�����ش����׳�SHCryptException��
			/// �ļ�������ش����׳�SHFileException��
			/// ����ͨ��SHException�����������׳������д���
			/// �����붨����Common�µ�SHErrorCode��
			/// 
			/// </summary>
			public ref class CMail
			{
			public:
				/// <summary>
				/// ��ǰ����״̬
				/// </summary>
				enum class CallbackStatus
				{
					/// <summary>
					/// ��Ч״̬�����ڳ�ʼ������Ч���ж�
					/// </summary>
					Invalid = 0,
					/// <summary>
					/// ������ʼ����ʱ��ȡ�ܵĲ�����
					/// </summary>
					Begin,
					/// <summary>
					/// ǰ��һ��
					/// </summary>
					Step,
					/// <summary>
					/// �������
					/// </summary>
					End
				};

				/// <summary>
				/// ������Ϣ
				/// </summary>
				[StructLayout(LayoutKind::Sequential, Pack = 4, CharSet = CharSet::Unicode)]
				value struct CallbackInfo
				{
				public:
					/// <summary>
					/// ����״̬
					/// </summary>
					CallbackStatus euStatus;
					/// <summary>
					/// ��ǰ�Ĳ���
					/// </summary>
					unsigned int nCurStep;
					/// <summary>
					/// �ܵĲ���
					/// </summary>
					unsigned int nTotalStep;
					/// <summary>
					/// ��ǰ�������ļ�
					/// </summary>
					[MarshalAs(UnmanagedType::ByValTStr, SizeConst = CLen::PathMaxLen+1)]
					String^ strFile;
				};

				/// <summary>
				/// �ص�����ί�У����ڻ�ȡ��ǰ�Ĳ���״̬
				/// </summary>
				/// <param name="stInfo_">������Ϣ</param>
				/// <returns>�������false��˵��ȡ����ǰ����</returns>
				[UnmanagedFunctionPointer(CallingConvention::StdCall)]
				delegate bool CallbackFun(CallbackInfo% stInfo_);

				/// <summary>
				/// �趨�ص������������Ҫ��ȡ��ǰ��������Ľ��ȣ�
				/// ����Ҫ���趨�ص�������
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="delFun_">Ҫ�趨�Ļص�����</param>
				static void SetCallback(IntPtr hFile_, CallbackFun^ delFun_);

				/// <summary>
				/// �����ļ�
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nOwnerID_">�ļ�������ID</param>
				/// <param name="byPriKey_">�ļ�������˽Կ</param>
				/// <param name="nKeyLen_">˽Կ����</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <param name="strName_">�ļ������ߣ������ߣ�����</param>
				/// <param name="strEmail_">�ļ�����������</param>
				/// <param name="euDataAlg_">��������ʱ��ʹ�õ��㷨���Գ��㷨��һ��ΪAES��</param>
				/// <returns>�ļ����</returns>
				static IntPtr Create
					(
					String^ strFile_, 
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nOwnerID_, 
					array<unsigned char>^ byPriKey_, 
					int nKeyLen_,
					String^ strCryptDll_,
					String^ strName_, 
					String^ strEmail_,
					SecAlg::Crypto::Alg euDataAlg_
					);

				/// <summary>
				/// �����ļ�
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nOwnerID_">�ļ�������ID</param>
				/// <param name="byPriKey_">�ļ�������˽Կ</param>
				/// <param name="strName_">�ļ������ߣ������ߣ�����</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <param name="strEmail_">�ļ�����������</param>
				/// <param name="euDataAlg_">��������ʱ��ʹ�õ��㷨���Գ��㷨��һ��ΪAES��</param>
				/// <returns>�ļ����</returns>
				static IntPtr Create
					(
					String^ strFile_, 
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nOwnerID_, 
					array<unsigned char>^ byPriKey_, 
					String^ strCryptDll_,
					String^ strName_, 
					String^ strEmail_,
					SecAlg::Crypto::Alg euDataAlg_
					)
				{
					return Create(strFile_, euAlg_, nCorpID_, nOwnerID_, byPriKey_, byPriKey_->Length, strCryptDll_, strName_, strEmail_, euDataAlg_);
				}

				/// <summary>
				/// �����ļ�����������ʱʹ��AES��
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nOwnerID_">�ļ�������ID</param>
				/// <param name="byPriKey_">�ļ�������˽Կ</param>
				/// <param name="nKeyLen_">˽Կ����</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <returns>�ļ����</returns>
				static IntPtr Create
					(
					String^ strFile_, 
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nOwnerID_, 
					array<unsigned char>^ byPriKey_,
					int nKeyLen_,
					String^ strCryptDll_
					)
				{
					return Create(strFile_, euAlg_, nCorpID_, nOwnerID_, byPriKey_, nKeyLen_, strCryptDll_, nullptr, nullptr, SecAlg::Crypto::Alg::AES);
				}

				/// <summary>
				/// ���ļ�
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nUserCipherId_">�û�ID</param>
				/// <param name="byPriKey_">�û�˽Կ</param>
				/// <param name="nKeyLen_">˽Կ����</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <returns>�ļ����</returns>
				static IntPtr Open
					(
					String^ strFile_, 
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nUserCipherId_, 
					array<unsigned char>^ byPriKey_,
					int nKeyLen_,
					String^ strCryptDll_
					);

				/// <summary>
				/// ���ļ�
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nUserCipherId_">�û�ID</param>
				/// <param name="byPriKey_">�û�˽Կ</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <returns>�ļ����</returns>
				static IntPtr Open
					(
					String^ strFile_, 
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nUserCipherId_, 
					array<unsigned char>^ byPriKey_,
					String^ strCryptDll_
					)
				{
					return Open(strFile_, euAlg_, nCorpID_, nUserCipherId_, byPriKey_, byPriKey_->Length, strCryptDll_);
				}

				/// <summary>
				/// �رմ򿪵��ļ����ļ�ʹ����ɺ�һ��Ҫ�ر�
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				static void Close(IntPtr hFile_);

				/// <summary>
				/// ����ļ�/�ļ��У����Ϊ�ļ��л��Զ�ö�����ļ�/�ļ��У�
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				static void Pack
					(
					IntPtr hFile_,
					String^ strFile_
					);

				/// <summary>
				/// ����ļ�
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="strPath_">������ļ�������ڵ��ļ���</param>
				static void Unpack
					(
					IntPtr hFile_,
					String^ strPath_
					);

				/// <summary>
				/// �����ݽ���ǩ����ǩ���󣬾Ͳ��ܼ������������
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nOwnerID_">�ļ�������ID</param>
				/// <param name="byPriKey_">ǩ���õ�˽Կ</param>
				/// <param name="euHash_">���ڶ����ݽ���ɢ�е��㷨��һ��ΪSha1��</param>
				static void Sign
					(
					IntPtr hFile_,
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nOwnerID_, 
					array<unsigned char>^ byPriKey_,
					SecAlg::Hash::Alg euHash_
					);


				/// <summary>
				/// ������ǩ�����н�����֤����֤���ļ�ָ��λ�����ݿ�ʼλ�ã�����ֱ�Ӷ�ȡ��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nOwnerID_">�ļ�������ID</param>
				/// <param name="byPubKey_">��֤�õĹ�Կ</param>
				/// <returns>��֤ͨ��������true�����򣬷���false</returns>
				static bool Verify
					(
					IntPtr hFile_, 
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nOwnerID_,
					array<unsigned char>^ byPubKey_
					);

				/// <summary>
				///  ����û�����������Ȩ�ޣ�Operate���û���¼�ɹ��󣬲ſɣ�
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nUserCipherId_">Ҫ��ӵ��û�ID</param>
				/// <param name="byPubKey_">�û���Կ</param>
				/// <param name="strName_">�û�����</param>
				/// <param name="strEmail_">�û�����</param>
				static void AddUser
					(
					IntPtr hFile_,
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nUserCipherId_, 
					array<unsigned char>^ byPubKey_, 
					String^ strName_, 
					String^ strEmail_
					);

				/// <summary>
				/// ɾ���û�����������Ȩ�ޣ�Operate���û���¼�ɹ��󣬲ſɣ�
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nUserCipherId_">Ҫɾ�����û�ID</param>
				static void RemoveUser(IntPtr hFile_, unsigned int nCorpID_, unsigned int nUserCipherId_);

				/// <summary>
				/// ��ȡ�ļ����û������뱣֤�ļ�û�б���
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <returns>�û���Ϣ�б�</returns>
				static List<KFile::UserInfo^>^ GetUsers(String^ strFile_)
				{
					return KFile::GetUsers(strFile_);
				}

				/// <summary>
				/// ��ӿ�����Ϣ
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="dtStart_">�ļ����õ���ʼʱ��(��������1601-1-1��</param>
				/// <param name="dtEnd_">�ļ����õĽ���ʱ�䣨��������9998-12-31��</param>
				/// <param name="nCount_">�ļ�����ʹ�õĴ�����0Ϊû�����ƣ�</param>
				static void AddCtrlInfo(IntPtr hFile_, DateTime dtStart_, DateTime dtEnd_, int nCount_);

				/// <summary>
				/// ��ӿ�����Ϣ��ֻ���ƴ�����������ʱ�䡣
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="nCount_">�ļ�����ʹ�õĴ�����0Ϊû�����ƣ�</param>
				static void AddCtrlInfo(IntPtr hFile_, int nCount_);

				/// <summary>
				/// ��ȡ������Ϣ
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="dtStart_">�ļ����õ���ʼʱ�䣨û������ʱΪDateTime.MinValue��</param>
				/// <param name="dtEnd_">�ļ����õĽ���ʱ�䣨û������ʱΪDateTime.MaxValue��</param>
				/// <returns>�ļ�����ʹ�õĴ�����0Ϊû�����ƣ�</returns>
				static int GetCtrlInfo(IntPtr hFile_, [Out] DateTime% dtStart_, [Out] DateTime% dtEnd_);
			};
		} // CreAPI
	} // Xugd
} // SHCre