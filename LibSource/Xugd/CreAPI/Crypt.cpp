//////////////////////////////////////////////////////////////////////////
//	Crypt.cpp
#include "stdafx.h"
#include <vcclr.h>
#include "Crypt.h"

#include "CSHException.h"
#include "CreCrypt.h"
#pragma comment(lib, "CreCrypt.lib")

namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
#define XCryptCatch	\
	XCatchParam	\
	XCatchCrypt	\
	XCatchAll

			//////////////////////////////////////////////////////////////////////////
			//		SecAlg
			//////////////////////////////////////////////////////////////////////////
			unsigned int SecAlg::Add(String^ strProvider_)
			{
				pin_ptr<const wchar_t> pProvider = PtrToStringChars(strProvider_);

				try 
				{
					return CCAddProvider(pProvider);
				}
				XCatchDll
				XCryptCatch;
			}

			unsigned int SecAlg::AddByPath(String^ strDllName_)
			{
				pin_ptr<const wchar_t> pDllName = PtrToStringChars(strDllName_);

				try 
				{
					return CCAddProviderByPath(pDllName);
				}
				XCatchDll
					XCryptCatch;
			}

			unsigned int SecAlg::TryAdd(String^ strDllName_)
			{
				pin_ptr<const wchar_t> pDllName = PtrToStringChars(strDllName_);

				try 
				{
					return CCTryAddProvider(pDllName);
				}
				XCatchDll
					XCryptCatch;
			}

			void SecAlg::Remove(unsigned int nProviderID_)
			{
				try 
				{
					CCRemoveProvider(nProviderID_);
				}
				XCatchAll;
			}

			bool SecAlg::IsAdd(unsigned int nProviderID_)
			{				
				return (!!CCIsProviderAdd(nProviderID_));
			}

			int SecAlg::GetCount()
			{
				return CCGetProviderCount();
			}

			unsigned int SecAlg::Enum(int nIndex_)
			{
				try 
				{
					return CCEnumProvider(nIndex_);
				}
				XCryptCatch;
			}

			//////////////////////////////////////////////////////////////////////////
			//		Crypto
			//////////////////////////////////////////////////////////////////////////
			int SecAlg::Crypto::GetCount(unsigned int nProviderID_)
			{
				try 
				{
					return CCGetCryptCount(nProviderID_);
				}
				XCryptCatch;
			}

			SecAlg::Crypto::Alg SecAlg::Crypto::Enum(unsigned int nProviderID_, int nIndex_)
			{
				try 
				{
					return (Alg)(int)CCEnumCrypt(nProviderID_, nIndex_);
				}
				XCryptCatch;
			}

			IntPtr SecAlg::Crypto::GetAlg(unsigned int nProviderID_, Alg euAlg_)
			{
				try 
				{
					return IntPtr( CCGetCryptAlg(nProviderID_, (CryptAlgID)(int)euAlg_) );
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::ReleaseAlg(IntPtr hAlg_)
			{
				try
				{
					CCReleaseCryptAlg(hAlg_.ToPointer());
				}
				XCryptCatch;
			}

			int SecAlg::Crypto::GetKeyLen(IntPtr hAlg_)
			{
				try 
				{
					return CCGetKeyLen(hAlg_.ToPointer());
				}
				XCryptCatch;
			}

			int SecAlg::Crypto::GetBlockSize(IntPtr hAlg_)
			{
				try 
				{
					return CCGetBlockSize(hAlg_.ToPointer());
				}
				XCryptCatch;
			}

			int SecAlg::Crypto::GetCipherLen(IntPtr hAlg_, int nDataLen_)
			{
				try 
				{
					return CCGetCipherLen(hAlg_.ToPointer(), nDataLen_);
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::GenKey(IntPtr hAlg_, array<unsigned char>^ bySeed_, int nKeyLen_)
			{
				pin_ptr<const unsigned char> pBuff =  XPtrToBytes(bySeed_);

				try 
				{
					CCGenKey(hAlg_.ToPointer(), pBuff, XPtrGetLen(bySeed_), nKeyLen_);
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::DestroyKey(IntPtr hAlg_)
			{
				try 
				{
					CCDestroyKey(hAlg_.ToPointer());
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::ImportKey(IntPtr hAlg_, array<unsigned char>^ byKey_, int nLen_, KeyType euKeyType_)
			{
				pin_ptr<const unsigned char> pKey =  XPtrToBytes(byKey_);

				try 
				{
					CCImportKey(hAlg_.ToPointer(), pKey, nLen_, (CryptKeyType)(int)euKeyType_);
				}
				XCryptCatch;
			}
			
			array<unsigned char>^ SecAlg::Crypto::ExportKey(IntPtr hAlg_, KeyType euKeyType_)
			{
				int nLen = (euKeyType_ == KeyType::Symm) ? CLen::SymmKeyMaxSize : CLen::RSAKeyMaxSize;

				try 
				{
					array<unsigned char>^ byKey = gcnew array<unsigned char>(nLen);
					pin_ptr<unsigned char> pBuff = &byKey[0];
					CCExportKey(hAlg_.ToPointer(), pBuff, nLen, (CryptKeyType)(int)euKeyType_);
					if( nLen != byKey->Length )
					{
						array<unsigned char>^ byRealKey = gcnew array<unsigned char>(nLen);
						Array::Copy(byKey, byRealKey, nLen);
						return byRealKey;
					}

					return byKey;
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::Encrypt(IntPtr hAlg_, array<unsigned char>^ byData_, int nDataLen_, array<unsigned char>^ byCipher_, int% nCipherLen_, bool bFinal_)
			{
				try
				{
					pin_ptr<const unsigned char> pData =  XPtrToBytes(byData_);
					pin_ptr<unsigned char> pCipher =  XPtrToBytes(byCipher_);
					int nOut = nCipherLen_;

					CCEncrypt(hAlg_.ToPointer(), pData, nDataLen_, pCipher, nOut, bFinal_);
					nCipherLen_ = nOut;
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::Decrypt(IntPtr hAlg_, array<unsigned char>^ byCipher_, int nCipherLen_, array<unsigned char>^ byData_, int% nDataLen_, bool bFinal_)
			{
				try
				{
					pin_ptr<const unsigned char> pCipher =  XPtrToBytes(byCipher_);
					pin_ptr<unsigned char> pData =  XPtrToBytes(byData_);
					int nOut = nDataLen_;

					CCDecrypt(hAlg_.ToPointer(), pCipher, nCipherLen_, pData, nOut, bFinal_);
					nDataLen_ = nOut;
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::Sign(IntPtr hAlg_, array<unsigned char>^ byMsg_, int nMsgLen_, array<unsigned char>^ bySign_, int% nSignLen_)
			{
				try 
				{
					pin_ptr<const unsigned char> pMsg =  XPtrToBytes(byMsg_);
					pin_ptr<unsigned char> pSign =  XPtrToBytes(bySign_);
					int nOut = nSignLen_;

					CCSign(hAlg_.ToPointer(), pMsg, nMsgLen_, pSign, nOut);
					nSignLen_ = nOut;
				}
				XCryptCatch;
			}

			bool SecAlg::Crypto::Verify(IntPtr hAlg_, array<unsigned char>^ byMsg_, int nMsgLen_, array<unsigned char>^ bySign_, int nSignLen_)
			{
				try 
				{
					pin_ptr<const unsigned char> pMsg =  XPtrToBytes(byMsg_);
					pin_ptr<const unsigned char> pSign =  XPtrToBytes(bySign_);

					return (!!CCVerify(hAlg_.ToPointer(), pMsg, nMsgLen_, pSign, nSignLen_));
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::FileEncrypt(IntPtr hAlg_, array<unsigned char>^ byData_, int nLen_, unsigned int nOffSet_)
			{
				try 
				{
					pin_ptr<unsigned char> pData =  XPtrToBytes(byData_);
					CCFileEncrypt(hAlg_.ToPointer(), pData, nLen_, nOffSet_);
				}
				XCryptCatch;
			}

			void SecAlg::Crypto::FileDecrypt(IntPtr hAlg_, array<unsigned char>^ byData_, int nLen_, unsigned int nOffSet_)
			{
				try 
				{
					pin_ptr<unsigned char> pData =  XPtrToBytes(byData_);
					CCFileDecrypt(hAlg_.ToPointer(), pData, nLen_, nOffSet_);
				}
				XCryptCatch;
			}

			int SecAlg::Crypto::ConvertKey(IntPtr hAlg_, array<unsigned char>^ byKey_)
			{
				try 
				{
					pin_ptr<unsigned char> pKey =  XPtrToBytes(byKey_);
					int nKey = XPtrGetLen(byKey_);
					CCConvertKey(hAlg_.ToPointer(), pKey, nKey);
					return nKey;
				}
				XCryptCatch;
			}

			array<unsigned char>^ SecAlg::Crypto::GetPubKey(String^ strDllPath_, Alg euAlg_, array<unsigned char>^ byPrivate_, int nLen_)
			{
				pin_ptr<const wchar_t> pDll = PtrToStringChars(strDllPath_);

				try 
				{
					array<unsigned char>^ byPublic = gcnew array<unsigned char>(nLen_);
					Array::Copy(byPrivate_, byPublic, nLen_);

					pin_ptr<unsigned char> pKey = XPtrToBytes(byPublic);
					CCGetPubKey(pDll, (CryptAlgID)(int)euAlg_, pKey, nLen_);

					return byPublic;
				}
				XCryptCatch;
			}

			unsigned int SecAlg::Crypto::GetKeyID(array<unsigned char>^ byKey_)
			{
				try 
				{
					pin_ptr<const unsigned char> pKey =  XPtrToBytes(byKey_);
					return CCGetKeyID(pKey, XPtrGetLen(byKey_));
				}
				XCryptCatch;
			}

			array<unsigned char>^ SecAlg::Crypto::BuildKey(String^ strDllPath_, Alg euAlg_, array<unsigned char>^ bySeed_)
			{
				pin_ptr<const wchar_t> pDll = PtrToStringChars(strDllPath_);
				pin_ptr<const unsigned char> pBuff =  XPtrToBytes(bySeed_);

				try 
				{
					int nKeyLen = CLen::RSAKeyMaxSize;
					BYTE byKey[CLen::RSAKeyMaxSize];
					CCBuildKey(pDll, (CryptAlgID)(int)euAlg_, pBuff, XPtrGetLen(bySeed_), byKey, nKeyLen);

					array<unsigned char>^ aryKey = gcnew array<unsigned char>(nKeyLen);
					Marshal::Copy(IntPtr(byKey), aryKey, 0, nKeyLen);
					return aryKey;
				}
				XCryptCatch;
			}

			//////////////////////////////////////////////////////////////////////////
			//	Hash Alg
			//////////////////////////////////////////////////////////////////////////
			int SecAlg::Hash::GetCount(unsigned int nProviderID_)
			{
				try 
				{
					return CCGetHashCount(nProviderID_);
				}
				XCryptCatch;
			}

			SecAlg::Hash::Alg SecAlg::Hash::Enum(unsigned int nProviderID_, int nIndex_)
			{
				try
				{
					return (Alg)(int)CCEnumHash(nProviderID_, nIndex_);
				}
				XCryptCatch;
			}

			IntPtr SecAlg::Hash::GetAlg(unsigned int nProviderID_, Alg euAlg_)
			{
				try 
				{
					return IntPtr(CCGetHashAlg(nProviderID_, (HashAlgID)(int)euAlg_));
				}
				XCryptCatch;
			}

			void SecAlg::Hash::ReleaseAlg(IntPtr hAlg_)
			{
				try 
				{
					CCReleaseHashAlg(hAlg_.ToPointer());
				}
				XCryptCatch;
			}

			int SecAlg::Hash::GetSize(IntPtr hAlg_)
			{
				try 
				{
					return CCGetHashSize(hAlg_.ToPointer());
				}
				XCryptCatch;
			}

			void SecAlg::Hash::Init(IntPtr hAlg_)
			{
				try 
				{
					CCHashInit(hAlg_.ToPointer());
				}
				XCryptCatch;
			}

			void SecAlg::Hash::Update(IntPtr hAlg_, array<unsigned char>^ byData_, int nLen_)
			{
				try 
				{
					pin_ptr<const unsigned char> pData = XPtrToBytes(byData_);
					CCHashUpdate(hAlg_.ToPointer(), pData, XPtrGetLen(byData_));
				}
				XCryptCatch;
			}
			
			array<unsigned char>^ SecAlg::Hash::Final(IntPtr hAlg_)
			{
				try 
				{
					int nLen = GetSize(hAlg_);
					array<unsigned char>^ byHash = gcnew array<unsigned char>(nLen);
					pin_ptr<unsigned char> pHash = &byHash[0];
					CCHashFinal(hAlg_.ToPointer(), pHash, nLen);
					return byHash;
				}
				XCryptCatch;
			}

			void SecAlg::Hash::HashData(unsigned int nProviderID_, Alg euAlg_, array<unsigned char>^ byData_, int nDataLen_, array<unsigned char>^ byHash_, int% nHashLen_)
			{
				try 
				{
					pin_ptr<const unsigned char> pData = XPtrToBytes(byData_);
					pin_ptr<unsigned char> pHash = XPtrToBytes(byHash_);
					int nLen = nHashLen_;

					CCHashData(nProviderID_, (HashAlgID)(int)euAlg_, pData, nDataLen_, pHash, nLen);
					nHashLen_ = nLen;
				}
				XCryptCatch;
			}

		} // CreAPI
	} // Xugd
} // SHCre