using Assets.Nashet.Scripts.Contracts.Models;
using Assets.Nashet.Scripts.Universal.Contracts.Patterns;

namespace Nashet.Scripts.Contracts.ViewModels
{
	public interface IExtraBonusWindowViewModel : ISubscriber<ISlotMachineModel>, IPropertyChangeNotifier<IExtraBonusWindowViewModel>
	{
		float extraBonusSum { get; }

		void Initialize(ISlotMachineModel slotMachineModel);
	}
}