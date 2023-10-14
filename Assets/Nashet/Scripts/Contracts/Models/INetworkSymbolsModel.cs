using Assets.Nashet.Scripts.Data.Configs;
using System.Collections.Generic;

namespace Assets.Nashet.Scripts.Contracts.Models
{
	public interface INetworkSymbolsModel
	{
		bool IsInitialized { get; }
		int bonusPrize { get; }

		List<SymbolData> GetSymbols(int reel);
		void Prepare();
	}
}