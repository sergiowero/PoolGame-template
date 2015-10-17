using System;
using UnityEngine;
using ProtoBuf;

namespace PVPSdk
{
	public class ProtobufMeta<T> : AbstractProtobufMeta where T: global::ProtoBuf.IExtensible
	{
		public int error_code;
		public T protbufMeta;

	}
}

