//////////////////////////////////////////////////////////////////////////
//	CSymbol.cpp
#include "stdafx.h"
#include <vcclr.h>
#include "CSymbol.h"

#include "CSHException.h"
#include "CreSymbol.h"

using namespace System::Net;
using namespace SHCre::Xugd::Common;

#pragma comment(lib, "CreSymbol.lib")


namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
#define XSymbolCatch	\
	XCatchParam	\
	XCatchWin	\
	XCatchAll

			//////////////////////////////////////////////////////////////////////////
			//			Implementation
			//////////////////////////////////////////////////////////////////////////
			unsigned int CSymbol::GetPcID(String^ strPcInfo_)
			{
				try 
				{
					array<unsigned char>^ byPcInfo = XConvert::UnicodeString2Bytes(strPcInfo_);
					pin_ptr<unsigned char> pBuff = &byPcInfo[0];
					return CSGetPcID(pBuff, byPcInfo->Length);
				}
				XSymbolCatch;
			}

			array<unsigned char>^ CSymbol::GetPcSymbol(String^ strPcInfo_)
			{
				try 
				{
					array<unsigned char>^	bySymbol = gcnew array<unsigned char>(CLen::MD5Size);
					pin_ptr<unsigned char>	pSymbol = &bySymbol[0];
					array<unsigned char>^ byPcInfo = XConvert::UnicodeString2Bytes(strPcInfo_);
					pin_ptr<unsigned char> pBuff = &byPcInfo[0];
					CSGetPcSymbol(pSymbol, pBuff, byPcInfo->Length);

					return bySymbol;
				}
				XSymbolCatch;
			}

			array<unsigned char>^ CSymbol::GetDriveGuid(WCHAR whDrive_)
			{
				try 
				{
					WCHAR	wzGuid[CLen::HardSNMaxLen+1];
					CSGetDriveGuid(whDrive_, wzGuid);

					Guid gidDrive = Guid(gcnew String(wzGuid));
					return gidDrive.ToByteArray();
				}
				XSymbolCatch;
			}

			String^ CSymbol::GetDiskSN()
			{
				try 
				{
					WCHAR	wzSN[CLen::HardSNMaxLen+1];
					CSGetDiskSN(wzSN);

					return gcnew String(wzSN);
				}
				XSymbolCatch;
			}

			String^ CSymbol::GetCpuSN()
			{
				try 
				{
					WCHAR	wzSN[CLen::HardSNMaxLen+1];
					CSGetCpuSN(wzSN);

					return gcnew String(wzSN);
				}
				XSymbolCatch;
			}

			String^ CSymbol::GetBiosSN()
			{
				try 
				{
					WCHAR	wzSN[CLen::HardSNMaxLen+1];
					CSGetBiosSN(wzSN);

					return gcnew String(wzSN);
				}
				XSymbolCatch;
			}
		} // CreAPI
	} // Xugd
} // SHCre