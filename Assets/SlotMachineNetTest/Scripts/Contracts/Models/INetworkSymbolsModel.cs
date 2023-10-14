using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using System.Collections.Generic;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.Models
{
	public interface INetworkSymbolsModel
	{
		bool IsInitialized { get; }
		int bonusPrize { get; }

		List<SymbolData> GetSymbols(int reel);
		void Prepare();
	}
}