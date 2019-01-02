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
			/// 对加密文件读写接口库(CreKFile.dll)的封装,
			/// 相关常量在CLen中定义
			/// 
			/// 出错处理--函数出错时，会抛出异常：
			/// 参数相关错误，抛出SHParamException；
			/// 加解密操作相关错误，抛出SHCryptException；
			/// 文件操作相关错误，抛出SHFileException；
			/// 可以通过SHException，捕获我们抛出的所有错误；
			/// 错误码定义在Common下的SHErrorCode。
			/// 
			/// </summary>
			public ref class KFile
			{
			public:
				/// <summary>
				/// 用户权限
				/// </summary>
				[Flags]
				enum class Rights
				{
					/// <summary>
					/// 无效权限，用于初始化或有效性判断
					/// </summary>
					None = 0,
					/// <summary>
					/// 只读权限
					/// </summary>
					Read = 0x01,
					/// <summary>
					/// 写权限
					/// </summary>
					Write = 0x02,
					/// <summary>
					/// 删除权限
					/// </summary>
					Delete = 0x04,
					/// <summary>
					/// 显示权限
					/// </summary>
					Display = 0x08,
					/// <summary>
					/// 复制权限
					/// </summary>
					Copy = 0x010,
					/// <summary>
					/// 打印权限
					/// </summary>
					Print = 0x020,
					/// <summary>
					/// 另存为权限
					/// </summary>
					SaveAs = 0x040,
					/// <summary>
					/// 操作权限（可添加、删除用户等）
					/// </summary>
					Operate = 0x080,
					/// <summary>
					/// 读写权限
					/// </summary>
					ReadWrite = Read | Write
				};

				/// <summary>
				/// 文件类型
				/// </summary>
				[Flags]
				enum class FileType
				{
					/// <summary>
					/// 普通文件，未加密也未签名
					/// </summary>
					None = 0,
					/// <summary>
					/// 加密的文件
					/// </summary>
					Encrypted = 0x01,
					/// <summary>
					/// 签名的文件，只有Sign过的文件才会添加此标识
					/// </summary>
					Signed = 0x02,
					/// <summary>
					/// 通过口令保护的文件，会自动添加Encrypted标识
					/// </summary>
					ENByPsw = 0x04
				};

				/// <summary>
				/// 用户类型
				/// </summary>
				enum class UserType
				{
					/// <summary>
					/// 无效类型，用于初始化或有效性判断
					/// </summary>
					Invalid = 0,
					/// <summary>
					/// 文件所有者（创建者）
					/// </summary>
					Owner,
					/// <summary>
					/// 普通用户
					/// </summary>
					User
				};

				/// <summary>
				/// 枚举用户时，用户信息
				/// </summary>
				[StructLayout(LayoutKind::Sequential, Pack = 4, CharSet = CharSet::Unicode)]
				ref struct UserInfo
				{
				public:
					/// <summary>
					/// 用户类型
					/// </summary>
					UserType euType;
					/// <summary>
					/// 公司ID
					/// </summary>
					unsigned int nCorpID;
					/// <summary>
					/// 用户ID
					/// </summary>
					unsigned int nUserCipherId;
					/// <summary>
					/// 用户权限
					/// </summary>
					Rights euRights;
					/// <summary>
					/// 用户名
					/// </summary>
					[MarshalAs(UnmanagedType::ByValTStr, SizeConst = CLen::NameMaxLen+1)]
					String^ strName;
					/// <summary>
					/// 用户邮箱
					/// </summary>
					[MarshalAs(UnmanagedType::ByValTStr, SizeConst = CLen::EmailMaxLen+1)]
					String^ strEmail;
				};

				/// <summary>
				/// 创建文件
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <param name="euType_">文件类型</param>
				/// <param name="nRecordSize_">存放在文件中记录的大小</param>
				/// <param name="nHeaderSize_">存放在文件中头大小（使用AddHeader与GetHeader来处理）</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <param name="strName_">文件所有者（创建者）名称</param>
				/// <param name="strEmail_">文件所有者邮箱</param>
				/// <param name="euAlg_">加密数据时，使用的算法（对称算法，一般为AES）</param>
				/// <returns>文件句柄</returns>
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
				/// 创建文件
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <param name="euType_">文件类型</param>
				/// <param name="nRecordSize_">存放在文件中记录的大小</param>
				/// <param name="nHeaderSize_">存放在文件中头大小（使用AddHeader与GetHeader来处理）</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <returns>文件句柄</returns>
				static IntPtr Create(String^ strFile_, FileType euType_, int nRecordSize_, int nHeaderSize_, String^ strCryptDll_)
				{
					return Create(strFile_, euType_, nRecordSize_, nHeaderSize_, strCryptDll_, nullptr, nullptr, SecAlg::Crypto::Alg::AES);
				}

				/// <summary>
				/// 打开文件
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <returns>文件句柄</returns>
				static IntPtr Open(String^ strFile_, String^ strCryptDll_);

				/// <summary>
				/// 关闭打开的文件
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				static void Close(IntPtr hFile_);

				/// <summary>
				/// 重设文件指针，移到数据开始出（即可以直接读取数据）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				static void Reset(IntPtr hFile_);

				/// <summary>
				///  获取文件中头的大小
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <returns>头大小</returns>
				static int GetHeaderSize(IntPtr hFile_);

				/// <summary>
				///  获取文件中记录的大小
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <returns>记录大小</returns>
				static int GetRecordSize(IntPtr hFile_);

				/// <summary>
				/// 获取文件中记录条数
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <returns>记录条数</returns>
				static int GetRecordCount(IntPtr hFile_);

				/// <summary>
				/// 获取文件的类型
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <returns>文件的类型</returns>
				static FileType GetType(IntPtr hFile_);

				/// <summary>
				/// 设定密钥；对于加密的文件，只有设定后才能读写
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nCipherId_">用户ID</param>
				/// <param name="byPriKey_">私钥</param>
				/// <param name="nKeyLen_">密钥长度</param>
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
				/// 设定密钥；对于加密的文件，只有设定后才能读写
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nCipherId_">用户ID</param>
				/// <param name="byPriKey_">私钥</param>
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
				/// 设定口令；对于加密的文件，只有设定后才能读写
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="strPsw_">文件口令</param>
				static void SetPsw
					(
					IntPtr hFile_,
					String^ strPsw_
					);

				/// <summary>
				/// 添加头（大小为创建时设定的大小）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="byHeader_">要添加的头信息</param>
				static void AddHeader(IntPtr hFile_, array<unsigned char>^ byHeader_);

				/// <summary>
				/// 获取头（大小为创建时设定的大小）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="byHeader_">获取的头信息</param>
				static void GetHeader(IntPtr hFile_, array<unsigned char>^ byHeader_);

				/// <summary>
				/// 获取头（大小为创建时设定的大小）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <returns>获取的头</returns>
				static array<unsigned char>^ GetHeader(IntPtr hFile_)
				{
					int nSize = GetHeaderSize(hFile_);
					array<unsigned char>^ byHeader = gcnew array<unsigned char>(nSize);
					if(nSize > 0)
						GetHeader(hFile_, byHeader);

					return byHeader;
				}

				/// <summary>
				/// 添加记录（记录大小为创建时设定的大小）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="byRecord_">要添加的记录</param>
				/// <param name="bEncrypt_">是否需要对记录进行加密</param>
				static void AddRecord
					(
					IntPtr hFile_,
					array<unsigned char>^ byRecord_,
					bool bEncrypt_
					);

				/// <summary>
				/// 添加记录，并自动加密（记录大小为创建时设定的大小）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="byRecord_">要添加的记录</param>
				static void AddRecord(IntPtr hFile_, array<unsigned char>^ byRecord_)
				{
					AddRecord(hFile_, byRecord_, true);
				}

				/// <summary>
				/// 获取记录（记录大小为创建时设定的大小）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="byRecord_">获取的记录（缓冲区大小不能小于RecordSize）</param>
				/// <param name="bDecrypt_">是否需要对记录进行解密</param>
				static void GetRecord
					(
					IntPtr hFile_,
					array<unsigned char>^ byRecord_,
					bool bDecrypt_
					);

				/// <summary>
				/// 获取记录（记录大小为创建时设定的大小）：
				/// 没有多余信息时，抛出SHFileException(HandleEof)
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="bDecrypt_">是否需要对记录进行解密</param>
				/// <returns>获取的记录</returns>
				static array<unsigned char>^ GetRecord(IntPtr hFile_, bool bDecrypt_)
				{
					int nSize = GetRecordSize(hFile_);
					array<unsigned char>^ byRecord = gcnew array<unsigned char>(nSize);
					GetRecord(hFile_, byRecord, bDecrypt_);
					return byRecord;
				}

				/// <summary>
				/// 获取记录，并自动解密（记录大小为创建时设定的大小）：
				/// 没有多余信息时，抛出SHFileException(HandleEof)
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <returns>获取的记录</returns>        
				static array<unsigned char>^ GetRecord(IntPtr hFile_)
				{
					return GetRecord(hFile_, true);
				}

				/// <summary>
				/// 读取数据（自动解密，需要已设定了密钥）：
				/// 没有多余信息时，抛出SHFileException(HandleEof)
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="byData_">获取的数据（输入时，数组空间不能小于nLen）</param>
				/// <param name="nLen_">要获取数据长度</param>
				static void Read
					(
					IntPtr hFile_,
					array<unsigned char>^ byData_,
					int nLen_
					);

				/// <summary>
				/// 读取数据（自动解密，需要已设定了密钥）：
				/// 没有多余信息时，抛出SHFileException(HandleEof)
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="nLen_">要获取数据长度</param>
				/// <returns>获取的数据</returns>
				static array<unsigned char>^ Read(IntPtr hFile_, int nLen_)
				{
					array<unsigned char>^ byData = gcnew array<unsigned char>(nLen_);
					Read(hFile_, byData, nLen_);
					return byData;
				}

				/// <summary>
				/// 写数据（自动加密，需要已设定了密钥）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="byData_">要写的数据</param>
				/// <param name="nLen_">数据长度</param>
				static void Write
					(
					IntPtr hFile_,
					array<unsigned char>^ byData_,
					int nLen_
					);

				/// <summary>
				/// 写数据（自动加密，需要已设定了密钥）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="byData_">要写的数据</param>
				static void Write(IntPtr hFile_, array<unsigned char>^ byData_)
				{
					Write(hFile_, byData_, byData_->Length);
				}

				/// <summary>
				/// 对数据进行签名：签名后，就不能继续添加数据了
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nOwnerID_">文件所有者ID</param>
				/// <param name="byPriKey_">签名用的私钥</param>
				/// <param name="euHash_">用于对数据进行散列的算法（一般为Sha1）</param>
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
				/// 使用SHA1对数据进行签名（使用SHA1对数据进行散列）：签名后，就不能继续添加数据了
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nOwnerID_">文件所有者ID</param>
				/// <param name="byPriKey_">签名用的私钥</param>
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
				/// 对数据签名进行进行验证（验证后，文件指针位于数据开始位置，可以直接读取）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nOwnerID_">文件所有者ID</param>
				/// <param name="byPubKey_">验证用的公钥</param>
				/// <returns>验证通过，返回true；否则，返回false</returns>
				static bool Verify
					(
					IntPtr hFile_, 
					SecAlg::Crypto::Alg euAlg_,
					unsigned int nCorpID_,
					unsigned int nOwnerID_,
					array<unsigned char>^ byPubKey_
					);

				/// <summary>
				///  添加用户（必须以有权限（Operate）用户登录成功后，才可）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nCipherId_">要添加的用户ID</param>
				/// <param name="byPubKey_">用户公钥</param>
				/// <param name="strName_">用户名称</param>
				/// <param name="strEmail_">用户邮箱</param>
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
				/// 删除用户（必须以有权限（Operate）用户登录成功后，才可）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nCipherId_">要删除的用户ID</param>
				static void RemoveUser(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_);

				/// <summary>
				/// 获取文件中用户数量
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <returns>用户数量</returns>
				static int UserCount(IntPtr hFile_);

				/// <summary>
				/// 获取文件中指定的用户信息
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="nIndex_">用户索引</param>
				/// <returns>用户信息</returns>
				static UserInfo^ EnumUser(IntPtr hFile_, int nIndex_);

				/// <summary>
				/// 获取文件的用户，必须保证文件没有被打开
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <returns>用户信息列表</returns>
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
				/// 设定用户权限
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nCipherId_">用户ID</param>
				/// <param name="euRight_">用户权限</param>
				static void SetRights(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_, Rights euRight_);

				/// <summary>
				/// 获取用户权限
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nCipherId_">用户ID</param>
				/// <returns>用户权限</returns>
				static Rights GetRights(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_);
			};
		} // CreAPI
	} // Xugd
} // SHCre