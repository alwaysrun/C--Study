//////////////////////////////////////////////////////////////////////////
//	CMail.cpp
#include "stdafx.h"
#include <vcclr.h>
#include "CMail.h"

#include "CSHException.h"
#include "CreCMail.h"
#pragma comment(lib, "CreCMail.lib")

namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
#define XMailCatch	\
	XCatchParam	\
	XCatchFile	\
	XCatchCrypt	\
	XCatchAll

			//////////////////////////////////////////////////////////////////////////
			//		CMail
			//////////////////////////////////////////////////////////////////////////
			void CMail::SetCallback(IntPtr hFile_, CallbackFun^ delFun_)
			{
				try 
				{
					IntPtr	pFunc = Marshal::GetFunctionPointerForDelegate(delFun_);
					PCMCallbackFun	pCallback = static_cast<PCMCallbackFun>(pFunc.ToPointer());
					
					CMSetCallback(hFile_.ToPointer(), pCallback);
				}
				XCatchParam;
			}

			IntPtr CMail::Create
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
				)
			{
				pin_ptr<const wchar_t> pFile = PtrToStringChars(strFile_);
				pin_ptr<const wchar_t> pDll = PtrToStringChars(strCryptDll_);
				pin_ptr<const wchar_t> pName = PtrToStringChars(strName_);
				pin_ptr<const wchar_t> pEmail = PtrToStringChars(strEmail_);
				pin_ptr<const unsigned char> pKey = XPtrToBytes(byPriKey_);

				try 
				{
					return IntPtr(CMCreate(pFile, (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nOwnerID_, pKey, nKeyLen_,
								pDll, pName, pEmail, (CryptAlgID)(unsigned int)euDataAlg_));
				}
				XMailCatch;
			}

			IntPtr CMail::Open
				(
				String^ strFile_, 
				SecAlg::Crypto::Alg euAlg_,
				unsigned int nCorpID_,
				unsigned int nUserCipherId_, 
				array<unsigned char>^ byPriKey_,
				int nKeyLen_,
				String^ strCryptDll_
				)
			{
				pin_ptr<const wchar_t> pFile = PtrToStringChars(strFile_);
				pin_ptr<const wchar_t> pDll = PtrToStringChars(strCryptDll_);
				pin_ptr<const unsigned char> pKey = XPtrToBytes(byPriKey_);

				try 
				{
					return IntPtr( CMOpen(pFile, (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nUserCipherId_, pKey, nKeyLen_, pDll) );
				}
				XMailCatch;
			}

			void CMail::Close(IntPtr hFile_)
			{
				try 
				{
					CMClose(hFile_.ToPointer());
				}
				XMailCatch;
			}

			void CMail::Pack(IntPtr hFile_, String^ strFile_)
			{
				pin_ptr<const wchar_t> pFile = PtrToStringChars(strFile_);

				try 
				{
					return CMPack(hFile_.ToPointer(), pFile);
				}
				XCatchCancel
				XMailCatch;
			}

			void CMail::Unpack(IntPtr hFile_, String^ strPath_)
			{
				pin_ptr<const wchar_t> pPath = PtrToStringChars(strPath_);

				try 
				{
					return CMUnpack(hFile_.ToPointer(), pPath);
				}
				XCatchCancel
				XMailCatch;
			}

			void CMail::Sign(IntPtr hFile_, SecAlg::Crypto::Alg euAlg_, unsigned int nCorpID_, unsigned int nOwnerID_, array<unsigned char>^ byPriKey_, SecAlg::Hash::Alg euHash_ )
			{
				pin_ptr<const unsigned char> pBuff = XPtrToBytes(byPriKey_);

				try 
				{
					CMSign(hFile_.ToPointer(), (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nOwnerID_, 
						pBuff, XPtrGetLen(byPriKey_), (HashAlgID)(unsigned int)euHash_);
				}
				XMailCatch;
			}
			
			bool CMail::Verify
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
					return !!CMVerify(hFile_.ToPointer(), (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nOwnerID_, pBuff, XPtrGetLen(byPubKey_));
				}
				XMailCatch;
			}

			void CMail::AddUser
				(
				IntPtr hFile_,
				SecAlg::Crypto::Alg euAlg_,
				unsigned int nCorpID_,
				unsigned int nUserCipherId_, 
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
					CMAddUser(hFile_.ToPointer(), (CryptAlgID)(unsigned int)euAlg_, nCorpID_, nUserCipherId_, pPub, XPtrGetLen(byPubKey_), pName, pEmail);
				}
				XMailCatch;
			}

			void CMail::RemoveUser(IntPtr hFile_, unsigned int nCorpID_, unsigned int nUserCipherId_)
			{
				try 
				{
					CMRemoveUser(hFile_.ToPointer(), nCorpID_, nUserCipherId_);
				}
				XMailCatch;
			}

			void CMail::AddCtrlInfo(IntPtr hFile_, DateTime dtStart_, DateTime dtEnd_, int nCount_)
			{
				try 
				{
					struct _FILETIME ftStart, ftEnd;
					LARGE_INTEGER liStart, liEnd;
					liStart.QuadPart = dtStart_.ToFileTime();
					ftStart.dwHighDateTime = liStart.HighPart;
					ftStart.dwLowDateTime = liStart.LowPart;

					liEnd.QuadPart = dtEnd_.ToFileTime();
					ftEnd.dwHighDateTime = liEnd.HighPart;
					ftEnd.dwLowDateTime = liEnd.LowPart;

					CMAddCtrlInfo(hFile_.ToPointer(), ftStart, ftEnd, nCount_);
				}
				XMailCatch;
			}

			void CMail::AddCtrlInfo(IntPtr hFile_, int nCount_)
			{
				try 
				{
					struct _FILETIME ftZero;
					ftZero.dwHighDateTime = ftZero.dwLowDateTime = 0;

					CMAddCtrlInfo(hFile_.ToPointer(), ftZero, ftZero, nCount_);
				}
				XMailCatch;
			}

			int CMail::GetCtrlInfo(IntPtr hFile_, [Out] DateTime% dtStart_, [Out] DateTime% dtEnd_)
			{
				try 
				{
					struct _FILETIME ftStart, ftEnd;
					int nCount = CMGetCtrlInfo(hFile_.ToPointer(), ftStart, ftEnd);

					if( ftStart.dwHighDateTime == 0 )
					{
						dtStart_ = DateTime::MinValue;
					}
					else
					{
						LARGE_INTEGER liStart;
						liStart.HighPart = ftStart.dwHighDateTime;
						liStart.LowPart = ftStart.dwLowDateTime;
						dtStart_ = DateTime::FromFileTime(liStart.QuadPart);
					}
	
					if( ftEnd.dwHighDateTime == 0 )
					{
						dtEnd_ = DateTime::MaxValue;
					}
					else
					{
						LARGE_INTEGER liEnd;
						liEnd.HighPart = ftEnd.dwHighDateTime;
						liEnd.LowPart = ftEnd.dwLowDateTime;
						dtEnd_ = DateTime::FromFileTime(liEnd.QuadPart);
					}
					return nCount;
				}
				XMailCatch;
			}

		} // CreAPI
	} // Xugd
} // SHCre