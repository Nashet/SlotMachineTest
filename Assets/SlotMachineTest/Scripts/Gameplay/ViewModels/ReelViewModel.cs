using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using Nashet.SlotMachine.Gameplay.Views;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.ViewModels
{
	public class ReelViewModel : MonoBehaviour, IReelViewModel
	{
		public event PropertyChangedEventHandler<IReelViewModel> OnPropertyChanged;
		public SymbolConfig decorativeSymbol => model.decorativeSymbol;
		public SymbolConfig selectedSymbol => model.selectedSymbol;

		[SerializeField] private ReelView reelView;

		private IPlayerSoundsView playerSoundsView;
		private IReelModel model;
		private ISlotMachineViewModel slotMachineViewModel;

		internal void Initialize(GameplayConfig gameplayConfig, IPlayerSoundsView playerSoundsView, IReelModel reelModel, ISlotMachineViewModel slotMachineViewModel)
		{
			this.slotMachineViewModel = slotMachineViewModel;
			this.playerSoundsView = playerSoundsView;
			model = reelModel;
			model.OnPropertyChanged += PropertyChangedHandler;

			OnPropertyChanged += reelView.PropertyChangedHandler;
			slotMachineViewModel.OnPropertyChanged += reelView.PropertyChangedHandler;
			OnPropertyChanged += playerSoundsView.PropertyChangedHandler;
		}

		private void OnDestroy()
		{
			if (model != null)
				model.OnPropertyChanged -= PropertyChangedHandler;

			OnPropertyChanged -= reelView.PropertyChangedHandler;
			OnPropertyChanged -= playerSoundsView.PropertyChangedHandler;
			slotMachineViewModel.OnPropertyChanged -= reelView.PropertyChangedHandler;
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}

		public void PropertyChangedHandler(IReelModel sender, string propertyName)
		{
			RiseOnPropertyChanged(propertyName);
		}
	}
}
