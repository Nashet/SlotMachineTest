namespace Nashet.Contracts.Patterns
{
	public interface IFakeRandomStrategy<T>
	{
		bool IsFinished { get; }
		T Get();
		void Reset();
	}
}
