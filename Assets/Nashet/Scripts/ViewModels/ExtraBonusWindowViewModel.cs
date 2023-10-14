using Assets.Nashet.Scripts.Contracts.Models;
using Assets.Nashet.Scripts.Contracts.Views;
using Assets.Nashet.Scripts.Universal.Contracts.Patterns;
using Assets.Nashet.Scripts.Universal.ViewModels;
using Nashet.Scripts.Contracts.ViewModels;
using UnityEngine;

namespace Assets.Nashet.Scripts.ViewModels
{
	public class ExtraBonusWindowViewModel : MonoViewModel, IExtraBonusWindowViewModel
	{
		public event PropertyChangedEventHandler<IExtraBonusWindowViewModel> OnPropertyChanged;

		public float extraBonusSum => slotMachineModel.extraBonusSum;

		private ISlotMachineModel slotMachineModel;
		private IExtraBonusWindowView extraBonusWindowView;

		public void Initialize(ISlotMachineModel slotMachineModel)
		{
			this.extraBonusWindowView = GetComponent<IExtraBonusWindowView>();
			this.slotMachineModel = slotMachineModel;
			SubscribeTo(slotMachineModel);
			extraBonusWindowView.SubscribeTo(this);
		}

		private void OnDestroy()
		{
			extraBonusWindowView.UnSubscribeFrom(this);
			this.UnSubscribeFrom(slotMachineModel);
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
	}
}