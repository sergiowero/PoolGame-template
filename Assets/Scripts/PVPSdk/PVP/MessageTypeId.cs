using System;

namespace PVPSdk
{
	public class MessageTypeId
	{

		public const int Socket_CheckToken_Client = 1;
		public const int Socket_CheckToken_Server = 2;
        public const int Socket_HeartBeat_Notice = 3;

        public const int Lobby_GetLobbyList_Client = 201;
        public const int Lobby_GetLobbyList_Server = 202;

		public const int Lobby_EnterLobby_Client = 203;
		public const int Lobby_EnterLobby_Server = 204;

//		// 创建房间
//		public const int Room_CreateRoom_Client = 101;
//		public const int Room_CreateRoom_Server = 102;

		// 进入房间 
		public const int Room_EnterRoom_Client = 109;
		public const int Room_EnterRoom_Server = 110;

//		// 列出所有可见房间
//		public const int Room_GetRoomList_Client = 111;
//		public const int Room_GetRoomList_Server = 112;

		// 对指定对象发消息
//        public const int Message_CustomTargetMessage_Client = 105;
//        public const int Message_CustomTargetMessage_Server = 106;

		// 发对房间多人发消息
        public const int Room_NewMessage_Client = 107;
        public const int Room_NewMessage_Server = 108;


//		// 聊天
//		public const int Chat_SendChat_Client = 103;
//		public const int Chat_SendChat_Server = 104;
//
//        //房间内
//		public const int Room_SendChat_Client = 113;
//		public const int Room_SendChat_Server = 114;

//		public const int Room_SendRoomCacheMessage_Client = 115;
//		public const int Room_ReceiveRoomCacheMessage_Server = 116;

//		public const int Room_RoomCacheMessageList_Client = 117;
//		public const int Room_RoomCacheMessageList_Server = 118;

		public const int Room_OtherMemberEnterRoom_Server = 119;

		public const int Room_NewRoomMaster_Server = 120;

//		public const int Room_StartGame_Client = 121;
//		public const int Room_FinishGame_Client = 122;

		public const int Room_LeaveRoom_Client = 123;
		public const int Room_LeaveRoom_Server = 124;

		public const int Room_OtherMemberLeaveRoom_Broadcast = 125;

//		public const int Room_StartBattle_Client = 126;
//		public const int Room_StartBattle_Server = 127;
//
//		public const int Room_NextTurn_Client = 128;
//		public const int Room_NextTurn_Server = 129;

        public const int Room_UpdateMemberCustomData_Client = 130;
        public const int Room_UpdateMemberCustomData_Broadcast = 131;
        public const int Room_UpdateMemberCustomData_Server = 132;

        public const int Room_UpdateRoomCustomData_Client = 133;
        public const int Room_UpdateRoomCustomData_Broadcast = 134;
        public const int Room_UpdateRoomCustomData_Server = 135;

//        public const int Room_EnterRandomRoom_Client = 136;
//        public const int Room_EnterRandomRoom_Server = 137;

        public const int Room_MatchOpponent_Client = 138;
        public const int Room_MatchOpponent_Server = 139;
        public const int Room_OtherLoseConnection_Broadcast = 140;
//        public const int Room_NextTurn_Broadcast = 141;
//        public const int Room_CompleteBattle_Client = 142;
//        public const int Room_CompleteBattle_Broadcast = 143;
        public const int Room_CancelMatchOpponent_Client = 144;
        public const int Room_CancelMatchOpponent_Server = 145;

        public const int Network_Offline_Notice = 90001;
        public const int Network_Lock_Notice = 90002;
        public const int Network_UnLock_Notice = 90003;
	}
}

