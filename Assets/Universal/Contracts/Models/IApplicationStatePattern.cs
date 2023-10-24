using Assets.CommonNashet.Contracts.Patterns;

namespace Assets.CommonNashet.Contracts.Models
{
	public interface IApplicationStatePattern : IStatePattern<IApplicationState>, IUpdatable
	{
	}
}