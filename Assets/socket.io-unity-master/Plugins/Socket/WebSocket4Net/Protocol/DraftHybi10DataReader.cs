using Socket.WebSocket4Net.Default;
using Socket.WebSocket4Net.Protocol.FramePartReader;
using Socket.WebSocket4Net.SuperSocket.ClientEngine.Protocol;
using System.Collections.Generic;

namespace Socket.WebSocket4Net.Protocol
{
	internal class DraftHybi10DataReader : IClientCommandReader<WebSocketCommandInfo>
	{
		private List<WebSocketDataFrame> m_PreviousFrames;
		private WebSocketDataFrame m_Frame;
		private IDataFramePartReader m_PartReader;
		private int m_LastPartLength;

		public DraftHybi10DataReader()
		{
			this.m_Frame = new WebSocketDataFrame(new ArraySegmentList());
			this.m_PartReader = DataFramePartReader.NewReader;
		}

		public int LeftBufferSize
		{
			get { return this.m_Frame.InnerData.Count; }
		}

		public IClientCommandReader<WebSocketCommandInfo> NextCommandReader
		{
			get { return this; }
		}

		protected void AddArraySegment(
		  ArraySegmentList segments,
		  byte[] buffer,
		  int offset,
		  int length,
		  bool isReusableBuffer)
		{
			segments.AddSegment(buffer, offset, length, isReusableBuffer);
		}

		public WebSocketCommandInfo GetCommandInfo(
		  byte[] readBuffer,
		  int offset,
		  int length,
		  out int left)
		{
			this.AddArraySegment(this.m_Frame.InnerData, readBuffer, offset, length, true);
			IDataFramePartReader nextPartReader;
			int num = this.m_PartReader.Process(this.m_LastPartLength, this.m_Frame, out nextPartReader);
			if (num < 0)
			{
				left = 0;
				return null;
			}

			left = num;
			if (left > 0)
				this.m_Frame.InnerData.TrimEnd(left);
			if (nextPartReader == null)
			{
				WebSocketCommandInfo socketCommandInfo;
				if (this.m_Frame.FIN)
				{
					if (this.m_PreviousFrames != null && this.m_PreviousFrames.Count > 0)
					{
						this.m_PreviousFrames.Add(this.m_Frame);
						this.m_Frame = new WebSocketDataFrame(new ArraySegmentList());
						socketCommandInfo = new WebSocketCommandInfo(m_PreviousFrames);
						this.m_PreviousFrames = null;
					}
					else
					{
						socketCommandInfo = new WebSocketCommandInfo(this.m_Frame);
						this.m_Frame.Clear();
					}
				}
				else
				{
					if (this.m_PreviousFrames == null)
						this.m_PreviousFrames = new List<WebSocketDataFrame>();
					this.m_PreviousFrames.Add(this.m_Frame);
					this.m_Frame = new WebSocketDataFrame(new ArraySegmentList());
					socketCommandInfo = null;
				}

				this.m_LastPartLength = 0;
				this.m_PartReader = DataFramePartReader.NewReader;
				return socketCommandInfo;
			}

			this.m_LastPartLength = this.m_Frame.InnerData.Count - num;
			this.m_PartReader = nextPartReader;
			return null;
		}
	}
}