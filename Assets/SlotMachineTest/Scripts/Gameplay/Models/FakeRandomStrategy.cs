using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.ViewModels
{
	public class FakeRandomStrategy : IFakeRandomStrategy<SymbolConfig>
	{
		private GameplayConfig gameplayConfig;

		public FakeRandomStrategy(GameplayConfig gameplayConfig)
		{
			this.gameplayConfig = gameplayConfig;
		}

		SymbolConfig IFakeRandomStrategy<SymbolConfig>.Get()
		{
			var random = Random.Range(0, gameplayConfig.availableSymbols.Count);
			return gameplayConfig.availableSymbols[random];
		}
	}
}
