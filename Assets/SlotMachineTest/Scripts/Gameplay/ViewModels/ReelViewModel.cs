using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using Nashet.SlotMachine.Gameplay.Views;
using System.Collections;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.ViewModels
{
	public class ReelViewModel : MonoBehaviour, IReelViewModel
	{
		[SerializeField] private ReelView reelView;
		public SymbolConfig decorativeSymbol { get; private set; }
		public SymbolConfig selectedSymbol { get; private set; }

		private GameplayConfig gameplayConfig;
		private WaitForSeconds oneSymbolSpinTime;
		private IFakeRandomStrategy<SymbolConfig> randomStrategy;
		private IPlayerSoundsView playerSoundsView;

		internal void Initialize(GameplayConfig gameplayConfig, IFakeRandomStrategy<SymbolConfig> fakeRandomStrategy, IPlayerSoundsView playerSoundsView)
		{
			this.gameplayConfig = gameplayConfig;
			this.randomStrategy = fakeRandomStrategy;
			this.playerSoundsView = playerSoundsView;
			oneSymbolSpinTime = new WaitForSeconds(gameplayConfig.oneSymbolSpinTime);
			OnPropertyChanged += reelView.PropertyChangedHandler;
			OnPropertyChanged += playerSoundsView.PropertyChangedHandler;
		}

		private void OnDestroy()
		{
			OnPropertyChanged -= reelView.PropertyChangedHandler;
			OnPropertyChanged -= playerSoundsView.PropertyChangedHandler;
		}

		public event PropertyChangedEventHandler<IReelViewModel> OnPropertyChanged;

		public void StartNewRound()
		{
			StartCoroutine(SpinReel());
		}

		private IEnumerator SpinReel() //todo put it in a model
		{
			for (int i = 0; i < gameplayConfig.amountOfDecorateSymbolsPerSpin; i++)
			{

				SetSymbol(randomStrategy.Get());
				RiseOnPropertyChanged(nameof(decorativeSymbol));
				yield return oneSymbolSpinTime;
			}

			SetSymbol(randomStrategy.Get());
			selectedSymbol = decorativeSymbol;
			RiseOnPropertyChanged(nameof(selectedSymbol));
		}

		private void SetSymbol(SymbolConfig config)
		{
			decorativeSymbol = config;
			RiseOnPropertyChanged(nameof(decorativeSymbol));
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}
	}
}
