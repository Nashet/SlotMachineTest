using Assets.SlotMachineNetTest.Scripts.Contracts.InputViews;
using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Contracts.Views;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using Assets.SlotMachineNetTest.Scripts.Universal.ViewModels;
using System.Collections.Generic;

namespace Assets.SlotMachineNetTest.Scripts.ViewModels
{
	/// <summary>
	/// Purpose of that class is to setup nested view models and own model/view
	/// </summary>
	public class SlotMachineViewModel : ViewModel, ISlotMachineViewModel
	{
		public event PropertyChangedEventHandler<ISlotMachineViewModel> OnPropertyChanged;

		public int lastSpinScores => slotMachineModel.lastSpinScores;
		public float extraBonusSum => slotMachineModel.extraBonusSum;

		private IList<IReelViewModel> reelVMlList;
		private IPlayerInput playerInput;

		private ISlotMachineModel slotMachineModel;
		private IPlayerSoundsView playerSoundsView;

		public SlotMachineViewModel(IPlayerSoundsView playerSoundsView, ISlotMachineModel slotMachineModel, IPlayerInput playerInput, IList<IReelViewModel> reelVMlList)
		{
			this.reelVMlList = reelVMlList;
			this.playerInput = playerInput;
			this.slotMachineModel = slotMachineModel;
			this.playerSoundsView = playerSoundsView;
			playerInput.OnSpinButtonClicked += OnSpinButtonClickedHandler;
			this.SubscribeTo(slotMachineModel);
			playerSoundsView.SubscribeTo(this);

			for (int i = 0; i < reelVMlList.Count; i++)
			{
				var reelVM = reelVMlList[i];
				this.SubscribeTo(reelVM);
				//keep it for now
				reelVM.InitializeOld(slotMachineModel.reelModelsList[i]);
			}
		}

		private void OnDestroy()
		{
			if (playerInput != null)
				playerInput.OnSpinButtonClicked -= OnSpinButtonClickedHandler;
			if (playerSoundsView != null)
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

		public void Dispose()
		{
			OnDestroy();
		}
	}
}
