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
			/// 对加密数据读写接口库(CreKData.dll)的封装,
			/// 相关常量在CLen中定义
			/// 
			/// 出错处理--函数出错时，会抛出异常：
			/// 参数相关错误，抛出SHParamException；
			/// 加解密操作相关错误，抛出SHCryptException；
			/// 数据操作相关错误，抛出SHFileException；
			/// 可以通过SHException，捕获我们抛出的所有错误；
			/// 错误码定义在Common下的SHErrorCode。
			/// 
			/// </summary>
			public ref class KData
			{
			public:
				/// <summary>
				/// 数据类型
				/// </summary>
				[Flags]
				enum class DataType
				{
					/// <summary>
					/// 无效，用于初始化
					/// </summary>
					None = 0,
					/// <summary>
					/// 加密的数据
					/// </summary>
					Encrypted = 0x01,
					/// <summary>
					/// 签名的数据，只有Sign过的数据才会添加此标识
					/// </summary>
					Signed = 0x02,
					/// <summary>
					/// 通过口令保护的数据，会自动添加Encrypted标识
					/// </summary>
					ENByPsw = 0x04
				};

				/// <summary>
				/// 创建，用于加密或签名记录
				/// </summary>
				/// <param name="euType_">数据类型</param>
				/// <param name="nRecordSize_">记录的大小</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				/// <param name="strName_">所有者（创建者）名称</param>
				/// <param name="strEmail_">文所有者邮箱</param>
				/// <returns>数据句柄</returns>
				static IntPtr Create
					(
					DataType euType_,
					int nRecordSize_,
					String^ strCryptDll_,
					String^ strName_,
					String^ strEmail_
					);

				/// <summary>
				/// 使用默认加密库创建，用于加密或签名记录
				/// </summary>
				/// <param name="euType_">数据类型</param>
				/// <param name="nRecordSize_">记录的大小</param>
				/// <returns>数据句柄</returns>
				static IntPtr Create(DataType euType_, int nRecordSize_)
				{
					return Create(euType_, nRecordSize_, nullptr, nullptr, nullptr);
				}

				/// <summary>
				/// 打开，用于解密或验证记录
				/// </summary>
				/// <param name="nRecordSize_">记录的大小</param>
				/// <param name="strCryptDll_">加密库的路径（null使用默认加密库）</param>
				static IntPtr Open(int nRecordSize_, String^ strCryptDll_);

				/// <summary>
				/// 使用默认加密库打开，用于解密或验证记录
				/// </summary>
				/// <param name="nRecordSize_">记录的大小</param>
				static IntPtr Open(int nRecordSize_)
				{
					return Open(nRecordSize_, nullptr);
				}

				/// <summary>
				/// 关闭打开的句柄
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				static void Close(IntPtr hData_);

				/// <summary>
				///  获取数据中记录的大小
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <returns>记录大小</returns>
				static int GetRecordSize(IntPtr hData_);

				/// <summary>
				///  获取数据中加密后记录的大小
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <returns>加密后记录大小（包括头与加密者信息）</returns>
				static int GetCryptSize(IntPtr hData_);

				/// <summary>
				///  获取数据中签名后记录的大小
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <returns>签名后记录大小（包括头与签名者信息）</returns>
				static int GetSignSize(IntPtr hData_);

				/// <summary>
				/// 设定密钥
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpId_">公司ID</param>
				/// <param name="nCipherId_">用户的密钥标识</param>
				/// <param name="byKey_">密钥</param>
				/// <param name="nKeyLen_">密钥长度</param>
				/// <param name="bPrivate_">是否是私钥</param>
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
				/// 设定密钥
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <param name="euAlg_">密钥对应的算法</param>
				/// <param name="nCorpId_">公司ID</param>
				/// <param name="nCipherId_">用户的密钥标识</param>
				/// <param name="byKey_">密钥</param>
				/// <param name="bPrivate_">是否是私钥</param>
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
				/// 设定口令
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <param name="strPsw_">口令</param>
				static void SetPsw(IntPtr hData_, String^ strPsw_);

				/// <summary>
				/// 加密数据（记录）
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <param name="byData_">要加密的数据</param>
				/// <returns>加密后的数据（包括头与加密者信息）</returns>
				static array<unsigned char>^ Encrypt
					(
					IntPtr hData_,
					array<unsigned char>^ byData_
					);

				/// <summary>
				/// 解密数据
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <param name="byCipher_">要解密的数据（包括头与加密者信息）</param>
				/// <returns>解密后的数据</returns>
				static array<unsigned char>^ Decrypt
					(
					IntPtr hData_,
					array<unsigned char>^ byCipher_
					);

				/// <summary>
				/// 签名数据（记录）
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <param name="byData_">要签名的数据</param>
				/// <returns>签名后的数据（包括头与签名信息）</returns>
				static array<unsigned char>^ Sign
					(
					IntPtr hData_,
					array<unsigned char>^ byData_
					);

				/// <summary>
				/// 验证数据
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <param name="hData_">要验证的数据（包括头与签名信息）</param>
				/// <returns>原数据</returns>
				static array<unsigned char>^ VerifyData
					(
					IntPtr hData_,
					array<unsigned char>^ bySign_
					);

				/// <summary>
				/// 验证数据
				/// </summary>
				/// <param name="hData_">数据句柄</param>
				/// <param name="hData_">要验证的数据（包括头与签名信息）</param>
				/// <returns>验证通过，返回true；否则，返回false</returns>
				static bool VerifyOnly
					(
					IntPtr hData_,
					array<unsigned char>^ bySign_
					);
			};
		} // CreAPI
	} // Xugd
} // SHCre