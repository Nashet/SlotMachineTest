namespace Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns
{
	public delegate void PropertyChangedEventHandler<T>(T sender, string propertyName);

	/// <summary>
	/// Pupose of that interface is to give common base for view models or other notifiers
	/// </summary>
	public interface IPropertyChangeNotifier<T>
	{
		event PropertyChangedEventHandler<T> OnPropertyChanged;
		void RiseOnPropertyChanged(string propertyName);
	}
}