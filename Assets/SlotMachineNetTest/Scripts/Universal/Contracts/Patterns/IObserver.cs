namespace Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns
{
	public interface IObserver
	{
		// Receive update from subject
		void Update(IObservable subject);
	}
}
