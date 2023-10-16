using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Contracts.Views;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using Assets.SlotMachineNetTest.Scripts.Universal.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Views;
using UnityEngine;
using Zenject;

namespace Assets.SlotMachineNetTest.Scripts.ViewModels
{
	public class ReelViewModel : MonoViewModel, IReelViewModel
	{
		public event PropertyChangedEventHandler<IReelViewModel> OnPropertyChanged;
		public SymbolData decorativeSymbol => reelModel.decorativeSymbol;
		public SymbolData selectedSymbol => reelModel.selectedSymbol;

		[SerializeField] private ReelView reelView;

		private IPlayerSoundsView playerSoundsView;
		private IReelModel reelModel;
		private ISlotMachineViewModel slotMachineViewModel;

		[Inject]
		private void Construct(IPlayerSoundsView playerSoundsView, ISlotMachineViewModel slotMachineViewModel)
		{
			this.slotMachineViewModel = slotMachineViewModel;
			this.playerSoundsView = playerSoundsView;
			reelView.SubscribeTo(this);
			reelView.SubscribeTo(slotMachineViewModel);
			playerSoundsView.SubscribeTo(this);
		}

		//keep it for now
		public void InitializeOld(IReelModel reelModel)
		{
			this.reelModel = reelModel;
			this.SubscribeTo(this.reelModel);
		}

		private void OnDestroy()
		{
			this.UnSubscribeFrom(reelModel);
			reelView.UnSubscribeFrom(this);
			playerSoundsView.UnSubscribeFrom(this);
			reelView.UnSubscribeFrom(slotMachineViewModel);
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}

		public void PropertyChangedHandler(IReelModel sender, string propertyName)
		{
			RiseOnPropertyChanged(propertyName);
		}

		public void SubscribeTo(IReelModel sender)
		{
			sender.OnPropertyChanged += PropertyChangedHandler;
		}

		public void UnSubscribeFrom(IReelModel sender)
		{
			sender.OnPropertyChanged -= PropertyChangedHandler;
		}
	}
}
