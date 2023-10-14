using Nashet.SlotMachine.Data.Configs;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface INetworkSymbolsModel
	{
		bool IsInitialized { get; }
		int bonusPrize { get; }

		List<SymbolData> GetSymbols(int reel);
		void Prepare();
	}
}