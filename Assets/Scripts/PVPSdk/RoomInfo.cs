using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PVPProtobuf;
using System.IO;
using ProtoBuf;
using PVPProtobuf_Token;

namespace PVPSdk {
    
	public class RoomInfo {
        public class MemberInfo {

            public uint uid;
            public uint customDataNumber;
            public Dictionary<string, byte[]> custom_data;

            public MemberInfo (PVPProtobuf.RoomInfo.MemberInfo member) {
                this.uid = member.uid;
                this.customDataNumber = member.custom_data_number;
                custom_data = new Dictionary<string, byte[]>();
                foreach (PVPProtobuf.Pair item in member.custom_data) {
                    custom_data [item.key] = item.value;
                }
            }

            /// <summary>
            /// 内部调用
            /// </summary>
            /// <param name="c">C.</param>
            internal void OnUpdateCustomData(PVP.MemberCustomData c){
                this.customDataNumber = c.customDataNumber;
                this.custom_data.Clear ();
                foreach(KeyValuePair<String, byte[]> item in c.custom_data){// i=0;i<c.custom_data.Count;i++){
                    this.custom_data[item.Key] = item.Value;
                }
            }
        }

		public int room_id;
		public string name;
        public int max_fighter_number {
            get;
            private set;
        }
        public int fighter_number {
            get;
            private set;
        }
        public int max_spectator_number {
            get;
            private set;
        }
        public int spectator_number {
            get;
            private set;
        }
        public bool is_open {
            get;
            private set;
        }
        public bool is_visible {
            get;
            private set;
        }
        public Dictionary<uint, MemberInfo> member_infos {
            get;
            private set;
        }
        public MemberInfo masterMember {
            get;
            private set;
        }
        public Dictionary<string, byte[]> customData {
            get;
            private set;
        }
        public uint customDataNumber {
            get;
            private set;
        }
        public uint room_data_number {
            get;
            private set;
        }

        public List<uint> random_sequence {
            get;
            private set;
        }

        public RoomInfo(PVPProtobuf.RoomInfo room_info) {
			this.room_id = room_info.room_id;
			this.name = room_info.name;
            this.max_fighter_number = room_info.max_fighter_number;
            this.fighter_number = room_info.fighter_number;
            this.max_spectator_number = room_info.max_spectator_number;
            this.spectator_number = room_info.spectator_number;
			this.is_open = room_info.is_open;
			this.is_visible = room_info.is_visible;
            this.customData = new Dictionary<string, byte[]> ();
            this.customDataNumber = room_info.custom_data_number;
            foreach (PVPProtobuf.Pair item in room_info.custom_data) {
                this.customData [item.key] = item.value;
            }
            member_infos = new Dictionary<uint, MemberInfo> ();

            foreach (PVPProtobuf.RoomInfo.MemberInfo member in room_info.member_infos) {
                MemberInfo member_info = new MemberInfo (member);
                member_infos [member.uid] = member_info;
			}
            this.random_sequence = new List<uint> ();
            this.random_sequence.AddRange (room_info.random_sequence);
		}

        public byte[] GetCustomDataByKey(string key){
            if (this.customData != null && this.customData.ContainsKey(key)) {
                return this.customData [key];
            }else{
                return null;
            }
        }

        internal void OnUpdateCustomData(PVP.NewCustomData c){
            this.customDataNumber = c.customDataNumber;
            foreach(KeyValuePair<String, byte[]> item in c.new_data){// i=0;i<c.custom_data.Count;i++){
                this.customData[item.Key] = item.Value;
            }
            for (int i = 0; i < c.deleted_data.Count; i++) {
                if (this.customData.ContainsKey (c.deleted_data [i])) {
                    this.customData.Remove (c.deleted_data [i]);
                }
            }
        }


        /// <summary>
        /// 在房间内发送消息
        /// </summary>
        /// <param name="command_id">自定义的消息分类</param>
        /// <param name="message">消息内容</param>
        /// <param name="target_type">接收消息的玩家类型</param>
        public bool OpSendNewMessage (int command_id, byte[] message, int target_type)
        {
            return PVP.ICM.internetClient.SendNewMessage (command_id, message, target_type);
        }

        public bool OpUpdateMemberCustomData (PVP.UpdateCustomDataRange update_range, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, float timeout = 5)
        {
            return PVP.ICM.internetClient.UpdateMemberCustomData (update_range, being_updated_data, being_deleted_data, check_data, check_data_not_exits, timeout);
        }

        public bool OpUpdateRoomCustomData (PVP.UpdateCustomDataRange update_range, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, float timeout = 5)
        {
            return PVP.ICM.internetClient.UpdateRoomCustomData (update_range, being_updated_data, being_deleted_data, check_data, check_data_not_exits, timeout);
        }


        public bool OpLeaveRoom (float timeout = 5){
            return PVP.ICM.internetClient.LeaveRoom ();
        }


    }
}