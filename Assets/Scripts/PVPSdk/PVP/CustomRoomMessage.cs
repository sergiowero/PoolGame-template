using System;


namespace PVPSdk
{
    public class RoomNewMessage
    {

        public uint from_uid {
            get;
            private set;
        }

        public int custom_command_id {
            get;
            private set;
        }

        public byte[] message {
            get;
            private set;
        }



        public RoomNewMessage (PVPProtobuf.Room_NewMessage_Broadcast meta) {
            this.from_uid = meta.from_uid;
            this.custom_command_id = meta.custom_command_id;
            this.message = meta.message;
        }
    }
}

