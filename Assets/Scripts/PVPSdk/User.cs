using System;

using PVPProtobuf_Token;
using UnityEngine;

namespace PVPSdk
{
    /// <summary>
    /// 用户的全局信息
    /// </summary>
    public class User
    {
//        public enum LoginType
//        {
//            Account = 1,
//            Guest = 2,
//            Facebook = 3,
//        }
//
//        public enum DeviceType
//        {
//            Android = 1,
//            IOS = 2,
//            OTHER = 99,
//        }

        public uint uid{ get; private set; }

        public string name{ get; private set; }

        public PVP.LoginType loginType;

        public User (PVPProtobuf_Token.User_LoginOrRegister_Response userInfo)
        {
            this.uid = userInfo.uid;
            this.name = userInfo.name;
            this.loginType = (PVP.LoginType)userInfo.login_type;
        }

        /// <summary>
        /// 登出，清除本地数据
        /// </summary>
        public void LoginOut ()
        {
            PVPGlobal.isLogined = false;
            PVPGlobal.lobbyInfo = null;
            PVPGlobal.roomInfo = null;
            PVPGlobal.userInfo = null;
            PVPGlobal.localAppUserInfo = null;
            PVP.ICM.internetClient.Close ();
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="loginType">Login type.</param>
        /// <param name="account">Account.</param>
        /// <param name="password">Password.</param>
        /// <param name="facebookAccessToken">Facebook access token.</param>
        /// <param name="timeout">Timeout.</param>
        public static void OpLogin (PVP.LoginType loginType, string account = "", string password = "", string facebookAccessToken = "", float timeout = 5)
        {
            PVPGlobal.isLogined = false;

            PVP.ICM.internetClient.Login( loginType, account, password, facebookAccessToken, timeout);

            if (PVPGlobal.handlerRegister.lockSocketEventHandler != null) {
                PVPGlobal.handlerRegister.lockSocketEventHandler ();
            }
        }

        public bool OpConnectServer (){
            return PVP.ICM.internetClient.ConnectServer ();
        }

        public bool GetLobbyList (float timeout = 5){
            return PVP.ICM.internetClient.GetLobbyList ();
        }
    }
}