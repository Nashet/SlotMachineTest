using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Models;

namespace Assets.SlotMachineNetTest.Scripts.Models.ApplicationState
{
	public class DefaultApplicationState : IApplicationState
	{
		public bool IsFinished => true;

		public void EnteredStateHandler()
		{
		}

		public bool IsFailed() => false;

		public void Update()
		{
		}
	}
}

