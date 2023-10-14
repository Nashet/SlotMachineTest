using Nashet.Common.Data;
using Nashet.Contracts.Services;
using Socket.Newtonsoft.Json;
using Socket.Quobject.SocketIoClientDotNet.Client;
using System.Collections.Generic;

namespace Nashet.Common.Services
{
	public class SocketIOService : ISocketClientService
	{
		public event MessageReceivedDelegate OnMessageReceived;

		protected ISocket socket = null;
		private List<string> openedSockets = new List<string>();

		public bool IsConnected { get; private set; }

		public void Connect(string host)
		{
			if (socket == null)
			{
				socket = new FSocket(host);
				socket.On(QSocket.EVENT_CONNECT, () =>
				{
					IsConnected = true;
					UnityEngine.Debug.Log($"[{nameof(SocketIOService)}]Connected to socket {host}");
				});
			}
		}

		public void Subscribe(string socketName)
		{
			if (openedSockets.Contains(socketName))
			{
				return;
			}
			UnityEngine.Debug.Log($"[{nameof(SocketIOService)}]Subscribed to socket {socketName}");
			openedSockets.Add(socketName);
			socket.On(socketName, (rawData) =>
			{
				string str = rawData.ToString();

				//todo: handle exceptions
				SocketData data = JsonConvert.DeserializeObject<SocketData>(str);
				UnityEngine.Debug.Log($"[{nameof(SocketIOService)}]Got socket message from {data.id} {socketName} {data.msg}");
				OnMessageReceived?.Invoke(socketName, data);
			});

			socket.On(QSocket.EVENT_ERROR, () =>
			{
				UnityEngine.Debug.LogError($"[{nameof(SocketIOService)}]Socket error");
			});
			socket.On(QSocket.EVENT_CONNECT_ERROR, () =>
			{
				UnityEngine.Debug.LogError($"[{nameof(SocketIOService)}]Socket connection error");

			});
			socket.On(QSocket.EVENT_DISCONNECT, () =>
			{
				UnityEngine.Debug.LogError($"[{nameof(SocketIOService)}]Socket disconnected");
			});
		}

		public void Disconnect()
		{

			if (socket != null)
			{
				socket.Disconnect();
				IsConnected = false;
			}
		}

		public void Send(string socketName, SocketData data)
		{
			if (this.socket == null)
			{
				return;
			}
			data.msg = data.msg.Replace("\"", ""); //todo make a proper fix
			var str = JsonConvert.SerializeObject(data);
			this.socket.Emit(socketName, str);
			UnityEngine.Debug.Log($"[{nameof(SocketIOService)}]Sent {str} to socket {socketName}");
		}

		public void Dispose()
		{
			Disconnect();
		}
	}
}