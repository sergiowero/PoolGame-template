//using System;
//using System.Collections;
//using System.Collections.Generic;
//using PVPProtobuf;
//
//namespace PVPSdk
//{
//	/// <summary>
//	/// http 网络层请求
//	/// </summary>
//	public class HttpClient
//	{
//		public HttpClient ()
//		{
//		}
//
//		private static Dictionary<string, ProtoBuf.IExtensible> protoPool = new Dictionary<string, ProtoBuf.IExtensible> ();
//
//		/// <summary>
//		/// 发送登录请求
//		/// </summary>
//		public static void SendLoginReuqest(User_Login_Req loginRequest, Delegate OnFinish ){
//			string protocol_key = loginRequest.ToString();
//			if (!protoPool.ContainsKey( protocol_key )) {
//				protoPool [protocol_key] = loginRequest;
//				//            AsyncHttp request = new AsyncHttp (); 
//				//            StartCoroutine(
//				//                request.PostRequest( proto, OnFinish )
//				//            );  
//
//				HttpRequestHandler request = new HttpRequestHandler (); 
//				HttpProtocol<User_Login_Req, User_Login_Res> p = new HttpProtocol<User_Login_Req, User_Login_Res> ();
//				p.SetReqMsg (loginRequest);
//				Callback<HttpRequestHandler.NetworkMsgType, int, AbstractHttpProtocol> c = this.OnLoginRes;
////					new Callback<HttpRequestHandler.NetworkMsgType, int, AbstractHttpProtocol>( OnLoginRes );
//				// AbstractHttpProtocol proto, Callback<NetworkMsgType, int, AbstractHttpProtocol> fin 
//				request.PostRequest (p, c);
//			}   
//		}
//	}
//}
//
