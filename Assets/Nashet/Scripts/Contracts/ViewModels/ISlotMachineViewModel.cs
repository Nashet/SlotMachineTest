using Assets.Nashet.Scripts.Contracts.Models;
using Assets.Nashet.Scripts.Universal.Contracts.Patterns;

namespace Assets.Nashet.Scripts.Contracts.ViewModels
{
	public interface ISlotMachineViewModel : IPropertyChangeNotifier<ISlotMachineViewModel>, ISubscriber<IReelViewModel>, ISubscriber<ISlotMachineModel>
	{
		int lastSpinScores { get; }
		float extraBonusSum { get; }
	}
}