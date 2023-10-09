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
		public SymbolConfig currentSymbol { get; private set; }

		private GameplayConfig gameplayConfig;
		private WaitForSeconds oneSymbolSpinTime;
		private IFakeRandomStrategy<SymbolConfig> randomStrategy;

		internal void Initialize(GameplayConfig gameplayConfig, FakeRandomStrategy fakeRandomStrategy)
		{
			this.gameplayConfig = gameplayConfig;
			this.randomStrategy = fakeRandomStrategy;
			oneSymbolSpinTime = new WaitForSeconds(gameplayConfig.oneSymbolSpinTime);
			OnPropertyChanged += reelView.PropertyChangedHandler;
		}

		private void OnDestroy()
		{
			OnPropertyChanged -= reelView.PropertyChangedHandler;
		}

		public event PropertyChangedEventHandler<IReelViewModel> OnPropertyChanged;

		public void StartNewRound()
		{
			StartCoroutine(SpinReel());
		}

		private IEnumerator SpinReel()
		{
			for (int i = 0; i < gameplayConfig.amountOfDecorateSymbolsPerSpin; i++)
			{

				SetSymbol(randomStrategy.Get());

				yield return oneSymbolSpinTime;
			}

			SetSymbol(randomStrategy.Get());
		}

		private void SetSymbol(SymbolConfig config)
		{
			currentSymbol = config;
			RiseOnPropertyChanged(nameof(currentSymbol));
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}
	}
}
