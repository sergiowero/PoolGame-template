using System;

namespace PVPSdk
{
	/// <summary>
	/// 用户的全局信息
	/// </summary>
	public class UserInfo
	{
        public uint uid{ get; private set;}
        public string name{ get;private set;}
        public LoginType loginType;

        public UserInfo (PVPProtobuf_Token.User_LoginOrRegister_Res userInfo) {
            this.uid = userInfo.uid;
            this.name = userInfo.name;
            this.loginType = (LoginType)userInfo.login_type;
		}
	}
}

