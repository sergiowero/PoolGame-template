using System;
using System.Collections.Generic;
using PVPProtobuf;

namespace PVPSdk.PVP
{
    /// <summary>
    /// 更新自定义数据，包括用户名称，
    /// </summary>
    public class NewCustomData
    {
        public uint member_uid { get; private set; }
        public uint customDataNumber{ get; private set;}
        public Dictionary<string, byte[]> new_data {
            get;private set;
        }

        public List<string> deleted_data { get; private set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="PVPSdk.RoomBroadcastCustomData"/> class.
        /// </summary>
        /// <param name="custom_data_number">Custom data number.</param>
        /// <param name="new_data">New data.</param>
        /// <param name="deleted_data">Deleted data.</param>
        public NewCustomData (uint member_uid, uint customDataNumber, List<Pair> new_data, List<string> deleted_data ) {
            this.member_uid = member_uid;
            this.customDataNumber = customDataNumber;
            this.new_data = new Dictionary<string, byte[]> ();
            foreach (Pair item in new_data) {
                this.new_data [item.key] = item.value;
            }
            this.deleted_data = new List<string> ();
            this.deleted_data.AddRange (deleted_data);
        }
    }
}

