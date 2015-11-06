using System;
using System.Collections;
using System.Collections.Generic;
using PVPProtobuf;

namespace PVPSdk.PVP {
    public class UpdateAppUserInfoResult {
        public int level {
            get;
            private set;
        }

        public Int64 score {
            get;
            private set;
        }

        public int winTimes {
            get;
            private set;
        }
        public int loseTimes {
            get;
            private set;
        }

        public uint number { get; private set; }

        public uint customDataNumber{ get; private set;}

        public Dictionary<string, byte[]> updatedData {
            get;private set;
        }

        public List<string> deletedData { get; private set; }

        public UpdateAppUserInfoResult(int errorCode, PVPProtobuf.Appuser_UpdateInfo_Response r){
            this.level = r.level;
            this.score = r.score;
            this.winTimes = r.win_times;
            this.loseTimes = r.lose_times;
            this.number = r.number;
            this.customDataNumber = r.custom_data_number;
            this.updatedData = new Dictionary<string, byte[]> ();
            this.deletedData = new List<string> ();
            if (errorCode == ErrorCode.SUCCESS) {
                
                foreach (Pair item in r.updated_data) {
                    this.updatedData [item.key] = item.value;
                }

                this.deletedData.AddRange (r.deleted_data);
            } else {
                foreach (Pair item in r.check_data) {
                    this.updatedData [item.key] = item.value;
                }
                this.deletedData.AddRange (r.check_data_not_exist);
            }
        }

    }
}

