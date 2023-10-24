using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Services;
using Assets.SlotMachineNetTest.Scripts.Universal.Models;
using System.Collections;
using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Models
{
	public class ReelModel : Model, IReelModel
	{
		public event PropertyChangedEventHandler<IReelModel> OnPropertyChanged;

		private ReactiveProperty<SymbolData> _selectedSymbol = new();
		public SymbolData selectedSymbol => _selectedSymbol.Value;

		private ReactiveProperty<SymbolData> _decorativeSymbol = new();
		public SymbolData decorativeSymbol => _decorativeSymbol.Value;

		private IConfigService configService;
		private GameplayData gameplayConfig => configService.GetConfig<GameplayData>();
		private IFakeRandomStrategy<SymbolData> randomStrategy;
		private CoroutineHelper coroutineHelper;

		public ReelModel(IFakeRandomStrategy<SymbolData> randomStrategy, IConfigService configService)
		{
			this.configService = configService;
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
