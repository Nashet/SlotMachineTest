using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.Views
{
	public interface IReelView : ISubscriber<IReelViewModel>, ISubscriber<ISlotMachineViewModel>
	{
	}
}