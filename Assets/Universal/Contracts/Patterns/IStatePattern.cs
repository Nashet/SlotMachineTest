namespace Assets.CommonNashet.Contracts.Patterns
{
	public delegate void StateChangedDelegate<T>(T newState);

	public interface IStatePattern<T> where T : IState
	{
		T State { get; }

		event StateChangedDelegate<T> OnStateChanged;

		void ChangeStateTo(T state);
	}
}