using Nashet.Data.Configs;
using System.Collections.Generic;

namespace Nashet.Contracts.Model
{
	public interface INetworkSymbolsModel
	{
		bool IsInitialized { get; }
		int bonusPrize { get; }

		List<SymbolData> GetSymbols(int reel);
		void Prepare();
	}
}