using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Models.ApplicationState
{
	public class AbortApplicationState : ApplicationState
	{
		public override bool IsFinished => true;

		public override string ToString()
		{
			return nameof(AbortApplicationState);
		}
		public override void EnteredStateHandler()
		{
			Debug.LogError("Application has failed. Unloading... bip bop");
		}

		public override bool IsFailed() => false;

		public override void Update()
		{
		}
	}
}

