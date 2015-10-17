using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.Caching;
using System;



namespace PVPSdk{

	/// <summary>
	/// http 协议是有响应包，可以进行
	/// </summary>
	public class HttpRequestHandler
	{
	    // http 响应报 ，在哪个级别返回数据
		// 网络返回消息类型
	    public enum NetworkMsgType {
	        network = 1,
	        server,
	        protocol,
	    };


	    AbstractHttpProtocol protoBuffer;
	    Callback<NetworkMsgType, string, AbstractHttpProtocol> onFinish;
		int time_out = Config.protocol_request_timeout;
		int connect_time_out = Config.connect_timeout;

		public void PostRequest ( AbstractHttpProtocol proto, Callback<NetworkMsgType, string, AbstractHttpProtocol> onFinish ) {
	        this.protoBuffer = proto;
			this.onFinish = onFinish;

	        Uri uri = new Uri (proto.GetRequestUrl());
	        HTTPRequest request = new HTTPRequest (uri, HTTPMethods.Post, true, true, OnDownloaded);
	        request.ConnectTimeout = TimeSpan.FromSeconds (connect_time_out);
	        request.Timeout = TimeSpan.FromSeconds (time_out);
	        request.RawData = proto.postBody;
	        request.EnableTimoutForStreaming = true;
	        HTTPManager.SendRequest(request);
	    }

	    void OnDownloaded(HTTPRequest req, HTTPResponse resp) {
			Debug.Log ("OnDownloaded");
	        NetworkMsgType error = NetworkMsgType.network;
	        string message = "未知错误";
	        if (resp != null) {

	            if (resp.IsSuccess) {
	                byte[] data = resp.Data;
	                protoBuffer.SetResMsg (data, data.Length);
	                error = NetworkMsgType.protocol;
	                message = "请求成功";
	            } else {
					if (resp.StatusCode == 500 ||
	                    resp.StatusCode == 501 ||
	                    resp.StatusCode == 503 ||
	                    resp.StatusCode == 403) {
	                    message = resp.DataAsText;
	                    error = NetworkMsgType.server;
	                }
	            }
	        }
//			return;
	        onFinish (error, message, protoBuffer);
	    }
	}

}
