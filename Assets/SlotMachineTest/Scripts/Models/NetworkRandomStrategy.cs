using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Data.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Models
{
	/// <summary>
	/// Gives collection of pseudo random symbols
	/// </summary>
	public class NetworkRandomStrategy : IFakeRandomStrategy<SymbolData>
	{
		public bool IsFinished => counter >= availableSymbols.Count && availableSymbols.Count != 0;

		public bool IsInitialized => networkSymbols != null && networkSymbols.IsInitialized;


		private INetworkSymbolsModel networkSymbols;
		private int id;
		private int counter;
		private IEnumerator<SymbolData> symbolEnumerator;
		private List<SymbolData> availableSymbols = new List<SymbolData>();

		public NetworkRandomStrategy(GameplayData gameplayConfig, int id, INetworkSymbolsModel networkSymbols)
		{
			this.networkSymbols = networkSymbols;
			this.id = id;
			symbolEnumerator = GetSymbols().GetEnumerator();
		}

		public void Reset()
		{
			counter = 0;
		}

		public SymbolData Get()
		{
			availableSymbols = networkSymbols.GetSymbols(id);
			if (availableSymbols.Count == 0)
				return null; //meaning data is not received yet

			symbolEnumerator.MoveNext();

			SymbolData symbol = symbolEnumerator.Current;
			return symbol;
		}

		private IEnumerable<SymbolData> GetSymbols()
		{
			int i = 0;
			while (true)
			{
				counter++;
				yield return availableSymbols[i];

				i++;
				if (i >= availableSymbols.Count)
				{
					i = 0;
				}
			}
		}
	}
}