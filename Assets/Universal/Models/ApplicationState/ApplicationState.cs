using Assets.CommonNashet.Contracts.Models;

namespace Assets.SlotMachineNetTest.Scripts.Models.ApplicationState
{
	public abstract class ApplicationState : IApplicationState
	{
		public abstract bool IsFinished { get; }

		public abstract override string ToString();
		public abstract void EnteredStateHandler();
		public abstract bool IsFailed();
		public abstract void Update();
	}
}

