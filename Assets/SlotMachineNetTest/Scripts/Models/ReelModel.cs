using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using Assets.SlotMachineNetTest.Scripts.Universal.Models;
using System.Collections;
using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Models
{
	public class ReelModel : Model, IReelModel
	{
		public event PropertyChangedEventHandler<IReelModel> OnPropertyChanged;

		public ReactiveProperty<SymbolData> _selectedSymbol = new ReactiveProperty<SymbolData>();
		public SymbolData selectedSymbol => _selectedSymbol.Value;

		public ReactiveProperty<SymbolData> _decorativeSymbol = new ReactiveProperty<SymbolData>();
		public SymbolData decorativeSymbol => _decorativeSymbol.Value;


		private GameplayData gameplayConfig;
		private IFakeRandomStrategy<SymbolData> randomStrategy;
		private CoroutineHelper coroutineHelper;

		public ReelModel(IFakeRandomStrategy<SymbolData> randomStrategy, GameplayData gameplayConfig)
		{
			this.gameplayConfig = gameplayConfig;
			this.randomStrategy = randomStrategy;
			var gameObject = new GameObject("CoroutineHolder");
			coroutineHelper = gameObject.AddComponent<CoroutineHelper>();

			_selectedSymbol.Subscribe(x => RiseOnPropertyChanged(nameof(selectedSymbol)));
			_decorativeSymbol.Subscribe(x => RiseOnPropertyChanged(nameof(decorativeSymbol)));
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
					_decorativeSymbol.Value = nextSymbol;
				}

				yield return new WaitForSeconds(gameplayConfig.oneSymbolSpinTime);
			}

			_selectedSymbol.Value = decorativeSymbol;
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}
	}
}
