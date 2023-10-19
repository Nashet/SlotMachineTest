namespace Socket.WebSocket4Net.Protocol.FramePartReader
{
	internal class MaskKeyReader : DataFramePartReader
	{
		public override int Process(
		  int lastLength,
		  WebSocketDataFrame frame,
		  out IDataFramePartReader nextPartReader)
		{
			int lastLength1 = lastLength + 4;
			if (frame.Length < lastLength1)
			{
				nextPartReader = this;
				return -1;
			}

			frame.MaskKey = frame.InnerData.ToArrayData(lastLength, 4);
			if (frame.ActualPayloadLength == 0L)
			{
				nextPartReader = null;
				return (int)(frame.Length - (long)lastLength1);
			}

			nextPartReader = new PayloadDataReader();
			if (frame.Length > lastLength1)
				return nextPartReader.Process(lastLength1, frame, out nextPartReader);
			return 0;
		}
	}
}