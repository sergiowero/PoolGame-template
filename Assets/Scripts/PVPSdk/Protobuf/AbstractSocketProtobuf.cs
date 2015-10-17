using System;
using System.IO;

namespace PVPSdk
{
	/// <summary>
	/// socket 的协议，这里面的纯粹封装了协议，请求不一定有响应报，
	/// 或者说协议只是一个信息报
	/// </summary>
	public abstract class AbstractSocketProtobuf{

		public string tValue { get; set; }

		public int error_code = -1;
		public bool show_loading = true;
		public bool handle_error = true;

		public Byte[] postBody ;

		public MemoryStream resStream = null;

		public abstract string GetRequestUrl();
		public abstract void SetResMsg(Stream stream, long length);
		public abstract void SetResMsg(byte[] stream, long length);

		public override string ToString () {
			return  "_Msg";
		}

		public string GetBroadcastMsg () {
			return this.ToString();
		}
	}
}

