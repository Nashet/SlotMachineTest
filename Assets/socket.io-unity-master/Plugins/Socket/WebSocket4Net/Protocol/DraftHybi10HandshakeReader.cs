using Socket.WebSocket4Net.Default;

namespace Socket.WebSocket4Net.Protocol
{
	internal class DraftHybi10HandshakeReader : HandshakeReader
	{
		public DraftHybi10HandshakeReader(WebSocket websocket)
		  : base(websocket) { }

		public override WebSocketCommandInfo GetCommandInfo(
		  byte[] readBuffer,
		  int offset,
		  int length,
		  out int left)
		{
			WebSocketCommandInfo commandInfo = base.GetCommandInfo(readBuffer, offset, length, out left);
			if (commandInfo == null)
				return null;
			if (!HandshakeReader.BadRequestCode.Equals(commandInfo.Key))
				this.NextCommandReader = new DraftHybi10DataReader();
			return commandInfo;
		}
	}
}