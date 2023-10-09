using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using System.Collections.Generic;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.ViewModels
{
	public class FakeRandomStrategy : IFakeRandomStrategy<SymbolConfig>
	{
		public bool IsFinished => counter >= expectedSequenceSize;

		private GameplayConfig gameplayConfig;
		private int counter;
		private int expectedSequenceSize;
		private IEnumerator<SymbolConfig> symbolEnumerator;

		public FakeRandomStrategy(GameplayConfig gameplayConfig)
		{
			this.gameplayConfig = gameplayConfig;
			symbolEnumerator = GetSymbols().GetEnumerator();

		}

		public void Reset()
		{
			expectedSequenceSize = gameplayConfig.availableSymbols.Count + Random.Range(0, gameplayConfig.randomAmountOfDecorateSymbolsPerSpin);
			var skipRandomElements = Random.Range(0, gameplayConfig.availableSymbols.Count);
			for (var i = 0; i < skipRandomElements; i++)
			{
				symbolEnumerator.MoveNext();
			}
			counter = 0;
		}

		public SymbolConfig Get()
		{
			symbolEnumerator.MoveNext();

			SymbolConfig symbol = symbolEnumerator.Current;
			return symbol;
		}

		private IEnumerable<SymbolConfig> GetSymbols()
		{
			int i = 0;
			while (true)
			{
				counter++;
				yield return gameplayConfig.availableSymbols[i];

				i++;
				if (i >= gameplayConfig.availableSymbols.Count)
				{
					i = 0;
				}
			}
		}
	}
}
