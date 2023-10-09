using Nashet.Contracts.Patterns;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface ISlotMachineViewModel : IPropertyChangeNotifier<ISlotMachineViewModel>, ISubscriber<IReelViewModel>
	{
		int lastSpinScores { get; }
		float extraBonusSum { get; }
	}
}