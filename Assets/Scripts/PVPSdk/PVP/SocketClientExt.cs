using System;
using System.IO;
using UnityEngine;

namespace PVPSdk.PVP
{
	public static class SocketClientExt
	{
		public static byte [] SubArray (this byte [] array, int startIndex, int length)
		{
			if (array == null || array.Length == 0)
				return new byte[0];

			if (startIndex < 0 || length <= 0)
				return new byte [0];

			if (startIndex + length > array.Length)
				return new byte [0];

			if (startIndex == 0 && array.Length == length)
				return array;

			byte [] subArray = new byte [length];
			Array.Copy (array, startIndex, subArray, 0, length);

			return subArray;
		}

		public static byte[] CheckBigEndian(this byte[] array){
			if (BitConverter.IsLittleEndian) {
				Array.Reverse (array);
			}
			return array;
		}

		/// <summary>
		/// 在原来的数据上操作
		/// </summary>
		/// <param name="array">Array.</param>
		public static byte[] Reverse(this byte[] array){
			Array.Reverse (array);
			return array;
		}

		/// <summary>
		/// Reads the bytes.
		/// </summary>
		/// <returns>The bytes.</returns>
		/// <param name="stream">Stream.</param>
		/// <param name="buffer">Buffer.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="length">Length.</param>
		public static byte [] ReadBytes (this System.IO.Stream stream, byte [] buffer, int offset, int length)
		{
			Debug.LogError ("stream.Position");
			Debug.LogError (stream.Position);
			var len = stream.Read (buffer, offset, length);
			if (len < 1)
				return buffer.SubArray (0, offset);

			var tmp = 0;
			while (len < length) {
				tmp = stream.Read (buffer, offset + len, length - len);
				if (tmp < 1)
					break;

				len += tmp;
			}

			return len < length
				? buffer.SubArray (0, offset + len)
					: buffer;
		}

	}

}

