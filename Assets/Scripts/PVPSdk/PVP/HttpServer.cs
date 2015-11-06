using System;
using UnityEngine;


namespace PVPSdk.PVP
{
	public class HttpServer 
	{

		private HttpServer ()
		{
		}

		private HttpServer _instance = null;

		/// <summary>
		/// 返回单例
		/// </summary>
		/// <value>The instance.</value>
		public HttpServer Instance{
			get{
				if (this._instance == null) {
					this._instance = new HttpServer ();
				}
				return this._instance;
			}
		}
	}
}

