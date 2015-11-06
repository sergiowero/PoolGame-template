using System;
using System.Collections.Generic;

namespace PVPSdk.PVP
{
    public class Handler{
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

        public delegate void AppUserUpdateInfoResponseHandler (int errorCode,UpdateAppUserInfoResult meta);
        public delegate void AppUserUpdateCustomDataResponseHandler (int errorCode,NewCustomData meta);

        public delegate void AppUserGetUserInfoResponseHandler (int errorCode,List<UInt32> uids);

        //广播的消息
        public delegate void RoomNewMessageBroadcastHandler (int errorCode,RoomNewMessage m);
        public delegate void OtherMemberEnterRoomBroadcastHandler (int errorCode,List<PVPSdk.RoomInfo.MemberInfo> newMembers);
        public delegate void OtherMemberLeaveRoomBroadcastHandler (int errorCode,List<uint> member_uids);
        public delegate void RoomMemberCustomDataBroadcastHandler (int errorCode,MemberCustomData meta);
        public delegate void RoomCustomDataBroadcastHandler (int errorCode,NewCustomData meta);
        public delegate void OtherLoseConnectionBroadcastHandler (int errorCode,uint other_uid);

        //网络事件
        public delegate void UnlockSocketEventHandler ();
        public delegate void LockSocketEventHandler ();
        public delegate void NetworkerrorEventHandler ();
    }
}

