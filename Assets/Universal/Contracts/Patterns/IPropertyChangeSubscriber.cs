namespace Assets.CommonNashet.Contracts.Patterns
{
	/// <summary>
	/// use it togther with IPropertyChangeNotifier
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IPropertyChangeSubscriber<T>
	{
		void PropertyChangedHandler(T sender, string propertyName);
		void SubscribeTo(T sender);
		void UnSubscribeFrom(T sender);
	}
}
