using System;

namespace PVPSdk.PVP
{
    public class MessageTypeId
    {

        public const int Socket_CheckToken_Request = 1;
        public const int Socket_CheckToken_Response = 2;
        public const int Socket_HeartBeat_Notice = 3;

        public const int Lobby_GetLobbyList_Request = 201;
        public const int Lobby_GetLobbyList_Response = 202;

        public const int Lobby_EnterLobby_Request = 203;
        public const int Lobby_EnterLobby_Response = 204;

        //		// 创建房间
        //		public const int Room_CreateRoom_Request = 101;
        //		public const int Room_CreateRoom_Response = 102;

        // 进入房间
        public const int Room_EnterRoom_Request = 109;
        public const int Room_EnterRoom_Response = 110;

        //		// 列出所有可见房间
        //		public const int Room_GetRoomList_Request = 111;
        //		public const int Room_GetRoomList_Response = 112;

        // 对指定对象发消息
        //        public const int Message_CustomTargetMessage_Request = 105;
        //        public const int Message_CustomTargetMessage_Response = 106;

        // 发对房间多人发消息
        public const int Room_NewMessage_Request = 107;
        public const int Room_NewMessage_Response = 108;


        //		// 聊天
        //		public const int Chat_SendChat_Request = 103;
        //		public const int Chat_SendChat_Response = 104;
        //
        //        //房间内
        //		public const int Room_SendChat_Request = 113;
        //		public const int Room_SendChat_Response = 114;

        //		public const int Room_SendRoomCacheMessage_Request = 115;
        //		public const int Room_ReceiveRoomCacheMessage_Response = 116;

        //		public const int Room_RoomCacheMessageList_Request = 117;
        //		public const int Room_RoomCacheMessageList_Response = 118;

        public const int Room_OtherMemberEnterRoom_Response = 119;

        public const int Room_NewRoomMaster_Response = 120;

        //		public const int Room_StartGame_Request = 121;
        //		public const int Room_FinishGame_Request = 122;

        public const int Room_LeaveRoom_Request = 123;
        public const int Room_LeaveRoom_Response = 124;

        public const int Room_OtherMemberLeaveRoom_Broadcast = 125;

        //		public const int Room_StartBattle_Request = 126;
        //		public const int Room_StartBattle_Response = 127;
        //
        //		public const int Room_NextTurn_Request = 128;
        //		public const int Room_NextTurn_Response = 129;

        public const int Room_UpdateMemberCustomData_Request = 130;
        public const int Room_UpdateMemberCustomData_Broadcast = 131;
        public const int Room_UpdateMemberCustomData_Response = 132;

        public const int Room_UpdateRoomCustomData_Request = 133;
        public const int Room_UpdateRoomCustomData_Broadcast = 134;
        public const int Room_UpdateRoomCustomData_Response = 135;

        //        public const int Room_EnterRandomRoom_Request = 136;
        //        public const int Room_EnterRandomRoom_Response = 137;

        public const int Room_MatchOpponent_Request = 138;
        public const int Room_MatchOpponent_Response = 139;
        public const int Room_OtherLoseConnection_Broadcast = 140;
        //        public const int Room_NextTurn_Broadcast = 141;
        //        public const int Room_CompleteBattle_Request = 142;
        //        public const int Room_CompleteBattle_Broadcast = 143;
        public const int Room_CancelMatchOpponent_Request = 144;
        public const int Room_CancelMatchOpponent_Response = 145;

        public const int AppUser_UpdateCustomData_Request = 301;
        public const int AppUser_UpdateCustomData_Response = 302;
        public const int AppUser_GetInfo_Request = 303;
        public const int AppUser_GetInfo_Response = 304;
        public const int AppUser_UpdateInfo_Request = 305;
        public const int AppUser_UpdateInfo_Response = 306;



        public const int Network_Offline_Notice = 90001;
        public const int Network_Lock_Notice = 90002;
        public const int Network_UnLock_Notice = 90003;
    }
}

