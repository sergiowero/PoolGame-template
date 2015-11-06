using System;
using System.Collections.Generic;
using PVPProtobuf;

namespace PVPSdk.PVP
{
    public class MemberCustomData
    {
        public uint customDataNumber{ get; private set;}
        public uint member_uid {
            get;
            private set;
        }
        public Dictionary<string, byte[]> custom_data {
            get;private set;
        }

        public MemberCustomData (Room_UpdateMemberCustomData_Broadcast custom_data )
        {
            this.member_uid = custom_data.member_uid;
            this.customDataNumber = custom_data.custom_data_number;
            this.custom_data = new Dictionary<string, byte[]> ();
            foreach (Pair item in custom_data.updated_data) {
                this.custom_data [item.key] = item.value;
            }

            foreach (string key in custom_data.deleted_data) {
                if (this.custom_data.ContainsKey (key)) {
                    this.custom_data.Remove (key);
                }
            }
        }
    }
}

