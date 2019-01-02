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
			/// 对计算机标识，序列号获取接口(CreSymbol.dll)的封装,
			/// Mac地址与IP地址的字符串表示与字节序列表示间的转换，参见XConvert类
			/// 
			/// 出错处理--函数出错时，会抛出异常：
			/// 参数相关错误，抛出SHParamException；
			/// 动态库相关错误，抛出SHDllException；
			/// 网络（网卡地址获取）相关错误，抛出SHNetException；
			/// 调用系统API相关错误，抛出SHWinException；
			/// 可以通过SHException，捕获我们抛出的所有错误；
			/// 错误码定义在Common下的SHErrorCode。
			/// 
			/// </summary>
			public ref class CSymbol
			{
			public:
				/// <summary>
				/// 根据PC信息，获取计算机的ID
				/// </summary>
				/// <param name="strPcInfo_">PC信息(一般为网卡地址、硬盘序列号以及CPU序列号等)</param>
				/// <returns>计算机ID</returns>
				static unsigned int GetPcID(String^ strPcInfo_);

				/// <summary>
				/// 根据PC信息，获取计算机的标识
				/// </summary>
				/// <param name="strPcInfo_">PC信息(一般为网卡地址、硬盘序列号以及CPU序列号等)</param>
				/// <returns>计算机标识(MD5Size长的数组)</returns>
				static array<unsigned char>^ GetPcSymbol(String^ strPcInfo_);

				/// <summary>
				/// 获取网卡地址（如果网卡禁用，则无法获取）
				/// </summary>
				/// <returns>网卡mac地址</returns>
				static array<unsigned char>^ GetMacAddr()
				{
					NetworkInterface^ netInter = GetLocalConnectInterface();
					PhysicalAddress^ phyAddr = netInter->GetPhysicalAddress();
					return phyAddr->GetAddressBytes();
				}

				/// <summary>
				/// 获取IPv4地址（如果网卡禁用，则无法获取）
				/// </summary>
				/// <returns>网卡的IPv4地址</returns>
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
				/// 获取IPv4网关地址（如果网卡禁用，则无法获取）
				/// </summary>
				/// <returns>网卡的IPv4网关地址</returns>
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
				/// 获取IPv6地址（如果网卡禁用，则无法获取）
				/// </summary>
				/// <returns>网卡的IPv6地址</returns>
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
				/// 获取IPv6网关地址（如果网卡禁用，则无法获取）
				/// </summary>
				/// <returns>网卡的IPv6网关地址</returns>
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
				/// 获取指定盘符唯一标识的字节序列号
				/// </summary>
				/// <param name="whDrive_">盘符</param>
				/// <returns>盘符唯一标识</returns>
				static array<unsigned char>^ GetDriveGuid(WCHAR whDrive_);

				/// <summary>
				/// 获取系统盘唯一标识的字节序列号
				/// </summary>
				/// <returns>唯一标识字节序列</returns>
				static array<unsigned char>^ GetSysDriveGuid()
				{
					return GetDriveGuid(Environment::SystemDirectory[0]);
				}

				/// <summary>
				/// 获取硬盘序列号
				/// </summary>
				/// <returns>硬盘序列号</returns>
				static String^ GetDiskSN();

				/// <summary>
				/// 获取CPU序列号（只有intel pIII以后的CPU才能获取到）
				/// </summary>
				/// <returns>CPU序列号</returns>
				static String^ GetCpuSN();

				/// <summary>
				/// 获取BIOS序列号（只有AMI、Award以及Phoenix才可以获取到）
				/// </summary>
				/// <returns>BIOS序列号</returns>
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
							if( (netIter->Name == L"本地连接") || (netIter->Name == L"Local Area Connection") )
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