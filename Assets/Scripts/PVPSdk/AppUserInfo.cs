using System;

using System.Collections.Generic;
using PVPProtobuf;

namespace PVPSdk
{

//
//    public class AppUserCustomDataMeta{
//        public Dictionary<string, byte[]> _data {
//            get;
//            private set;
//        }
//    }

    /// <summary>
    /// 应用里面的用户信息
    /// </summary>
    public class AppUserInfo {
        public uint uid {
            get;
            protected set;
        }
        public uint customDataNumber {
            get;
            protected set;
        }
        public Dictionary<string, byte[]> customData {
            get;
            protected set;
        }

        public string name {
            get;
            protected set;
        }

        public string avatar {
            get;
            protected set;
        }

        public int level {
            get;
            protected set;
        }

        public Int64 score {
            get;
            protected set;
        }

        public int winTimes{
            get;
            protected set;
        }

        public int loseTimes {
            get;
            protected set;
        }

        public UInt32 number {
            get;
            protected set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PVPSdk.AppUserInfo"/> class.
        /// </summary>
        internal AppUserInfo (PVPProtobuf.AppUserInfo u) {
            this.uid = u.uid;
            this.avatar = u.avatar;
            this.level = u.level;
            this.score = u.score;
            this.loseTimes = u.lose_times;
            this.winTimes = u.win_times;
            this.name = u.name;
            this.number = u.number;
            this.customDataNumber = u.custom_data_number;
            this.customData = new Dictionary<string, byte[]> ();
            foreach (PVPProtobuf.Pair item in u.custom_data) {
                this.customData [item.key] = item.value;
            }
        }

        internal void FillAppUserInfo (PVPProtobuf.AppUserInfo u){
            this.uid = u.uid;
            this.avatar = u.avatar;
            this.level = u.level;
            this.score = u.score;
            this.loseTimes = u.lose_times;
            this.winTimes = u.win_times;
            this.name = u.name;
            this.number = u.number;
            this.customDataNumber = u.custom_data_number;
            this.customData.Clear ();
            foreach (PVPProtobuf.Pair item in u.custom_data) {
                this.customData [item.key] = item.value;
            }
        }

        internal void OnUpdateCustomData (uint customDataNumber,  List<Pair> updatedData, List<string> deletedData){
            this.customDataNumber = customDataNumber;
            for (int i = 0; i < updatedData.Count; i++) {
                this.customData [updatedData [i].key] = updatedData [i].value;
            }

            for (int i = 0; i > deletedData.Count; i++) {
                if (this.customData.ContainsKey (deletedData [i])) {
                    this.customData.Remove (deletedData [i]);
                }
            }
        }

        internal void OnUpdateInfo(PVP.UpdateAppUserInfoResult r){
            if (r.level >= 0) {
                this.level = r.level;
            }
            if (r.score >= 0) {
                this.score = r.score;
            }

            if (r.winTimes >= 0) {
                this.winTimes = r.winTimes;
            }

            if (r.loseTimes >= 0) {
                this.loseTimes = r.loseTimes;
            }

            if (r.number > 0) {
                this.number = r.number;
            }

            if (r.customDataNumber > 0) {
                this.customDataNumber = r.customDataNumber;
            }

            if (this.customDataNumber < r.customDataNumber) {
                this.customDataNumber = customDataNumber;
            }

            foreach(KeyValuePair<string, byte[]> item in r.updatedData){
                this.customData [item.Key] = item.Value;
            }

            for (int i = 0; i > r.deletedData.Count; i++) {
                if (this.customData.ContainsKey (r.deletedData [i])) {
                    this.customData.Remove (r.deletedData [i]);
                }
            }
        }
    }
}

