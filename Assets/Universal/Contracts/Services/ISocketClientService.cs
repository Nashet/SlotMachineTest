using Assets.CommonNashet.Data;
using System;

namespace Assets.CommonNashet.Contracts.Services
{
	public delegate void MessageReceivedDelegate(string socketName, SocketData data);
	public interface ISocketClientService : IService, IDisposable
	{
		event MessageReceivedDelegate OnMessageReceived;
		void Connect(string host);

		bool IsConnected { get; }
		void Disconnect();
		void Send(string socket, SocketData data);
		void Subscribe(string socketName);
	}
}