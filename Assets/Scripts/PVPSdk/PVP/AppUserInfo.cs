using System;

using System.Collections.Generic;

namespace PVPSdk
{
    /// <summary>
    /// 应用里面的用户信息
    /// </summary>
    public class AppUserInfo {
        public uint uid {
            get;
            private set;
        }
        public int custom_data_number {
            get;
            private set;
        }
        public Dictionary<string, byte[]> custom_data {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PVPSdk.AppUserInfo"/> class.
        /// </summary>
        public AppUserInfo (PVPProtobuf.AppUserInfo u) {
            this.uid = u.uid;
            this.custom_data_number = u.custom_data_number;
            this.custom_data = new Dictionary<string, byte[]> ();
            foreach (PVPProtobuf.Pair item in u.custom_data) {
                this.custom_data [item.key] = item.value;
            }
        }
    }
}

