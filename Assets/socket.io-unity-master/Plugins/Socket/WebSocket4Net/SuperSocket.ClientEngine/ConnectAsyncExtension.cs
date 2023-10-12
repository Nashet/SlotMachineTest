using Socket.WebSocket4Net.CompilerServices;
using System;
using System.Net;
using System.Net.Sockets;

namespace Socket.WebSocket4Net.SuperSocket.ClientEngine
{
	public static class ConnectAsyncExtension
	{
		private static void SocketAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
		{
			e.Completed -= new EventHandler<SocketAsyncEventArgs>(ConnectAsyncExtension.SocketAsyncEventCompleted);
			ConnectAsyncExtension.ConnectToken userToken = (ConnectAsyncExtension.ConnectToken)e.UserToken;
			e.UserToken = null;
			userToken.Callback(sender as global::System.Net.Sockets.Socket, userToken.State, e);
		}

		private static SocketAsyncEventArgs CreateSocketAsyncEventArgs(
		  EndPoint remoteEndPoint,
		  ConnectedCallback callback,
		  object state)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
			socketAsyncEventArgs.UserToken = new ConnectAsyncExtension.ConnectToken()
			{
				State = state,
				Callback = callback
			};
			socketAsyncEventArgs.RemoteEndPoint = remoteEndPoint;
			socketAsyncEventArgs.Completed +=
			  new EventHandler<SocketAsyncEventArgs>(ConnectAsyncExtension.SocketAsyncEventCompleted);
			return socketAsyncEventArgs;
		}

		[Extension]
		private static void ConnectAsyncInternal(
		  EndPoint remoteEndPoint,
		  ConnectedCallback callback,
		  object state)
		{
			if (remoteEndPoint is DnsEndPoint)
			{
				DnsEndPoint dnsEndPoint = (DnsEndPoint)remoteEndPoint;
				IAsyncResult hostAddresses = Dns.BeginGetHostAddresses(dnsEndPoint.Host,
				  new AsyncCallback(ConnectAsyncExtension.OnGetHostAddresses),
				   new ConnectAsyncExtension.DnsConnectState()
				   {
					   Port = dnsEndPoint.Port,
					   Callback = callback,
					   State = state
				   });
				if (!hostAddresses.CompletedSynchronously)
					return;
				ConnectAsyncExtension.OnGetHostAddresses(hostAddresses);
			}
			else
			{
				SocketAsyncEventArgs socketAsyncEventArgs =
				  ConnectAsyncExtension.CreateSocketAsyncEventArgs(remoteEndPoint, callback, state);
				new global::System.Net.Sockets.Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
				  .ConnectAsync(socketAsyncEventArgs);
			}
		}

		private static IPAddress GetNextAddress(
		  ConnectAsyncExtension.DnsConnectState state,
		  out global::System.Net.Sockets.Socket attempSocket)
		{
			IPAddress ipAddress = null;
			attempSocket = null;
			int nextAddressIndex = state.NextAddressIndex;
			while (attempSocket == null)
			{
				if (nextAddressIndex >= state.Addresses.Length)
					return null;
				ipAddress = state.Addresses[nextAddressIndex++];
				if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
					attempSocket = state.Socket6;
				else if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
					attempSocket = state.Socket4;
			}

			state.NextAddressIndex = nextAddressIndex;
			return ipAddress;
		}

		private static void OnGetHostAddresses(IAsyncResult result)
		{
			ConnectAsyncExtension.DnsConnectState asyncState = result.AsyncState as ConnectAsyncExtension.DnsConnectState;
			IPAddress[] hostAddresses;
			try
			{
				hostAddresses = Dns.EndGetHostAddresses(result);
			}
			catch
			{
				asyncState.Callback(null, asyncState.State, null);
				return;
			}

			if (hostAddresses == null || hostAddresses.Length <= 0)
			{
				asyncState.Callback(null, asyncState.State, null);
			}
			else
			{
				asyncState.Addresses = hostAddresses;
				ConnectAsyncExtension.CreateAttempSocket(asyncState);
				global::System.Net.Sockets.Socket attempSocket;
				IPAddress nextAddress = ConnectAsyncExtension.GetNextAddress(asyncState, out attempSocket);
				if (nextAddress == null)
				{
					asyncState.Callback(null, asyncState.State, null);
				}
				else
				{
					SocketAsyncEventArgs e = new SocketAsyncEventArgs();
					e.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectAsyncExtension.SocketConnectCompleted);
					IPEndPoint ipEndPoint = new IPEndPoint(nextAddress, asyncState.Port);
					e.RemoteEndPoint = ipEndPoint;
					e.UserToken = asyncState;
					if (attempSocket.ConnectAsync(e))
						return;
					ConnectAsyncExtension.SocketConnectCompleted(attempSocket, e);
				}
			}
		}

		private static void SocketConnectCompleted(object sender, SocketAsyncEventArgs e)
		{
			ConnectAsyncExtension.DnsConnectState userToken = e.UserToken as ConnectAsyncExtension.DnsConnectState;
			if (e.SocketError == SocketError.Success)
			{
				ConnectAsyncExtension.ClearSocketAsyncEventArgs(e);
				userToken.Callback((global::System.Net.Sockets.Socket)sender, userToken.State, e);
			}
			else if (e.SocketError != SocketError.HostUnreachable && e.SocketError != SocketError.ConnectionRefused)
			{
				ConnectAsyncExtension.ClearSocketAsyncEventArgs(e);
				userToken.Callback(null, userToken.State, e);
			}
			else
			{
				global::System.Net.Sockets.Socket attempSocket;
				IPAddress nextAddress = ConnectAsyncExtension.GetNextAddress(userToken, out attempSocket);
				if (nextAddress == null)
				{
					ConnectAsyncExtension.ClearSocketAsyncEventArgs(e);
					e.SocketError = SocketError.HostUnreachable;
					userToken.Callback(null, userToken.State, e);
				}
				else
				{
					e.RemoteEndPoint = new IPEndPoint(nextAddress, userToken.Port);
					if (attempSocket.ConnectAsync(e))
						return;
					ConnectAsyncExtension.SocketConnectCompleted(attempSocket, e);
				}
			}
		}

		private static void ClearSocketAsyncEventArgs(SocketAsyncEventArgs e)
		{
			e.Completed -= new EventHandler<SocketAsyncEventArgs>(ConnectAsyncExtension.SocketConnectCompleted);
			e.UserToken = null;
		}

		[Extension]
		public static void ConnectAsync(
		  EndPoint remoteEndPoint,
		  ConnectedCallback callback,
		  object state)
		{
			ConnectAsyncExtension.ConnectAsyncInternal(remoteEndPoint, callback, state);
		}

		private static void CreateAttempSocket(ConnectAsyncExtension.DnsConnectState connectState)
		{
			if (global::System.Net.Sockets.Socket.OSSupportsIPv6)
				connectState.Socket6 = new global::System.Net.Sockets.Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
			connectState.Socket4 = new global::System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		private class ConnectToken
		{
			public object State { get; set; }

			public ConnectedCallback Callback { get; set; }
		}

		private class DnsConnectState
		{
			public IPAddress[] Addresses { get; set; }

			public int NextAddressIndex { get; set; }

			public int Port { get; set; }

			public global::System.Net.Sockets.Socket Socket4 { get; set; }

			public global::System.Net.Sockets.Socket Socket6 { get; set; }

			public object State { get; set; }

			public ConnectedCallback Callback { get; set; }
		}
	}
}