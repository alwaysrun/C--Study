//////////////////////////////////////////////////////////////////////////
//	FCrush.cpp
#include "stdafx.h"
#include <vcclr.h>
#include "FCrush.h"
#include "CSHException.h"

#include "CreFCrush.h"
#pragma comment(lib, "CreFCrush.lib")


namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
			void FCrush::FileDelete(String^ strFileName_, CrushLevel euLevel_, bool bDealName_)
			{
				pin_ptr<const wchar_t> pFile = PtrToStringChars(strFileName_);

				try 
				{
					CFCFileDelete(pFile, (int)euLevel_, bDealName_);
				}
				XCatchParam
				XCatchFile
				XCatchCancel
				XCatchAll;
			}

			void FCrush::DirDelete(String^ strPathName_, CrushLevel euLevel_, bool bDealName_, bool bRecursion_, String^ strPattern_)
			{
				pin_ptr<const wchar_t> pPath = PtrToStringChars(strPathName_);
				pin_ptr<const wchar_t> pPattern = PtrToStringChars(strPattern_);

				try 
				{
					CFCDirDelete(pPath, (int)euLevel_, bDealName_, bRecursion_, pPattern);
				}
				XCatchParam
				XCatchFile
				XCatchCancel
				XCatchAll;
			}
		} // CreAPI
	} // Xugd
} // SHCre