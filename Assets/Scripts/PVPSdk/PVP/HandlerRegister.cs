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
    /// <summary>
    /// PVP sdk. 对外只用访问 PVPSdk 就好，不要访问任何其他的类
    /// </summary>
    public sealed class HandlerRegister 
    {
        public Handler.NetworkerrorEventHandler networkerrorEventHandler;

        /// <summary>
        /// 登录响应委托实例
        /// </summary>
        public Handler.LoginOrRegisterResponHandler loginOrRegisterEventHandler;

        public Handler.LobbyGetLobbyListResponHandler lobbyGetLobbyListEventHandler;

        /// <summary>
        /// 进入指定大厅响应
        /// </summary>
        public Handler.EnterLobbyResponseHandler enterLobbyResponseHandler;

        public Handler.GetRoomListResponseHandler getRoomListResponseHandler;

        public Handler.EnteredRoomResponseHandler enterRoomResponseHandler;

        /// <summary>
        /// 接收到房间消息的响应
        /// </summary>
        public Handler.RoomNewMessageBroadcastHandler roomNewMessageEventHandler;

       
        /// <summary>
        /// 长连接自动重连成功的响应
        /// </summary>
        public Handler.SocketCheckTokenResponseHandler socketCheckTokenEventHandler;


        public Handler.OtherMemberEnterRoomBroadcastHandler otherMemberEnterRoomEventHandler;


        /// <summary>
        /// 其他玩家离开房间的响应
        /// </summary>
        public Handler.OtherMemberLeaveRoomBroadcastHandler otherMemberLeaveRoomEventHandler;

        public Handler.RoomMemberCustomDataResponseHandler roomMemberCustomDataResponseHandler;

        /// <summary>
        /// 房间内某个玩家更新自定义晚间数据的响应
        /// </summary>
        public Handler.RoomMemberCustomDataBroadcastHandler roomMemberCustomDataBroadcastHandler;

        /// <summary>
        /// 房间自定义内容有更新的响应事件
        /// </summary>
        public Handler.RoomCustomDataBroadcastHandler updateRoomCustomDataBroadcastHandler;


        public Handler.LeaveRoomResponseHandler leaveRoomResponseHandler;

        /// <summary>
        /// 匹配对手成功额响应
        /// </summary>
        public Handler.RoomMatchOpponentResponseHandler RoomMatchOpponentResponseHandler;

       

        /// <summary>
        /// 其他玩家网络断链响应
        /// </summary>
        public Handler.OtherLoseConnectionBroadcastHandler otherLoseConnectionBroadcastHandler;


        /// <summary>
        /// 网络层解锁
        /// </summary>
        public Handler.UnlockSocketEventHandler unlockSocketEventhandler;


        /// <summary>
        /// 网络层正在尝试发送数据或者重连，需要游戏根据业务，禁止用户操作或者进行其他处理
        /// </summary>
        public Handler.LockSocketEventHandler lockSocketEventHandler;


        public Handler.RoomCancelMatchOpponentResonseHandler roomCancelMatchOpponentResonseHandler;

       

        public Handler.AppUserUpdateCustomDataResponseHandler appUserUpdateCustomDataResponseHandler;


        public Handler.AppUserUpdateInfoResponseHandler appUserUpdateInfoResponseHandler;

        public Handler.AppUserGetUserInfoResponseHandler appUserGetUserInfoResponseHandler;
    }
}