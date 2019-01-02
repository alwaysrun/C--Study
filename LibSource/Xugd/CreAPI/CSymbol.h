#pragma once
using namespace System;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Net::NetworkInformation;
using namespace System::Runtime::InteropServices;

using namespace SHCre::Xugd::Common;

namespace SHCre
{
	namespace Xugd
	{
		namespace CreAPI
		{
			/// <summary>
			/// �Լ������ʶ�����кŻ�ȡ�ӿ�(CreSymbol.dll)�ķ�װ,
			/// Mac��ַ��IP��ַ���ַ�����ʾ���ֽ����б�ʾ���ת�����μ�XConvert��
			/// 
			/// ������--��������ʱ�����׳��쳣��
			/// ������ش����׳�SHParamException��
			/// ��̬����ش����׳�SHDllException��
			/// ���磨������ַ��ȡ����ش����׳�SHNetException��
			/// ����ϵͳAPI��ش����׳�SHWinException��
			/// ����ͨ��SHException�����������׳������д���
			/// �����붨����Common�µ�SHErrorCode��
			/// 
			/// </summary>
			public ref class CSymbol
			{
			public:
				/// <summary>
				/// ����PC��Ϣ����ȡ�������ID
				/// </summary>
				/// <param name="strPcInfo_">PC��Ϣ(һ��Ϊ������ַ��Ӳ�����к��Լ�CPU���кŵ�)</param>
				/// <returns>�����ID</returns>
				static unsigned int GetPcID(String^ strPcInfo_);

				/// <summary>
				/// ����PC��Ϣ����ȡ������ı�ʶ
				/// </summary>
				/// <param name="strPcInfo_">PC��Ϣ(һ��Ϊ������ַ��Ӳ�����к��Լ�CPU���кŵ�)</param>
				/// <returns>�������ʶ(MD5Size��������)</returns>
				static array<unsigned char>^ GetPcSymbol(String^ strPcInfo_);

				/// <summary>
				/// ��ȡ������ַ������������ã����޷���ȡ��
				/// </summary>
				/// <returns>����mac��ַ</returns>
				static array<unsigned char>^ GetMacAddr()
				{
					NetworkInterface^ netInter = GetLocalConnectInterface();
					PhysicalAddress^ phyAddr = netInter->GetPhysicalAddress();
					return phyAddr->GetAddressBytes();
				}

				/// <summary>
				/// ��ȡIPv4��ַ������������ã����޷���ȡ��
				/// </summary>
				/// <returns>������IPv4��ַ</returns>
				static array<unsigned char>^ GetIPv4Addr()
				{
					UnicastIPAddressInformationCollection^ ipCollect = GetLocalConnectIpCollection();
					for( int i=0 ; i<ipCollect->Count ; ++i )
					{
						if(ipCollect[i]->Address->AddressFamily == AddressFamily::InterNetwork)
							return ipCollect[i]->Address->GetAddressBytes();
					}

					throw gcnew SHNetException(gcnew String(L"Local Area Connection of IPv4 not found"), (int)SHErrorCode::NotFound);
				}

				/// <summary>
				/// ��ȡIPv4���ص�ַ������������ã����޷���ȡ��
				/// </summary>
				/// <returns>������IPv4���ص�ַ</returns>
				static array<unsigned char>^ GetIPv4Gateway()
				{
					GatewayIPAddressInformationCollection^ ipGateways = GetLocalConnectGateways();
					for( int i=0 ; i<ipGateways->Count ; ++i )
					{
						if( ipGateways[i]->Address->AddressFamily == AddressFamily::InterNetwork )
							return ipGateways[i]->Address->GetAddressBytes();
					}

					throw gcnew SHNetException(gcnew String(L"Local Area Connection of IPv4 gateway not found"), (int)SHErrorCode::NotFound);
				}

				/// <summary>
				/// ��ȡIPv6��ַ������������ã����޷���ȡ��
				/// </summary>
				/// <returns>������IPv6��ַ</returns>
				static array<unsigned char>^ GetIPv6Addr()
				{
					UnicastIPAddressInformationCollection^ ipCollect = GetLocalConnectIpCollection();
					for( int i=0 ; i<ipCollect->Count ; ++i )
					{
						if(ipCollect[i]->Address->AddressFamily == AddressFamily::InterNetworkV6)
							return ipCollect[i]->Address->GetAddressBytes();
					}

					throw gcnew SHNetException(gcnew String(L"Local Area Connection of IPv4 not found"), (int)SHErrorCode::NotFound);
				}

				/// <summary>
				/// ��ȡIPv6���ص�ַ������������ã����޷���ȡ��
				/// </summary>
				/// <returns>������IPv6���ص�ַ</returns>
				static array<unsigned char>^ GetIPv6Gateway()
				{
					GatewayIPAddressInformationCollection^ ipGateways = GetLocalConnectGateways();
					for( int i=0 ; i<ipGateways->Count ; ++i )
					{
						if( ipGateways[i]->Address->AddressFamily == AddressFamily::InterNetworkV6 )
							return ipGateways[i]->Address->GetAddressBytes();
					}

					throw gcnew SHNetException(gcnew String(L"Local Area Connection of IPv6 gateway not found"), (int)SHErrorCode::NotFound);
				}				

				/// <summary>
				/// ��ȡָ���̷�Ψһ��ʶ���ֽ����к�
				/// </summary>
				/// <param name="whDrive_">�̷�</param>
				/// <returns>�̷�Ψһ��ʶ</returns>
				static array<unsigned char>^ GetDriveGuid(WCHAR whDrive_);

				/// <summary>
				/// ��ȡϵͳ��Ψһ��ʶ���ֽ����к�
				/// </summary>
				/// <returns>Ψһ��ʶ�ֽ�����</returns>
				static array<unsigned char>^ GetSysDriveGuid()
				{
					return GetDriveGuid(Environment::SystemDirectory[0]);
				}

				/// <summary>
				/// ��ȡӲ�����к�
				/// </summary>
				/// <returns>Ӳ�����к�</returns>
				static String^ GetDiskSN();

				/// <summary>
				/// ��ȡCPU���кţ�ֻ��intel pIII�Ժ��CPU���ܻ�ȡ����
				/// </summary>
				/// <returns>CPU���к�</returns>
				static String^ GetCpuSN();

				/// <summary>
				/// ��ȡBIOS���кţ�ֻ��AMI��Award�Լ�Phoenix�ſ��Ի�ȡ����
				/// </summary>
				/// <returns>BIOS���к�</returns>
				static String^ GetBiosSN();

			private:
				static UnicastIPAddressInformationCollection^ GetLocalConnectIpCollection()
				{
					return GetLocalConnectInterface()->GetIPProperties()->UnicastAddresses;
				}

				static GatewayIPAddressInformationCollection^ GetLocalConnectGateways()
				{
					return GetLocalConnectInterface()->GetIPProperties()->GatewayAddresses;
				}

				static NetworkInterface^ GetLocalConnectInterface()
				{
					array<NetworkInterface^>^ aryNetInterface = NetworkInterface::GetAllNetworkInterfaces();
					for(int i=0 ; i<aryNetInterface->Length ; ++i)
					{
						NetworkInterface^ netIter = aryNetInterface[i];
						if(netIter->NetworkInterfaceType == NetworkInterfaceType::Ethernet)
						{
							if( (netIter->Name == L"��������") || (netIter->Name == L"Local Area Connection") )
							{
								return netIter;
							}
						}
					}

					throw gcnew SHNetException(gcnew String(L"Local Area Connection not found"), (int)SHErrorCode::NotFound);
				}
			};
		} // CreAPI
	} // Xugd
} // SHCre