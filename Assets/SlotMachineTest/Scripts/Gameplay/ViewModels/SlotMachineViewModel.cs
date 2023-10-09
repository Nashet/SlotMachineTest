using Nashet.Common.Services;
using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using Nashet.SlotMachine.Gameplay.InputView;
using Nashet.SlotMachine.Gameplay.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.ViewModels
{
	/// <summary>
	/// Purpose of that class is to setup nested view models and own model/view
	/// </summary>
	public class SlotMachineViewModel : MonoBehaviour, ISlotMachineViewModel
	{
		[SerializeField] private PlayerInput playerInput;
		[SerializeField] private PlayerSoundsView playerSoundsView;
		[SerializeField] private List<ReelViewModel> reelVMlList = new();
		[SerializeField] private string configHolderName;

		public event PropertyChangedEventHandler<ISlotMachineViewModel> OnPropertyChanged;

		public int lastSpinScores { get; protected set; }

		private List<SymbolConfig> selectedSymbols = new List<SymbolConfig>();
		private bool isSpinInProgress;

		private void Awake()
		{
			var configService = new SOConfigService(configHolderName);
			Initialize(configService.GetConfig<GameplayConfig>());
		}
		public void Initialize(GameplayConfig gameplayConfig)
		{
			playerInput.OnSpinButtonClicked += OnSpinButtonClickedHandler;
			OnPropertyChanged += playerSoundsView.PropertyChangedHandler;

			var randomStrategy = new FakeRandomStrategy(gameplayConfig);
			for (int i = 0; i < reelVMlList.Count; i++)
			{
				var item = reelVMlList[i];
				item.Initialize(gameplayConfig, randomStrategy, playerSoundsView);
				item.OnPropertyChanged += PropertyChangedHandler;
			}
		}

		private void OnDestroy()
		{
			playerInput.OnSpinButtonClicked -= OnSpinButtonClickedHandler;
			OnPropertyChanged -= playerSoundsView.PropertyChangedHandler;
		}

		private void OnSpinButtonClickedHandler()
		{
			StartNewRound();
		}

		private void StartNewRound()  //todo put it in a model
		{
			if (isSpinInProgress)
				return;
			isSpinInProgress = true;
			selectedSymbols.Clear();
			foreach (var item in reelVMlList)
			{
				item.StartNewRound();
			}
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, nameof(lastSpinScores));
		}

		public void PropertyChangedHandler(IReelViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IReelViewModel.selectedSymbol))
			{
				HandleReelsStop(sender);
			}
		}

		private void HandleReelsStop(IReelViewModel sender)
		{
			selectedSymbols.Add(sender.selectedSymbol);
			var allReelsHaveStopped = selectedSymbols.Count == reelVMlList.Count;
			if (!allReelsHaveStopped)
			{
				return;
			}

			lastSpinScores = IsAllSymbolsSame() ? selectedSymbols[0].prize3InRow : 0;
			isSpinInProgress = false;
			RiseOnPropertyChanged(nameof(lastSpinScores));
		}

		private bool IsAllSymbolsSame()
		{
			var firstSymbol = selectedSymbols[0];
			for (int i = 0; i < selectedSymbols.Count; i++)
			{
				SymbolConfig item = selectedSymbols[i];
				if (item != firstSymbol)
					return false;
			}
			return true;
		}
	}
}
