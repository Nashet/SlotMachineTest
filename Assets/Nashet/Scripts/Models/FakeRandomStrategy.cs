﻿using Nashet.Contracts.Patterns;
using Nashet.Data.Configs;
using System.Collections.Generic;
using UnityEngine;

namespace Nashet.Models
{
	public class FakeRandomStrategy : IFakeRandomStrategy<SymbolData>
	{
		public bool IsFinished => counter >= expectedSequenceSize;

		public bool IsInitialized => true;

		private GameplayData gameplayConfig;
		private int counter;
		private int expectedSequenceSize;
		private IEnumerator<SymbolData> symbolEnumerator;

		public FakeRandomStrategy(GameplayData gameplayConfig)
		{
			this.gameplayConfig = gameplayConfig;
			symbolEnumerator = GetSymbols().GetEnumerator();
		}

		public void Reset()
		{
			expectedSequenceSize = gameplayConfig.amountOfDecorateSymbolsPerSpin + Random.Range(0, gameplayConfig.randomAmountOfDecorateSymbolsPerSpin);
			var skipRandomElements = Random.Range(0, gameplayConfig.availableSymbols.Count);
			for (var i = 0; i < skipRandomElements; i++)
			{
				symbolEnumerator.MoveNext();
			}
			counter = 0;
		}

		public SymbolData Get()
		{
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