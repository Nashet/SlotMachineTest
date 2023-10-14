using Nashet.Contracts.Model;
using Nashet.Contracts.Patterns;

namespace Nashet.Contracts.ViewModel
{
	public interface IExtraBonusWindowViewModel : ISubscriber<ISlotMachineModel>, IPropertyChangeNotifier<IExtraBonusWindowViewModel>
	{
		float extraBonusSum { get; }

		void Initialize(ISlotMachineModel slotMachineModel);
	}
}