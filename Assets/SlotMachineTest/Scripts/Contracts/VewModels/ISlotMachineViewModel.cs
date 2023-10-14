using Nashet.Contracts.Model;
using Nashet.Contracts.Patterns;

namespace Nashet.Contracts.ViewModel
{
	public interface ISlotMachineViewModel : IPropertyChangeNotifier<ISlotMachineViewModel>, ISubscriber<IReelViewModel>, ISubscriber<ISlotMachineModel>
	{
		int lastSpinScores { get; }
		float extraBonusSum { get; }
	}
}