using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using System;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels
{
	public interface ISlotMachineViewModel : IPropertyChangeNotifier<ISlotMachineViewModel>, ISubscriber<IReelViewModel>, ISubscriber<ISlotMachineModel>, IDisposable
	{
		int lastSpinScores { get; }
		float extraBonusSum { get; }
	}
}