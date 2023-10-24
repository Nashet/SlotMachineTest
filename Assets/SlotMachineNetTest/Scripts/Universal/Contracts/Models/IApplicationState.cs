using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Models
{
	public interface IApplicationState : IState, IUpdatable
	{
		bool IsFinished { get; }
		bool IsFailed();
	}
}