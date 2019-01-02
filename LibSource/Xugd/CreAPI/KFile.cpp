//////////////////////////////////////////////////////////////////////////
//	KFile.cpp
#include "stdafx.h"
#include <vcclr.h>
#include "KFile.h"

#include "CSHException.h"
#include "CreKFile.h"
#pragma comment(lib, "CreKFile.lib")

namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
#define XKFileCatch	\
	XCatchParam	\
	XCatchFile	\
	XCatchAll

			//////////////////////////////////////////////////////////////////////////
			//		KFile
			//////////////////////////////////////////////////////////////////////////
			IntPtr KFile::Create
				(
				String^ strFile_,
				FileType euType_,
				int nRecordSize_,
				int nHeaderSize_,
				String^ strCryptDll_,
				String^ strName_,
				String^ strEmail_,
				SecAlg::Crypto::Alg euAlg_
				)
			{
				pin_ptr<const wchar_t> pFile = PtrToStringChars(strFile_);
				pin_ptr<const wchar_t> pDll = PtrToStringChars(strCryptDll_);
				pin_ptr<const wchar_t> pName = PtrToStringChars(strName_);
				pin_ptr<const wchar_t> pEmail = PtrToStringChars(strEmail_);

				try 
				{
					return IntPtr(CKFCreateWithProvider(pFile, (CKFileType)(int)euType_, nRecordSize_, nHeaderSize_, pDll,
							pName, pEmail, (CryptAlgID)(unsigned int)euAlg_));
				}
				XKFileCatch;
			}

			IntPtr KFile::Open(String^ strFile_, String^ strCryptDll_)
			{
				pin_ptr<const wchar_t> pFile = PtrToStringChars(strFile_);
				pin_ptr<const wchar_t> pDll = PtrToStringChars(strCryptDll_);

				try 
				{
					return IntPtr(CKFOpenWithProvider(pFile, pDll));
				}
				XKFileCatch;
			}

			void KFile::Close(IntPtr hFile_)
			{
				try 
				{
					CKFClose(hFile_.ToPointer());
				}
				XKFileCatch;
			}

			void KFile::Reset(IntPtr hFile_)
			{
				try 
				{
					CKFReset(hFile_.ToPointer());
				}
				XKFileCatch;
			}

			int KFile::GetHeaderSize(IntPtr hFile_)
			{
				try 
				{
					return CKFGetHeaderSize(hFile_.ToPointer());
				}
				XKFileCatch;
			}

			int KFile::GetRecordSize(IntPtr hFile_)
			{
				try 
				{
					return CKFGetRecordSize(hFile_.ToPointer());
				}
				XKFileCatch;
			}

			int KFile::GetRecordCount(IntPtr hFile_)
			{
				try 
				{
					return CKFGetRecordCount(hFile_.ToPointer());
				}
				XKFileCatch;
			}

			KFile::FileType KFile::GetType(IntPtr hFile_)
			{
				try 
				{
					return (FileType)(int)CKFGetType(hFile_.ToPointer());
				}
				XKFileCatch;
			}

			void KFile::SetKey
				(
				IntPtr hFile_,
				SecAlg::Crypto::Alg euAlg_,
				unsigned int nCorpID_,
				unsigned int nCipherId_,
				array<unsigned char>^ byPriKey_,
				int nKeyLen_
				)
			{
				pin_ptr<const unsigned char> pBuff = XPtrToBytes(byPriKey_);

				try 
				{
					CKFSetKey(hFile_.ToPointer(), (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nCipherId_, pBuff, nKeyLen_);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::SetPsw(IntPtr hFile_, String^ strPsw_)
			{
				pin_ptr<const wchar_t> pPsw = PtrToStringChars(strPsw_);

				try 
				{
					CKFSetPsw(hFile_.ToPointer(), pPsw);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::AddHeader(IntPtr hFile_, array<unsigned char>^ byHeader_)
			{
				pin_ptr<const unsigned char> pBuff = XPtrToBytes(byHeader_);

				try 
				{
					CKFAddHeader(hFile_.ToPointer(), pBuff);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::GetHeader(IntPtr hFile_, array<unsigned char>^ byHeader_)
			{
				pin_ptr<unsigned char> pBuff = XPtrToBytes(byHeader_);

				try 
				{
					CKFGetHeader(hFile_.ToPointer(), pBuff);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::AddRecord(IntPtr hFile_, array<unsigned char>^ byRecord_, bool bEncrypt_)
			{
				pin_ptr<const unsigned char> pBuff = XPtrToBytes(byRecord_);

				try 
				{
					CKFAddRecord(hFile_.ToPointer(), pBuff, bEncrypt_);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::GetRecord(IntPtr hFile_, array<unsigned char>^ byRecord_, bool bDecrypt_)
			{
				pin_ptr<unsigned char> pBuff = XPtrToBytes(byRecord_);

				try 
				{
					CKFGetRecord(hFile_.ToPointer(), pBuff, bDecrypt_);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::Read(IntPtr hFile_, array<unsigned char>^ byData_, int nLen_ )
			{
				pin_ptr<unsigned char> pBuff = XPtrToBytes(byData_);

				try 
				{
					CKFRead(hFile_.ToPointer(), pBuff, nLen_);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::Write(IntPtr hFile_, array<unsigned char>^ byData_, int nLen_)
			{
				pin_ptr<const unsigned char> pBuff = XPtrToBytes(byData_);

				try 
				{
					CKFWrite(hFile_.ToPointer(), pBuff, nLen_);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::Sign
				(
				IntPtr hFile_,
				SecAlg::Crypto::Alg euAlg_,
				unsigned int nCorpID_,
				unsigned int nOwnerID_, 
				array<unsigned char>^ byPriKey_,
				SecAlg::Hash::Alg euHash_
				)
			{
				pin_ptr<const unsigned char> pBuff = XPtrToBytes(byPriKey_);

				try 
				{
					CKFSign(hFile_.ToPointer(), (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nOwnerID_, 
						pBuff, XPtrGetLen(byPriKey_), (HashAlgID)(unsigned int)euHash_);
				}
				XCatchCrypt
				XKFileCatch;
			}

			bool KFile::Verify
				(
				IntPtr hFile_, 
				SecAlg::Crypto::Alg euAlg_,
				unsigned int nCorpID_,
				unsigned int nOwnerID_,
				array<unsigned char>^ byPubKey_
				)
			{
				pin_ptr<const unsigned char> pBuff = XPtrToBytes(byPubKey_);

				try 
				{
					return !!CKFVerify(hFile_.ToPointer(), (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nOwnerID_, pBuff, XPtrGetLen(byPubKey_));
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::AddUser
				(
				IntPtr hFile_,
				SecAlg::Crypto::Alg euAlg_,
				unsigned int nCorpID_,
				unsigned int nCipherId_, 
				array<unsigned char>^ byPubKey_, 
				String^ strName_, 
				String^ strEmail_
				)
			{
				pin_ptr<const unsigned char> pPub = XPtrToBytes(byPubKey_);
				pin_ptr<const wchar_t> pName = PtrToStringChars(strName_);
				pin_ptr<const wchar_t> pEmail = PtrToStringChars(strEmail_);

				try 
				{
					CKFAddUser(hFile_.ToPointer(), (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nCipherId_, pPub, XPtrGetLen(byPubKey_), pName, pEmail);
				}
				XCatchCrypt
				XKFileCatch;
			}

			void KFile::RemoveUser(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_)
			{
				try 
				{
					CKFRemoveUser(hFile_.ToPointer(), nCorpID_, nCipherId_);
				}
				XCatchCrypt
				XKFileCatch;
			}

			int KFile::UserCount(IntPtr hFile_)
			{
				try 
				{
					return CKFUserCount(hFile_.ToPointer());
				}
				XKFileCatch;
			}

			KFile::UserInfo^ KFile::EnumUser(IntPtr hFile_, int nIndex_)
			{
				try 
				{
					CKFUserInfo	stUserInfo;
					CKFEnumUser(hFile_.ToPointer(), nIndex_, stUserInfo);

					UserInfo^ pInfo = gcnew UserInfo();
					Marshal::PtrToStructure(IntPtr(&stUserInfo), pInfo);
					return pInfo;
				}
				XKFileCatch;
			}

			void KFile::SetRights(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_, Rights euRight_)
			{
				try 
				{
					CKFSetRights(hFile_.ToPointer(), nCorpID_, nCipherId_, (DWORD)euRight_);
				}
				XKFileCatch;
			}

			KFile::Rights KFile::GetRights(IntPtr hFile_, unsigned int nCorpID_, unsigned int nCipherId_)
			{
				try 
				{
					return (Rights)CKFGetRights(hFile_.ToPointer(), nCorpID_, nCipherId_);
				}
				XKFileCatch;
			}
			
		} // CreAPI
	} // Xugd
} // SHCre