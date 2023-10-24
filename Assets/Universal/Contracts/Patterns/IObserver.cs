namespace Assets.CommonNashet.Contracts.Patterns
{
	public interface IObserver
	{
		// Receive update from subject
		void Update(IObservable subject);
	}
}
