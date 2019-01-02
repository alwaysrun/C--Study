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
			/// �Լ����ļ���д�ӿڿ�(CreKFile.dll)�ķ�װ,
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
			public ref class KFile
			{
			public:
				/// <summary>
				/// �û�Ȩ��
				/// </summary>
				[Flags]
				enum class Rights
				{
					/// <summary>
					/// ��ЧȨ�ޣ����ڳ�ʼ������Ч���ж�
					/// </summary>
					None = 0,
					/// <summary>
					/// ֻ��Ȩ��
					/// </summary>
					Read = 0x01,
					/// <summary>
					/// дȨ��
					/// </summary>
					Write = 0x02,
					/// <summary>
					/// ɾ��Ȩ��
					/// </summary>
					Delete = 0x04,
					/// <summary>
					/// ��ʾȨ��
					/// </summary>
					Display = 0x08,
					/// <summary>
					/// ����Ȩ��
					/// </summary>
					Copy = 0x010,
					/// <summary>
					/// ��ӡȨ��
					/// </summary>
					Print = 0x020,
					/// <summary>
					/// ���ΪȨ��
					/// </summary>
					SaveAs = 0x040,
					/// <summary>
					/// ����Ȩ�ޣ�����ӡ�ɾ���û��ȣ�
					/// </summary>
					Operate = 0x080,
					/// <summary>
					/// ��дȨ��
					/// </summary>
					ReadWrite = Read | Write
				};

				/// <summary>
				/// �ļ�����
				/// </summary>
				[Flags]
				enum class FileType
				{
					/// <summary>
					/// ��ͨ�ļ���δ����Ҳδǩ��
					/// </summary>
					None = 0,
					/// <summary>
					/// ���ܵ��ļ�
					/// </summary>
					Encrypted = 0x01,
					/// <summary>
					/// ǩ�����ļ���ֻ��Sign�����ļ��Ż���Ӵ˱�ʶ
					/// </summary>
					Signed = 0x02,
					/// <summary>
					/// ͨ����������ļ������Զ����Encrypted��ʶ
					/// </summary>
					ENByPsw = 0x04
				};

				/// <summary>
				/// �û�����
				/// </summary>
				enum class UserType
				{
					/// <summary>
					/// ��Ч���ͣ����ڳ�ʼ������Ч���ж�
					/// </summary>
					Invalid = 0,
					/// <summary>
					/// �ļ������ߣ������ߣ�
					/// </summary>
					Owner,
					/// <summary>
					/// ��ͨ�û�
					/// </summary>
					User
				};

				/// <summary>
				/// ö���û�ʱ���û���Ϣ
				/// </summary>
				[StructLayout(LayoutKind::Sequential, Pack = 4, CharSet = CharSet::Unicode)]
				ref struct UserInfo
				{
				public:
					/// <summary>
					/// �û�����
					/// </summary>
					UserType euType;
					/// <summary>
					/// ��˾ID
					/// </summary>
					unsigned int nCorpID;
					/// <summary>
					/// �û�ID
					/// </summary>
					unsigned int nUserCipherId;
					/// <summary>
					/// �û�Ȩ��
					/// </summary>
					Rights euRights;
					/// <summary>
					/// �û���
					/// </summary>
					[MarshalAs(UnmanagedType::ByValTStr, SizeConst = CLen::NameMaxLen+1)]
					String^ strName;
					/// <summary>
					/// �û�����
					/// </summary>
					[MarshalAs(UnmanagedType::ByValTStr, SizeConst = CLen::EmailMaxLen+1)]
					String^ strEmail;
				};

				/// <summary>
				/// �����ļ�
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <param name="euType_">�ļ�����</param>
				/// <param name="nRecordSize_">������ļ��м�¼�Ĵ�С</param>
				/// <param name="nHeaderSize_">������ļ���ͷ��С��ʹ��AddHeader��GetHeader������</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <param name="strName_">�ļ������ߣ������ߣ�����</param>
				/// <param name="strEmail_">�ļ�����������</param>
				/// <param name="euAlg_">��������ʱ��ʹ�õ��㷨���Գ��㷨��һ��ΪAES��</param>
				/// <returns>�ļ����</returns>
				static IntPtr Create
					(
					String^ strFile_,
					FileType euType_,
					int nRecordSize_,
					int nHeaderSize_,
					String^ strCryptDll_,
					String^ strName_,
					String^ strEmail_,
					SecAlg::Crypto::Alg euAlg_
					);

				/// <summary>
				/// �����ļ�
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <param name="euType_">�ļ�����</param>
				/// <param name="nRecordSize_">������ļ��м�¼�Ĵ�С</param>
				/// <param name="nHeaderSize_">������ļ���ͷ��С��ʹ��AddHeader��GetHeader������</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <returns>�ļ����</returns>
				static IntPtr Create(String^ strFile_, FileType euType_, int nRecordSize_, int nHeaderSize_, String^ strCryptDll_)
				{
					return Create(strFile_, euType_, nRecordSize_, nHeaderSize_, strCryptDll_, nullptr, nullptr, SecAlg::Crypto::Alg::AES);
				}

				/// <summary>
				/// ���ļ�
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <param name="strCryptDll_">���ܿ��·����nullʹ��Ĭ�ϼ��ܿ⣩</param>
				/// <returns>�ļ����</returns>
				static IntPtr Open(String^ strFile_, String^ strCryptDll_);

				/// <summary>
				/// �رմ򿪵��ļ�
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				static void Close(IntPtr hFile_);

				/// <summary>
				/// �����ļ�ָ�룬�Ƶ����ݿ�ʼ����������ֱ�Ӷ�ȡ���ݣ�
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				static void Reset(IntPtr hFile_);

				/// <summary>
				///  ��ȡ�ļ���ͷ�Ĵ�С
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <returns>ͷ��С</returns>
				static int GetHeaderSize(IntPtr hFile_);

				/// <summary>
				///  ��ȡ�ļ��м�¼�Ĵ�С
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <returns>��¼��С</returns>
				static int GetRecordSize(IntPtr hFile_);

				/// <summary>
				/// ��ȡ�ļ��м�¼����
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <returns>��¼����</returns>
				static int GetRecordCount(IntPtr hFile_);

				/// <summary>
				/// ��ȡ�ļ�������
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <returns>�ļ�������</returns>
				static FileType GetType(IntPtr hFile_);

				/// <summary>
				/// �趨��Կ�����ڼ��ܵ��ļ���ֻ���趨����ܶ�д
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nCipherId_">�û�ID</param>
				/// <param name="byPriKey_">˽Կ</param>
				/// <param name="nKeyLen_">��Կ����</param>
				static void SetKey
					(
					IntPtr hFile_,
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nCipherId_,
					array<unsigned char>^ byPriKey_,
					int nKeyLen_
					);

				/// <summary>
				/// �趨��Կ�����ڼ��ܵ��ļ���ֻ���趨����ܶ�д
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nCipherId_">�û�ID</param>
				/// <param name="byPriKey_">˽Կ</param>
				static void SetKey
					(
					IntPtr hFile_,
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nCipherId_,
					array<unsigned char>^ byPriKey_
					)
				{
					SetKey(hFile_, euAlg_, nCorpID_, nCipherId_, byPriKey_, byPriKey_->Length );
				}

				/// <summary>
				/// �趨������ڼ��ܵ��ļ���ֻ���趨����ܶ�д
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="strPsw_">�ļ�����</param>
				static void SetPsw
					(
					IntPtr hFile_,
					String^ strPsw_
					);

				/// <summary>
				/// ���ͷ����СΪ����ʱ�趨�Ĵ�С��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="byHeader_">Ҫ��ӵ�ͷ��Ϣ</param>
				static void AddHeader(IntPtr hFile_, array<unsigned char>^ byHeader_);

				/// <summary>
				/// ��ȡͷ����СΪ����ʱ�趨�Ĵ�С��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="byHeader_">��ȡ��ͷ��Ϣ</param>
				static void GetHeader(IntPtr hFile_, array<unsigned char>^ byHeader_);

				/// <summary>
				/// ��ȡͷ����СΪ����ʱ�趨�Ĵ�С��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <returns>��ȡ��ͷ</returns>
				static array<unsigned char>^ GetHeader(IntPtr hFile_)
				{
					int nSize = GetHeaderSize(hFile_);
					array<unsigned char>^ byHeader = gcnew array<unsigned char>(nSize);
					if(nSize > 0)
						GetHeader(hFile_, byHeader);

					return byHeader;
				}

				/// <summary>
				/// ��Ӽ�¼����¼��СΪ����ʱ�趨�Ĵ�С��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="byRecord_">Ҫ��ӵļ�¼</param>
				/// <param name="bEncrypt_">�Ƿ���Ҫ�Լ�¼���м���</param>
				static void AddRecord
					(
					IntPtr hFile_,
					array<unsigned char>^ byRecord_,
					bool bEncrypt_
					);

				/// <summary>
				/// ��Ӽ�¼�����Զ����ܣ���¼��СΪ����ʱ�趨�Ĵ�С��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="byRecord_">Ҫ��ӵļ�¼</param>
				static void AddRecord(IntPtr hFile_, array<unsigned char>^ byRecord_)
				{
					AddRecord(hFile_, byRecord_, true);
				}

				/// <summary>
				/// ��ȡ��¼����¼��СΪ����ʱ�趨�Ĵ�С��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="byRecord_">��ȡ�ļ�¼����������С����С��RecordSize��</param>
				/// <param name="bDecrypt_">�Ƿ���Ҫ�Լ�¼���н���</param>
				static void GetRecord
					(
					IntPtr hFile_,
					array<unsigned char>^ byRecord_,
					bool bDecrypt_
					);

				/// <summary>
				/// ��ȡ��¼����¼��СΪ����ʱ�趨�Ĵ�С����
				/// û�ж�����Ϣʱ���׳�SHFileException(HandleEof)
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="bDecrypt_">�Ƿ���Ҫ�Լ�¼���н���</param>
				/// <returns>��ȡ�ļ�¼</returns>
				static array<unsigned char>^ GetRecord(IntPtr hFile_, bool bDecrypt_)
				{
					int nSize = GetRecordSize(hFile_);
					array<unsigned char>^ byRecord = gcnew array<unsigned char>(nSize);
					GetRecord(hFile_, byRecord, bDecrypt_);
					return byRecord;
				}

				/// <summary>
				/// ��ȡ��¼�����Զ����ܣ���¼��СΪ����ʱ�趨�Ĵ�С����
				/// û�ж�����Ϣʱ���׳�SHFileException(HandleEof)
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <returns>��ȡ�ļ�¼</returns>        
				static array<unsigned char>^ GetRecord(IntPtr hFile_)
				{
					return GetRecord(hFile_, true);
				}

				/// <summary>
				/// ��ȡ���ݣ��Զ����ܣ���Ҫ���趨����Կ����
				/// û�ж�����Ϣʱ���׳�SHFileException(HandleEof)
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="byData_">��ȡ�����ݣ�����ʱ������ռ䲻��С��nLen��</param>
				/// <param name="nLen_">Ҫ��ȡ���ݳ���</param>
				static void Read
					(
					IntPtr hFile_,
					array<unsigned char>^ byData_,
					int nLen_
					);

				/// <summary>
				/// ��ȡ���ݣ��Զ����ܣ���Ҫ���趨����Կ����
				/// û�ж�����Ϣʱ���׳�SHFileException(HandleEof)
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="nLen_">Ҫ��ȡ���ݳ���</param>
				/// <returns>��ȡ������</returns>
				static array<unsigned char>^ Read(IntPtr hFile_, int nLen_)
				{
					array<unsigned char>^ byData = gcnew array<unsigned char>(nLen_);
					Read(hFile_, byData, nLen_);
					return byData;
				}

				/// <summary>
				/// д���ݣ��Զ����ܣ���Ҫ���趨����Կ��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="byData_">Ҫд������</param>
				/// <param name="nLen_">���ݳ���</param>
				static void Write
					(
					IntPtr hFile_,
					array<unsigned char>^ byData_,
					int nLen_
					);

				/// <summary>
				/// д���ݣ��Զ����ܣ���Ҫ���趨����Կ��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="byData_">Ҫд������</param>
				static void Write(IntPtr hFile_, array<unsigned char>^ byData_)
				{
					Write(hFile_, byData_, byData_->Length);
				}

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
				/// ʹ��SHA1�����ݽ���ǩ����ʹ��SHA1�����ݽ���ɢ�У���ǩ���󣬾Ͳ��ܼ������������
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="euAlg_">��Կ��Ӧ���㷨</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nOwnerID_">�ļ�������ID</param>
				/// <param name="byPriKey_">ǩ���õ�˽Կ</param>
				static void Sign
					(
					IntPtr hFile_,
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nOwnerID_, 
					array<unsigned char>^ byPriKey_
					)
				{
					Sign(hFile_, euAlg_, nCorpID_, nOwnerID_, byPriKey_, SecAlg::Hash::Alg::SHA);
				}

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
				/// <param name="nCipherId_">Ҫ��ӵ��û�ID</param>
				/// <param name="byPubKey_">�û���Կ</param>
				/// <param name="strName_">�û�����</param>
				/// <param name="strEmail_">�û�����</param>
				static void AddUser
					(
					IntPtr hFile_,
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nCipherId_, 
					array<unsigned char>^ byPubKey_, 
					String^ strName_, 
					String^ strEmail_
					);

				/// <summary>
				/// ɾ���û�����������Ȩ�ޣ�Operate���û���¼�ɹ��󣬲ſɣ�
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nCipherId_">Ҫɾ�����û�ID</param>
				static void RemoveUser(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_);

				/// <summary>
				/// ��ȡ�ļ����û�����
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <returns>�û�����</returns>
				static int UserCount(IntPtr hFile_);

				/// <summary>
				/// ��ȡ�ļ���ָ�����û���Ϣ
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="nIndex_">�û�����</param>
				/// <returns>�û���Ϣ</returns>
				static UserInfo^ EnumUser(IntPtr hFile_, int nIndex_);

				/// <summary>
				/// ��ȡ�ļ����û������뱣֤�ļ�û�б���
				/// </summary>
				/// <param name="strFile_">�ļ�����ȫ·����</param>
				/// <returns>�û���Ϣ�б�</returns>
				static List<UserInfo^>^ GetUsers(String^ strFile_)
				{
					IntPtr hFile = IntPtr::Zero;
					List<UserInfo^>^  lstInfo = nullptr;
					try 
					{
						hFile = Open(strFile_, nullptr);
						int nCount = UserCount(hFile);
						lstInfo = gcnew List<UserInfo^>(nCount);
						for( int i=0 ; i<nCount ; ++i )
						{
							lstInfo->Add(EnumUser(hFile, i));
						}
					}
					finally
					{
						if(IntPtr::Zero != hFile )
							Close(hFile);
					}

					return lstInfo;
				}

				/// <summary>
				/// �趨�û�Ȩ��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nCipherId_">�û�ID</param>
				/// <param name="euRight_">�û�Ȩ��</param>
				static void SetRights(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_, Rights euRight_);

				/// <summary>
				/// ��ȡ�û�Ȩ��
				/// </summary>
				/// <param name="hFile_">�ļ����</param>
				/// <param name="nCorpID_">��˾ID</param>
				/// <param name="nCipherId_">�û�ID</param>
				/// <returns>�û�Ȩ��</returns>
				static Rights GetRights(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_);
			};
		} // CreAPI
	} // Xugd
} // SHCre