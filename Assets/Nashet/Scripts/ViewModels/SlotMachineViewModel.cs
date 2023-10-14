using Assets.Nashet.Scripts.Contracts.Models;
using Assets.Nashet.Scripts.Contracts.ViewModels;
using Assets.Nashet.Scripts.Data.Configs;
using Assets.Nashet.Scripts.InputViews;
using Assets.Nashet.Scripts.Models;
using Assets.Nashet.Scripts.Universal.Contracts.Patterns;
using Assets.Nashet.Scripts.Universal.Contracts.Services;
using Assets.Nashet.Scripts.Universal.Services;
using Assets.Nashet.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Nashet.Scripts.ViewModels
{
	/// <summary>
	/// Purpose of that class is to setup nested view models and own model/view
	/// </summary>
	public class SlotMachineViewModel : MonoBehaviour, ISlotMachineViewModel
	{
		public event PropertyChangedEventHandler<ISlotMachineViewModel> OnPropertyChanged;

		public int lastSpinScores => slotMachineModel.lastSpinScores;
		public float extraBonusSum => slotMachineModel.extraBonusSum;

		[SerializeField] private PlayerInput playerInput;
		[SerializeField] private PlayerSoundsView playerSoundsView;
		[SerializeField] private ExtraBonusWindowViewModel extraBonusWindowViewModel;
		[SerializeField] private List<ReelViewModel> reelVMlList = new List<ReelViewModel>();
		[SerializeField] private string configHolderName;
		private ISlotMachineModel slotMachineModel;
		internal ISocketClientService socketService;

		private void Awake()
		{
			var configService = new SOConfigService(configHolderName);
			Initialize(configService.GetConfig<GameplayData>());
		}

		public void Initialize(GameplayData gameplayConfig)
		{
			slotMachineModel = new SlotMachineModel(gameplayConfig, reelVMlList, socketService);
			playerInput.OnSpinButtonClicked += OnSpinButtonClickedHandler;
			this.SubscribeTo(slotMachineModel);
			playerSoundsView.SubscribeTo(this);
			extraBonusWindowViewModel.Initialize(slotMachineModel);

			for (int i = 0; i < reelVMlList.Count; i++)
			{
				var reelVM = reelVMlList[i];
				this.SubscribeTo(reelVM);
				reelVM.Initialize(gameplayConfig, playerSoundsView, slotMachineModel.reelModelsList[i], this);
			}
		}

		private void OnDestroy()
		{
			playerInput.OnSpinButtonClicked -= OnSpinButtonClickedHandler;
			playerSoundsView.UnSubscribeFrom(this);

			this.UnSubscribeFrom(slotMachineModel);
			for (int i = 0; i < reelVMlList.Count; i++)
			{
				var item = reelVMlList[i];
				this.UnSubscribeFrom(item);
			}
		}

		private void OnSpinButtonClickedHandler()
		{
			slotMachineModel.StartNewRound();
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}

		public void PropertyChangedHandler(IReelViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IReelViewModel.selectedSymbol))
			{
				slotMachineModel.HandleReelStop(sender.selectedSymbol);
			}
		}

		public void PropertyChangedHandler(ISlotMachineModel sender, string propertyName)
		{
			switch (propertyName)
			{
				case nameof(ISlotMachineModel.lastSpinScores):
					RiseOnPropertyChanged(nameof(lastSpinScores));
					break;
				case nameof(ISlotMachineModel.extraBonusSum):
					RiseOnPropertyChanged(nameof(extraBonusSum));
					break;
				// Add more cases for other properties if needed
				default:
					break;
			}
		}

		public void SubscribeTo(IReelViewModel sender)
		{
			sender.OnPropertyChanged += PropertyChangedHandler;
		}

		public void UnSubscribeFrom(IReelViewModel sender)
		{
			sender.OnPropertyChanged -= PropertyChangedHandler;
		}

		public void SubscribeTo(ISlotMachineModel sender)
		{
			sender.OnPropertyChanged += PropertyChangedHandler;
		}

		public void UnSubscribeFrom(ISlotMachineModel sender)
		{
			sender.OnPropertyChanged -= PropertyChangedHandler;
		}
	}
}
