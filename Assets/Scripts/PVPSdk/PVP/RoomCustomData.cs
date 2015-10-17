using System;
using System.Collections.Generic;
using PVPProtobuf;

namespace PVPSdk
{
    public class RoomCustomData
    {
        public int custom_data_number{ get; private set;}
        public Dictionary<string, byte[]> custom_data {
            get;private set;
        }

        public RoomCustomData (Room_UpdateRoomCustomData_Broadcast custom_data ) {
            this.custom_data_number = custom_data.custom_data_number;
            this.custom_data = new Dictionary<string, byte[]> ();
            foreach (Pair item in custom_data.custom_data) {
                this.custom_data [item.key] = item.value;
            }
        }

        public RoomCustomData (Room_UpdateRoomCustomData_Server custom_data ) {
            this.custom_data_number = custom_data.custom_data_number;
            this.custom_data = new Dictionary<string, byte[]> ();
            foreach (Pair item in custom_data.custom_data) {
                this.custom_data [item.key] = item.value;
            }
        }
    }
}

