using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.CommonNashet.Contracts.Patterns;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.Views
{
	public interface IReelView : IPropertyChangeSubscriber<IReelViewModel>, IPropertyChangeSubscriber<ISlotMachineViewModel>
	{
	}
}