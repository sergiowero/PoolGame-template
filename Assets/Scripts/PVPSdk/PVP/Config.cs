using System;

namespace PVPSdk
{
	public class Config
	{
//		public static string HttpUri = "http://192.168.1.168/";
        public static string HttpUri = "http://dev.pvp.monthurs.com/";

		public static string LoginRequest = "User/Login";
        
		public static int protocol_request_timeout = 30;
		public static int connect_timeout = 30;
	}
}