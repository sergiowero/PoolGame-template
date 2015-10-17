using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using ProtoBuf;
using System.Text.RegularExpressions;


namespace PVPSdk{
	/// <summary>
	/// http 的请求协议，http 一定有响应报
	/// </summary>
	public class HttpProtocol<T1, T2> : AbstractHttpProtocol where T1:global::ProtoBuf.IExtensible where T2: global::ProtoBuf.IExtensible  {
	    private const string TAG = "[Protocol]";

		public int error_code = 0;

	    public HttpProtocol(){
	        string reqName = typeof(T1).ToString();

	        if (!string.IsNullOrEmpty(reqName)) {
	            string reqNameStr = reqName.Substring (reqName.IndexOf ('.') + 1);
	            string[] reqNameSplit = reqNameStr.Split ('_');
	            if (reqNameSplit != null && reqNameSplit.Length != 0) {
	                this.objName = reqNameSplit [0];
	                this.method = reqNameSplit [1];
	            } else {
	                Debug.LogError (TAG + " reqName split error!");
	            }
	        } else {
	            Debug.LogError (TAG + " reqName is null!");
	        }
	        this.tValue = Toolkit.GetUnixTime().ToString();
	    }


	    public void SetReqMsg( T1 obj )  {
	        MemoryStream stream = new MemoryStream();
	        ProtoBuf.Serializer.Serialize<T1>(stream, obj);
	        stream.Position = 0;
	        this.postBody = new byte[stream.Length + 4 ];
	        byte[] bytes = BitConverter.GetBytes(Toolkit.GetUnixTime());
	        Array.Copy(bytes, this.postBody, 4);
	        stream.Read (this.postBody, 4, (int)stream.Length);
	    }

	    public override string GetRequestUrl () {
			string request_url = Config.HttpUri + objName + "/" + method;
	        Debug.Log ("Protocol GetRequestUrl = " + request_url);
	        return request_url;
	    }

	    public T2 GetResMsg (){
	        if (resStream != null && resStream.Length > 0) {
				
				byte[] buf = resStream.GetBuffer ();
				int error_code = BitConverter.ToInt32 (buf, 0);
				Debug.Log ("GetResMsg error_code = " + error_code);
				resStream.Position = 4;
	            Debug.Log ("GetResMsg resStream.Length = " + resStream.Length + ", T2 = " + typeof(T2).ToString());
	            return ProtoBuf.Serializer.Deserialize<T2>( resStream );
	        } else {
	            Debug.Log ("GetResMsg resStream.Length < 0");
                return default(T2);
	        }
	    }

	    public override void SetResMsg(byte[] stream, long length){

	        if (resStream != null) {
	            resStream.Close ();
	            resStream = null;
	        }

	        resStream = new MemoryStream ();
	        resStream.Write (stream, 0, (int)length);

	        if (length >= 4) {
	            base.error_code = System.BitConverter.ToInt32(stream, 0);
	            Debug.Log ("SetResMsg error_code = " + error_code + ", stream read = " + length);
	        }
	    }

	    public override void SetResMsg(Stream stream, long length){

	        if (resStream != null) {
	            resStream.Close ();
	            resStream = null;
	        }

	        resStream = new MemoryStream ();
	        byte[] buffer = new byte[length];
	        int numBytesToRead = (int)length;
	        int numBytesRead = 0;
	        do {

	            int read = stream.Read (buffer, numBytesRead, numBytesToRead);
	            numBytesRead += read;
	            numBytesToRead -= read;
	            Debug.Log ("SetResMsg length = " + length);

	        }while (numBytesToRead > 0);
	            
	        if (numBytesRead >= 4) {
	            base.error_code = System.BitConverter.ToInt32(buffer, 0);
	            Debug.Log ("SetResMsg error_code = " + error_code + ", stream read = " + numBytesRead);
	//            read = Toolkit.CopyStream (stream, resStream);
	//            Debug.Log ("SetResMsg resStream len = " + resStream.Length + ", stream read = " + read);
	        }
	        if (numBytesRead > 4) {

	            resStream.Write (buffer, 0, numBytesRead);
	        }
	    }
	}
}