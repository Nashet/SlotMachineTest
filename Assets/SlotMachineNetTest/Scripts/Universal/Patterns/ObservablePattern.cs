using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using System;
using System.Collections.Generic;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Universal.Patterns
{
	/// <summary>
	/// That is another implementation of observable pattern
	/// Comparing to IPropertyChangeNotifier:
	/// requires much less writing, but it requires class cast.
	/// And since its not attached to particular class, there is a chance of wrong type subscription 
	/// !!It Doesnt say which property was changed!!
	/// </summary>
	public abstract class ObservablePattern : IObservable
	{
		// List of subscribers. In real life, the list of subscribers can be
		// stored more comprehensively (categorized by event type, etc.).
		private List<IObserver> _observers = new List<IObserver>();

		// The subscription management methods.
		public void Attach(IObserver observer)
		{
			Console.WriteLine("Subject: Attached an observer.");
			this._observers.Add(observer);
		}

		public void Detach(IObserver observer)
		{
			this._observers.Remove(observer);
			Console.WriteLine("Subject: Detached an observer.");
		}

		// Trigger an update in each subscriber.
		public void Notify()
		{
			Console.WriteLine("Subject: Notifying observers...");

			foreach (var observer in _observers)
			{
				observer.Update(this);
			}
		}
	}
}