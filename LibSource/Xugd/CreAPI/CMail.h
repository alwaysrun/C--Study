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
			/// 对钥匙读写接口库(CreCMail.dll)的封装,
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
			public ref class CMail
			{
			public:
				/// <summary>
				/// 当前操作状态
				/// </summary>
				enum class CallbackStatus
				{
					/// <summary>
					/// 无效状态，用于初始化或有效性判断
					/// </summary>
					Invalid = 0,
					/// <summary>
					/// 操作开始（此时获取总的步数）
					/// </summary>
					Begin,
					/// <summary>
					/// 前进一步
					/// </summary>
					Step,
					/// <summary>
					/// 操作完成
					/// </summary>
					End
				};

				/// <summary>
				/// 操作信息
				/// </summary>
				[StructLayout(LayoutKind::Sequential, Pack = 4, CharSet = CharSet::Unicode)]
				value struct CallbackInfo
				{
				public:
					/// <summary>
					/// 操作状态
					/// </summary>
					CallbackStatus euStatus;
					/// <summary>
					/// 当前的步数
					/// </summary>
					unsigned int nCurStep;
					/// <summary>
					/// 总的步数
					/// </summary>
					unsigned int nTotalStep;
					/// <summary>
					/// 当前操作的文件
					/// </summary>
					[MarshalAs(UnmanagedType::ByValTStr, SizeConst = CLen::PathMaxLen+1)]
					String^ strFile;
				};

				/// <summary>
				/// 回调函数委托，用于获取当前的操作状态
				/// </summary>
				/// <param name="stInfo_">操作信息</param>
				/// <returns>如果返回false，说明取消当前操作</returns>
				[UnmanagedFunctionPointer(CallingConvention::StdCall)]
				delegate bool CallbackFun(CallbackInfo% stInfo_);

				/// <summary>
				/// 设定回调函数：如果需要获取当前打包与解包的进度，
				/// 就需要先设定回调函数。
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="delFun_">要设定的回调函数</param>
				static void SetCallback(IntPtr hFile_, CallbackFun^ delFun_);

				/// <summary>
				/// 创建文件
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nOwnerID_">文件所有者ID</param>
				/// <param name="byPriKey_">文件所有者私钥</param>
				/// <param name="nKeyLen_">私钥长度</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <param name="strName_">文件所有者（创建者）名称</param>
				/// <param name="strEmail_">文件所有者邮箱</param>
				/// <param name="euDataAlg_">加密数据时，使用的算法（对称算法，一般为AES）</param>
				/// <returns>文件句柄</returns>
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
				/// 创建文件
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nOwnerID_">文件所有者ID</param>
				/// <param name="byPriKey_">文件所有者私钥</param>
				/// <param name="strName_">文件所有者（创建者）名称</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <param name="strEmail_">文件所有者邮箱</param>
				/// <param name="euDataAlg_">加密数据时，使用的算法（对称算法，一般为AES）</param>
				/// <returns>文件句柄</returns>
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
				/// 创建文件（加密数据时使用AES）
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nOwnerID_">文件所有者ID</param>
				/// <param name="byPriKey_">文件所有者私钥</param>
				/// <param name="nKeyLen_">私钥长度</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <returns>文件句柄</returns>
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
				/// 打开文件
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nUserCipherId_">用户ID</param>
				/// <param name="byPriKey_">用户私钥</param>
				/// <param name="nKeyLen_">私钥长度</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <returns>文件句柄</returns>
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
				/// 打开文件
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nUserCipherId_">用户ID</param>
				/// <param name="byPriKey_">用户私钥</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <returns>文件句柄</returns>
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
				/// 关闭打开的文件，文件使用完成后一定要关闭
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				static void Close(IntPtr hFile_);

				/// <summary>
				/// 打包文件/文件夹（如果为文件夹会自动枚举子文件/文件夹）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="strFile_">文件名（全路径）</param>
				static void Pack
					(
					IntPtr hFile_,
					String^ strFile_
					);

				/// <summary>
				/// 解包文件
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="strPath_">解包的文件所存放在的文件夹</param>
				static void Unpack
					(
					IntPtr hFile_,
					String^ strPath_
					);

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
				/// <param name="nUserCipherId_">要添加的用户ID</param>
				/// <param name="byPubKey_">用户公钥</param>
				/// <param name="strName_">用户名称</param>
				/// <param name="strEmail_">用户邮箱</param>
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
				/// 删除用户（必须以有权限（Operate）用户登录成功后，才可）
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="nCorpID_">公司ID</param>
				/// <param name="nUserCipherId_">要删除的用户ID</param>
				static void RemoveUser(IntPtr hFile_, unsigned int nCorpID_, unsigned int nUserCipherId_);

				/// <summary>
				/// 获取文件的用户，必须保证文件没有被打开
				/// </summary>
				/// <param name="strFile_">文件名（全路径）</param>
				/// <returns>用户信息列表</returns>
				static List<KFile::UserInfo^>^ GetUsers(String^ strFile_)
				{
					return KFile::GetUsers(strFile_);
				}

				/// <summary>
				/// 添加控制信息
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="dtStart_">文件可用的起始时间(不能早于1601-1-1）</param>
				/// <param name="dtEnd_">文件可用的结束时间（不能晚于9998-12-31）</param>
				/// <param name="nCount_">文件允许使用的次数（0为没有限制）</param>
				static void AddCtrlInfo(IntPtr hFile_, DateTime dtStart_, DateTime dtEnd_, int nCount_);

				/// <summary>
				/// 添加控制信息，只控制次数，不控制时间。
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="nCount_">文件允许使用的次数（0为没有限制）</param>
				static void AddCtrlInfo(IntPtr hFile_, int nCount_);

				/// <summary>
				/// 获取控制信息
				/// </summary>
				/// <param name="hFile_">文件句柄</param>
				/// <param name="dtStart_">文件可用的起始时间（没有限制时为DateTime.MinValue）</param>
				/// <param name="dtEnd_">文件可用的结束时间（没有限制时为DateTime.MaxValue）</param>
				/// <returns>文件允许使用的次数（0为没有限制）</returns>
				static int GetCtrlInfo(IntPtr hFile_, [Out] DateTime% dtStart_, [Out] DateTime% dtEnd_);
			};
		} // CreAPI
	} // Xugd
} // SHCre