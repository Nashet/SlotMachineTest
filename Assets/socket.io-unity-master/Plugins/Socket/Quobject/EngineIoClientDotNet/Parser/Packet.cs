using Socket.Quobject.EngineIoClientDotNet.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Socket.Quobject.EngineIoClientDotNet.Parser
{
	public class Packet
	{
		private bool SupportsBinary = false;
		public static readonly string OPEN = "open";
		public static readonly string CLOSE = "close";
		public static readonly string PING = "ping";
		public static readonly string PONG = "pong";
		public static readonly string UPGRADE = "upgrade";
		public static readonly string MESSAGE = "message";
		public static readonly string NOOP = "noop";
		public static readonly string ERROR = "error";
		private static readonly int MAX_INT_CHAR_LENGTH = int.MaxValue.ToString().Length;

		private static readonly Dictionary<string, byte> _packets = new Dictionary<string, byte>() {
	  {
		Packet.OPEN,
		 0
	  }, {
		Packet.CLOSE,
		 1
	  }, {
		Packet.PING,
		 2
	  }, {
		Packet.PONG,
		 3
	  }, {
		Packet.MESSAGE,
		 4
	  }, {
		Packet.UPGRADE,
		 5
	  }, {
		Packet.NOOP,
		 6
	  }
	};

		private static readonly Dictionary<byte, string> _packetsList = new Dictionary<byte, string>();
		private static readonly Packet _err = new Packet(Packet.ERROR, "parser error");

		static Packet()
		{
			foreach (KeyValuePair<string, byte> packet in Packet._packets)
				Packet._packetsList.Add(packet.Value, packet.Key);
		}

		public string Type { get; set; }

		public object Data { get; set; }

		public Packet(string type, object data)
		{
			this.Type = type;
			this.Data = data;
		}

		public Packet(string type)
		{
			this.Type = type;
			this.Data = null;
		}

		internal void Encode(IEncodeCallback callback, bool utf8encode = false)
		{
			if (this.Data is byte[])
			{
				if (!this.SupportsBinary)
					this.EncodeBase64Packet(callback);
				else
					this.EncodeByteArray(callback);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Packet._packets[this.Type]);
				if (this.Data != null)
					stringBuilder.Append(utf8encode ? UTF8.Encode((string)this.Data) : (string)this.Data);
				callback.Call(stringBuilder.ToString());
			}
		}

		private void EncodeBase64Packet(IEncodeCallback callback)
		{
			byte[] data = this.Data as byte[];
			if (data == null)
				throw new Exception("byteData == null");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("b");
			stringBuilder.Append(Packet._packets[this.Type]);
			stringBuilder.Append(Convert.ToBase64String(data));
			callback.Call(stringBuilder.ToString());
		}

		private void EncodeByteArray(IEncodeCallback callback)
		{
			byte[] data = this.Data as byte[];
			if (data == null)
				throw new Exception("byteData == null");
			byte[] numArray = new byte[1 + data.Length];
			numArray[0] = Packet._packets[this.Type];
			Array.Copy(data, 0, numArray, 1, data.Length);
			callback.Call(numArray);
		}

		internal static Packet DecodePacket(string data, bool utf8decode = false)
		{
			if (data.StartsWith("b"))
				return Packet.DecodeBase64Packet(data.Substring(1));
			int result;
			if (!int.TryParse(data.Substring(0, 1), out result))
				result = -1;
			if (utf8decode)
			{
				try
				{
					data = UTF8.Decode(data);
				}
				catch (Exception)
				{
					return Packet._err;
				}
			}

			if (result < 0 || result >= Packet._packetsList.Count)
				return Packet._err;
			if (data.Length > 1)
				return new Packet(Packet._packetsList[(byte)result], data.Substring(1));
			return new Packet(Packet._packetsList[(byte)result], null);
		}

		private static Packet DecodeBase64Packet(string msg)
		{
			int result;
			if (!int.TryParse(msg.Substring(0, 1), out result))
				result = -1;
			if (result < 0 || result >= Packet._packetsList.Count)
				return Packet._err;
			msg = msg.Substring(1);
			byte[] numArray = Convert.FromBase64String(msg);
			return new Packet(Packet._packetsList[(byte)result], numArray);
		}

		internal static Packet DecodePacket(byte[] data)
		{
			int num = data[0];
			byte[] numArray = new byte[data.Length - 1];
			Array.Copy(data, 1, numArray, 0, numArray.Length);
			return new Packet(Packet._packetsList[(byte)num], numArray);
		}

		internal static void EncodePayload(Packet[] packets, IEncodeCallback callback)
		{
			if (packets.Length == 0)
			{
				callback.Call(new byte[0]);
			}
			else
			{
				List<byte[]> results = new List<byte[]>(packets.Length);
				Packet.EncodePayloadCallback encodePayloadCallback = new Packet.EncodePayloadCallback(results);
				foreach (Packet packet in packets)
					packet.Encode(encodePayloadCallback, true);
				callback.Call(Buffer.Concat(results.ToArray()));
			}
		}

		public static void DecodePayload(string data, IDecodePayloadCallback callback)
		{
			if (string.IsNullOrEmpty(data))
			{
				callback.Call(Packet._err, 0, 1);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				int startIndex = 0;
				for (int length = data.Length; startIndex < length; ++startIndex)
				{
					char ch = Convert.ToChar(data.Substring(startIndex, 1));
					if (ch != ':')
					{
						stringBuilder.Append(ch);
					}
					else
					{
						int result;
						if (!int.TryParse(stringBuilder.ToString(), out result))
						{
							callback.Call(Packet._err, 0, 1);
							return;
						}

						string data1;
						try
						{
							data1 = data.Substring(startIndex + 1, result);
						}
						catch (ArgumentOutOfRangeException)
						{
							callback.Call(Packet._err, 0, 1);
							return;
						}

						if ((uint)data1.Length > 0U)
						{
							Packet packet = Packet.DecodePacket(data1, true);
							if (Packet._err.Type == packet.Type && Packet._err.Data == packet.Data)
							{
								callback.Call(Packet._err, 0, 1);
								return;
							}

							if (!callback.Call(packet, startIndex + result, length))
								return;
						}

						startIndex += result;
						stringBuilder = new StringBuilder();
					}
				}

				if (stringBuilder.Length <= 0)
					return;
				callback.Call(Packet._err, 0, 1);
			}
		}

		public static void DecodePayload(byte[] data, IDecodePayloadCallback callback)
		{
			ByteBuffer byteBuffer = ByteBuffer.Wrap(data);
			List<object> objectList = new List<object>();
			int num1;
			int num2;
			for (int index = 0; byteBuffer.Capacity - index > 0; index = num1 + (num2 + 1))
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag1 = (byteBuffer.Get(index) & byte.MaxValue) == 0;
				bool flag2 = false;
				int num3 = 1;
				while (true)
				{
					int num4 = byteBuffer.Get(num3 + index) & byte.MaxValue;
					if (num4 != byte.MaxValue)
					{
						if (stringBuilder.Length <= Packet.MAX_INT_CHAR_LENGTH)
						{
							stringBuilder.Append(num4);
							++num3;
						}
						else
							break;
					}
					else
						goto label_7;
				}

				flag2 = true;
				label_7:
				if (flag2)
				{
					callback.Call(Packet._err, 0, 1);
					return;
				}

				num1 = index + (stringBuilder.Length + 1);
				num2 = int.Parse(stringBuilder.ToString());
				byteBuffer.Position(1 + num1);
				byteBuffer.Limit(num2 + 1 + num1);
				byte[] numArray = new byte[byteBuffer.Remaining()];
				byteBuffer.Get(numArray, 0, numArray.Length);
				if (flag1)
					objectList.Add(Packet.ByteArrayToString(numArray));
				else
					objectList.Add(numArray);
				byteBuffer.Clear();
				byteBuffer.Position(num2 + 1 + num1);
			}

			int count = objectList.Count;
			for (int index = 0; index < count; ++index)
			{
				object obj = objectList[index];
				if (obj is string)
					callback.Call(Packet.DecodePacket((string)obj, true), index, count);
				else if (obj is byte[])
					callback.Call(Packet.DecodePacket((byte[])obj), index, count);
			}
		}

		internal static byte[] StringToByteArray(string str)
		{
			int length = str.Length;
			byte[] numArray = new byte[length];
			for (int index = 0; index < length; ++index)
				numArray[index] = (byte)str[index];
			return numArray;
		}

		internal static string ByteArrayToString(byte[] bytes)
		{
			return Encoding.ASCII.GetString(bytes);
		}

		private class EncodePayloadCallback : IEncodeCallback
		{
			private readonly List<byte[]> _results;

			public EncodePayloadCallback(List<byte[]> results)
			{
				this._results = results;
			}

			public void Call(object data)
			{
				if (data is string)
				{
					string str1 = (string)data;
					string str2 = str1.Length.ToString();
					byte[] numArray = new byte[str2.Length + 2];
					numArray[0] = 0;
					for (int startIndex = 0; startIndex < str2.Length; ++startIndex)
						numArray[startIndex + 1] = byte.Parse(str2.Substring(startIndex, 1));
					numArray[numArray.Length - 1] = byte.MaxValue;
					this._results.Add(Buffer.Concat(new byte[2][] {
			numArray,
			Packet.StringToByteArray(str1)
		  }));
				}
				else
				{
					byte[] numArray1 = (byte[])data;
					string str = numArray1.Length.ToString();
					byte[] numArray2 = new byte[str.Length + 2];
					numArray2[0] = 1;
					for (int startIndex = 0; startIndex < str.Length; ++startIndex)
						numArray2[startIndex + 1] = byte.Parse(str.Substring(startIndex, 1));
					numArray2[numArray2.Length - 1] = byte.MaxValue;
					this._results.Add(Buffer.Concat(new byte[2][] {
			numArray2,
			numArray1
		  }));
				}
			}
		}
	}
}