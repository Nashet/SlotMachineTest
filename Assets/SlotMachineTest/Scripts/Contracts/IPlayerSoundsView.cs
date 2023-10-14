using Nashet.Contracts.Patterns;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface IPlayerSoundsView : ISubscriber<ISlotMachineViewModel>, ISubscriber<IReelViewModel>
	{
	}
}