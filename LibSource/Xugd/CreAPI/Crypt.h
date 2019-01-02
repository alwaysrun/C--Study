#pragma once
using namespace System;
using namespace System::Runtime::InteropServices;

using namespace SHCre::Xugd::Common;

namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
			/// <summary>
			/// 对加解密算法公共接口库(CreCrypt.dll)的封装：包括对称、非对称以及散列算法。
			/// 相关常量在CLen中定义
			/// 
			/// 出错处理--函数出错时，会抛出异常：
			/// 参数相关错误，抛出SHParamException；
			/// 加解密操作相关错误，抛出SHCryptException；
			/// 动态库相关错误，抛出SHDllException；
			/// 文件操作相关错误，抛出SHFileException；
			/// 可以通过SHException，捕获我们抛出的所有错误；
			/// 错误码定义在Common下的SHErrorCode。
			/// 
			///	加密算法使用：
			///		1、通过CCGetCryptAlg获取算法句柄
			///		2、通过CCGenKey或CCImportKey创建或导入密钥
			///		3、CCEncrypt/CCDecrypt进行加解密操作（非对称CCSign/CCVerify签名验证）
			///		4、CCReleaseCryptAlg释放算法句柄
			///	散列算法使用：
			///		只对一块数据散列可通过CCHashData完成，对多块数据则：
			///		1、通过CCGetHashAlg获取散列算法句柄
			///		2、CCHashInit初始化算法
			///		3、对每块数据通过CCHashUpdate计算散列
			///		4、通过CCHashFinal获取散列值
			///		5、CCReleaseHashAlg释放算法句柄
			/// 
			/// </summary>
			public ref class SecAlg
			{
			private:
				static String^	_strDefCryptDll = L"CreSCrypt.dll";

			public:
				/// <summary>
				/// 添加算法库(从配置文件CreCrypt.ini中获取提供者信息)，算法库只有添加后才可使用
				/// </summary>
				/// <param name="strProvider_">配置文件中算法库提供者名称对应的键值</param>
				/// <returns>算法库的唯一标识</returns>
				static unsigned int Add
					(
					String^ strProvider_
					);

				/// <summary>
				/// 添加缺省算法库(从配置文件CreCrypt.ini中获取键值为CKDefault的值指定的库)，算法库只有添加后才可使用
				/// </summary>
				/// <returns>算法库的唯一标识</returns>
				static unsigned int Add()
				{
					return Add(nullptr);
				}

				/// <summary>
				/// 通过路径直接加载算法库
				/// </summary>
				/// <param name="strDllName_">要加载的库名（一般要与此库放在同一路径下）或全路径</param>
				/// <returns>算法库的唯一标识</returns>
				static unsigned int AddByPath
					(
					String^ strDllName_
					);

				/// <summary>
				/// 通过路径直接加载算法库
				/// </summary>
				/// <param name="strDllName_">算法库（如果为null，则使用默认配置库；如果加载失败，会再尝试加载CreSCrypt.dll，如果还失败，则抛出异常）</param>
				/// <returns>算法库的唯一标识</returns>
				static unsigned int TryAdd
					(
					String^ strDllName_
					);

				/// <summary>
				/// 移除算法库
				/// </summary>
				/// <param name="nProviderID_">算法库的唯一标识</param>
				static void Remove(unsigned int nProviderID_);

				/// <summary>
				/// 判断算法库是否已添加
				/// </summary>
				/// <param name="nProviderID_">算法库的唯一标识</param>
				/// <returns>已添加，返回true；否则，返回false</returns>
				static bool IsAdd(unsigned int nProviderID_);

				/// <summary>
				/// 获取已添加的算法库提供者数量
				/// </summary>
				/// <returns>已添加的算法库数量</returns>
				static int GetCount();

				/// <summary>
				/// 枚举算法库的唯一标识
				/// </summary>
				/// <param name="nIndex_">算法库的索引</param>
				/// <returns>算法库的唯一标识</returns>
				static unsigned int Enum(int nIndex_);

				/// <summary>
				/// 加解密算法相关类
				/// </summary>
				ref class Crypto
				{
				public:
					/// <summary>
					/// 密码算法标识
					/// </summary>
					enum class Alg
					{
						/// <summary>
						/// 无效标识，用于初始化或有效性判断(0)
						/// </summary>
						Invalid = 0,
						/// <summary>
						/// 只用于加密的RSA(0x01)
						/// </summary>
						RSAEncrypt = 0x01,
						/// <summary>
						/// 只用于签名的RSA(0x02)
						/// </summary>
						RSASign = 0x02,
						/// <summary>
						/// RSA算法（非对称算法，可加密也可签名）
						/// </summary>
						RSA = RSAEncrypt | RSASign,
						/// <summary>
						/// 高级加密标准（对称算法）(0x010)
						/// </summary>
						AES = 0x010,
						/// <summary>
						/// 国际数据加密算法（对称算法）
						/// </summary>
						IDEA
					};

					/// <summary>
					/// 密钥类型
					/// </summary>
					[Flags]
					enum class KeyType
					{
						/// <summary>
						/// 无效标识，用于初始化或有效性判断
						/// </summary>
						Invalid = 0,
						/// <summary>
						/// 对称算法的密钥
						/// </summary>
						Symm = 0x01,
						/// <summary>
						/// 公钥
						/// </summary>
						Public = 0x02,
						/// <summary>
						/// 私钥
						/// </summary>
						Private = 0x04
					};

					/// <summary>
					/// 获取提供的加密算法数量
					/// </summary>
					/// <param name="nProviderID_">算法库的唯一标识</param>
					/// <returns>加密算法数量</returns>
					static int GetCount(unsigned int nProviderID_);

					/// <summary>
					/// 枚举加密算法
					/// </summary>
					/// <param name="nProviderID_">算法库的唯一标识</param>
					/// <param name="nIndex_">算法索引</param>
					/// <returns>算法标识</returns>
					static Alg Enum(unsigned int nProviderID_, int nIndex_);

					/// <summary>
					/// 获取算法句柄，使用完成后需要通过ReleaseAlg来释放
					/// </summary>
					/// <param name="nProviderID_">算法库的唯一标识</param>
					/// <param name="euAlg_">算法标识</param>
					/// <returns>算法句柄</returns>
					static IntPtr GetAlg(unsigned int nProviderID_, Alg euAlg_);

					/// <summary>
					/// 获取算法句柄，使用完成后需要通过ReleaseAlg来释放
					/// </summary>
					/// <param name="strDllPath_">算法库（如果为null，则使用默认配置库；如果加载失败，会再尝试加载CreSCrypt.dll，如果还失败，则抛出异常）</param>
					/// <param name="euAlg_">算法标识</param>
					/// <returns>算法句柄</returns>
					static IntPtr GetAlg(String^ strDllPath_, Alg euAlg_ )
					{
						return GetAlg(TryAdd(strDllPath_), euAlg_);
					}

					/// <summary>
					/// 释放算法句柄
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					static void ReleaseAlg(IntPtr hAlg_);

					/// <summary>
					/// 获取钥匙长度，只有创建了密钥后才能获取正确的长度
					/// </summary>
					/// <param name="hAlg_">算法库的唯一标识</param>
					/// <returns>密钥长度</returns>
					static int GetKeyLen(IntPtr hAlg_);

					/// <summary>
					/// 获取加密块长度，只有创建了密钥后才能获取正确的长度
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <returns>加密块长度</returns>
					static int GetBlockSize(IntPtr hAlg_);

					/// <summary>
					/// 根据数据长度获取密文长度(在填充情况下的长度，作为最后一块)
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="nDataLen_">数据的长度</param>
					/// <returns>密文的长度</returns>
					static int GetCipherLen(IntPtr hAlg_, int nDataLen_);

					/// <summary>
					/// 生成密钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="bySeed_">用于生成密钥的种子（传递null或长度为零，则生成随机种子）</param>
					/// <param name="nKeyLen_">生成的密钥长度</param>
					static void GenKey(IntPtr hAlg_, array<unsigned char>^ bySeed_, int nKeyLen_);

					/// <summary>
					/// 生成密钥，相同的种子生成相同的密钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="strSeed_">用于生成密钥的种子</param>
					/// <param name="nKeyLen_">生成的密钥长度</param>
					static void GenKey(IntPtr hAlg_, String^ strSeed_, int nKeyLen_)
					{
						array<unsigned char>^ bySeed = XConvert::UnicodeString2Bytes(strSeed_);
						GenKey(hAlg_, bySeed, nKeyLen_);
					}

					/// <summary>
					/// 生成缺省长度的密钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="strSeed_">用于生成密钥的种子</param>
					static void GenKey(IntPtr hAlg_, String^ strSeed_)
					{
						GenKey(hAlg_, strSeed_, 0);
					}

					/// <summary>
					/// 生成缺省长度的密钥（根据随机数生成，每次生成的都不一样）
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					static void GenKey(IntPtr hAlg_)
					{
						GenKey(hAlg_, String::Empty, 0);
					}

					/// <summary>
					/// 销毁密钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					static void DestroyKey(IntPtr hAlg_);

					/// <summary>
					/// 导入密钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byKey_">要导入的密钥</param>
					/// <param name="nLen_">要导入的密钥长度</param>
					/// <param name="euKeyType_">密钥类型</param>
					static void ImportKey(IntPtr hAlg_, array<unsigned char>^ byKey_, int nLen_, KeyType euKeyType_);

					/// <summary>
					/// 导入密钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byKey_">要导入的密钥</param>
					/// <param name="euKeyType_">密钥类型</param>
					static void ImportKey(IntPtr hAlg_, array<unsigned char>^ byKey_, KeyType euKeyType_)
					{
						ImportKey(hAlg_, byKey_, byKey_->Length, euKeyType_);
					}

					/// <summary>
					/// 导出密钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="euKeyType_">密钥类型</param>
					/// <returns>密钥</returns>
					static array<unsigned char>^ ExportKey(IntPtr hAlg_, KeyType euKeyType_);

					/// <summary>
					/// 加密数据
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byData_">要加密的数据</param>
					/// <param name="nDataLen_">要加密的数据的长度</param>
					/// <param name="byCipher_">加密后的数据，如果为nullptr则只返回加密数据的长度</param>
					/// <param name="nCipherLen_">输入时为密文缓冲区长度，输出时为加密后的数据长度</param>
					/// <param name="bFinal_">是否是最后一块数据，如果是算法会自动填充为数据块的整数倍，否则需要用户保证是数据块的整数倍</param>
					static void Encrypt
						(
						IntPtr hAlg_,
						array<unsigned char>^ byData_,
						int nDataLen_,
						array<unsigned char>^ byCipher_,
						int% nCipherLen_,
						bool bFinal_
						);

					/// <summary>
					/// 加密数据
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byData_">要加密的数据</param>
					/// <param name="nDataLen_">要加密数据的长度（不能大于数据的实际长度）</param>
					/// <param name="bFinal_">是否是最后一块数据，如果是算法会自动填充为数据块的整数倍，否则需要用户保证是数据块的整数倍</param>
					/// <returns>加密后的数据</returns>
					static array<unsigned char>^ Encrypt(IntPtr hAlg_, array<unsigned char>^ byData_, int nDataLen_, bool bFinal_)
					{
						int nCipherLen = nDataLen_;
						if (bFinal_)
							nCipherLen = GetCipherLen(hAlg_, nDataLen_);
						array<unsigned char>^ byCipher = gcnew array<unsigned char>(nCipherLen);
						Encrypt(hAlg_, byData_, nDataLen_, byCipher, nCipherLen, bFinal_);
						return byCipher;
					}

					/// <summary>
					/// 加密数据
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byData_">要加密的数据</param>
					/// <param name="bFinal_">是否是最后一块数据，如果是算法会自动填充为数据块的整数倍，否则需要用户保证是数据块的整数倍</param>
					/// <returns>加密后的数据</returns>
					static array<unsigned char>^ Encrypt(IntPtr hAlg_, array<unsigned char>^ byData_, bool bFinal_)
					{
						return Encrypt(hAlg_, byData_, byData_->Length, bFinal_);
					}

					/// <summary>
					/// 加密最后一块数据，自动对数据进行填充
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byData_">要加密的数据</param>
					/// <param name="nDataLen_">要加密数据的长度（不能大于数据的实际长度）</param>
					/// <returns>加密后的数据</returns>
					static array<unsigned char>^ Encrypt(IntPtr hAlg_, array<unsigned char>^ byData_, int nDataLen_)
					{
						return Encrypt(hAlg_, byData_, nDataLen_, true);
					}

					/// <summary>
					/// 加密最后一块数据，自动对数据进行填充
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byData_">要加密的数据</param>
					/// <returns>加密后的数据</returns>
					static array<unsigned char>^ Encrypt(IntPtr hAlg_, array<unsigned char>^ byData_)
					{
						return Encrypt(hAlg_, byData_, byData_->Length);
					}

					/// <summary>
					/// 解密数据
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byCipher_">要解密的数据</param>
					/// <param name="nCipherLen_">要解密的数据的长度</param>
					/// <param name="byData_">解密的数据，如果为nullptr则只返回解密数据的长度</param>
					/// <param name="nDataLen_">输入时为数据缓冲区长度，输出时为解密后的数据长度</param>
					/// <param name="bFinal_">是否是最后一块数据，如果是算法会自动提取填充长度，否则直接解密</param>
					static void Decrypt
						(
						IntPtr hAlg_,
						array<unsigned char>^ byCipher_,
						int nCipherLen_,
						array<unsigned char>^ byData_,
						int% nDataLen_,
						bool bFinal_
						);

					/// <summary>
					/// 解密数据
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byCipher_">要解密的数据</param>
					/// <param name="byCipher_">要解密的数据的长度（不能大于实际长度）</param>
					/// <param name="bFinal_">是否是最后一块数据，如果是算法会自动提取填充长度，否则直接解密</param>
					/// <returns>解密的数据</returns>
					static array<unsigned char>^ Decrypt(IntPtr hAlg_, array<unsigned char>^ byCipher_, int nCipherLen_, bool bFinal_)
					{
						int nDataLen = nCipherLen_;
						array<unsigned char>^ byData = gcnew array<unsigned char>(nDataLen);
						Decrypt(hAlg_, byCipher_, nCipherLen_, byData, nDataLen, bFinal_);
						if (nDataLen != nCipherLen_)
						{
							array<unsigned char>^ byRet = gcnew array<unsigned char>(nDataLen);
							Array::Copy(byData, byRet, nDataLen);
							return byRet;
						}

						return byData;
					}

					/// <summary>
					/// 解密数据
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byCipher_">要解密的数据</param>
					/// <param name="bFinal_">是否是最后一块数据，如果是算法会自动提取填充长度，否则直接解密</param>
					/// <returns>解密的数据</returns>
					static array<unsigned char>^ Decrypt(IntPtr hAlg_, array<unsigned char>^ byCipher_, bool bFinal_)
					{
						return Decrypt(hAlg_, byCipher_, byCipher_->Length, bFinal_);
					}

					/// <summary>
					/// 解密最后一块数据，算法会自动提取填充长度
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byCipher_">要解密的数据</param>
					/// <param name="byCipher_">要解密的数据的长度（不能大于实际长度）</param>
					/// <returns>解密的数据</returns>
					static array<unsigned char>^ Decrypt(IntPtr hAlg_, array<unsigned char>^ byCipher_, int nCipherLen_)
					{
						return Decrypt(hAlg_, byCipher_, nCipherLen_, true);
					}

					/// <summary>
					/// 解密最后一块数据，算法会自动提取填充长度
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byCipher_">要解密的数据</param>
					/// <returns>解密的数据</returns>
					static array<unsigned char>^ Decrypt(IntPtr hAlg_, array<unsigned char>^ byCipher_)
					{
						return Decrypt(hAlg_, byCipher_, byCipher_->Length);
					}

					/// <summary>
					/// 签名数据：一般都是对数据的散列值进行签名（自动填充到数据块的整数倍）
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byMsg_">要签名的数据</param>
					/// <param name="nMsgLen_">要签名的数据的长度</param>
					/// <param name="bySign_">签名结果数据，如果为nullptr则只返回签名数据的长度</param>
					/// <param name="nSignLen_">输入时为缓冲区长度，输出时为签名后的数据长度</param>
					static void Sign
						(
						IntPtr hAlg_,
						array<unsigned char>^ byMsg_,
						int nMsgLen_,
						array<unsigned char>^ bySign_,
						int% nSignLen_
						);

					/// <summary>
					/// 签名数据：一般都是对数据的散列值进行签名（自动填充到数据块的整数倍）
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byMsg_">要签名的数据</param>
					/// <param name="nMsgLen_">要签名的数据的长度（不能大于实际的长度）</param>
					/// <returns>签名结果数据</returns>
					static array<unsigned char>^ Sign(IntPtr hAlg_, array<unsigned char>^ byMsg_, int nMsgLen_)
					{
						int nSignLen = CLen::RSASignMaxSize;
						array<unsigned char>^ bySign = gcnew array<unsigned char>(nSignLen);
						Sign(hAlg_, byMsg_, nMsgLen_, bySign, nSignLen);
						if (nSignLen != bySign->Length)
						{
							array<unsigned char>^ byRet = gcnew array<unsigned char>(nSignLen);
							Array::Copy(bySign, byRet, nSignLen);
							return byRet;
						}

						return bySign;
					}

					/// <summary>
					/// 签名数据：一般都是对数据的散列值进行签名（自动填充到数据块的整数倍）
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byMsg_">要签名的数据</param>
					/// <returns>签名结果数据</returns>
					static array<unsigned char>^ Sign(IntPtr hAlg_, array<unsigned char>^ byMsg_)
					{
						return Sign(hAlg_, byMsg_, byMsg_->Length);
					}

					/// <summary>
					/// 验证签名数据
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byMsg_">要验证的数据</param>
					/// <param name="nMsgLen_">数据长度</param>
					/// <param name="bySign_">数据的签名</param>
					/// <param name="nSignLen_">签名长度</param>
					/// <returns>验证通过，返回true；否则，返回false</returns>
					static bool Verify
						(
						IntPtr hAlg_,
						array<unsigned char>^ byMsg_,
						int nMsgLen_,
						array<unsigned char>^ bySign_,
						int nSignLen_
						);

					/// <summary>
					/// 验证签名数据
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byMsg_">要验证的数据</param>
					/// <param name="bySign_">数据的签名</param>
					/// <returns>验证通过，返回true；否则，返回false</returns>
					static bool Verify(IntPtr hAlg_, array<unsigned char>^ byMsg_, array<unsigned char>^ bySign_)
					{
						return Verify(hAlg_, byMsg_, byMsg_->Length, bySign_, bySign_->Length);
					}

					/// <summary>
					/// 对文件内容进行加密：同样内容在不同位置出现，加密后内容不同
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byData_">数据时为加密数据，输出时为加密后的数据</param>
					/// <param name="nLen_">要加密的数据的长度（必须为数据块的整数倍）</param>
					/// <param name="nOffSet_">数据在文件中的偏移位置（必须为sizeof(int)的整数倍）</param>
					static void FileEncrypt
						(
						IntPtr hAlg_,
						array<unsigned char>^ byData_,
						int nLen_,
						unsigned int nOffSet_
						);

					/// <summary>
					/// 对加密文件内容进行解密
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byData_">输入时为要解密的数据，输出时为解密后的数据</param>
					/// <param name="nLen_">要解密密的数据的长度（必须为数据块的整数倍）</param>
					/// <param name="nOffSet_">数据在文件中的偏移位置（必须为sizeof(int)的整数倍）</param>
					static void FileDecrypt
						(
						IntPtr hAlg_,
						array<unsigned char>^ byData_,
						int nLen_,
						unsigned int nOffSet_
						);

					/// <summary>
					/// 通过私钥获取公钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byKey_">输入时为私钥，输出时为公钥</param>
					/// <returns>公钥长度</returns>
					static int ConvertKey(IntPtr hAlg_, array<unsigned char>^ byKey_);

					/// <summary>
					/// 通过私钥获取公钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byPrivate_">私钥</param>
					/// <param name="nLen_">私钥长度</param>
					/// <returns>公钥</returns>
					static array<unsigned char>^ GetPubKey(IntPtr hAlg_, array<unsigned char>^ byPrivate_, int nLen_)
					{
						array<unsigned char>^ byPublic = gcnew array<unsigned char>(nLen_);
						Array::Copy(byPrivate_, byPublic, nLen_);
						ConvertKey(hAlg_, byPublic);
						return byPublic;
					}

					/// <summary>
					/// 通过私钥获取公钥
					/// </summary>
					/// <param name="hAlg_">算法句柄</param>
					/// <param name="byPrivate_">私钥</param>
					/// <returns>公钥</returns>
					static array<unsigned char>^ GetPubKey(IntPtr hAlg_, array<unsigned char>^ byPrivate_)
					{
						return GetPubKey(hAlg_, byPrivate_, byPrivate_->Length);
					}

					/// <summary>
					/// 通过私钥获取公钥
					/// </summary>
					/// <param name="strDllPath_">密钥库提供者文件（包括路径）</param>
					/// <param name="euAlg_">算法标识</param>
					/// <param name="byPrivate_">私钥</param>
					/// <param name="nLen_">私钥长度</param>
					/// <returns>公钥</returns>
					static array<unsigned char>^ GetPubKey
						(
						String^ strDllPath_,
						Alg euAlg_,
						array<unsigned char>^ byPrivate_,
						int nLen_
						);

					/// <summary>
					/// 通过私钥获取公钥
					/// </summary>
					/// <param name="strDllPath_">密钥库提供者文件（包括路径）</param>
					/// <param name="euAlg_">算法标识</param>
					/// <param name="byPrivate_">私钥</param>
					/// <returns>公钥</returns>
					static array<unsigned char>^ GetPubKey
						(
						String^ strDllPath_,
						Alg euAlg_,
						array<unsigned char>^ byPrivate_
						)
					{
						return GetPubKey(strDllPath_, euAlg_, byPrivate_, byPrivate_->Length);
					}

					/// <summary>
					/// 获取用户的标识（用户当前使用密钥的ID）
					/// </summary>
					/// <param name="byKey_">要获取的ID的私钥</param>
					/// <returns>密钥ID</returns>
					static unsigned int GetKeyID(array<unsigned char>^ byKey_);

					/// <summary>
					/// 根据输入参数构建密钥
					/// </summary>
					/// <param name="strDllPath_">密钥库提供者文件（包括路径）</param>
					/// <param name="euAlg_">算法标识</param>
					/// <param name="bySeed_">用于创建密钥的种子</param>
					/// <returns>生成的密钥（RSA为私钥，对称算法为密钥）</returns>
					static array<unsigned char>^ BuildKey
						(
						String^ strDllPath_,
						Alg euAlg_,
						array<unsigned char>^ bySeed_
						);

					/// <summary>
					/// 根据输入参数构建密钥
					/// </summary>
					/// <param name="strDllPath_">密钥库提供者文件（包括路径）</param>
					/// <param name="euAlg_">算法标识</param>
					/// <param name="strSeed_">用于创建密钥的种子</param>
					/// <returns>生成的密钥（RSA为私钥，对称算法为密钥）</returns>
					static array<unsigned char>^ BuildKey
						(
						String^ strDllPath_,
						Alg euAlg_,
						String^ strSeed_
						)
					{
						array<unsigned char>^ bySeed = System::Text::Encoding::Unicode->GetBytes(strSeed_);
						return BuildKey(strDllPath_, euAlg_, bySeed);
					}
				};


				/// <summary>
				/// 散列算法相关类
				/// </summary>
				ref class Hash
				{
				public:
					/// <summary>
					/// 散列算法标识
					/// </summary>
					enum class Alg
					{
						/// <summary>
						/// 无效标识，用于初始化或有效性判断
						/// </summary>
						Invalid = 0,
						/// <summary>
						/// 消息摘要算法第五版，128bit（16Byte）
						/// </summary>
						MD5,
						/// <summary>
						/// 安全散列算法（SHA1，160bit（20Byte））
						/// </summary>
						SHA,
						/// <summary>
						/// 256bit（32Byte）
						/// </summary>
						SHA256 = 8,
						/// <summary>
						/// 384bit（48Byte）
						/// </summary>
						SHA384,
						/// <summary>
						/// 512bit（64Byte）
						/// </summary>
						SHA512
					};

					/// <summary>
					/// 获取散列算法的数量
					/// </summary>
					/// <param name="nProviderID_">算法库的唯一标识</param>
					/// <returns>散列算法数量</returns>
					static int GetCount(unsigned int nProviderID_);

					/// <summary>
					/// 枚举散列算法
					/// </summary>
					/// <param name="nProviderID_">算法库的唯一标识</param>
					/// <param name="nIndex_">散列算法索引</param>
					/// <returns>散列算法标识</returns>
					static Alg Enum(unsigned int nProviderID_, int nIndex_);

					/// <summary>
					/// 获取散列算法句柄：使用完成后，要通过ReleaseAlg来释放
					/// </summary>
					/// <param name="nProviderID_">算法库的唯一标识</param>
					/// <param name="euAlg_">散列算法标识</param>
					/// <returns>算法句柄</returns>
					static IntPtr GetAlg(unsigned int nProviderID_, Alg euAlg_);

					/// <summary>
					/// 获取算法句柄，使用完成后需要通过ReleaseAlg来释放
					/// </summary>
					/// <param name="strDllPath_">算法库（如果为null，则使用默认配置库；如果加载失败，会再尝试加载CreSCrypt.dll，如果还失败，则抛出异常）</param>
					/// <param name="euAlg_">散列算法标识</param>
					/// <returns>算法句柄</returns>
					static IntPtr GetAlg(String^ strDllPath_, Alg euAlg_ )
					{
						return GetAlg(TryAdd(strDllPath_), euAlg_);
					}

					/// <summary>
					/// 释放散列算法句柄
					/// </summary>
					/// <param name="hAlg_">散列算法句柄</param>
					static void ReleaseAlg(IntPtr hAlg_);

					/// <summary>
					/// 获取散列值的大小（byte）
					/// </summary>
					/// <param name="hAlg_">散列算法句柄</param>
					/// <returns>散列值大小</returns>
					static int GetSize(IntPtr hAlg_);

					/// <summary>
					/// 初始化散列算法，再重新计算散列值前，需要先初始化
					/// </summary>
					/// <param name="hAlg_">散列算法句柄</param>
					static void Init(IntPtr hAlg_);

					/// <summary>
					/// 更新散列值（计算数据的散列值，并添加到原来的散列值里面）
					/// </summary>
					/// <param name="hAlg_">散列算法句柄</param>
					/// <param name="byData_">要计算散列值的数据</param>
					/// <param name="nLen_">数据长度</param>
					static void Update
						(
						IntPtr hAlg_,
						array<unsigned char>^ byData_,
						int nLen_
						);

					/// <summary>
					/// 更新散列值（计算数据的散列值，并添加到原来的散列值里面）
					/// </summary>
					/// <param name="hAlg_">散列算法句柄</param>
					/// <param name="byData_">要计算散列值的数据</param>
					static void Update(IntPtr hAlg_, array<unsigned char>^ byData_)
					{
						Update(hAlg_, byData_, byData_->Length);
					}

					/// <summary>
					/// 完成散列值计算，并获取散列值
					/// </summary>
					/// <param name="hAlg_">散列算法句柄</param>
					/// <returns>散列值</returns>
					static array<unsigned char>^ Final(IntPtr hAlg_);

					/// <summary>
					/// 完成散列值计算，并获取散列值
					/// </summary>
					/// <param name="nProviderID_">算法库的唯一标识</param>
					/// <param name="euAlg_">散列算法</param>
					/// <param name="byData_">要计算散列值的数据</param>
					/// <param name="nDataLen_">数据长度</param>
					/// <param name="byHash_">获取散列值</param>
					/// <param name="nHashLen_">输入时为缓冲区长度，输出时为散列值长度</param>
					static void HashData
						(
						unsigned int nProviderID_,
						Alg euAlg_,
						array<unsigned char>^ byData_,
						int nDataLen_,
						array<unsigned char>^ byHash_,
						int% nHashLen_
						);

					/// <summary>
					/// 完成SHA散列值计算，并获取散列值
					/// </summary>
					/// <param name="nProviderID_">算法库的唯一标识</param>
					/// <param name="byData_">要计算散列值的数据</param>
					/// <returns>散列值</returns>
					static array<unsigned char>^ HashData(unsigned int nProviderID_, array<unsigned char>^ byData_)
					{
						int nLen = CLen::HashSize;
						array<unsigned char>^ byHash = gcnew array<unsigned char>(nLen);
						HashData(nProviderID_, Alg::SHA, byData_, byData_->Length, byHash, nLen);
						return byHash;
					}
				};     
			};
		} // CreAPI
	} // Xugd
} // SHCre