using Nashet.SlotMachine.Configs;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface INetworkSymbolsModel
	{
		bool IsInitialized { get; }
		int bonusPrize { get; }

		List<SymbolConfig> GetSymbols(int reel);
		void Prepare();
	}
}