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
			/// �Լӽ����㷨�����ӿڿ�(CreCrypt.dll)�ķ�װ�������Գơ��ǶԳ��Լ�ɢ���㷨��
			/// ��س�����CLen�ж���
			/// 
			/// ������--��������ʱ�����׳��쳣��
			/// ������ش����׳�SHParamException��
			/// �ӽ��ܲ�����ش����׳�SHCryptException��
			/// ��̬����ش����׳�SHDllException��
			/// �ļ�������ش����׳�SHFileException��
			/// ����ͨ��SHException�����������׳������д���
			/// �����붨����Common�µ�SHErrorCode��
			/// 
			///	�����㷨ʹ�ã�
			///		1��ͨ��CCGetCryptAlg��ȡ�㷨���
			///		2��ͨ��CCGenKey��CCImportKey����������Կ
			///		3��CCEncrypt/CCDecrypt���мӽ��ܲ������ǶԳ�CCSign/CCVerifyǩ����֤��
			///		4��CCReleaseCryptAlg�ͷ��㷨���
			///	ɢ���㷨ʹ�ã�
			///		ֻ��һ������ɢ�п�ͨ��CCHashData��ɣ��Զ��������
			///		1��ͨ��CCGetHashAlg��ȡɢ���㷨���
			///		2��CCHashInit��ʼ���㷨
			///		3����ÿ������ͨ��CCHashUpdate����ɢ��
			///		4��ͨ��CCHashFinal��ȡɢ��ֵ
			///		5��CCReleaseHashAlg�ͷ��㷨���
			/// 
			/// </summary>
			public ref class SecAlg
			{
			private:
				static String^	_strDefCryptDll = L"CreSCrypt.dll";

			public:
				/// <summary>
				/// ����㷨��(�������ļ�CreCrypt.ini�л�ȡ�ṩ����Ϣ)���㷨��ֻ����Ӻ�ſ�ʹ��
				/// </summary>
				/// <param name="strProvider_">�����ļ����㷨���ṩ�����ƶ�Ӧ�ļ�ֵ</param>
				/// <returns>�㷨���Ψһ��ʶ</returns>
				static unsigned int Add
					(
					String^ strProvider_
					);

				/// <summary>
				/// ���ȱʡ�㷨��(�������ļ�CreCrypt.ini�л�ȡ��ֵΪCKDefault��ֵָ���Ŀ�)���㷨��ֻ����Ӻ�ſ�ʹ��
				/// </summary>
				/// <returns>�㷨���Ψһ��ʶ</returns>
				static unsigned int Add()
				{
					return Add(nullptr);
				}

				/// <summary>
				/// ͨ��·��ֱ�Ӽ����㷨��
				/// </summary>
				/// <param name="strDllName_">Ҫ���صĿ�����һ��Ҫ��˿����ͬһ·���£���ȫ·��</param>
				/// <returns>�㷨���Ψһ��ʶ</returns>
				static unsigned int AddByPath
					(
					String^ strDllName_
					);

				/// <summary>
				/// ͨ��·��ֱ�Ӽ����㷨��
				/// </summary>
				/// <param name="strDllName_">�㷨�⣨���Ϊnull����ʹ��Ĭ�����ÿ⣻�������ʧ�ܣ����ٳ��Լ���CreSCrypt.dll�������ʧ�ܣ����׳��쳣��</param>
				/// <returns>�㷨���Ψһ��ʶ</returns>
				static unsigned int TryAdd
					(
					String^ strDllName_
					);

				/// <summary>
				/// �Ƴ��㷨��
				/// </summary>
				/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
				static void Remove(unsigned int nProviderID_);

				/// <summary>
				/// �ж��㷨���Ƿ������
				/// </summary>
				/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
				/// <returns>����ӣ�����true�����򣬷���false</returns>
				static bool IsAdd(unsigned int nProviderID_);

				/// <summary>
				/// ��ȡ����ӵ��㷨���ṩ������
				/// </summary>
				/// <returns>����ӵ��㷨������</returns>
				static int GetCount();

				/// <summary>
				/// ö���㷨���Ψһ��ʶ
				/// </summary>
				/// <param name="nIndex_">�㷨�������</param>
				/// <returns>�㷨���Ψһ��ʶ</returns>
				static unsigned int Enum(int nIndex_);

				/// <summary>
				/// �ӽ����㷨�����
				/// </summary>
				ref class Crypto
				{
				public:
					/// <summary>
					/// �����㷨��ʶ
					/// </summary>
					enum class Alg
					{
						/// <summary>
						/// ��Ч��ʶ�����ڳ�ʼ������Ч���ж�(0)
						/// </summary>
						Invalid = 0,
						/// <summary>
						/// ֻ���ڼ��ܵ�RSA(0x01)
						/// </summary>
						RSAEncrypt = 0x01,
						/// <summary>
						/// ֻ����ǩ����RSA(0x02)
						/// </summary>
						RSASign = 0x02,
						/// <summary>
						/// RSA�㷨���ǶԳ��㷨���ɼ���Ҳ��ǩ����
						/// </summary>
						RSA = RSAEncrypt | RSASign,
						/// <summary>
						/// �߼����ܱ�׼���Գ��㷨��(0x010)
						/// </summary>
						AES = 0x010,
						/// <summary>
						/// �������ݼ����㷨���Գ��㷨��
						/// </summary>
						IDEA
					};

					/// <summary>
					/// ��Կ����
					/// </summary>
					[Flags]
					enum class KeyType
					{
						/// <summary>
						/// ��Ч��ʶ�����ڳ�ʼ������Ч���ж�
						/// </summary>
						Invalid = 0,
						/// <summary>
						/// �Գ��㷨����Կ
						/// </summary>
						Symm = 0x01,
						/// <summary>
						/// ��Կ
						/// </summary>
						Public = 0x02,
						/// <summary>
						/// ˽Կ
						/// </summary>
						Private = 0x04
					};

					/// <summary>
					/// ��ȡ�ṩ�ļ����㷨����
					/// </summary>
					/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
					/// <returns>�����㷨����</returns>
					static int GetCount(unsigned int nProviderID_);

					/// <summary>
					/// ö�ټ����㷨
					/// </summary>
					/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
					/// <param name="nIndex_">�㷨����</param>
					/// <returns>�㷨��ʶ</returns>
					static Alg Enum(unsigned int nProviderID_, int nIndex_);

					/// <summary>
					/// ��ȡ�㷨�����ʹ����ɺ���Ҫͨ��ReleaseAlg���ͷ�
					/// </summary>
					/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
					/// <param name="euAlg_">�㷨��ʶ</param>
					/// <returns>�㷨���</returns>
					static IntPtr GetAlg(unsigned int nProviderID_, Alg euAlg_);

					/// <summary>
					/// ��ȡ�㷨�����ʹ����ɺ���Ҫͨ��ReleaseAlg���ͷ�
					/// </summary>
					/// <param name="strDllPath_">�㷨�⣨���Ϊnull����ʹ��Ĭ�����ÿ⣻�������ʧ�ܣ����ٳ��Լ���CreSCrypt.dll�������ʧ�ܣ����׳��쳣��</param>
					/// <param name="euAlg_">�㷨��ʶ</param>
					/// <returns>�㷨���</returns>
					static IntPtr GetAlg(String^ strDllPath_, Alg euAlg_ )
					{
						return GetAlg(TryAdd(strDllPath_), euAlg_);
					}

					/// <summary>
					/// �ͷ��㷨���
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					static void ReleaseAlg(IntPtr hAlg_);

					/// <summary>
					/// ��ȡԿ�׳��ȣ�ֻ�д�������Կ����ܻ�ȡ��ȷ�ĳ���
					/// </summary>
					/// <param name="hAlg_">�㷨���Ψһ��ʶ</param>
					/// <returns>��Կ����</returns>
					static int GetKeyLen(IntPtr hAlg_);

					/// <summary>
					/// ��ȡ���ܿ鳤�ȣ�ֻ�д�������Կ����ܻ�ȡ��ȷ�ĳ���
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <returns>���ܿ鳤��</returns>
					static int GetBlockSize(IntPtr hAlg_);

					/// <summary>
					/// �������ݳ��Ȼ�ȡ���ĳ���(���������µĳ��ȣ���Ϊ���һ��)
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="nDataLen_">���ݵĳ���</param>
					/// <returns>���ĵĳ���</returns>
					static int GetCipherLen(IntPtr hAlg_, int nDataLen_);

					/// <summary>
					/// ������Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="bySeed_">����������Կ�����ӣ�����null�򳤶�Ϊ�㣬������������ӣ�</param>
					/// <param name="nKeyLen_">���ɵ���Կ����</param>
					static void GenKey(IntPtr hAlg_, array<unsigned char>^ bySeed_, int nKeyLen_);

					/// <summary>
					/// ������Կ����ͬ������������ͬ����Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="strSeed_">����������Կ������</param>
					/// <param name="nKeyLen_">���ɵ���Կ����</param>
					static void GenKey(IntPtr hAlg_, String^ strSeed_, int nKeyLen_)
					{
						array<unsigned char>^ bySeed = XConvert::UnicodeString2Bytes(strSeed_);
						GenKey(hAlg_, bySeed, nKeyLen_);
					}

					/// <summary>
					/// ����ȱʡ���ȵ���Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="strSeed_">����������Կ������</param>
					static void GenKey(IntPtr hAlg_, String^ strSeed_)
					{
						GenKey(hAlg_, strSeed_, 0);
					}

					/// <summary>
					/// ����ȱʡ���ȵ���Կ��������������ɣ�ÿ�����ɵĶ���һ����
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					static void GenKey(IntPtr hAlg_)
					{
						GenKey(hAlg_, String::Empty, 0);
					}

					/// <summary>
					/// ������Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					static void DestroyKey(IntPtr hAlg_);

					/// <summary>
					/// ������Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byKey_">Ҫ�������Կ</param>
					/// <param name="nLen_">Ҫ�������Կ����</param>
					/// <param name="euKeyType_">��Կ����</param>
					static void ImportKey(IntPtr hAlg_, array<unsigned char>^ byKey_, int nLen_, KeyType euKeyType_);

					/// <summary>
					/// ������Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byKey_">Ҫ�������Կ</param>
					/// <param name="euKeyType_">��Կ����</param>
					static void ImportKey(IntPtr hAlg_, array<unsigned char>^ byKey_, KeyType euKeyType_)
					{
						ImportKey(hAlg_, byKey_, byKey_->Length, euKeyType_);
					}

					/// <summary>
					/// ������Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="euKeyType_">��Կ����</param>
					/// <returns>��Կ</returns>
					static array<unsigned char>^ ExportKey(IntPtr hAlg_, KeyType euKeyType_);

					/// <summary>
					/// ��������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byData_">Ҫ���ܵ�����</param>
					/// <param name="nDataLen_">Ҫ���ܵ����ݵĳ���</param>
					/// <param name="byCipher_">���ܺ�����ݣ����Ϊnullptr��ֻ���ؼ������ݵĳ���</param>
					/// <param name="nCipherLen_">����ʱΪ���Ļ��������ȣ����ʱΪ���ܺ�����ݳ���</param>
					/// <param name="bFinal_">�Ƿ������һ�����ݣ�������㷨���Զ����Ϊ���ݿ����������������Ҫ�û���֤�����ݿ��������</param>
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
					/// ��������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byData_">Ҫ���ܵ�����</param>
					/// <param name="nDataLen_">Ҫ�������ݵĳ��ȣ����ܴ������ݵ�ʵ�ʳ��ȣ�</param>
					/// <param name="bFinal_">�Ƿ������һ�����ݣ�������㷨���Զ����Ϊ���ݿ����������������Ҫ�û���֤�����ݿ��������</param>
					/// <returns>���ܺ������</returns>
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
					/// ��������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byData_">Ҫ���ܵ�����</param>
					/// <param name="bFinal_">�Ƿ������һ�����ݣ�������㷨���Զ����Ϊ���ݿ����������������Ҫ�û���֤�����ݿ��������</param>
					/// <returns>���ܺ������</returns>
					static array<unsigned char>^ Encrypt(IntPtr hAlg_, array<unsigned char>^ byData_, bool bFinal_)
					{
						return Encrypt(hAlg_, byData_, byData_->Length, bFinal_);
					}

					/// <summary>
					/// �������һ�����ݣ��Զ������ݽ������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byData_">Ҫ���ܵ�����</param>
					/// <param name="nDataLen_">Ҫ�������ݵĳ��ȣ����ܴ������ݵ�ʵ�ʳ��ȣ�</param>
					/// <returns>���ܺ������</returns>
					static array<unsigned char>^ Encrypt(IntPtr hAlg_, array<unsigned char>^ byData_, int nDataLen_)
					{
						return Encrypt(hAlg_, byData_, nDataLen_, true);
					}

					/// <summary>
					/// �������һ�����ݣ��Զ������ݽ������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byData_">Ҫ���ܵ�����</param>
					/// <returns>���ܺ������</returns>
					static array<unsigned char>^ Encrypt(IntPtr hAlg_, array<unsigned char>^ byData_)
					{
						return Encrypt(hAlg_, byData_, byData_->Length);
					}

					/// <summary>
					/// ��������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byCipher_">Ҫ���ܵ�����</param>
					/// <param name="nCipherLen_">Ҫ���ܵ����ݵĳ���</param>
					/// <param name="byData_">���ܵ����ݣ����Ϊnullptr��ֻ���ؽ������ݵĳ���</param>
					/// <param name="nDataLen_">����ʱΪ���ݻ��������ȣ����ʱΪ���ܺ�����ݳ���</param>
					/// <param name="bFinal_">�Ƿ������һ�����ݣ�������㷨���Զ���ȡ��䳤�ȣ�����ֱ�ӽ���</param>
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
					/// ��������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byCipher_">Ҫ���ܵ�����</param>
					/// <param name="byCipher_">Ҫ���ܵ����ݵĳ��ȣ����ܴ���ʵ�ʳ��ȣ�</param>
					/// <param name="bFinal_">�Ƿ������һ�����ݣ�������㷨���Զ���ȡ��䳤�ȣ�����ֱ�ӽ���</param>
					/// <returns>���ܵ�����</returns>
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
					/// ��������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byCipher_">Ҫ���ܵ�����</param>
					/// <param name="bFinal_">�Ƿ������һ�����ݣ�������㷨���Զ���ȡ��䳤�ȣ�����ֱ�ӽ���</param>
					/// <returns>���ܵ�����</returns>
					static array<unsigned char>^ Decrypt(IntPtr hAlg_, array<unsigned char>^ byCipher_, bool bFinal_)
					{
						return Decrypt(hAlg_, byCipher_, byCipher_->Length, bFinal_);
					}

					/// <summary>
					/// �������һ�����ݣ��㷨���Զ���ȡ��䳤��
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byCipher_">Ҫ���ܵ�����</param>
					/// <param name="byCipher_">Ҫ���ܵ����ݵĳ��ȣ����ܴ���ʵ�ʳ��ȣ�</param>
					/// <returns>���ܵ�����</returns>
					static array<unsigned char>^ Decrypt(IntPtr hAlg_, array<unsigned char>^ byCipher_, int nCipherLen_)
					{
						return Decrypt(hAlg_, byCipher_, nCipherLen_, true);
					}

					/// <summary>
					/// �������һ�����ݣ��㷨���Զ���ȡ��䳤��
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byCipher_">Ҫ���ܵ�����</param>
					/// <returns>���ܵ�����</returns>
					static array<unsigned char>^ Decrypt(IntPtr hAlg_, array<unsigned char>^ byCipher_)
					{
						return Decrypt(hAlg_, byCipher_, byCipher_->Length);
					}

					/// <summary>
					/// ǩ�����ݣ�һ�㶼�Ƕ����ݵ�ɢ��ֵ����ǩ�����Զ���䵽���ݿ����������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byMsg_">Ҫǩ��������</param>
					/// <param name="nMsgLen_">Ҫǩ�������ݵĳ���</param>
					/// <param name="bySign_">ǩ��������ݣ����Ϊnullptr��ֻ����ǩ�����ݵĳ���</param>
					/// <param name="nSignLen_">����ʱΪ���������ȣ����ʱΪǩ��������ݳ���</param>
					static void Sign
						(
						IntPtr hAlg_,
						array<unsigned char>^ byMsg_,
						int nMsgLen_,
						array<unsigned char>^ bySign_,
						int% nSignLen_
						);

					/// <summary>
					/// ǩ�����ݣ�һ�㶼�Ƕ����ݵ�ɢ��ֵ����ǩ�����Զ���䵽���ݿ����������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byMsg_">Ҫǩ��������</param>
					/// <param name="nMsgLen_">Ҫǩ�������ݵĳ��ȣ����ܴ���ʵ�ʵĳ��ȣ�</param>
					/// <returns>ǩ���������</returns>
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
					/// ǩ�����ݣ�һ�㶼�Ƕ����ݵ�ɢ��ֵ����ǩ�����Զ���䵽���ݿ����������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byMsg_">Ҫǩ��������</param>
					/// <returns>ǩ���������</returns>
					static array<unsigned char>^ Sign(IntPtr hAlg_, array<unsigned char>^ byMsg_)
					{
						return Sign(hAlg_, byMsg_, byMsg_->Length);
					}

					/// <summary>
					/// ��֤ǩ������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byMsg_">Ҫ��֤������</param>
					/// <param name="nMsgLen_">���ݳ���</param>
					/// <param name="bySign_">���ݵ�ǩ��</param>
					/// <param name="nSignLen_">ǩ������</param>
					/// <returns>��֤ͨ��������true�����򣬷���false</returns>
					static bool Verify
						(
						IntPtr hAlg_,
						array<unsigned char>^ byMsg_,
						int nMsgLen_,
						array<unsigned char>^ bySign_,
						int nSignLen_
						);

					/// <summary>
					/// ��֤ǩ������
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byMsg_">Ҫ��֤������</param>
					/// <param name="bySign_">���ݵ�ǩ��</param>
					/// <returns>��֤ͨ��������true�����򣬷���false</returns>
					static bool Verify(IntPtr hAlg_, array<unsigned char>^ byMsg_, array<unsigned char>^ bySign_)
					{
						return Verify(hAlg_, byMsg_, byMsg_->Length, bySign_, bySign_->Length);
					}

					/// <summary>
					/// ���ļ����ݽ��м��ܣ�ͬ�������ڲ�ͬλ�ó��֣����ܺ����ݲ�ͬ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byData_">����ʱΪ�������ݣ����ʱΪ���ܺ������</param>
					/// <param name="nLen_">Ҫ���ܵ����ݵĳ��ȣ�����Ϊ���ݿ����������</param>
					/// <param name="nOffSet_">�������ļ��е�ƫ��λ�ã�����Ϊsizeof(int)����������</param>
					static void FileEncrypt
						(
						IntPtr hAlg_,
						array<unsigned char>^ byData_,
						int nLen_,
						unsigned int nOffSet_
						);

					/// <summary>
					/// �Լ����ļ����ݽ��н���
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byData_">����ʱΪҪ���ܵ����ݣ����ʱΪ���ܺ������</param>
					/// <param name="nLen_">Ҫ�����ܵ����ݵĳ��ȣ�����Ϊ���ݿ����������</param>
					/// <param name="nOffSet_">�������ļ��е�ƫ��λ�ã�����Ϊsizeof(int)����������</param>
					static void FileDecrypt
						(
						IntPtr hAlg_,
						array<unsigned char>^ byData_,
						int nLen_,
						unsigned int nOffSet_
						);

					/// <summary>
					/// ͨ��˽Կ��ȡ��Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byKey_">����ʱΪ˽Կ�����ʱΪ��Կ</param>
					/// <returns>��Կ����</returns>
					static int ConvertKey(IntPtr hAlg_, array<unsigned char>^ byKey_);

					/// <summary>
					/// ͨ��˽Կ��ȡ��Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byPrivate_">˽Կ</param>
					/// <param name="nLen_">˽Կ����</param>
					/// <returns>��Կ</returns>
					static array<unsigned char>^ GetPubKey(IntPtr hAlg_, array<unsigned char>^ byPrivate_, int nLen_)
					{
						array<unsigned char>^ byPublic = gcnew array<unsigned char>(nLen_);
						Array::Copy(byPrivate_, byPublic, nLen_);
						ConvertKey(hAlg_, byPublic);
						return byPublic;
					}

					/// <summary>
					/// ͨ��˽Կ��ȡ��Կ
					/// </summary>
					/// <param name="hAlg_">�㷨���</param>
					/// <param name="byPrivate_">˽Կ</param>
					/// <returns>��Կ</returns>
					static array<unsigned char>^ GetPubKey(IntPtr hAlg_, array<unsigned char>^ byPrivate_)
					{
						return GetPubKey(hAlg_, byPrivate_, byPrivate_->Length);
					}

					/// <summary>
					/// ͨ��˽Կ��ȡ��Կ
					/// </summary>
					/// <param name="strDllPath_">��Կ���ṩ���ļ�������·����</param>
					/// <param name="euAlg_">�㷨��ʶ</param>
					/// <param name="byPrivate_">˽Կ</param>
					/// <param name="nLen_">˽Կ����</param>
					/// <returns>��Կ</returns>
					static array<unsigned char>^ GetPubKey
						(
						String^ strDllPath_,
						Alg euAlg_,
						array<unsigned char>^ byPrivate_,
						int nLen_
						);

					/// <summary>
					/// ͨ��˽Կ��ȡ��Կ
					/// </summary>
					/// <param name="strDllPath_">��Կ���ṩ���ļ�������·����</param>
					/// <param name="euAlg_">�㷨��ʶ</param>
					/// <param name="byPrivate_">˽Կ</param>
					/// <returns>��Կ</returns>
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
					/// ��ȡ�û��ı�ʶ���û���ǰʹ����Կ��ID��
					/// </summary>
					/// <param name="byKey_">Ҫ��ȡ��ID��˽Կ</param>
					/// <returns>��ԿID</returns>
					static unsigned int GetKeyID(array<unsigned char>^ byKey_);

					/// <summary>
					/// �����������������Կ
					/// </summary>
					/// <param name="strDllPath_">��Կ���ṩ���ļ�������·����</param>
					/// <param name="euAlg_">�㷨��ʶ</param>
					/// <param name="bySeed_">���ڴ�����Կ������</param>
					/// <returns>���ɵ���Կ��RSAΪ˽Կ���Գ��㷨Ϊ��Կ��</returns>
					static array<unsigned char>^ BuildKey
						(
						String^ strDllPath_,
						Alg euAlg_,
						array<unsigned char>^ bySeed_
						);

					/// <summary>
					/// �����������������Կ
					/// </summary>
					/// <param name="strDllPath_">��Կ���ṩ���ļ�������·����</param>
					/// <param name="euAlg_">�㷨��ʶ</param>
					/// <param name="strSeed_">���ڴ�����Կ������</param>
					/// <returns>���ɵ���Կ��RSAΪ˽Կ���Գ��㷨Ϊ��Կ��</returns>
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
				/// ɢ���㷨�����
				/// </summary>
				ref class Hash
				{
				public:
					/// <summary>
					/// ɢ���㷨��ʶ
					/// </summary>
					enum class Alg
					{
						/// <summary>
						/// ��Ч��ʶ�����ڳ�ʼ������Ч���ж�
						/// </summary>
						Invalid = 0,
						/// <summary>
						/// ��ϢժҪ�㷨����棬128bit��16Byte��
						/// </summary>
						MD5,
						/// <summary>
						/// ��ȫɢ���㷨��SHA1��160bit��20Byte����
						/// </summary>
						SHA,
						/// <summary>
						/// 256bit��32Byte��
						/// </summary>
						SHA256 = 8,
						/// <summary>
						/// 384bit��48Byte��
						/// </summary>
						SHA384,
						/// <summary>
						/// 512bit��64Byte��
						/// </summary>
						SHA512
					};

					/// <summary>
					/// ��ȡɢ���㷨������
					/// </summary>
					/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
					/// <returns>ɢ���㷨����</returns>
					static int GetCount(unsigned int nProviderID_);

					/// <summary>
					/// ö��ɢ���㷨
					/// </summary>
					/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
					/// <param name="nIndex_">ɢ���㷨����</param>
					/// <returns>ɢ���㷨��ʶ</returns>
					static Alg Enum(unsigned int nProviderID_, int nIndex_);

					/// <summary>
					/// ��ȡɢ���㷨�����ʹ����ɺ�Ҫͨ��ReleaseAlg���ͷ�
					/// </summary>
					/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
					/// <param name="euAlg_">ɢ���㷨��ʶ</param>
					/// <returns>�㷨���</returns>
					static IntPtr GetAlg(unsigned int nProviderID_, Alg euAlg_);

					/// <summary>
					/// ��ȡ�㷨�����ʹ����ɺ���Ҫͨ��ReleaseAlg���ͷ�
					/// </summary>
					/// <param name="strDllPath_">�㷨�⣨���Ϊnull����ʹ��Ĭ�����ÿ⣻�������ʧ�ܣ����ٳ��Լ���CreSCrypt.dll�������ʧ�ܣ����׳��쳣��</param>
					/// <param name="euAlg_">ɢ���㷨��ʶ</param>
					/// <returns>�㷨���</returns>
					static IntPtr GetAlg(String^ strDllPath_, Alg euAlg_ )
					{
						return GetAlg(TryAdd(strDllPath_), euAlg_);
					}

					/// <summary>
					/// �ͷ�ɢ���㷨���
					/// </summary>
					/// <param name="hAlg_">ɢ���㷨���</param>
					static void ReleaseAlg(IntPtr hAlg_);

					/// <summary>
					/// ��ȡɢ��ֵ�Ĵ�С��byte��
					/// </summary>
					/// <param name="hAlg_">ɢ���㷨���</param>
					/// <returns>ɢ��ֵ��С</returns>
					static int GetSize(IntPtr hAlg_);

					/// <summary>
					/// ��ʼ��ɢ���㷨�������¼���ɢ��ֵǰ����Ҫ�ȳ�ʼ��
					/// </summary>
					/// <param name="hAlg_">ɢ���㷨���</param>
					static void Init(IntPtr hAlg_);

					/// <summary>
					/// ����ɢ��ֵ���������ݵ�ɢ��ֵ������ӵ�ԭ����ɢ��ֵ���棩
					/// </summary>
					/// <param name="hAlg_">ɢ���㷨���</param>
					/// <param name="byData_">Ҫ����ɢ��ֵ������</param>
					/// <param name="nLen_">���ݳ���</param>
					static void Update
						(
						IntPtr hAlg_,
						array<unsigned char>^ byData_,
						int nLen_
						);

					/// <summary>
					/// ����ɢ��ֵ���������ݵ�ɢ��ֵ������ӵ�ԭ����ɢ��ֵ���棩
					/// </summary>
					/// <param name="hAlg_">ɢ���㷨���</param>
					/// <param name="byData_">Ҫ����ɢ��ֵ������</param>
					static void Update(IntPtr hAlg_, array<unsigned char>^ byData_)
					{
						Update(hAlg_, byData_, byData_->Length);
					}

					/// <summary>
					/// ���ɢ��ֵ���㣬����ȡɢ��ֵ
					/// </summary>
					/// <param name="hAlg_">ɢ���㷨���</param>
					/// <returns>ɢ��ֵ</returns>
					static array<unsigned char>^ Final(IntPtr hAlg_);

					/// <summary>
					/// ���ɢ��ֵ���㣬����ȡɢ��ֵ
					/// </summary>
					/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
					/// <param name="euAlg_">ɢ���㷨</param>
					/// <param name="byData_">Ҫ����ɢ��ֵ������</param>
					/// <param name="nDataLen_">���ݳ���</param>
					/// <param name="byHash_">��ȡɢ��ֵ</param>
					/// <param name="nHashLen_">����ʱΪ���������ȣ����ʱΪɢ��ֵ����</param>
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
					/// ���SHAɢ��ֵ���㣬����ȡɢ��ֵ
					/// </summary>
					/// <param name="nProviderID_">�㷨���Ψһ��ʶ</param>
					/// <param name="byData_">Ҫ����ɢ��ֵ������</param>
					/// <returns>ɢ��ֵ</returns>
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