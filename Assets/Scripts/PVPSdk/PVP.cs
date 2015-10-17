using UnityEngine;

namespace PVPSdk {
    public sealed class PVP
    {
        /// <summary>
        /// sdk 版本号
        /// </summary>
        public const string Version = "0.1.1";
        public static string appKey = "";

        public static int PROTO_VERSION = 1;

        /// <summary>
        /// 客户端是否已经登陆
        /// </summary>
        public static bool isLogined = false;

        public static UserInfo userInfo;
        public static LobbyInfo lobbyInfo;
        public static RoomInfo roomInfo;
        public static AppUserInfo appUserInfo;

        public static bool isDebug = false;

        private static Client _client = null;
        public static Client client{
            get{
                if (_client == null) {
                    GameObject go = new GameObject ();
                    _client = go.AddComponent<Client> ();
                    MonoBehaviour.DontDestroyOnLoad (go);
                }
                return _client;
            }
        }

        public static void Init(string appKey){
            PVP.appKey = appKey;
        }
    }
}

