using Socket.WebSocket4Net.Default;

namespace Socket.WebSocket4Net.Command
{
	public class Close : WebSocketCommandBase
	{
		public override void ExecuteCommand(WebSocket session, WebSocketCommandInfo commandInfo)
		{
			if (session.StateCode == 2)
			{
				session.CloseWithoutHandshake();
			}
			else
			{
				short num = commandInfo.CloseStatusCode;
				if (num <= 0)
					num = session.ProtocolProcessor.CloseStatusCode.NoStatusCode;
				session.Close(num, commandInfo.Text);
			}
		}

		public override string Name
		{
			get { return 8.ToString(); }
		}
	}
}