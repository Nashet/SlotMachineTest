using Nashet.SlotMachine.Configs;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface INetworkSymbolsModel
	{
		bool IsInitialized { get; }

		List<SymbolConfig> GetSymbols(int reel);
		void Prepare();
	}
}