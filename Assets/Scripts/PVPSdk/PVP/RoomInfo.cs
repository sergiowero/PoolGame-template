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
            public string name;
            public int custom_data_number;
            public Dictionary<string, byte[]> custom_data;

            public MemberInfo (PVPProtobuf.RoomInfo.MemberInfo member) {
                this.uid = member.uid;
                this.name = member.name;
                this.custom_data_number = member.custom_data_number;
                custom_data = new Dictionary<string, byte[]>();
                foreach (PVPProtobuf.Pair item in member.custom_data) {
                    custom_data [item.key] = item.value;
                }
            }


            public void UpdateCustomData(MemberCustomData c){
                this.custom_data_number = c.custom_data_number;
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
        public Dictionary<string, byte[]> custom_data {
            get;
            private set;
        }
        public int custom_data_number {
            get;
            private set;
        }
        public int room_data_number {
            get;
            private set;
        }
//        private float _server_time ;
//        public float round_duration;
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
            this.custom_data = new Dictionary<string, byte[]> ();
            this.custom_data_number = room_info.custom_data_number;
            foreach (PVPProtobuf.Pair item in room_info.custom_data) {
                this.custom_data [item.key] = item.value;
            }
            member_infos = new Dictionary<uint, MemberInfo> ();

            foreach (PVPProtobuf.RoomInfo.MemberInfo member in room_info.member_infos) {
                MemberInfo member_info = new MemberInfo (member);
                Debug.LogError (member_info.name);
                member_infos [member.uid] = member_info;
			}
            this.random_sequence = new List<uint> ();
            this.random_sequence.AddRange (room_info.random_sequence);
		}

        public byte[] GetCustomDataByKey(string key){
            if (this.custom_data != null && this.custom_data.ContainsKey(key)) {
                return this.custom_data [key];
            }else{
                return null;
            }
        }

        public void UpdateCustomData(RoomCustomData c){
            this.custom_data_number = c.custom_data_number;
            this.custom_data.Clear ();
            foreach(KeyValuePair<String, byte[]> item in c.custom_data){// i=0;i<c.custom_data.Count;i++){
                this.custom_data[item.Key] = item.Value;
            }
        }



    }
}