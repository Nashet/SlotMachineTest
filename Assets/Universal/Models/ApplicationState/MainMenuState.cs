using Assets.CommonNashet.Contracts.Models;

namespace Assets.SlotMachineNetTest.Scripts.Models.ApplicationState
{
	public class NoMainMenuState : MainMenuState
	{
	}
	public class MainMenuState : IApplicationState
	{
		public MainMenuState()
		{
		}

		bool IApplicationState.IsFinished => true;

		public void EnteredStateHandler()
		{

		}

		public bool IsFailed() => false;


		public void Update()
		{
		}
	}
}

