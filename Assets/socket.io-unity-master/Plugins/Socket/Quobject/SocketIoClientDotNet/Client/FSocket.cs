using Nashet.SlotMachine.Gameplay.Models;
using Socket.Quobject.EngineIoClientDotNet.ComponentEmitter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Socket.Quobject.SocketIoClientDotNet.Client
{
	public class FSocket : ISocket
	{
		private CoroutineHelper coroutineHelper;
		private float sendingDelay = 0.1f;
		private Dictionary<string, Action<object>> callBack = new Dictionary<string, Action<object>>();

		public FSocket(string URL)
		{
			var gameObject = new GameObject($"CoroutineHolder");
			coroutineHelper = gameObject.AddComponent<CoroutineHelper>();
		}

		public ISocket Disconnect()
		{
			return null;
		}

		public Emitter Emit(string eventString, object args)
		{
			coroutineHelper.StartCoroutine(SendCoroutine(eventString, args));
			return null;
		}

		public Emitter On(string eventString, ActionTrigger fn)
		{
			if (eventString == QSocket.EVENT_CONNECT)
				fn();

			return null;
		}

		public Emitter On(string eventString, Action<object> fn)
		{
			callBack[eventString] = fn;
			return null;
		}

		private IEnumerator SendCoroutine(string eventString, object args)
		{
			yield return new WaitForSeconds(sendingDelay);
			callBack[eventString](args);
		}
	}
}