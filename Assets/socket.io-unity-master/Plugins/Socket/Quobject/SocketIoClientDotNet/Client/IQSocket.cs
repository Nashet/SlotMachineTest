using Socket.Quobject.EngineIoClientDotNet.ComponentEmitter;
using System;

namespace Socket.Quobject.SocketIoClientDotNet.Client
{
	public interface ISocket
	{
		ISocket Disconnect();
		Emitter Emit(string eventString, object args);
		Emitter On(string eventString, ActionTrigger fn);
		Emitter On(string eventString, Action<object> fn);
	}
}