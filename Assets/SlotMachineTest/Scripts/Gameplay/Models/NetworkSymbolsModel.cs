using Nashet.Common.Services;
using Nashet.Contracts.Services;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using Socket.Newtonsoft.Json;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Models
{
	public class NetworkSymbolsModel : INetworkSymbolsModel
	{
		public bool IsInitialized => socketClient != null && socketClient.IsConnected;
		private const string symbolsChannel = "symbols";
		private const string orderSymbolsCommand = "gime new symbols plz";
		private ISocketClientService socketClient;
		private bool disposed;

		private Dictionary<string, SymbolConfig> symbolsLookup = new Dictionary<string, SymbolConfig>();
		private List<List<SymbolConfig>> availableSymbols = new List<List<SymbolConfig>>();

		public NetworkSymbolsModel(GameplayConfig gameplayConfig, ISocketClientService socketClientService, int amountOfReels)
		{
			socketClient = socketClientService;

			socketClient.Connect(gameplayConfig.WSURL);
			socketClient.Subscribe(symbolsChannel);
			socketClient.OnMessageReceived += MessageReceivedHandler;

			foreach (var item in gameplayConfig.availableSymbols)
			{
				symbolsLookup.Add(item.id, item);
			}
			for (int i = 0; i < amountOfReels; i++)
			{
				availableSymbols.Add(new List<SymbolConfig>());
			}
		}

		private void MessageReceivedHandler(string socketName, SocketData data)
		{
			if (socketName == symbolsChannel && data.msg != orderSymbolsCommand)
			{
				var receivedSymbols = JsonConvert.DeserializeObject<List<List<string>>>(data.msg);

				for (int i = 0; i < receivedSymbols.Count; i++)
				{
					availableSymbols[i].Clear();
					List<string> item = receivedSymbols[i];
					foreach (var symbol in item)
					{
						availableSymbols[i].Add(symbolsLookup[symbol]);
					}
				}
			}
		}

		public List<SymbolConfig> GetSymbols(int reel)
		{
			return availableSymbols[reel];
		}

		~NetworkSymbolsModel()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				// dispose unmanaged resources.
				if (disposing)
				{
					socketClient.OnMessageReceived -= MessageReceivedHandler;
					socketClient.Dispose();
				}

				disposed = true;
			}
		}

		public void Prepare()
		{
			var data = new SocketData { id = "client", msg = JsonConvert.SerializeObject(orderSymbolsCommand) };
			socketClient.Send(symbolsChannel, data);
		}
	}
}