using Assets.CommonNashet.Contracts.Patterns;

namespace Assets.CommonNashet.Contracts.Models
{
	public interface IApplicationState : IState, IUpdatable
	{
		bool IsFinished { get; }
		bool IsFailed();
	}
}