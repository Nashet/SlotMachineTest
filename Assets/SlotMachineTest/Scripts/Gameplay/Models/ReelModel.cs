using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using System.Collections;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.Models
{
	public class ReelModel : IReelModel
	{
		public event PropertyChangedEventHandler<IReelModel> OnPropertyChanged;

		public SymbolConfig selectedSymbol
		{
			get => _selectedSymbol;
			private set
			{
				_selectedSymbol = value;
				RiseOnPropertyChanged(nameof(selectedSymbol));
			}
		}
		public SymbolConfig decorativeSymbol
		{
			get => _decorativeSymbol;
			private set
			{
				_decorativeSymbol = value;
				RiseOnPropertyChanged(nameof(decorativeSymbol));
			}
		}

		private GameplayConfig gameplayConfig;
		private IFakeRandomStrategy<SymbolConfig> randomStrategy;
		private CoroutineHelper coroutineHelper;
		private SymbolConfig _selectedSymbol;
		private SymbolConfig _decorativeSymbol;

		public ReelModel(IFakeRandomStrategy<SymbolConfig> randomStrategy, GameplayConfig gameplayConfig)
		{
			this.gameplayConfig = gameplayConfig;
			this.randomStrategy = randomStrategy;
			var gameObject = new GameObject("CoroutineHolder");
			coroutineHelper = gameObject.AddComponent<CoroutineHelper>();
		}

		public void StartNewRound()
		{
			coroutineHelper.MakeCoroutine(SpinReel());
		}
		private IEnumerator SpinReel()
		{
			while (!randomStrategy.IsInitialized)
			{
				//todo handle if server doesnt respond
				yield return new WaitForSeconds(gameplayConfig.oneSymbolSpinTime);
			}

			randomStrategy.Reset();

			while (!randomStrategy.IsFinished)
			{
				var nextSymbol = randomStrategy.Get();
				if (nextSymbol != null) //null means that sequence isnt ready yet
				{
					decorativeSymbol = nextSymbol;
				}

				yield return new WaitForSeconds(gameplayConfig.oneSymbolSpinTime);
			}

			selectedSymbol = decorativeSymbol;
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}
	}
}
