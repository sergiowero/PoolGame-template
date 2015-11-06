using System;
using UnityEngine;

namespace PVPSdk
{
    public class LobbyInfo
    {
        public int lobby_id {
            get;
            private set;
        }

        public string name {
            get;
            private set;
        }

        public LobbyInfo (int lobby_id, string name)
        {
            this.lobby_id = lobby_id;
            this.name = name;
        }

        public LobbyInfo(PVPProtobuf.LobbyInfo lobby_info){
            this.lobby_id = lobby_info.lobby_id;
            this.name = lobby_info.name;
        }

        public bool OpMatchOpponent(int standard, int range = 0, float waitForMatch = 10){
            return PVP.ICM.internetClient.MatchOpponent (standard, range, waitForMatch);
        }

        public bool OpCancelMatchOpnnent (float timeout = 5){
            Debug.Log ("OpCancelMatchOpnnent");
            return PVP.ICM.internetClient.CancelMatchOpnnent (timeout);
        }


    }
}

