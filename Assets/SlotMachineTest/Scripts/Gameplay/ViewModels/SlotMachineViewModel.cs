using Nashet.Common.Services;
using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using Nashet.SlotMachine.Gameplay.InputView;
using Nashet.SlotMachine.Gameplay.Models;
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
		public event PropertyChangedEventHandler<ISlotMachineViewModel> OnPropertyChanged;

		public int lastSpinScores => model.lastSpinScores;

		[SerializeField] private PlayerInput playerInput;
		[SerializeField] private PlayerSoundsView playerSoundsView;
		[SerializeField] private ExtraBonusWindowViewModel extraBonusWindowViewModel;
		[SerializeField] private List<ReelViewModel> reelVMlList = new();
		[SerializeField] private string configHolderName;

		private ISlotMachineModel model;

		private void Awake()
		{
			var configService = new SOConfigService(configHolderName);
			Initialize(configService.GetConfig<GameplayConfig>());
		}
		public void Initialize(GameplayConfig gameplayConfig)
		{
			model = new SlotMachineModel(gameplayConfig, reelVMlList);
			model.OnPropertyChanged += PropertyChangedHandler;
			playerInput.OnSpinButtonClicked += OnSpinButtonClickedHandler;
			OnPropertyChanged += playerSoundsView.PropertyChangedHandler;
			extraBonusWindowViewModel.Initialize(model);
			for (int i = 0; i < reelVMlList.Count; i++)
			{
				var item = reelVMlList[i];
				item.OnPropertyChanged += PropertyChangedHandler;
				item.Initialize(gameplayConfig, playerSoundsView, model.reelModelsList[i]);
			}
		}

		private void OnDestroy()
		{
			playerInput.OnSpinButtonClicked -= OnSpinButtonClickedHandler;
			OnPropertyChanged -= playerSoundsView.PropertyChangedHandler;
			model.OnPropertyChanged -= PropertyChangedHandler;
			for (int i = 0; i < reelVMlList.Count; i++)
			{
				var item = reelVMlList[i];
				item.OnPropertyChanged -= PropertyChangedHandler;
			}
		}

		private void OnSpinButtonClickedHandler()
		{
			model.StartNewRound();
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}

		public void PropertyChangedHandler(IReelViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IReelViewModel.selectedSymbol))
			{
				model.HandleReelStop(sender.selectedSymbol);
			}
		}

		public void PropertyChangedHandler(ISlotMachineModel sender, string propertyName)
		{
			if (propertyName == nameof(ISlotMachineModel.lastSpinScores))
			{
				RiseOnPropertyChanged(nameof(lastSpinScores));
			}
		}
	}
}
