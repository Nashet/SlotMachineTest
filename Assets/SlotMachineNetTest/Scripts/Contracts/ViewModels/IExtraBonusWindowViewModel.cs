using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.CommonNashet.Contracts.Patterns;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels
{
	public interface IExtraBonusWindowViewModel : IPropertyChangeSubscriber<ISlotMachineModel>, IPropertyChangeNotifier<IExtraBonusWindowViewModel>
	{
		float extraBonusSum { get; }
	}
}