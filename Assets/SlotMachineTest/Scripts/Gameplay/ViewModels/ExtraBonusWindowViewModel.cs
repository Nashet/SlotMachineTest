using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Gameplay.Contracts;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.ViewModels
{
	public class ExtraBonusWindowViewModel : MonoBehaviour, IExtraBonusWindowViewModel
	{
		public event PropertyChangedEventHandler<IExtraBonusWindowViewModel> OnPropertyChanged;

		public float extraBonusSum => slotMachineModel.extraBonusSum;

		private ISlotMachineModel slotMachineModel;
		private IExtraBonusWindowView extraBonusWindowView;

		public void Initialize(ISlotMachineModel slotMachineModel)
		{
			this.extraBonusWindowView = GetComponent<IExtraBonusWindowView>();
			this.slotMachineModel = slotMachineModel;

			OnPropertyChanged += extraBonusWindowView.PropertyChangedHandler;
			slotMachineModel.OnPropertyChanged += PropertyChangedHandler;
		}

		private void OnDestroy()
		{
			OnPropertyChanged -= extraBonusWindowView.PropertyChangedHandler;
			slotMachineModel.OnPropertyChanged -= PropertyChangedHandler;
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
	}
}