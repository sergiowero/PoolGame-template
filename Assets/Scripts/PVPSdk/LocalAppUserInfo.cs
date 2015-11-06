using System;
using PVPProtobuf;
using System.Collections;
using System.Collections.Generic;

namespace PVPSdk
{
    
    public class LocalAppUserInfo : AppUserInfo
    {
        public LocalAppUserInfo(PVPProtobuf.AppUserInfo u) : base(u){
        }

        /// <summary>
        /// 进入大厅
        /// </summary>
        /// <returns><c>true</c>, if lobby was entered, <c>false</c> otherwise.</returns>
        public bool OpEnterLobby(int lobbyId, float timeout = 5){
            return PVP.ICM.internetClient.EnterLobby (lobbyId);
        }

        public bool OpUpdateAppUserInfo (int level = -1, Int64 score = -1, int winTimes = -1, int loseTimes = -1, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, float timeout = 5){
            if (level < this.level && score < this.score && winTimes < this.winTimes && loseTimes < this.loseTimes && (being_deleted_data == null || being_deleted_data.Count == 0) && (being_updated_data == null || being_updated_data.Count == 0)) {
                return false;
            }

            return PVP.ICM.internetClient.UpdateAppUserInfo (level, score, winTimes, loseTimes, being_updated_data, being_deleted_data,check_data, check_data_not_exits, timeout);
        }


        public bool OpUpdateAppUserCustomData (Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, float timeout = 5)
        {
            if (PVPGlobal.localAppUserInfo == null) {
                return false;
            }
            if ((being_deleted_data == null || being_deleted_data.Count == 0) && (being_updated_data == null || being_updated_data.Count == 0)) {
                return false;
            }
            return PVP.ICM.internetClient.UpdateAppUserCustomData (being_updated_data, being_deleted_data, check_data, check_data_not_exits, timeout);
        }

    }
}

