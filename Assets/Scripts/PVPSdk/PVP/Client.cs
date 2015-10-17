using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PVPProtobuf;
using TcpClient = SocketEx.TcpClient;
using System.IO;
using ProtoBuf;
using PVPProtobuf_Token;

namespace PVPSdk
{

    public class CUSTOM_MESSAGE_TARGET
    {
        public const int Other = 1;
        public const int ALL = 2;
    }


    //响应的委托
    public delegate void LoginOrRegisterResponHandler (int errorCode);
    public delegate void LobbyGetLobbyListResponHandler (int errorCode,List<LobbyInfo> lobby_info_list);
    public delegate void EnterLobbyResponseHandler (int errorCode);
    public delegate void GetRoomListResponseHandler (int errorCode,List<PVPSdk.RoomInfo> rooms);
    public delegate void EnteredRoomResponseHandler (int errorCode);
    public delegate void LeaveRoomResponseHandler (int errorCode);
    public delegate void RoomMatchOpponentResponseHandler (int errorCode);
    public delegate void SocketCheckTokenResponseHandler (int errorCode);
    public delegate void RoomCustomDataResponseHandler (int errorCode);
    public delegate void RoomMemberCustomDataResponseHandler (int errorCode);
    public delegate void RoomCancelMatchOpponentResonseHandler (int errorCode);


    //广播的消息
    public delegate void RoomNewMessageBroadcastHandler (int errorCode,RoomNewMessage m);
    public delegate void OtherMemberEnterRoomBroadcastHandler (int errorCode,List<PVPSdk.RoomInfo.MemberInfo> newMembers);
    public delegate void OtherMemberLeaveRoomBroadcastHandler (int errorCode,List<uint> member_uids);
    public delegate void RoomMemberCustomDataBroadcastHandler (int errorCode,MemberCustomData meta);
    public delegate void RoomCustomDataBroadcastHandler (int errorCode,RoomCustomData meta);
    public delegate void OtherLoseConnectionBroadcastHandler (int errorCode,uint other_uid);


    //网络事件
    public delegate void UnlockSocketEventHandler ();
    public delegate void LockSocketEventHandler ();
    public delegate void NetworkerrorEventHandler ();

    public enum LoginType
    {
        Account = 1,
        Guest = 2,
        Facebook = 3,
    }

    public enum DeviceType
    {
        Android = 1,
        IOS = 2,
        OTHER = 99,
    }


    public enum UpdateCustomDataRange
    {
        None = 0,

        /// <summary>
        /// 更新所有内容
        /// </summary>
        All = 1,

        /// <summary>
        /// 只更新发送的
        /// </summary>
        Sended = 2,
    }

    /// <summary>
    /// PVP sdk. 对外只用访问 PVPSdk 就好，不要访问任何其他的类
    /// </summary>
    public sealed class Client : MonoBehaviour
    {
        private class TcpRequestTimeoutChecker
        {
            public int requestId {
                get;
                private set;
            }

            public float timeout = 0;

            public int messageTypeId {
                get;
                private set;
            }

            public TcpRequestTimeoutChecker (int requestId, float timeout, int messageType)
            {
                this.timeout = timeout;
                this.messageTypeId = messageType;
                this.requestId = requestId;
            }
        }

        private Dictionary<int, TcpRequestTimeoutChecker> _tcpRequestTimeoutChecker = new Dictionary<int, TcpRequestTimeoutChecker> ();
        private List<int> _tmpTcpRequestTimeoutChecker = new List<int> ();

        /// <summary>
        /// The tcp client.
        /// </summary>
        private SocketClient _socketClient;



        private string token;
        private string ip;
        private int port;

        private Dictionary<int, float> _time_out_event_timer = new Dictionary<int, float> ();
        private Queue<ReceivedProtoEventArgs> _readQueue = new Queue<ReceivedProtoEventArgs> ();
        private static object _lock = new object ();

        public NetworkerrorEventHandler networkerrorEventHandler;

        public void _RaiseNetworkErrorEvent ()
        {
            if (this.networkerrorEventHandler != null) {
                this.networkerrorEventHandler ();
            }
        }

        //        private UInt16 __requestId = 0;

        private UInt16 _requestId = 0;

        private void Awake ()
        {
            this._ll.AddRange (Client._canSetTimeOutResponse);
            StartCoroutine (_RouteReceivedServerMessage ());
            StartCoroutine (_CheckTimeout ());
        }

        public bool isConnected {
            get {
                return this._socketClient != null && this._socketClient.isConnected;
            }
        }

