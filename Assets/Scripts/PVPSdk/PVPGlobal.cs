using UnityEngine;
using System;
using System.Collections.Generic;

namespace PVPSdk {
    public sealed class PVPGlobal
    {
        /// <summary>
        /// sdk 版本号
        /// </summary>
        public static string VERSION = "0.1.4";
//        public static string appKey = "";



        /// <summary>
        /// 客户端是否已经登陆
        /// </summary>
        public static bool isLogined = false;

        public static User userInfo;
        public static LobbyInfo lobbyInfo;
        public static RoomInfo roomInfo;

        /// <summary>
        /// 用户自己的信息
        /// </summary>
        public static LocalAppUserInfo localAppUserInfo;

        /// <summary>
        /// 应用内其他玩家的信息
        /// </summary>
        public static Dictionary<UInt32, AppUserInfo> appUserInfos = new Dictionary<uint, AppUserInfo> ();

        public static bool isDebug = false;

        public static PVP.HandlerRegister handlerRegister{
            get{
                return PVP.ICM.handlerRegister;
            }
        }

        public static void Init(string appKey){
            PVP.Config.appKey = appKey;
        }
    }
}

