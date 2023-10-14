using Nashet.Contracts.Model;
using Nashet.Contracts.Patterns;
using Nashet.Contracts.View;
using Nashet.Contracts.ViewModel;
using Nashet.Data.Configs;
using Nashet.Views;
using UnityEngine;

namespace Nashet.ViewModels
{
	public class ReelViewModel : MonoBehaviour, IReelViewModel
	{
		public event PropertyChangedEventHandler<IReelViewModel> OnPropertyChanged;
		public SymbolData decorativeSymbol => reelModel.decorativeSymbol;
		public SymbolData selectedSymbol => reelModel.selectedSymbol;

		[SerializeField] private ReelView reelView;

		private IPlayerSoundsView playerSoundsView;
		private IReelModel reelModel;
		private ISlotMachineViewModel slotMachineViewModel;

		internal void Initialize(GameplayData gameplayConfig, IPlayerSoundsView playerSoundsView, IReelModel reelModel, ISlotMachineViewModel slotMachineViewModel)
		{
			this.slotMachineViewModel = slotMachineViewModel;
			this.playerSoundsView = playerSoundsView;
			this.reelModel = reelModel;
			this.SubscribeTo(this.reelModel);
			reelView.SubscribeTo(this);
			reelView.SubscribeTo(slotMachineViewModel);
			playerSoundsView.SubscribeTo(this);
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
