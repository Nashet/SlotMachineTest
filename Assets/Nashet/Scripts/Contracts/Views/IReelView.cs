using Assets.Nashet.Scripts.Contracts.ViewModels;
using Assets.Nashet.Scripts.Universal.Contracts.Patterns;

namespace Assets.Nashet.Scripts.Contracts.Views
{
	public interface IReelView : ISubscriber<IReelViewModel>, ISubscriber<ISlotMachineViewModel>
	{
	}
}