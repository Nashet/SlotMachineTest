using Nashet.Contracts.Patterns;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface IExtraBonusWindowViewModel : ISubscriber<ISlotMachineModel>, IPropertyChangeNotifier<IExtraBonusWindowViewModel>
	{
		float extraBonusSum { get; }

		void Initialize(ISlotMachineModel slotMachineModel);
	}
}