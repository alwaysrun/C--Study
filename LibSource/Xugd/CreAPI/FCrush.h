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
			/// 文件粉碎相关的函数；
			/// 错误码定义在Common下的SHErrorCode。
			/// </summary>
			public ref class FCrush
			{
			public:
				/// <summary>
				/// 回调函数状态
				/// </summary>
				enum class CallbackStatus
				{
					/// <summary>
					/// 无效，用于初始化
					/// </summary>
					Invalid = 0,
					/// <summary>
					/// 开始粉碎文件
					/// </summary>
					Begin,
					/// <summary>
					/// 正在粉碎文件
					/// </summary>
					Step,
					/// <summary>
					/// 粉碎文件结束
					/// </summary>
					End
				};

				/// <summary>
				/// 回调函数返回值
				/// </summary>
				enum class CallbackRet
				{
					/// <summary>
					/// 无效，用于初始化
					/// </summary>
					Invalid = 0,
					/// <summary>
					/// 回调成功（一般返回此值）
					/// </summary>
					OK,
					/// <summary>
					/// 取消当前操作（停止文件的粉碎）
					/// </summary>
					Cancel,
					/// <summary>
					/// 回调出错
					/// </summary>
					Error
				};

				/// <summary>
				/// 粉碎强度
				/// </summary>
				enum class CrushLevel
				{
					/// <summary>
					/// 普通
					/// </summary>
					Normal=1,
					/// <summary>
					/// 强
					/// </summary>
					Strong,
					/// <summary>
					/// 极强
					/// </summary>
					VeryStrong
				};

				/// <summary>
				/// 回调函数参数信息，指示有关进度相关信息
				/// </summary>  
				[StructLayout(LayoutKind::Sequential, Pack = 4, CharSet = CharSet::Unicode)]
				value struct CallbackInfo
				{
				public:
					/// <summary>
					/// 粉碎文件的进展状态
					/// </summary>
					CallbackStatus euStatus; 

					/// <summary>
					/// 当前处理的文件名，只有在Start下有效
					/// </summary>
					[MarshalAs(UnmanagedType::ByValTStr, SizeConst = CLen::PathMaxLen+1)]
					String^ strFileName;           

					/// <summary>
					///粉碎文件的当前进度
					/// </summary>
					unsigned int nCurStep;

					/// <summary>
					///粉碎文件的总进度
					/// </summary>
					unsigned int nTotalStep;		
				};    

				/// <summary>
				/// 回调函数委托，如果需要获取粉碎文件的进度或取消粉碎，就必须设定
				/// </summary>        
				/// <param name="stInfo_">回调函数信息</param>
				/// <returns>返回给调用者的信息</returns>
				[UnmanagedFunctionPointer(CallingConvention::StdCall)]
				delegate CallbackRet CallbackFun(CallbackInfo% pInfo_);

				/// <summary>
				/// 设定回调函数委托
				/// </summary>        
				/// <param name="delCallback_">要设定的委托</param>
				[DllImport("CreFCrush.dll", EntryPoint = "CFCSetCallback", CallingConvention = CallingConvention::Winapi)]
				static void SetCallback(CallbackFun^ delCallback_);      

				/// <summary>
				/// 安全删除指定的文件，出错抛出异常：SHParamException，参数错误；SHFileException，文件操作出错；
				/// SHUserCancelException，用户取消了当前操作；SHException，其他错误。
				/// </summary>
				/// <param name="strFileName_">要删除的文件名（包括完整路径）</param>
				/// <param name="euLevel_">清理的强度,强度越高，越难以恢复</param>
				/// <param name="bDealName_">是否对文件名进行处理: false，不处理；true，处理</param>       
				static void FileDelete
				(
					String^ strFileName_,
					CrushLevel euLevel_,
					bool bDealName_
				);

				/// <summary>
				/// 安全删除目录、子目录及目录下指定类型的文件。
				/// 出错抛出异常：SHParamException，参数错误；SHFileException，文件操作出错；
				/// SHUserCancelException，用户取消了当前操作；SHException，其他错误。
				/// </summary>
				/// <param name="strPathName_">要删除的目录名（包括完整路径）</param>
				/// <param name="euLevel_">清理的强度级别，强度越高，粉碎越彻底，同时耗时越长</param>
				/// <param name="bDealName_">是否对文件/文件夹名进行处理: false，不处理；true，处理</param>
				/// <param name="bRecursion_">是否递归处理子文件夹:false，不递归，只处理当前文件夹；true，递归处理所有子文件夹</param> 
				/// <param name="strPattern_">要删除的文件的类型（使用通配符，如*.*删除所有文件，*.doc删除所有Word文档）</param>        
				static void DirDelete
				(
					String^ strPathName_,
					CrushLevel euLevel_,
					bool bDealName_,
					bool bRecursion_,
					String^ strPattern_
				);    
			};
		} // CreAPI
	} // Xugd
} // SHCre