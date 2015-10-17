using System;
using System.IO;

namespace PVPSdk{
	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractHttpProtocol {
	    protected string objName { get; set; }
	    
	    protected string method { get; set; }
	    
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
	        return objName + "_" + method + "_Msg";
	    }
	    
	    public string GetBroadcastMsg () {
	        return this.ToString();
	    }
	}

}