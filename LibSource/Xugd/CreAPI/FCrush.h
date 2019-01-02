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
			/// �ļ�������صĺ�����
			/// �����붨����Common�µ�SHErrorCode��
			/// </summary>
			public ref class FCrush
			{
			public:
				/// <summary>
				/// �ص�����״̬
				/// </summary>
				enum class CallbackStatus
				{
					/// <summary>
					/// ��Ч�����ڳ�ʼ��
					/// </summary>
					Invalid = 0,
					/// <summary>
					/// ��ʼ�����ļ�
					/// </summary>
					Begin,
					/// <summary>
					/// ���ڷ����ļ�
					/// </summary>
					Step,
					/// <summary>
					/// �����ļ�����
					/// </summary>
					End
				};

				/// <summary>
				/// �ص���������ֵ
				/// </summary>
				enum class CallbackRet
				{
					/// <summary>
					/// ��Ч�����ڳ�ʼ��
					/// </summary>
					Invalid = 0,
					/// <summary>
					/// �ص��ɹ���һ�㷵�ش�ֵ��
					/// </summary>
					OK,
					/// <summary>
					/// ȡ����ǰ������ֹͣ�ļ��ķ��飩
					/// </summary>
					Cancel,
					/// <summary>
					/// �ص�����
					/// </summary>
					Error
				};

				/// <summary>
				/// ����ǿ��
				/// </summary>
				enum class CrushLevel
				{
					/// <summary>
					/// ��ͨ
					/// </summary>
					Normal=1,
					/// <summary>
					/// ǿ
					/// </summary>
					Strong,
					/// <summary>
					/// ��ǿ
					/// </summary>
					VeryStrong
				};

				/// <summary>
				/// �ص�����������Ϣ��ָʾ�йؽ��������Ϣ
				/// </summary>  
				[StructLayout(LayoutKind::Sequential, Pack = 4, CharSet = CharSet::Unicode)]
				value struct CallbackInfo
				{
				public:
					/// <summary>
					/// �����ļ��Ľ�չ״̬
					/// </summary>
					CallbackStatus euStatus; 

					/// <summary>
					/// ��ǰ������ļ�����ֻ����Start����Ч
					/// </summary>
					[MarshalAs(UnmanagedType::ByValTStr, SizeConst = CLen::PathMaxLen+1)]
					String^ strFileName;           

					/// <summary>
					///�����ļ��ĵ�ǰ����
					/// </summary>
					unsigned int nCurStep;

					/// <summary>
					///�����ļ����ܽ���
					/// </summary>
					unsigned int nTotalStep;		
				};    

				/// <summary>
				/// �ص�����ί�У������Ҫ��ȡ�����ļ��Ľ��Ȼ�ȡ�����飬�ͱ����趨
				/// </summary>        
				/// <param name="stInfo_">�ص�������Ϣ</param>
				/// <returns>���ظ������ߵ���Ϣ</returns>
				[UnmanagedFunctionPointer(CallingConvention::StdCall)]
				delegate CallbackRet CallbackFun(CallbackInfo% pInfo_);

				/// <summary>
				/// �趨�ص�����ί��
				/// </summary>        
				/// <param name="delCallback_">Ҫ�趨��ί��</param>
				[DllImport("CreFCrush.dll", EntryPoint = "CFCSetCallback", CallingConvention = CallingConvention::Winapi)]
				static void SetCallback(CallbackFun^ delCallback_);      

				/// <summary>
				/// ��ȫɾ��ָ�����ļ��������׳��쳣��SHParamException����������SHFileException���ļ���������
				/// SHUserCancelException���û�ȡ���˵�ǰ������SHException����������
				/// </summary>
				/// <param name="strFileName_">Ҫɾ�����ļ�������������·����</param>
				/// <param name="euLevel_">�����ǿ��,ǿ��Խ�ߣ�Խ���Իָ�</param>
				/// <param name="bDealName_">�Ƿ���ļ������д���: false��������true������</param>       
				static void FileDelete
				(
					String^ strFileName_,
					CrushLevel euLevel_,
					bool bDealName_
				);

				/// <summary>
				/// ��ȫɾ��Ŀ¼����Ŀ¼��Ŀ¼��ָ�����͵��ļ���
				/// �����׳��쳣��SHParamException����������SHFileException���ļ���������
				/// SHUserCancelException���û�ȡ���˵�ǰ������SHException����������
				/// </summary>
				/// <param name="strPathName_">Ҫɾ����Ŀ¼������������·����</param>
				/// <param name="euLevel_">�����ǿ�ȼ���ǿ��Խ�ߣ�����Խ���ף�ͬʱ��ʱԽ��</param>
				/// <param name="bDealName_">�Ƿ���ļ�/�ļ��������д���: false��������true������</param>
				/// <param name="bRecursion_">�Ƿ�ݹ鴦�����ļ���:false�����ݹ飬ֻ����ǰ�ļ��У�true���ݹ鴦���������ļ���</param> 
				/// <param name="strPattern_">Ҫɾ�����ļ������ͣ�ʹ��ͨ�������*.*ɾ�������ļ���*.docɾ������Word�ĵ���</param>        
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