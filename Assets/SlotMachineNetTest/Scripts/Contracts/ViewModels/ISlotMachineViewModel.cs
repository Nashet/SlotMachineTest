using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels
{
	public interface ISlotMachineViewModel : IPropertyChangeNotifier<ISlotMachineViewModel>, ISubscriber<IReelViewModel>, ISubscriber<ISlotMachineModel>
	{
		int lastSpinScores { get; }
		float extraBonusSum { get; }
	}
}