using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using Assets.SlotMachineNetTest.Scripts.Universal.ViewModels;
using System;

namespace Assets.SlotMachineNetTest.Scripts.ViewModels
{
	public class ExtraBonusWindowViewModel : ViewModel, IExtraBonusWindowViewModel, IDisposable
	{
		public event PropertyChangedEventHandler<IExtraBonusWindowViewModel> OnPropertyChanged;

		public float extraBonusSum => slotMachineModel.extraBonusSum;

		private ISlotMachineModel slotMachineModel;

		public ExtraBonusWindowViewModel(ISlotMachineModel slotMachineModel)
		{
			this.slotMachineModel = slotMachineModel;
			SubscribeTo(slotMachineModel);
		}

		public void PropertyChangedHandler(ISlotMachineModel sender, string propertyName)
		{
			if (propertyName == nameof(ISlotMachineModel.extraBonusSum))
			{
				if (sender.extraBonusSum != 0)
				{
					RiseOnPropertyChanged(nameof(extraBonusSum));
				}
			}
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
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
			this.UnSubscribeFrom(slotMachineModel);
		}
	}
}