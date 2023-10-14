using Nashet.Contracts.Patterns;
using Nashet.Contracts.ViewModel;

namespace Nashet.Contracts.View
{
	public interface IPlayerSoundsView : ISubscriber<ISlotMachineViewModel>, ISubscriber<IReelViewModel>
	{
	}
}