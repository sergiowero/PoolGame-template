using System;
using System.Collections;
using TcpClient = SocketEx.TcpClient;
using UnityEngine;
using System.IO;
using PVPProtobuf;
using PVPProtobuf_Token;
using ProtoBuf;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace PVPSdk.PVP {


    /// <summary>
    /// Socket client state.
    /// </summary>
    public enum SocketClientState{
        None = 0,

        /// <summary>
        /// 网络层连接成功，但是没有验证token
        /// </summary>
        Connected,

        /// <summary>
        /// 验证了 token
        /// </summary>
        CheckTokened,

        Closed,
    }

    public class ReceivedProtoEventArgs : EventArgs{
        public UInt16 requestId{ get; set;}
        public int errorCode{ get; set; }
        public int messageTypeId{ get; set; }
        public byte[] bytes { get; set; }
    }

    public delegate void ReceivedProtoEventHandler(ReceivedProtoEventArgs e);
    public delegate void NetworkErrorEventHandler();
    public delegate void StartConnectEventHandler();

    /// <summary>
    /// 访问服务器的socket 客户端，支持自动重连,支持自动接收数据包
    /// </summary>
    public class SocketClient { 

        private string ip;
        private int port;
        private string token;

        private TcpClient _tcpClient;

        private bool _keepConnecting;
        public bool keepConnecting{
            get{
                return this._keepConnecting;
            }
        }
        private SocketClientState _state = SocketClientState.None;
        public SocketClientState state{
            get{
                return _state;
            }
        }
        private Thread _runConnect = null;
        private Thread _sendCheckTokenThread = null;
        private Thread _sendDataThread = null;
        private Thread _heartBeatThread = null;

        private int _bufferSize = 2048;
        private byte[] _buffer;
        private int _headerSize = 14;

        bool _writing = false;
        /// <summary>
        /// 优先发送的数据包
        /// </summary>
        private Queue<ReceivedProtoEventArgs> _socketWriteQueue = new Queue<ReceivedProtoEventArgs> ();
        private Queue<ReceivedProtoEventArgs> _writeQueue = new Queue<ReceivedProtoEventArgs> ();

        public event ReceivedProtoEventHandler receiveProtoEventHandler;
        public event NetworkErrorEventHandler networkErrorHandler;
        public event StartConnectEventHandler startConnectEventHandler;

        public bool isConnected {
            get {
                if (this._tcpClient == null || this._tcpClient.Client == null) {
                    return false;
                }
                return this._tcpClient.Client.Connected;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">Ip.</param>
        /// <param name="port">Port.</param>
        public void Connect (string ip, int port, string token) {
            this.ip = ip;
            this.port = port;
            this.token = token;
            if (this.isConnected){
                try{
                    this._tcpClient.Close ();
                }catch(Exception e){
                }
            }
            this._keepConnecting = true;
            _runConnect = new Thread (_Connect);
            _runConnect.Start ();
        }
        /// <summary>
        /// 连续重连次数
        /// </summary>
        public const int Max_Retry_Times = 3;

        private int _retry_times = 0;
        /// <summary>
        /// 这里面机会同时接受数据并解析
        /// 这段代码
        /// </summary>
        private void _Connect(){
            this._retry_times = 0;
            while(this._keepConnecting){
                //如果已经连接就等待50 毫秒，再次检测
                if (this.isConnected) {
                    Thread.Sleep (10);
                } else {
                    if(this.startConnectEventHandler != null){
                        this.startConnectEventHandler ();
                    }
                    //清掉已有的线程
                    if (_sendDataThread != null) {
                        try{
                            _sendDataThread.Abort();
                        }catch(Exception e){
                        }
                    }
                    if (_sendCheckTokenThread != null) {
                        try{
                            _sendCheckTokenThread.Abort ();
                        }catch(Exception e){
                        
                        }
                    }
                    if (_heartBeatThread != null) {
                        try{
                            _heartBeatThread.Abort();
                        }catch(Exception e){
                        }
                    }
                    this._tcpClient = new TcpClient ();
//                    this._tcpClient.Client.io //.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive
                    this._state = SocketClientState.Closed;
                    //如果没有就连接端口，并且获取数据流
                    try{
                        this._tcpClient.Connect (this.ip, this.port);
                    }catch(Exception e){
                        Debug.LogError (string.Format("IP {0} port {1} message {2} stackTrace {3}", this.ip, this.port, e.Message, e.StackTrace));
                        this._retry_times += 1;
                        ///抛出异常
                        if (this._retry_times >= Max_Retry_Times) {
                            this._keepConnecting = false;
                            if(this.networkErrorHandler != null){
                                this.networkErrorHandler ();
                            }
                        }
                    }

                    if (this.isConnected) {
                        this._retry_times = 0;
                        this._state = SocketClientState.Connected;
                        this._buffer = new byte[this._bufferSize];
                        this.ReadHeader ();

                        _sendDataThread = new Thread (this.doSendData);
                        _sendDataThread.Start ();
                        _sendCheckTokenThread = new Thread( this._SocketCheckToken );
                        _sendCheckTokenThread.Start ();
                        _heartBeatThread = new Thread (this._HeartBeat);
                        _heartBeatThread.Start ();
                    }
                }
            }
            this._tcpClient.Close ();
        }

        private void _HeartBeat(){
            while (true) {
                if (_socketWriteQueue.Count == 0 && _writeQueue.Count == 0) {
                    this.SendData (0, MessageTypeId.Socket_HeartBeat_Notice);
                }

                ///三秒钟处理一次
                Thread.Sleep (3000);
            }
        }

        private void _SocketCheckToken(){
            while (this._keepConnecting && this._tcpClient.Connected && this._state != SocketClientState.CheckTokened) {
                Socket_CheckToken_Request t = new Socket_CheckToken_Request ();
                t.proto_version = Config.PROTO_VERSION;
                t.token = this.token;
                if(PVPGlobal.localAppUserInfo != null){
                    t.app_user_info_number = PVPGlobal.localAppUserInfo.number;
                    t.app_user_info_custom_data_number = PVPGlobal.localAppUserInfo.customDataNumber;
                }

                this.SendData<Socket_CheckToken_Request>(0, MessageTypeId.Socket_CheckToken_Request, t, true);
                //间隔 0.5 秒
                Thread.Sleep (500);
            }
        }

		public void ReadHeader() {
			try {
				// Begin receiving the data from the remote device.
				this._tcpClient.Client.BeginReceive(
					this._buffer, 
					0, 
					this._headerSize,
					SocketFlags.None, 
					ar => {
                        int bytesRead = this._tcpClient.Client.EndReceive(ar);
						if (bytesRead == this._headerSize) {
                            UInt16 requestId = BitConverter.ToUInt16(this._buffer.SubArray(0,2).CheckBigEndian(), 0);
                            int errorCode = BitConverter.ToInt32 (this._buffer.SubArray (2, 4).CheckBigEndian (), 0);
							int messageTypeId = BitConverter.ToInt32 (this._buffer.SubArray (6, 4).CheckBigEndian (), 0);
                            int protoLength = BitConverter.ToInt32 (this._buffer.SubArray (10, 4).CheckBigEndian (), 0);

							ReceivedProtoEventArgs r = new ReceivedProtoEventArgs ();
							r.bytes = null;
							r.messageTypeId = messageTypeId;
							r.errorCode = errorCode;
                            r.requestId = requestId;
                            if(r.messageTypeId == MessageTypeId.Socket_CheckToken_Response && r.errorCode == ErrorCode.SUCCESS){
                                this._state = SocketClientState.CheckTokened;
                            }
							if (protoLength == 0) {
								if (receiveProtoEventHandler != null) {
									receiveProtoEventHandler (r);
								}
								ReadHeader();
                            } else {
								ReadBody(protoLength, r);
                            }
						}
					},
					null);
				
			} catch (Exception e) {
				this._tcpClient.Close();
				Debug.LogError (e.Message);
				Debug.LogError (e.StackTrace);
			}
		}

        public void ReadBody(int proto_length, ReceivedProtoEventArgs arg) {

			try {
				if (this._bufferSize < proto_length) {
					this._bufferSize = proto_length + _headerSize;
					this._buffer = new byte[this._bufferSize];
					// 填充一下协议id
				}

				// Begin receiving the data from the remote device.
				this._tcpClient.Client.BeginReceive( 
					this._buffer, 
					this._headerSize, 
					proto_length, 
					SocketFlags.None, 
					ar => {
						// Read data from the remote device.
						int bytesRead = this._tcpClient.Client.EndReceive(ar);
						Debug.Log ("ReadBody bytesRead = " + bytesRead);
						if (bytesRead == proto_length) {
                            if (receiveProtoEventHandler != null) {
								arg.bytes = this._buffer.SubArray (this._headerSize, proto_length);
								receiveProtoEventHandler (arg);
							}
						}
						ReadHeader();
					},
					null);
			} catch (Exception e) {
				this._tcpClient.Close();
				Debug.LogError (e.Message);
				Debug.LogError (e.StackTrace);
			}

		}
			
		public void doSendData () {

            while (this._keepConnecting && this.isConnected) {
                if (_socketWriteQueue.Count == 0  && _writeQueue.Count == 0) {
                    Thread.Sleep (15);
                    continue;
                }
                try {
                    Stream stream = this._tcpClient.GetStream ();
                    while(_socketWriteQueue.Count > 0) {
                        ReceivedProtoEventArgs item = _socketWriteQueue.Dequeue ();
//                        Debug.LogError(Time.);
                        Debug.Log(string.Format("_socketWriteQueue data {0}", item.bytes.Length));
                        stream.Write (item.bytes, 0, item.bytes.Length);

                        stream.Flush ();
                    }

                    if(this.state == SocketClientState.CheckTokened){
                        while (_writeQueue.Count > 0) {
                            ReceivedProtoEventArgs item = _writeQueue.Dequeue ();
                            if(PVPGlobal.isDebug){
                                Debug.Log(string.Format("send data {0}", item.bytes.Length));
                            }
                            stream.Write (item.bytes, 0, item.bytes.Length);
                            stream.Flush ();
                        }
                    }

                } catch (Exception ex) {
                    Debug.LogError (ex.Message);
                    Debug.LogError (ex.StackTrace);
                    this._tcpClient.Close ();
                }
            }
        }
		
        public bool SendData(UInt16 requestId, int messageTypeId){
            if (!this.isConnected) {
                return false;
            }
            byte[] bf = new byte[10];//+ b.Length];
            Array.Copy(this._UInt162ByteArray(requestId).CheckBigEndian(), bf, 2);
            Array.Copy (this._Int2ByteArray (messageTypeId).CheckBigEndian (), 0, bf, 2, 4);
            Array.Copy (this._Int2ByteArray (0).CheckBigEndian (), 0, bf, 6, 4);
            ReceivedProtoEventArgs item = new ReceivedProtoEventArgs ();
            item.messageTypeId = messageTypeId;
            item.bytes = bf;
            _writeQueue.Enqueue (item);
            return true;
        }

        public bool SendData<T>(UInt16 request_id, int messageTypeId, T t, bool is_token = false) where T:global::ProtoBuf.IExtensible {
            if (!this.isConnected) {
                return false;
            }
            byte[] b = this._Serialize<T> (t);
            byte[] bf = new byte[10 + b.Length];
            Array.Copy (this._UInt162ByteArray (request_id).CheckBigEndian (), bf, 2);
            Array.Copy (this._Int2ByteArray (messageTypeId).CheckBigEndian (), 0, bf, 2, 4);
            Array.Copy (this._Int2ByteArray (b.Length).CheckBigEndian (), 0, bf, 6, 4);
            Array.Copy (b, 0, bf, 10, b.Length);
            ReceivedProtoEventArgs item = new ReceivedProtoEventArgs ();
            item.messageTypeId = messageTypeId;
            item.bytes = bf;
            if (is_token) {
                this._socketWriteQueue.Enqueue (item);
            } else {
                _writeQueue.Enqueue (item);
            }
            Debug.Log ("messageTypeId = " + messageTypeId + ", send in queue");
            return true;
        }


   		/// <summary>
   		/// Int2s the byte array.
   		/// </summary>
   		/// <returns>The byte array.</returns>
   		/// <param name="intValue">Int value.</param>
		private byte[] _Int2ByteArray(int intValue){
			return BitConverter.GetBytes(intValue);
		}

        private byte[] _UInt162ByteArray(UInt16 value){
            return BitConverter.GetBytes (value);
        }

		/// <summary>
		/// Serialize the specified t.
		/// </summary>
		/// <param name="t">T.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private byte[] _Serialize<T>(T t) where T:global::ProtoBuf.IExtensible {
			MemoryStream ms = new MemoryStream ();
			ProtoBuf.Serializer.Serialize<T> (ms, t);

			return ms.GetBuffer ().SubArray(0, Convert.ToInt32( ms.Length));
		}

        public void Close(){
            this._keepConnecting = false;
            if (this._sendCheckTokenThread != null) {
                try{
                    this._sendCheckTokenThread.Abort ();
                }catch(Exception e){
                }
            }

            if (this._sendDataThread != null) {
                try{
                    this._sendDataThread.Abort();
                }catch(Exception e){
                }
            }
            if(this._heartBeatThread!=null){
                try{
                    this._heartBeatThread.Abort();
                }catch(Exception e){

                }

            }
            if(this._tcpClient!=null){
                try{
                    this._tcpClient.Close ();
                }catch(Exception e){
                }
            }
        }
    }
}