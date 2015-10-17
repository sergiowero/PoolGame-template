namespace PVPSdk
{
    public class ErrorCode
    {
        public const int SUCCESS = 0;

        public const int UNKNOWN = 1;
        public const int SERVICE_ERROR = 2;
        public const int METHOD_UNKNOWN = 3;
        public const int TOO_MANY_CALLS = 4;
        public const int BAD_IP = 5;
        public const int DB_ERROR = 6;
        public const int IS_CHEAT = 7;
        public const int NEED_LOGIN = 8;
        public const int ACCOUNT_OR_PASSWORD_ERROR = 9;
        public const int DATA_ERROR = 11;
        public const int CP_LIMIT = 12;
        public const int VERSION_IS_OLD = 13;
        public const int APP_NOT_EXIST = 14;

        public const int PARAM_ERROR = 100;
        public const int PARAM_TOKEN = 102;
        public const int INVALID_MOBILE = 103;
        public const int INVALID_EMAIL = 104;
        public const int CAN_NOT_REQUEST = 106;

        public const int LOBBY_GET_LIST_ERROR = 200;
        public const int LOBBY_NOT_IN_LOBBY_ERROR = 201;

        public const int USER_NO_EXIST = 10001;
        public const int USER_PASSWORD_ERROR = 10002;
        public const int USER_ACCOUNT_REPEATED = 10003;
        public const int USER_STATE_NOT_NORMAL = 10004;
        public const int USER_PASSWORD_FORMAT_ERROR = 10005;




        public const int ROOM_NOT_EXIST = 11001;
        public const int ROOM_CACHE_MESSAGE_NUMBER_NOT_EXIST = 11002;
        public const int ROOM_NOT_IN_ROOM_ERROR = 11003;
        public const int ROOM_TARGET_UID_NOT_IN_ROOM_ERROR = 11004;
        public const int ROOM_NO_MORE_ROOM_ERROR = 11005;
        public const int ROOM_UPDATE_MEMBER_CHECK_DATA_NOT_PASS = 11006;
        public const int ROOM_UPDATE_ROOM_CHECK_DATA_NOT_PASS = 11007;


        //客户端直接反馈的错误
        public const int RESPONSE_TIME_OUT = 90000;
    }
}