        private void _OnNetworkError ()
        {
            ReceivedProtoEventArgs arg = new ReceivedProtoEventArgs ();
            arg.errorCode = ErrorCode.SUCCESS;
            arg.messageTypeId = MessageTypeId.Network_Offline_Notice;
            this._readQueue.Enqueue (arg);
        }

        /// <summary>
        /// 登出，清除本地数据
        /// </summary>
        public void LoginOut ()
        {
            PVP.isLogined = false;
            PVP.lobbyInfo = null;
            PVP.roomInfo = null;
            PVP.userInfo = null;
            PVP.appUserInfo = null;
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="loginType">Login type.</param>
        /// <param name="account">Account.</param>
        /// <param name="password">Password.</param>
        /// <param name="facebookAccessToken">Facebook access token.</param>
        /// <param name="timeout">Timeout.</param>
        public void Login (LoginType loginType, string account = "", string password = "", string facebookAccessToken = "", float timeout = 5)
        {
            PVP.isLogined = false;
            if (this._socketClient != null) {
                this._tcpRequestTimeoutChecker.Clear ();
                this._socketClient.Close ();
            }

            string uniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
            PVPProtobuf_Token.User_LoginOrRegister_Req loginRequest = new PVPProtobuf_Token.User_LoginOrRegister_Req ();
            loginRequest.login_type = (Int32)loginType;
            loginRequest.account = account;
            loginRequest.password = password;
            if (loginType == LoginType.Guest) {
                loginRequest.unique_identifier = SystemInfo.deviceUniqueIdentifier;
            } else {
                loginRequest.unique_identifier = "";
            }
            loginRequest.facebook_access_token = facebookAccessToken;
            if (String.IsNullOrEmpty (PVP.appKey)) {
                throw new Exception ("You have not init pvp sdk , please call PVP.Init(string appKey) and set appKey. ");
            }
            loginRequest.app_key = PVP.appKey;
            #if UNITY_ANDROID
            loginRequest.device_type = (int)DeviceType.Android;
            #elif UNITY_IOS
            loginRequest.device_type = (int) DeviceType.IOS;
            #else
            loginRequest.device_type = (int) DeviceType.OTHER;
            #endif
            HttpProtocol<User_LoginOrRegister_Req, User_LoginOrRegister_Res> loginProtocol = new HttpProtocol<User_LoginOrRegister_Req, User_LoginOrRegister_Res> ();
            loginProtocol.SetReqMsg (loginRequest);
            HttpRequestHandler h = new HttpRequestHandler ();
            h.PostRequest (loginProtocol, _OnLoginRes);
            if (this.lockSocketEventHandler != null) {
                this.lockSocketEventHandler ();
            }
        }

        private void _OnLoginRes (HttpRequestHandler.NetworkMsgType type, string message, AbstractHttpProtocol proto)
        {
            if (type == HttpRequestHandler.NetworkMsgType.network) {
                //网络出现问题
                _RaiseNetworkErrorEvent ();
                return;
            }
            int errorCode = ErrorCode.SERVICE_ERROR;
            if (proto != null) {
                HttpProtocol<User_LoginOrRegister_Req, User_LoginOrRegister_Res> p = proto as HttpProtocol<User_LoginOrRegister_Req, User_LoginOrRegister_Res>;
                if (p != null) {
                    errorCode = p.error_code;
                    User_LoginOrRegister_Res res = p.GetResMsg ();
                    if (p.error_code == ErrorCode.SUCCESS && res != null && res.uid > 0) {
                        PVP.isLogined = true;
                        PVP.userInfo = new UserInfo (res);
                        //登陆成功
                        _StartSocketTcpConnect (res);
                    } else {
                        PVP.isLogined = false;        
                    }
                }
            }

            if (this.loginOrRegisterEventHandler != null) {
                this.loginOrRegisterEventHandler (errorCode);
            }
        }

        private static int[] _canSetTimeOutResponse = new int[] {
            MessageTypeId.Lobby_EnterLobby_Server, 
            MessageTypeId.Lobby_GetLobbyList_Server, 
            MessageTypeId.Room_CancelMatchOpponent_Server, 
            MessageTypeId.Room_LeaveRoom_Server,
            MessageTypeId.Room_UpdateMemberCustomData_Server,
            MessageTypeId.Room_UpdateRoomCustomData_Server,
            MessageTypeId.Room_EnterRoom_Server,
        };
        private List<int> _ll = new List<int> ();

        /// <summary>
        /// 路由接收到的数据
        /// </summary>
        /// <returns>The read message.</returns>
        private IEnumerator _RouteReceivedServerMessage ()
        {
            while (true) {
                while (this._readQueue.Count > 0) {
                    ReceivedProtoEventArgs e = this._readQueue.Dequeue ();
                    Debug.LogError (string.Format ("e.messageTypeId {0} ", e.messageTypeId));
                    UInt16 requestId = e.requestId;
                    if (this._ll.Contains (e.messageTypeId)) {
                        this._tcpRequestTimeoutChecker.Remove (e.requestId);
                        Debug.Log (e.requestId);
                    }

                    switch (e.messageTypeId) {
                    case MessageTypeId.Lobby_GetLobbyList_Server:
                        this._RaiseLobbyGetLobbyListResponse (e);
                        break;
                    case MessageTypeId.Lobby_EnterLobby_Server:
                        this._RaiseEnterLobbyResponse (e);
                        break;
                    case MessageTypeId.Room_EnterRoom_Server:
                        this._RaiseEnterRoomResponse (e);
                        break;
                    case MessageTypeId.Socket_CheckToken_Server:
                        this._RaiseCheckTokenMessageResponse (e);
                        break;
                    case MessageTypeId.Room_OtherMemberEnterRoom_Server:
                        this._RaiseOtherMemberEnterRoomMessageResponse (e);
                        break;
                    case MessageTypeId.Room_OtherMemberLeaveRoom_Broadcast:
                        this._RaiseOtherMemberLeaveRoomBroadcast (e);
                        break;
                    case MessageTypeId.Room_UpdateRoomCustomData_Server:
                        this._RaiseUpdateRoomCustomDataResponse (e);
                        break;
                    case MessageTypeId.Room_UpdateRoomCustomData_Broadcast:
                        this._RaiseUpdateRoomCustomDataBroadcast (e);
                        break;
                    case MessageTypeId.Room_LeaveRoom_Server:
                        this._RaiseLeaveRoomResponse (e);
                        break;
                    case MessageTypeId.Room_MatchOpponent_Server:
                        this._RaiseMatchOpponentResponse (e);
                        break;
                    case MessageTypeId.Room_OtherLoseConnection_Broadcast:
                        this._RaiseOtherLoseConnectionBroadcast (e);
                        break;
                    case MessageTypeId.Network_Offline_Notice:
                        this._RaiseNetworkErrorEvent ();
                        break;
                    case MessageTypeId.Network_Lock_Notice:
                        this._RaiseLockSocketEvent ();
                        break;
                    case MessageTypeId.Network_UnLock_Notice:
                        this._RaiseUnlockSocketEvent ();
                        break;
                    case MessageTypeId.Room_UpdateMemberCustomData_Server:
                        this._RaiseRoomUpdateMemberCustomDataResponseEvent (e);
                        break;
                    case MessageTypeId.Room_UpdateMemberCustomData_Broadcast:
                        this._RaiseRoomUpdateMemberCustomDataBroadcastEvent (e);
                        break;
                    case MessageTypeId.Room_CancelMatchOpponent_Server:
                        this._RaiseRoomCancelMatchOpponentResonse (e);
                        break;
                    default:
                        Debug.LogWarning (String.Format ("do nothing  {0}", e.messageTypeId));
                        break;
                    }

                }
                //返回一帧
                yield return 0;
            }    
        }

        private void _OnReceivedBytes (ReceivedProtoEventArgs e)
        {
            lock (_lock) {
                this._readQueue.Enqueue (e);
            }
        }



        /// <summary>
        /// 登录响应委托实例
        /// </summary>
        public event LoginOrRegisterResponHandler loginOrRegisterEventHandler;

        private void _StartSocketTcpConnect (User_LoginOrRegister_Res protobufMeta)
        {
            this.token = protobufMeta.token;
            this.ip = protobufMeta.ip;
            this.port = protobufMeta.port;

            this.ConnectServer ();
        }

        public bool ConnectServer ()
        {
            if (!PVP.isLogined) {
                return false;
            }
            if (_socketClient == null) {
                _socketClient = new SocketClient ();
                _socketClient.receiveProtoEventHandler += new ReceivedProtoEventHandler (_OnReceivedBytes);
                _socketClient.networkErrorHandler += new NetworkErrorEventHandler (_OnNetworkError);
                _socketClient.lockSocketEventhandler += new LockSocketEventHandler (delegate() {
                    ReceivedProtoEventArgs arg = new ReceivedProtoEventArgs ();
                    arg.errorCode = ErrorCode.SUCCESS;
                    arg.messageTypeId = MessageTypeId.Network_Lock_Notice;
                    this._readQueue.Enqueue (arg);
                });
            }
            // 连接服务器
            _socketClient.Connect (this.ip, this.port, this.token);
            return true;
        }

        /// <summary>
        /// 进入大厅
        /// </summary>
        /// <param name="lobby_id">Lobby identifier.</param>
        public bool EnterLobby (int lobby_id, float timeout = 5)
        {
            Lobby_EnterLobby_Client enterLobby = new Lobby_EnterLobby_Client ();
            enterLobby.lobby_id = lobby_id;
            if (_socketClient.SendData<Lobby_EnterLobby_Client> (++this._requestId, MessageTypeId.Lobby_EnterLobby_Client, enterLobby)) {
                this._tcpRequestTimeoutChecker [this._requestId] = new TcpRequestTimeoutChecker (this._requestId, timeout, MessageTypeId.Lobby_EnterLobby_Server);
                return true;
            } else {
                return false;
            }
        }


        /// <summary>
        /// 进入指定房间, time_out 超时时间 
        /// </summary>
        /// <param name="room_id">Room identifier.</param>
        /// <param name="timeOut">Time out.</param>
        public bool EnterRoom (int room_id, float timeout = 5)
        {
            Room_EnterRoom_Client enterRoom = new Room_EnterRoom_Client ();
            enterRoom.room_id = room_id;

            if (_socketClient.SendData<Room_EnterRoom_Client> (++this._requestId, MessageTypeId.Room_EnterRoom_Client, enterRoom)) {
                this._tcpRequestTimeoutChecker [this._requestId] = new TcpRequestTimeoutChecker (this._requestId, timeout, MessageTypeId.Room_EnterRoom_Server);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 在房间内发送消息
        /// </summary>
        /// <param name="command_id">自定义的消息分类</param>
        /// <param name="message">消息内容</param>
        /// <param name="target_type">接收消息的玩家类型</param>
        public bool SendNewMessage (int command_id, byte[] message, int target_type)
        {
            Room_NewMessage_Client new_message = new Room_NewMessage_Client ();
            new_message.custom_command_id = command_id;
            new_message.message = message;
            new_message.target_type = target_type;
            return _socketClient.SendData<Room_NewMessage_Client> (++this._requestId, MessageTypeId.Room_NewMessage_Client, new_message);
        }

        public event LobbyGetLobbyListResponHandler lobbyGetLobbyListEventHandler;

        private void _RaiseLobbyGetLobbyListResponse (ReceivedProtoEventArgs arg)
        {
            int errorCode = arg.errorCode;
            List<LobbyInfo> l = null;
            if (errorCode == ErrorCode.SUCCESS) {
                l = new List<LobbyInfo> ();
                using (MemoryStream memoryStream = new MemoryStream (arg.bytes)) {
                    memoryStream.Position = 0;
                    Lobby_GetLobbyList_Server lobby_GetLobbyList_Server = Serializer.Deserialize<Lobby_GetLobbyList_Server> (memoryStream);

                    for (int i = 0; i < lobby_GetLobbyList_Server.lobby_list.Count; i++) {
                        l.Add (new LobbyInfo (lobby_GetLobbyList_Server.lobby_list [i]));
                    }
                }
            }

            if (this.lobbyGetLobbyListEventHandler != null) {
                this.lobbyGetLobbyListEventHandler (errorCode, l);
            }
        }

        /// <summary>
        /// 进入指定大厅响应
        /// </summary>
        public event EnterLobbyResponseHandler enterLobbyResponseHandler;

        private void _RaiseEnterLobbyResponse (ReceivedProtoEventArgs arg)
        {
            int count = 0;
            if (arg.errorCode == 0 && arg.bytes != null) {
                using (MemoryStream memoryStream = new MemoryStream (arg.bytes)) {
                    memoryStream.Position = 0;

                    Lobby_EnterLobby_Server lobby_EnterLobby_Server = Serializer.Deserialize<Lobby_EnterLobby_Server> (memoryStream);

                    count = lobby_EnterLobby_Server.room_count;
                }
            }
            if (this.enterLobbyResponseHandler != null) {
                this.enterLobbyResponseHandler (arg.errorCode);
            }
        }

        public event GetRoomListResponseHandler getRoomListResponseHandler;

        public event EnteredRoomResponseHandler enterRoomResponseHandler;

        private void _RaiseEnterRoomResponse (ReceivedProtoEventArgs arg)
        {
            if (arg.errorCode == 0 && arg.bytes != null) {

                using (MemoryStream memoryStream = new MemoryStream (arg.bytes)) {
                    memoryStream.Position = 0;

                    PVPProtobuf.Room_EnteredRoom_Server enterRoomMessage = Serializer.Deserialize<Room_EnteredRoom_Server> (memoryStream);
                
                    if (enterRoomMessage.room_info != null) {
                        PVP.roomInfo = new PVPSdk.RoomInfo (enterRoomMessage.room_info);
                    }
                }
            }

            if (this.enterRoomResponseHandler != null) {
                this.enterRoomResponseHandler (arg.errorCode);
            }
        }

        /// <summary>
        /// 接收到房间消息的响应
        /// </summary>
        public event RoomNewMessageBroadcastHandler roomNewMessageEventHandler;

        private void _RaiseRoomNewMessageEvent (ReceivedProtoEventArgs arg)
        {
            if (arg.errorCode == 0 && arg.bytes != null) {

                using (MemoryStream memoryStream = new MemoryStream (arg.bytes)) {
                    memoryStream.Position = 0;

                    PVPProtobuf.Room_NewMessage_Broadcast new_message = Serializer.Deserialize<Room_NewMessage_Broadcast> (memoryStream);

                    if (this.roomNewMessageEventHandler != null) {
                        RoomNewMessage m = new RoomNewMessage (new_message);
                        this.roomNewMessageEventHandler (arg.errorCode, m);
                    }

                }
            }
        }

        /// <summary>
        /// 长连接自动重连成功的响应
        /// </summary>
        public SocketCheckTokenResponseHandler socketCheckTokenEventHandler;

        private void _RaiseCheckTokenMessageResponse (ReceivedProtoEventArgs arg)
        {
            Debug.LogError ("_RaiseCheckTokenMessageEvent");
            this._RaiseUnlockSocketEvent ();

            if (arg.errorCode == 0 && arg.bytes != null) {

                using (MemoryStream memoryStream = new MemoryStream (arg.bytes)) {
                    memoryStream.Position = 0;

                    PVPProtobuf.Socket_CheckToken_Server s = Serializer.Deserialize<Socket_CheckToken_Server> (memoryStream);
                    PVP.appUserInfo = new AppUserInfo (s.user_info);
                    if (s.lobby_info != null) {
                        PVP.lobbyInfo = new LobbyInfo (s.lobby_info);
                    } else {
                        PVP.lobbyInfo = null;        
                    }
                    if (s.room_info != null) {
                        PVP.roomInfo = new RoomInfo (s.room_info);
                    } else {
                        PVP.roomInfo = null;        
                    }
                }
            }

            if (socketCheckTokenEventHandler != null) {
                this.socketCheckTokenEventHandler (arg.errorCode);
            }
        }

        public event OtherMemberEnterRoomBroadcastHandler otherMemberEnterRoomEventHandler;

        private void _RaiseOtherMemberEnterRoomMessageResponse (ReceivedProtoEventArgs arg)
        {
        }

        /// <summary>
        /// 其他玩家离开房间的响应
        /// </summary>
        public event OtherMemberLeaveRoomBroadcastHandler otherMemberLeaveRoomEventHandler;

        private void _RaiseOtherMemberLeaveRoomBroadcast (ReceivedProtoEventArgs arg)
        {
            List<uint > member_uids = new List<uint> ();
            if (arg.errorCode == ErrorCode.SUCCESS) {
                Room_OtherMemberLeaveRoom_Server s = _Deserialize<Room_OtherMemberLeaveRoom_Server> (arg.bytes);
                member_uids.AddRange (s.member_ids);
                PVP.roomInfo = null;
            }
            if (this.otherMemberLeaveRoomEventHandler != null) {
                this.otherMemberLeaveRoomEventHandler (arg.errorCode, member_uids);
            }
        }

        /// <summary>
        /// 获取大厅列表
        /// </summary>
        public bool GetLobbyList (float timeout = 5)
        {
            Lobby_GetLobbyList_Client l = new Lobby_GetLobbyList_Client ();
            l.ext = 1;
            if (_socketClient.SendData<Lobby_GetLobbyList_Client> (++this._requestId, MessageTypeId.Lobby_GetLobbyList_Client, l)) {
                this._tcpRequestTimeoutChecker [this._requestId] = new TcpRequestTimeoutChecker (this._requestId, timeout, MessageTypeId.Lobby_GetLobbyList_Server);
                return true;
            } else {
                return false;
            }
        }

        public RoomMemberCustomDataResponseHandler roomMemberCustomDataResponseHandler;

        private void _RaiseRoomUpdateMemberCustomDataResponseEvent (ReceivedProtoEventArgs e)
        {
            int errorCode = e.errorCode;

            if (this.roomMemberCustomDataResponseHandler != null) {
                this.roomMemberCustomDataResponseHandler (errorCode);
            }
        }

        /// <summary>
        /// 房间内某个玩家更新自定义晚间数据的响应
        /// </summary>
        public RoomMemberCustomDataBroadcastHandler roomMemberCustomDataBroadcastHandler;

        private void _RaiseRoomUpdateMemberCustomDataBroadcastEvent (ReceivedProtoEventArgs e)
        {
            int errorCode = e.errorCode;
            MemberCustomData cache_info = null;
            if (errorCode == ErrorCode.SUCCESS) {
                Room_UpdateMemberCustomData_Broadcast b = Client._Deserialize<Room_UpdateMemberCustomData_Broadcast> (e.bytes);
                cache_info = new MemberCustomData (b);
                if (PVP.roomInfo.member_infos.ContainsKey (cache_info.member_uid)) {
                    PVP.roomInfo.member_infos [cache_info.member_uid].UpdateCustomData (cache_info);
                }
            }

            if (this.roomMemberCustomDataBroadcastHandler != null) {
                this.roomMemberCustomDataBroadcastHandler (errorCode, cache_info);
            }
        }

        private static T _Deserialize<T> (byte[] bytes)
        {
            MemoryStream memoryStream = new MemoryStream (bytes);
            memoryStream.Position = 0;

            T t = Serializer.Deserialize<T> (memoryStream);
            return t;
        }


        /// <summary>
        /// 更新玩家在房间内的自定义数据
        /// </summary>
        /// <param name="update_range">全量更新还是增量更新</param>
        /// <param name="custom_data">更新的内容</param>
        /// <param name="check_data">检测的内容.</param>
        public bool UpdateMemberCustomData (UpdateCustomDataRange update_range, Dictionary<String, byte[]> custom_data, Dictionary<String, byte[]> check_data = null, float timeout = 5)
        {
            Room_UpdateMemberCustomData_Client client = new Room_UpdateMemberCustomData_Client ();
            client.update_range = (int)update_range;

            foreach (KeyValuePair<string, byte[]> item in custom_data) {
                Pair p = new Pair ();
                p.key = item.Key;
                p.value = item.Value;
                client.custom_data.Add (p);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in custom_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.custom_data.Add (p);
                }
            }

            if (_socketClient.SendData<Room_UpdateMemberCustomData_Client> (++this._requestId, MessageTypeId.Room_UpdateMemberCustomData_Client, client)) {
                this._tcpRequestTimeoutChecker [this._requestId] = new TcpRequestTimeoutChecker (this._requestId, timeout, MessageTypeId.Room_UpdateMemberCustomData_Server);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 更新房间的自定义内容，这个内容是所有对战玩家都可以更新的，不包括观众
        /// </summary>
        /// <param name="update_range">更新的类型.</param>
        /// <param name="custom_data">更新的内容</param>
        /// <param name="check_data">检查的内容.</param>
        public bool UpdateRoomCustomData (UpdateCustomDataRange update_range, Dictionary<String, byte[]> custom_data, Dictionary<String, byte[]> check_data = null, float timeout = 5)
        {
            Room_UpdateRoomCustomData_Client client = new Room_UpdateRoomCustomData_Client ();
            client.update_type = (int)update_range;
            foreach (KeyValuePair<string, byte[]> item in custom_data) {
                PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                p.key = item.Key;
                p.value = item.Value;
                client.custom_data.Add (p);
            }
            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in custom_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.custom_data.Add (p);
                }
            }
            if (_socketClient.SendData<Room_UpdateRoomCustomData_Client> (++this._requestId, MessageTypeId.Room_UpdateRoomCustomData_Client, client)) {
                this._tcpRequestTimeoutChecker [this._requestId] = (new TcpRequestTimeoutChecker (this._requestId, timeout, MessageTypeId.Room_UpdateRoomCustomData_Server));
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 房间自定义内容有更新的响应事件
        /// </summary>
        public RoomCustomDataBroadcastHandler updateRoomCustomDataBroadcastHandler;

        private void _RaiseUpdateRoomCustomDataResponse (ReceivedProtoEventArgs e)
        {
            RoomCustomData d = null;
            if (e.errorCode == ErrorCode.SUCCESS) {
                //d = new RoomCustomData ();
                Room_UpdateRoomCustomData_Server u = _Deserialize<Room_UpdateRoomCustomData_Server> (e.bytes);
                d = new RoomCustomData (u);
                PVP.roomInfo.UpdateCustomData (d);
            }
            if (this.updateRoomCustomDataBroadcastHandler != null) {
                this.updateRoomCustomDataBroadcastHandler (e.errorCode, d);
            }
        }

        private void _RaiseUpdateRoomCustomDataBroadcast (ReceivedProtoEventArgs e)
        {
            RoomCustomData d = null;
            if (e.errorCode == ErrorCode.SUCCESS) {
                Room_UpdateRoomCustomData_Broadcast u = _Deserialize<Room_UpdateRoomCustomData_Broadcast> (e.bytes);
                d = new RoomCustomData (u);
                PVP.roomInfo.UpdateCustomData (d);
            }
            if (this.updateRoomCustomDataBroadcastHandler != null) {
                this.updateRoomCustomDataBroadcastHandler (e.errorCode, d);
            }
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        public bool LeaveRoom (float timeout = 5)
        {
            Room_LeaveRoom_Client client = new Room_LeaveRoom_Client ();
            client.ext = 1;
            if (_socketClient.SendData<PVPProtobuf.Room_LeaveRoom_Client> (++this._requestId, MessageTypeId.Room_LeaveRoom_Client, client)) {
                this._tcpRequestTimeoutChecker [this._requestId] = (new TcpRequestTimeoutChecker (this._requestId, timeout, MessageTypeId.Room_LeaveRoom_Server));
                return true;
            } else {
                return false;
            }
        }

        public LeaveRoomResponseHandler leaveRoomResponseHandler;

        /// <summary>
        /// Raises the leave room event.
        /// </summary>
        /// <param name="e">E.</param>
        private void _RaiseLeaveRoomResponse (ReceivedProtoEventArgs e)
        {
            if (e.errorCode == ErrorCode.SUCCESS) {
                PVP.roomInfo = null;
            }
            if (leaveRoomResponseHandler != null) {
                leaveRoomResponseHandler (e.errorCode);
            }
        }

        /// <summary>
        /// 匹配对手成功额响应
        /// </summary>
        public RoomMatchOpponentResponseHandler RoomMatchOpponentResponseHandler;

        private void _RaiseMatchOpponentResponse (ReceivedProtoEventArgs e)
        {
            if (e.errorCode == ErrorCode.SUCCESS) {
                Room_MatchOpponent_Server s = _Deserialize<Room_MatchOpponent_Server> (e.bytes);
                PVP.roomInfo = new RoomInfo (s.room_info);
            }

            if (RoomMatchOpponentResponseHandler != null) {
                RoomMatchOpponentResponseHandler (e.errorCode);
            }
        }

        /// <summary>
        /// 匹配对手
        /// </summary>
        /// <param name="standard">匹配的邓丽或者分数值.</param>
        /// <param name="range">匹配范围.</param>
        /// <param name="waitForMatch">等待匹配的时间， 0 永久等待</param>
        public bool MatchOpponent (int standard, int range = 0, float waitForMatch = 10)
        {
            Room_MatchOpponent_Client client = new Room_MatchOpponent_Client ();
            client.standard = standard;
            client.range = range;
            return _socketClient.SendData<Room_MatchOpponent_Client> (++this._requestId, MessageTypeId.Room_MatchOpponent_Client, client);
        }

        void OnDestroy ()
        {
            if (this._socketClient != null) {
                this._socketClient.Close ();
            }
        }

        /// <summary>
        /// 其他玩家网络断链响应
        /// </summary>
        public OtherLoseConnectionBroadcastHandler otherLoseConnectionBroadcastHandler;

        private void _RaiseOtherLoseConnectionBroadcast (ReceivedProtoEventArgs e)
        {
            uint other_uid = 0;
            if (e.errorCode == ErrorCode.SUCCESS) {
                PVP.roomInfo = null;
                Room_OtherLoseConnection_Broadcast client = _Deserialize<Room_OtherLoseConnection_Broadcast> (e.bytes);
                other_uid = client.uid;
                PVP.roomInfo = null;
            }
            if (otherLoseConnectionBroadcastHandler != null) {
                otherLoseConnectionBroadcastHandler (e.errorCode, other_uid);
            }

        }

        /// <summary>
        /// 网络层解锁
        /// </summary>
        public UnlockSocketEventHandler unlockSocketEventhandler;

        private void _RaiseUnlockSocketEvent ()
        {
            if (this.unlockSocketEventhandler != null) {
                this.unlockSocketEventhandler ();
            }
        }

        /// <summary>
        /// 网络层正在尝试发送数据或者重连，需要游戏根据业务，禁止用户操作或者进行其他处理
        /// </summary>
        public LockSocketEventHandler lockSocketEventHandler;

        private void _RaiseLockSocketEvent ()
        {
            if (this.lockSocketEventHandler != null) {
                this.lockSocketEventHandler ();
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns><c>true</c> if this instance cancel match opnnent; otherwise, <c>false</c>.</returns>
        public bool CancelMatchOpnnent (float timeout = 5)
        {
            if (_socketClient.SendData ((++this._requestId), MessageTypeId.Room_CancelMatchOpponent_Client)) {
                this._tcpRequestTimeoutChecker [this._requestId] = (new TcpRequestTimeoutChecker (this._requestId, timeout, MessageTypeId.Room_CancelMatchOpponent_Server));
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 记录跳出的一瞬间 是否 还在连接中
        /// </summary>
        private bool _onPause_keepconnect;

        void OnApplicationFocus (bool focus_status)
        {
            lock (_lock) {
                if (focus_status) {
                    if (this._socketClient != null) {
                        if (this._onPause_keepconnect) {
                            this._socketClient.Connect (this.ip, this.port, this.token);    
                        } else {

                        }
                    }

                           
                } else {
                    _onPause_keepconnect = false;
                    if (this._socketClient != null) {
                        _onPause_keepconnect = this._socketClient.keepConnecting;
                        this._socketClient.Close ();
                    }    
                }
            }
        }

        public RoomCancelMatchOpponentResonseHandler roomCancelMatchOpponentResonseHandler;

        private void _RaiseRoomCancelMatchOpponentResonse (ReceivedProtoEventArgs e)
        {
            if (this.roomCancelMatchOpponentResonseHandler != null) {
                this.roomCancelMatchOpponentResonseHandler (e.errorCode);
            }
        }

        /// <summary>
        /// 检测请求数据的超时
        /// </summary>
        /// <returns>The timeout.</returns>
        private  IEnumerator _CheckTimeout ()
        {
            float t = 0;
            TcpRequestTimeoutChecker c = null;
            while (true) {
                if (this._tcpRequestTimeoutChecker.Count > 0) {
                    this._tmpTcpRequestTimeoutChecker.Clear ();
                    t = Time.deltaTime;
                    foreach (KeyValuePair<int,TcpRequestTimeoutChecker> item in   this._tcpRequestTimeoutChecker) {
                        c = item.Value;
                        c.timeout -= t;
                        if (c.timeout <= 0) {
                            switch (c.messageTypeId) {
                            case MessageTypeId.Lobby_GetLobbyList_Server:
                                if (this.lobbyGetLobbyListEventHandler != null) {
                                    this.lobbyGetLobbyListEventHandler (ErrorCode.RESPONSE_TIME_OUT, null);
                                }
                                break;
                            case MessageTypeId.Lobby_EnterLobby_Server:
                                if (this.enterLobbyResponseHandler != null) {
                                    this.enterLobbyResponseHandler (ErrorCode.RESPONSE_TIME_OUT);                    
                                }
                                break;
                            case MessageTypeId.Room_EnterRoom_Server:
                                if (this.enterRoomResponseHandler != null) {
                                    this.enterRoomResponseHandler (ErrorCode.RESPONSE_TIME_OUT);                    
                                }
                                break;
                            case MessageTypeId.Room_UpdateRoomCustomData_Server:
                                if (this.updateRoomCustomDataBroadcastHandler != null) {
                                    this.updateRoomCustomDataBroadcastHandler (ErrorCode.RESPONSE_TIME_OUT, null);
                                }
                                break;
                            case MessageTypeId.Room_LeaveRoom_Server:
                                if (this.leaveRoomResponseHandler != null) {
                                    this.leaveRoomResponseHandler (ErrorCode.RESPONSE_TIME_OUT);
                                }
                                break;
                            case MessageTypeId.Room_CancelMatchOpponent_Server:
                                if (this.roomCancelMatchOpponentResonseHandler != null) {
                                    this.roomCancelMatchOpponentResonseHandler (ErrorCode.RESPONSE_TIME_OUT);                    
                                }
                                break;
                            default:
                                Debug.LogWarning (String.Format ("timeout no nothing  {0}", c.messageTypeId));
                                break;
                            }
                            this._tmpTcpRequestTimeoutChecker.Add (item.Key);
                        }
                    }

                    if (this._tmpTcpRequestTimeoutChecker.Count > 0) {
                        for (int i = 0; i < this._tmpTcpRequestTimeoutChecker.Count; i++) {
                            this._tcpRequestTimeoutChecker.Remove (this._tmpTcpRequestTimeoutChecker [i]);            
                        }
                        this._tmpTcpRequestTimeoutChecker.Clear ();
                    }
                }
                yield return null;
            }
        }
    }
}