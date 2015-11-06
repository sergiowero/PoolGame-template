using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PVPProtobuf;
using TcpClient = SocketEx.TcpClient;
using System.IO;
using ProtoBuf;
using PVPProtobuf_Token;

namespace PVPSdk.PVP
{

    public class CUSTOM_MESSAGE_TARGET
    {
        public const int Other = 1;
        public const int ALL = 2;
    }

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
    public sealed class InternetClient : MonoBehaviour
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


        public void _RaiseNetworkErrorEvent ()
        {
            if (ICM.handlerRegister != null && ICM.handlerRegister.networkerrorEventHandler != null) {
                ICM.handlerRegister.networkerrorEventHandler ();
            }
        }

        private UInt16 _RequestuestId = 0;

        private void Awake ()
        {
            this._timeoutMessageTypeIdList.AddRange (InternetClient._canSetTimeOutResponse);
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


        public void Close ()
        {
            if (this._socketClient != null) {
                this._socketClient.Close ();
                this._readQueue.Clear ();
                this._tcpRequestTimeoutChecker.Clear ();
            }
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
            this.Close ();

            string uniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
            PVPProtobuf_Token.User_LoginOrRegister_Request loginRequest = new PVPProtobuf_Token.User_LoginOrRegister_Request ();
            loginRequest.login_type = (Int32)loginType;
            loginRequest.account = account;
            loginRequest.password = password;
            if (loginType == LoginType.Guest) {
                loginRequest.unique_identifier = SystemInfo.deviceUniqueIdentifier;
            } else {
                loginRequest.unique_identifier = "";
            }
            loginRequest.facebook_access_token = facebookAccessToken;
            if (String.IsNullOrEmpty (Config.appKey)) {
                throw new Exception ("You have not init pvp sdk , please call PVPGlobal.Init(string appKey) and set appKey. ");
            }
            loginRequest.app_key = Config.appKey;
            #if UNITY_ANDROID
            loginRequest.device_type = (int)DeviceType.Android;
            #elif UNITY_IOS
            loginRequest.device_type = (int) DeviceType.IOS;
            #else
            loginRequest.device_type = (int) DeviceType.OTHER;
            #endif
            HttpProtocol<User_LoginOrRegister_Request, User_LoginOrRegister_Response> loginProtocol = new HttpProtocol<User_LoginOrRegister_Request, User_LoginOrRegister_Response> ();
            loginProtocol.SetReqMsg (loginRequest);
            Http.HttpRequestHandler h = new Http.HttpRequestHandler ();
            h.PostRequest (loginProtocol, _OnLoginRes);
        }

        private void _OnLoginRes (Http.HttpRequestHandler.NetworkMsgType type, string message, AbstractHttpProtocol proto)
        {
            if (type == Http.HttpRequestHandler.NetworkMsgType.network) {
                //网络出现问题
                _RaiseNetworkErrorEvent ();
                return;
            }
            int errorCode = ErrorCode.SERVICE_ERROR;
            if (proto != null) {
                HttpProtocol<User_LoginOrRegister_Request, User_LoginOrRegister_Response> p = proto as HttpProtocol<User_LoginOrRegister_Request, User_LoginOrRegister_Response>;
                if (p != null) {
                    errorCode = p.error_code;
                    User_LoginOrRegister_Response res = p.GetResMsg ();
                    if (p.error_code == ErrorCode.SUCCESS && res != null && res.uid > 0) {
            PVPGlobal.isLogined = true;
            PVPGlobal.userInfo = new User (res);
                        //登陆成功
                        _StartSocketTcpConnect (res);
                    } else {
            PVPGlobal.isLogined = false;        
                    }
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.loginOrRegisterEventHandler != null) {
                ICM.handlerRegister.loginOrRegisterEventHandler (errorCode);
            }
        }

        private static int[] _canSetTimeOutResponse = new int[] {
            MessageTypeId.Lobby_EnterLobby_Response, 
            MessageTypeId.Lobby_GetLobbyList_Response, 
            MessageTypeId.Room_CancelMatchOpponent_Response, 
            MessageTypeId.Room_LeaveRoom_Response,
            MessageTypeId.Room_UpdateMemberCustomData_Response,
            MessageTypeId.Room_UpdateRoomCustomData_Response,
            MessageTypeId.Room_EnterRoom_Response,
            MessageTypeId.AppUser_GetInfo_Response,
            MessageTypeId.AppUser_UpdateCustomData_Response,
            MessageTypeId.AppUser_UpdateInfo_Response,
        };

        private List<int> _timeoutMessageTypeIdList = new List<int> ();

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
                    if (this._timeoutMessageTypeIdList.Contains (e.messageTypeId)) {
                        this._tcpRequestTimeoutChecker.Remove (e.requestId);
                    }

                    switch (e.messageTypeId) {
                    case MessageTypeId.Lobby_GetLobbyList_Response:
                        this._RaiseLobbyGetLobbyListResponse (e);
                        break;
                    case MessageTypeId.Lobby_EnterLobby_Response:
                        this._RaiseEnterLobbyResponse (e);
                        break;
                    case MessageTypeId.Room_EnterRoom_Response:
                        this._RaiseEnterRoomResponse (e);
                        break;
                    case MessageTypeId.Socket_CheckToken_Response:
                        this._RaiseCheckTokenMessageResponse (e);
                        break;
                    case MessageTypeId.Room_OtherMemberEnterRoom_Response:
                        this._RaiseOtherMemberEnterRoomMessageResponse (e);
                        break;
                    case MessageTypeId.Room_OtherMemberLeaveRoom_Broadcast:
                        this._RaiseOtherMemberLeaveRoomBroadcast (e);
                        break;
                    case MessageTypeId.Room_UpdateRoomCustomData_Response:
                        this._RaiseUpdateRoomCustomDataResponse (e);
                        break;
                    case MessageTypeId.Room_UpdateRoomCustomData_Broadcast:
                        this._RaiseUpdateRoomCustomDataBroadcast (e);
                        break;
                    case MessageTypeId.Room_LeaveRoom_Response:
                        this._RaiseLeaveRoomResponse (e);
                        break;
                    case MessageTypeId.Room_MatchOpponent_Response:
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
                    case MessageTypeId.Room_UpdateMemberCustomData_Response:
                        this._RaiseRoomUpdateMemberCustomDataResponseEvent (e);
                        break;
                    case MessageTypeId.Room_UpdateMemberCustomData_Broadcast:
                        this._RaiseRoomUpdateMemberCustomDataBroadcastEvent (e);
                        break;
                    case MessageTypeId.Room_CancelMatchOpponent_Response:
                        this._RaiseRoomCancelMatchOpponentResponse (e);
                        break;
                    case MessageTypeId.AppUser_UpdateCustomData_Response:
                        this._RaiseAppUserUpdateCustomDataResponse (e);
                        break;
                    case MessageTypeId.AppUser_UpdateInfo_Response:
                        this._RaiseAppUserUpdateInfoResponse (e);
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


        private void _StartSocketTcpConnect (User_LoginOrRegister_Response protobufMeta)
        {
            this.token = protobufMeta.token;
            this.ip = protobufMeta.ip;
            this.port = protobufMeta.port;

            this.ConnectServer ();
        }

        public bool ConnectServer ()
        {
            if (!PVPGlobal.isLogined) {
                return false;
            }
            if (_socketClient == null) {
                _socketClient = new SocketClient ();
                _socketClient.receiveProtoEventHandler += new ReceivedProtoEventHandler (_OnReceivedBytes);
                _socketClient.networkErrorHandler += new NetworkErrorEventHandler (_OnNetworkError);
                _socketClient.startConnectEventHandler += new StartConnectEventHandler (delegate() {
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
            Lobby_EnterLobby_Request enterLobby = new Lobby_EnterLobby_Request ();
            enterLobby.lobby_id = lobby_id;
            if (_socketClient.SendData<Lobby_EnterLobby_Request> (++this._RequestuestId, MessageTypeId.Lobby_EnterLobby_Request, enterLobby)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Lobby_EnterLobby_Response);
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
            Room_EnterRoom_Request enterRoom = new Room_EnterRoom_Request ();
            enterRoom.room_id = room_id;

            if (_socketClient.SendData<Room_EnterRoom_Request> (++this._RequestuestId, MessageTypeId.Room_EnterRoom_Request, enterRoom)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Room_EnterRoom_Response);
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
            Room_NewMessage_Request new_message = new Room_NewMessage_Request ();
            new_message.custom_command_id = command_id;
            new_message.message = message;
            new_message.target_type = target_type;
            return _socketClient.SendData<Room_NewMessage_Request> (++this._RequestuestId, MessageTypeId.Room_NewMessage_Request, new_message);
        }

        private void _RaiseLobbyGetLobbyListResponse (ReceivedProtoEventArgs arg)
        {
            int errorCode = arg.errorCode;
            List<LobbyInfo> l = null;
            if (errorCode == ErrorCode.SUCCESS) {
                l = new List<LobbyInfo> ();
                using (MemoryStream memoryStream = new MemoryStream (arg.bytes)) {
                    memoryStream.Position = 0;
                    Lobby_GetLobbyList_Response lobby_GetLobbyList_Response = Serializer.Deserialize<Lobby_GetLobbyList_Response> (memoryStream);

                    for (int i = 0; i < lobby_GetLobbyList_Response.lobby_list.Count; i++) {
                        l.Add (new LobbyInfo (lobby_GetLobbyList_Response.lobby_list [i]));
                    }
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.lobbyGetLobbyListEventHandler != null) {
                ICM.handlerRegister.lobbyGetLobbyListEventHandler (errorCode, l);
            }
        }

        /// <summary>
        /// 进入指定大厅响应
        /// </summary>
        private void _RaiseEnterLobbyResponse (ReceivedProtoEventArgs arg)
        {
//            int count = 0;
            if (arg.errorCode == 0 && arg.bytes != null) {
                Lobby_EnterLobby_Response lobby_EnterLobby_Response = _Deserialize<Lobby_EnterLobby_Response> (arg.bytes);

                PVPGlobal.lobbyInfo = new LobbyInfo (lobby_EnterLobby_Response.lobby_id, lobby_EnterLobby_Response.name);
//                count = lobby_EnterLobby_Response.room_count;
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.enterLobbyResponseHandler != null) {
                ICM.handlerRegister.enterLobbyResponseHandler (arg.errorCode);
            }
        }



        private void _RaiseEnterRoomResponse (ReceivedProtoEventArgs arg)
        {
            if (arg.errorCode == 0 && arg.bytes != null) {

                PVPProtobuf.Room_EnteredRoom_Response enterRoomMessage = _Deserialize<Room_EnteredRoom_Response> (arg.bytes);

                if (enterRoomMessage.room_info != null) {
                    PVPGlobal.roomInfo = new PVPSdk.RoomInfo (enterRoomMessage.room_info);
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.enterRoomResponseHandler != null) {
                ICM.handlerRegister.enterRoomResponseHandler (arg.errorCode);
            }
        }


        private void _RaiseRoomNewMessageEvent (ReceivedProtoEventArgs arg)
        {
            if (arg.errorCode == 0 && arg.bytes != null) {

           
                PVPProtobuf.Room_NewMessage_Broadcast new_message = _Deserialize<Room_NewMessage_Broadcast> (arg.bytes);

                if (ICM.handlerRegister != null && ICM.handlerRegister.roomNewMessageEventHandler != null) {
                    RoomNewMessage m = new RoomNewMessage (new_message);
                    ICM.handlerRegister.roomNewMessageEventHandler (arg.errorCode, m);
                }

            }
        }



        private void _RaiseCheckTokenMessageResponse (ReceivedProtoEventArgs arg)
        {
            Debug.LogError ("_RaiseCheckTokenMessageEvent");
            this._RaiseUnlockSocketEvent ();
            if (arg.errorCode == 0 && arg.bytes != null && arg.bytes.Length > 0) {
                Debug.LogError (arg.bytes.Length);
                PVPProtobuf.Socket_CheckToken_Response s = _Deserialize<Socket_CheckToken_Response> (arg.bytes);
                if (s.user_info != null) {
                    PVPGlobal.localAppUserInfo = new LocalAppUserInfo (s.user_info);
                }
                if (s.lobby_info != null) {
                    PVPGlobal.lobbyInfo = new LobbyInfo (s.lobby_info);
                } else {
                    PVPGlobal.lobbyInfo = null;        
                }
                if (s.room_info != null) {
                    PVPGlobal.roomInfo = new RoomInfo (s.room_info);
                } else {
                    PVPGlobal.roomInfo = null;        
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.socketCheckTokenEventHandler != null) {
                ICM.handlerRegister.socketCheckTokenEventHandler (arg.errorCode);
            }
        }

        private void _RaiseOtherMemberEnterRoomMessageResponse (ReceivedProtoEventArgs arg)
        {
        }

        private void _RaiseOtherMemberLeaveRoomBroadcast (ReceivedProtoEventArgs arg)
        {
            List<uint > member_uids = new List<uint> ();
            if (arg.errorCode == ErrorCode.SUCCESS) {
                Room_OtherMemberLeaveRoom_Response s = _Deserialize<Room_OtherMemberLeaveRoom_Response> (arg.bytes);
                member_uids.AddRange (s.member_ids);
                PVPGlobal.roomInfo = null;
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.otherMemberLeaveRoomEventHandler != null) {
                ICM.handlerRegister.otherMemberLeaveRoomEventHandler (arg.errorCode, member_uids);
            }
        }

        /// <summary>
        /// 获取大厅列表
        /// </summary>
        public bool GetLobbyList (float timeout = 5)
        {
            Lobby_GetLobbyList_Request l = new Lobby_GetLobbyList_Request ();
            l.ext = 1;
            if (_socketClient.SendData<Lobby_GetLobbyList_Request> (++this._RequestuestId, MessageTypeId.Lobby_GetLobbyList_Request, l)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Lobby_GetLobbyList_Response);
                return true;
            } else {
                return false;
            }
        }



        private void _RaiseRoomUpdateMemberCustomDataResponseEvent (ReceivedProtoEventArgs e)
        {
            int errorCode = e.errorCode;

            if (ICM.handlerRegister != null && ICM.handlerRegister.roomMemberCustomDataResponseHandler != null) {
                ICM.handlerRegister.roomMemberCustomDataResponseHandler (errorCode);
            }
        }

        private void _RaiseRoomUpdateMemberCustomDataBroadcastEvent (ReceivedProtoEventArgs e)
        {
            int errorCode = e.errorCode;
            MemberCustomData customData = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                Room_UpdateMemberCustomData_Broadcast b = _Deserialize<Room_UpdateMemberCustomData_Broadcast> (e.bytes);
                customData = new MemberCustomData (b);

                if (errorCode == ErrorCode.SUCCESS) {
                    if (PVPGlobal.roomInfo.member_infos.ContainsKey (customData.member_uid)) {
                        PVPGlobal.roomInfo.member_infos [customData.member_uid].OnUpdateCustomData (customData);
                    }
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.roomMemberCustomDataBroadcastHandler != null) {
                ICM.handlerRegister.roomMemberCustomDataBroadcastHandler (errorCode, customData);
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
        public bool UpdateMemberCustomData (UpdateCustomDataRange update_range, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, float timeout = 5)
        {
            if ((being_deleted_data == null || being_deleted_data.Count == 0) && (being_updated_data == null || being_updated_data.Count == 0)) {
                return false;
            }

            Room_UpdateMemberCustomData_Request client = new Room_UpdateMemberCustomData_Request ();
            client.update_range = (int)update_range;

            if (being_updated_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    Pair p = new Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.being_updated_data.Add (p);
                }
            }

            if (being_deleted_data != null) {
                client.being_deleted_data.AddRange (being_deleted_data);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.check_data.Add (p);
                }
            }
            if (check_data_not_exits != null) {
                client.check_data_not_exist.AddRange (check_data_not_exits);
            }

            if (_socketClient.SendData<Room_UpdateMemberCustomData_Request> (++this._RequestuestId, MessageTypeId.Room_UpdateMemberCustomData_Request, client)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Room_UpdateMemberCustomData_Response);
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
        public bool UpdateRoomCustomData (UpdateCustomDataRange update_range, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, float timeout = 5)
        {
            if ((being_deleted_data == null || being_deleted_data.Count == 0) && (being_updated_data == null || being_updated_data.Count == 0)) {
                return false;
            }

            Room_UpdateRoomCustomData_Request client = new Room_UpdateRoomCustomData_Request ();
            client.update_range = (int)update_range;

            if (being_updated_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    Pair p = new Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.being_updated_data.Add (p);
                }
            }

            if (being_deleted_data != null) {
                client.being_deleted_data.AddRange (being_deleted_data);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.check_data.Add (p);
                }
            }
            if (check_data_not_exits != null) {
                client.check_data_not_exist.AddRange (check_data_not_exits);
            }

            if (_socketClient.SendData<Room_UpdateRoomCustomData_Request> (++this._RequestuestId, MessageTypeId.Room_UpdateRoomCustomData_Request, client)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Room_UpdateRoomCustomData_Response);
                return true;
            } else {
                return false;
            }
        }


        private void _RaiseUpdateRoomCustomDataResponse (ReceivedProtoEventArgs e)
        {
            NewCustomData d = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                Room_UpdateRoomCustomData_Response u = _Deserialize<Room_UpdateRoomCustomData_Response> (e.bytes);
                if (e.errorCode == ErrorCode.SUCCESS) {
                    //d = new RoomCustomData ();
                    d = new NewCustomData (PVPGlobal.userInfo.uid, u.custom_data_number, u.updated_data, u.deleted_data);
                } else {
                    d = new NewCustomData (PVPGlobal.userInfo.uid, u.custom_data_number, u.check_data, u.check_data_not_exist);
                }
                PVPGlobal.roomInfo.OnUpdateCustomData (d);
            }
            PVPGlobal.roomInfo.OnUpdateCustomData (d);
            if (ICM.handlerRegister != null && ICM.handlerRegister.updateRoomCustomDataBroadcastHandler != null) {
                ICM.handlerRegister.updateRoomCustomDataBroadcastHandler (e.errorCode, d);
            }
        }

        private void _RaiseUpdateRoomCustomDataBroadcast (ReceivedProtoEventArgs e)
        {
            NewCustomData d = null;
            if (e.errorCode == ErrorCode.SUCCESS) {
                Room_UpdateRoomCustomData_Broadcast u = _Deserialize<Room_UpdateRoomCustomData_Broadcast> (e.bytes);
                d = new NewCustomData (u.member_uid, u.custom_data_number, u.updated_data, u.deleted_data);
                PVPGlobal.roomInfo.OnUpdateCustomData (d);
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.updateRoomCustomDataBroadcastHandler != null) {
                ICM.handlerRegister.updateRoomCustomDataBroadcastHandler (e.errorCode, d);
            }
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        public bool LeaveRoom (float timeout = 5)
        {
            Room_LeaveRoom_Request client = new Room_LeaveRoom_Request ();
            client.ext = 1;
            if (_socketClient.SendData<PVPProtobuf.Room_LeaveRoom_Request> (++this._RequestuestId, MessageTypeId.Room_LeaveRoom_Request, client)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = (new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Room_LeaveRoom_Response));
                return true;
            } else {
                return false;
            }
        }



        /// <summary>
        /// Raises the leave room event.
        /// </summary>
        /// <param name="e">E.</param>
        private void _RaiseLeaveRoomResponse (ReceivedProtoEventArgs e)
        {
            if (e.errorCode == ErrorCode.SUCCESS) {
                PVPGlobal.roomInfo = null;
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.leaveRoomResponseHandler != null) {
                ICM.handlerRegister.leaveRoomResponseHandler (e.errorCode);
            }
        }



        /// <summary>
        /// 匹配成功对手后返回对手信息
        /// </summary>
        /// <param name="e">E.</param>
        private void _RaiseMatchOpponentResponse (ReceivedProtoEventArgs e)
        {
            if (e.errorCode == ErrorCode.SUCCESS) {
                Room_MatchOpponent_Response s = _Deserialize<Room_MatchOpponent_Response> (e.bytes);
                PVPGlobal.roomInfo = new RoomInfo (s.room_info);
                foreach (PVPProtobuf.AppUserInfo appUserInfo in s.appuser_infos) {
                    if (appUserInfo.uid == PVPGlobal.userInfo.uid) {
                        PVPGlobal.localAppUserInfo.FillAppUserInfo (appUserInfo);        
                    } else {
                        if (PVPGlobal.appUserInfos.ContainsKey (appUserInfo.uid)) {
                            PVPGlobal.appUserInfos [appUserInfo.uid].FillAppUserInfo (appUserInfo);            
                        } else {
                            PVPGlobal.appUserInfos [appUserInfo.uid] = new AppUserInfo (appUserInfo);            
                        }

                    }
                }
            }

            if (ICM.handlerRegister.RoomMatchOpponentResponseHandler != null) {
                ICM.handlerRegister.RoomMatchOpponentResponseHandler (e.errorCode);
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
            Room_MatchOpponent_Request client = new Room_MatchOpponent_Request ();
            client.standard = standard;
            client.range = range;
            return _socketClient.SendData<Room_MatchOpponent_Request> (++this._RequestuestId, MessageTypeId.Room_MatchOpponent_Request, client);
        }

        void OnDestroy ()
        {
            if (this._socketClient != null) {
                this._socketClient.Close ();
            }
        }



        private void _RaiseOtherLoseConnectionBroadcast (ReceivedProtoEventArgs e)
        {
            uint other_uid = 0;
            if (e.errorCode == ErrorCode.SUCCESS) {
                PVPGlobal.roomInfo = null;
                Room_OtherLoseConnection_Broadcast client = _Deserialize<Room_OtherLoseConnection_Broadcast> (e.bytes);
                other_uid = client.uid;
                PVPGlobal.roomInfo = null;
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.otherLoseConnectionBroadcastHandler != null) {
                ICM.handlerRegister.otherLoseConnectionBroadcastHandler (e.errorCode, other_uid);
            }

        }



        private void _RaiseUnlockSocketEvent ()
        {
            if (ICM.handlerRegister != null && ICM.handlerRegister.unlockSocketEventhandler != null) {
                ICM.handlerRegister.unlockSocketEventhandler ();
            }
        }


        private void _RaiseLockSocketEvent ()
        {
            if (ICM.handlerRegister != null && ICM.handlerRegister.lockSocketEventHandler != null) {
                ICM.handlerRegister.lockSocketEventHandler ();
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns><c>true</c> if this instance cancel match opnnent; otherwise, <c>false</c>.</returns>
        public bool CancelMatchOpnnent (float timeout = 5)
        {
            if (_socketClient.SendData ((++this._RequestuestId), MessageTypeId.Room_CancelMatchOpponent_Request)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = (new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Room_CancelMatchOpponent_Response));
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


        /// <summary>
        /// 匹配到对手
        /// </summary>
        /// <param name="e">E.</param>
        private void _RaiseRoomCancelMatchOpponentResponse (ReceivedProtoEventArgs e)
        {

            if (e.errorCode == ErrorCode.SUCCESS && e.bytes != null && e.bytes.Length > 0) {
                PVPProtobuf.Room_MatchOpponent_Response r = _Deserialize<PVPProtobuf.Room_MatchOpponent_Response> (e.bytes);
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.roomCancelMatchOpponentResonseHandler != null) {
                ICM.handlerRegister.roomCancelMatchOpponentResonseHandler (e.errorCode);
            }
        }


        private void _RaiseAppUserUpdateCustomDataResponse (ReceivedProtoEventArgs e)
        {
            NewCustomData meta = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                AppUser_UpdateCustomData_Response r = _Deserialize<AppUser_UpdateCustomData_Response> (e.bytes);
                if (e.errorCode == ErrorCode.SUCCESS) {
                    meta = new NewCustomData (PVPGlobal.userInfo.uid, r.custom_data_number, r.updated_data, r.deleted_data); 
                    PVPGlobal.localAppUserInfo.OnUpdateCustomData (r.custom_data_number, r.updated_data, r.deleted_data);
                } else {
                    meta = new NewCustomData (PVPGlobal.userInfo.uid, r.custom_data_number, r.check_data, r.check_data_not_exist);
                    PVPGlobal.localAppUserInfo.OnUpdateCustomData (r.custom_data_number, r.check_data, r.check_data_not_exist);
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.appUserUpdateCustomDataResponseHandler != null) {
                ICM.handlerRegister.appUserUpdateCustomDataResponseHandler (e.errorCode, meta);
            }
        }

        public bool UpdateAppUserInfo (int level = -1, Int64 score = -1, int winTimes = -1, int loseTimes = -1, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, float timeout = 5)
        {

            AppUser_UpdateInfo_Request client = new AppUser_UpdateInfo_Request ();


            client.level = level;
            client.score = score;
            client.win_times = winTimes;
            client.lose_times = loseTimes;

            if (being_updated_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    Pair p = new Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.being_updated_data.Add (p);
                }
            }

            if (being_deleted_data != null) {
                client.being_deleted_data.AddRange (being_deleted_data);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.check_data.Add (p);
                }
            }
            if (check_data_not_exits != null) {
                client.check_data_not_exist.AddRange (check_data_not_exits);
            }

            if (_socketClient.SendData<AppUser_UpdateInfo_Request> (++this._RequestuestId, MessageTypeId.AppUser_UpdateInfo_Request, client)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.AppUser_UpdateInfo_Response);
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseAppUserUpdateInfoResponse (ReceivedProtoEventArgs e)
        {
            UpdateAppUserInfoResult info = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                Appuser_UpdateInfo_Response r = _Deserialize<Appuser_UpdateInfo_Response> (e.bytes);
                info = new UpdateAppUserInfoResult (e.errorCode, r);
                PVPGlobal.localAppUserInfo.OnUpdateInfo (info);
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.appUserUpdateInfoResponseHandler != null) {
                ICM.handlerRegister.appUserUpdateInfoResponseHandler (e.errorCode, info);
            }
        }

        public bool UpdateAppUserCustomData (Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, float timeout = 5)
        {


            AppUser_UpdateCustomData_Request client = new AppUser_UpdateCustomData_Request ();

            if (being_updated_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    Pair p = new Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.being_updated_data.Add (p);
                }
            }

            if (being_deleted_data != null) {
                client.being_deleted_data.AddRange (being_deleted_data);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.check_data.Add (p);
                }
            }
            if (check_data_not_exits != null) {
                client.check_data_not_exist.AddRange (check_data_not_exits);
            }

            if (_socketClient.SendData<AppUser_UpdateCustomData_Request> (++this._RequestuestId, MessageTypeId.AppUser_UpdateCustomData_Request, client)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.AppUser_UpdateCustomData_Response);
                return true;
            } else {
                return false;
            }
        }

        public bool AppUserGetUserInfo (float timeout = 5)
        {
            if (_socketClient.SendData ((++this._RequestuestId), MessageTypeId.AppUser_GetInfo_Request)) {
                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.AppUser_GetInfo_Response);
                return true;
            } else {
                return false;
            }
        }


        private void _RaiseAppUserGetCustomDataResponse (ReceivedProtoEventArgs e)
        {
            List<UInt32> uids = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                AppUser_GetUserInfo_Response r = _Deserialize<AppUser_GetUserInfo_Response> (e.bytes);
                if (e.errorCode == ErrorCode.SUCCESS) {
                    uids = new List<uint> ();
                    foreach (PVPProtobuf.AppUserInfo item in r.user_infos) {
                        uids.Add (item.uid);
                        if (item.uid == PVPGlobal.userInfo.uid) {
                            PVPGlobal.localAppUserInfo.FillAppUserInfo (item);
                        } else {
                            if (PVPGlobal.appUserInfos.ContainsKey (item.uid)) {
                                PVPGlobal.appUserInfos [item.uid].FillAppUserInfo (item);                
                            } else {
                                PVPGlobal.appUserInfos [item.uid] = new AppUserInfo (item);                
                            }
                        }
                    }
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.appUserGetUserInfoResponseHandler != null) {
                ICM.handlerRegister.appUserGetUserInfoResponseHandler (e.errorCode, uids);
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
                            if (ICM.handlerRegister != null) {
                                switch (c.messageTypeId) {
                                case MessageTypeId.Lobby_GetLobbyList_Response:
                                    if (ICM.handlerRegister.lobbyGetLobbyListEventHandler != null) {
                                        ICM.handlerRegister.lobbyGetLobbyListEventHandler (ErrorCode.RESPONSE_TIME_OUT, null);
                                    }
                                    break;
                                case MessageTypeId.Lobby_EnterLobby_Response:
                                    if (ICM.handlerRegister.enterLobbyResponseHandler != null) {
                                        ICM.handlerRegister.enterLobbyResponseHandler (ErrorCode.RESPONSE_TIME_OUT);                    
                                    }
                                    break;
                                case MessageTypeId.Room_EnterRoom_Response:
                                    if (ICM.handlerRegister.enterRoomResponseHandler != null) {
                                        ICM.handlerRegister.enterRoomResponseHandler (ErrorCode.RESPONSE_TIME_OUT);                    
                                    }
                                    break;
                                case MessageTypeId.Room_UpdateRoomCustomData_Response:
                                    if (ICM.handlerRegister.updateRoomCustomDataBroadcastHandler != null) {
                                        ICM.handlerRegister.updateRoomCustomDataBroadcastHandler (ErrorCode.RESPONSE_TIME_OUT, null);
                                    }
                                    break;
                                case MessageTypeId.Room_LeaveRoom_Response:
                                    if (ICM.handlerRegister.leaveRoomResponseHandler != null) {
                                        ICM.handlerRegister.leaveRoomResponseHandler (ErrorCode.RESPONSE_TIME_OUT);
                                    }
                                    break;
                                case MessageTypeId.Room_CancelMatchOpponent_Response:
                                    if (ICM.handlerRegister.roomCancelMatchOpponentResonseHandler != null) {
                                        ICM.handlerRegister.roomCancelMatchOpponentResonseHandler (ErrorCode.RESPONSE_TIME_OUT);                    
                                    }
                                    break;
                                default:
                                    Debug.LogWarning (String.Format ("timeout no nothing  {0}", c.messageTypeId));
                                    break;
                                }
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
