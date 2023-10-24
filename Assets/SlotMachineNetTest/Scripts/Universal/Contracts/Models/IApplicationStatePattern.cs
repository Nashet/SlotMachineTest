using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Models
{
	public interface IApplicationStatePattern : IStatePattern<IApplicationState>, IUpdatable
	{
	}
}