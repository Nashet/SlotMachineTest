using System.Collections.Generic;
using System;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns
{
	public class ReactiveProperty<T>
	{
		private T value;
		private Action<T> onChanged;

		public T Value
		{
			get { return value; }
			set
			{
				if (!EqualityComparer<T>.Default.Equals(this.value, value))
				{
					this.value = value;
					onChanged?.Invoke(value);
				}
			}
		}

		public void Subscribe(Action<T> callback)
		{
			onChanged += callback;
		}

		public void Unsubscribe(Action<T> callback)
		{
			onChanged -= callback;
		}
	}
}