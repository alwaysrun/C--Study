//////////////////////////////////////////////////////////////////////////
//	KData.cpp
#include "stdafx.h"
#include <vcclr.h>
#include "KData.h"

#include "CSHException.h"
#include "CreKData.h"
#pragma comment(lib, "CreKData.lib")

namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
#define XKDataCatch	\
	XCatchParam	\
	XCatchFile	\
	XCatchAll

			//////////////////////////////////////////////////////////////////////////
			//		KFile
			//////////////////////////////////////////////////////////////////////////
			IntPtr KData::Create(DataType euType_, int nRecordSize_, String ^strCryptDll_, String ^strName_, String ^strEmail_)
			{
				pin_ptr<const wchar_t> pDll = PtrToStringChars(strCryptDll_);
				pin_ptr<const wchar_t> pName = PtrToStringChars(strName_);
				pin_ptr<const wchar_t> pEmail = PtrToStringChars(strEmail_);

				try 
				{
					return IntPtr(CKDCreate((CKDataType)(int)euType_, nRecordSize_, pDll, pName, pEmail));
				}
				XKDataCatch;
			}

			IntPtr KData::Open(int nRecordSize_, String ^strCryptDll_)
			{
				pin_ptr<const wchar_t> pDll = PtrToStringChars(strCryptDll_);

				try 
				{
					return IntPtr(CKDOpen(nRecordSize_, pDll));
				}
				XKDataCatch;
			}

			void KData::Close(IntPtr hData_)
			{
				try 
				{
					CKDClose(hData_.ToPointer());
				}
				XKDataCatch;
			}

			int KData::GetRecordSize(IntPtr hData_)
			{
				try 
				{
					return CKDGetRecordSize(hData_.ToPointer());
				}
				XKDataCatch;
			}

			int KData::GetCryptSize(IntPtr hData_)
			{
				try 
				{
					return CKDGetCryptSize(hData_.ToPointer());
				}
				XKDataCatch;
			}

			int KData::GetSignSize(IntPtr hData_)
			{
				try 
				{
					return CKDGetSignSize(hData_.ToPointer());
				}
				XKDataCatch;
			}

			void KData::SetKey(IntPtr hData_, SecAlg::Crypto::Alg euAlg_, unsigned int nCorpId_, unsigned int nCipherId_, 
				array<unsigned char>^ byKey_, int nKeyLen_, bool bPrivate_)
			{
				pin_ptr<const unsigned char> pBuff = XPtrToBytes(byKey_);

				try 
				{
					CKDSetKey(hData_.ToPointer(), (CryptAlgID)(int)euAlg_, nCorpId_, nCipherId_, 
							pBuff, nKeyLen_, !!bPrivate_);
				}
				XCatchCrypt
				XKDataCatch;
			}

			void KData::SetPsw(IntPtr hData_, String^ strPsw_)
			{
				pin_ptr<const wchar_t> pPsw = PtrToStringChars(strPsw_);

				try 
				{
					CKDSetPsw(hData_.ToPointer(), pPsw);
				}
				XCatchCrypt
				XKDataCatch;
			}

			array<unsigned char>^ KData::Encrypt(IntPtr hData_, array<unsigned char>^ byData_)
			{
				pin_ptr<const unsigned char> pData = XPtrToBytes(byData_);

				try 
				{
					int nLen = GetCryptSize(hData_);
					array<unsigned char>^ byCipher = gcnew array<unsigned char>(nLen);
					pin_ptr<unsigned char> pCipher = XPtrToBytes(byCipher);

					CKDEncrypt(hData_.ToPointer(), pData, pCipher, nLen);

					return byCipher;
				}
				XCatchCrypt
				XKDataCatch;
			}

			array<unsigned char>^ KData::Decrypt(IntPtr hData_, array<unsigned char>^ byCipher_)
			{
				pin_ptr<const unsigned char> pCipher = XPtrToBytes(byCipher_);

				try 
				{
					int nLen = GetRecordSize(hData_);
					array<unsigned char>^ byData = gcnew array<unsigned char>(nLen);
					pin_ptr<unsigned char> pData = XPtrToBytes(byData);

					CKDDecrypt(hData_.ToPointer(), pCipher, pData, nLen);

					return byData;
				}
				XCatchCrypt
				XKDataCatch;
			}

			array<unsigned char>^ KData::Sign(IntPtr hData_, array<unsigned char>^ byData_)
			{
				pin_ptr<const unsigned char> pData = XPtrToBytes(byData_);

				try 
				{
					int nLen = GetSignSize(hData_);
					array<unsigned char>^ bySign = gcnew array<unsigned char>(nLen);
					pin_ptr<unsigned char> pSign = XPtrToBytes(bySign);

					CKDSign(hData_.ToPointer(), pData, pSign, nLen);

					return bySign;
				}
				XCatchCrypt
				XKDataCatch;
			}

			array<unsigned char>^ KData::VerifyData(IntPtr hData_, array<unsigned char>^ bySign_)
			{
				pin_ptr<const unsigned char> pSign = XPtrToBytes(bySign_);

				try 
				{
					int nLen = GetRecordSize(hData_);
					array<unsigned char>^ byData = gcnew array<unsigned char>(nLen);
					pin_ptr<unsigned char> pData = XPtrToBytes(byData);

					CKDVerifyData(hData_.ToPointer(), pSign, pData, nLen);

					return byData;
				}
				XCatchCrypt
				XKDataCatch;
			}

			bool KData::VerifyOnly(IntPtr hData_, array<unsigned char>^ bySign_)
			{
				pin_ptr<const unsigned char> pSign = XPtrToBytes(bySign_);

				try 
				{
					return !!CKDVerifyOnly(hData_.ToPointer(), pSign);
				}
				XCatchCrypt
				XKDataCatch;
			}

		} // CreAPI
	} // Xugd
} // SHCre