namespace Socket.WebSocket4Net.Protocol.FramePartReader
{
	internal abstract class DataFramePartReader : IDataFramePartReader
	{
		static DataFramePartReader()
		{
			DataFramePartReader.FixPartReader =
			   new WebSocket4Net.Protocol.FramePartReader.FixPartReader();
			DataFramePartReader.ExtendedLenghtReader =
			   new WebSocket4Net.Protocol.FramePartReader.ExtendedLenghtReader();
			DataFramePartReader.MaskKeyReader =
			   new WebSocket4Net.Protocol.FramePartReader.MaskKeyReader();
			DataFramePartReader.PayloadDataReader =
			   new WebSocket4Net.Protocol.FramePartReader.PayloadDataReader();
		}

		public abstract int Process(
		  int lastLength,
		  WebSocketDataFrame frame,
		  out IDataFramePartReader nextPartReader);

		public static IDataFramePartReader NewReader
		{
			get { return DataFramePartReader.FixPartReader; }
		}

		protected static IDataFramePartReader FixPartReader { get; private set; }

		protected static IDataFramePartReader ExtendedLenghtReader { get; private set; }

		protected static IDataFramePartReader MaskKeyReader { get; private set; }

		protected static IDataFramePartReader PayloadDataReader { get; private set; }
	}
}