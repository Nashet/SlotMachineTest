using Nashet.Contracts.Patterns;
using Nashet.Contracts.ViewModel;

namespace Nashet.Contracts.View
{
	public interface IReelView : ISubscriber<IReelViewModel>, ISubscriber<ISlotMachineViewModel>
	{
	}
}