using System;
using System.Collections;
using System.Collections.Generic;

namespace Socket.Newtonsoft.Json.Utilities.LinqBridge
{
	internal sealed class Lookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable<IGrouping<TKey, TElement>>,
	IEnumerable
	{
		private readonly Dictionary<TKey, IGrouping<TKey, TElement>> _map;

		internal Lookup(IEqualityComparer<TKey> comparer)
		{
			this._map = new Dictionary<TKey, IGrouping<TKey, TElement>>(comparer);
		}

		internal void Add(IGrouping<TKey, TElement> item)
		{
			this._map.Add(item.Key, item);
		}

		internal IEnumerable<TElement> Find(TKey key)
		{
			IGrouping<TKey, TElement> grouping;
			if (!this._map.TryGetValue(key, out grouping))
				return null;
			return grouping;
		}

		public int Count
		{
			get { return this._map.Count; }
		}

		public IEnumerable<TElement> this[TKey key]
		{
			get
			{
				IGrouping<TKey, TElement> grouping;
				if (!this._map.TryGetValue(key, out grouping))
					return Enumerable.Empty<TElement>();
				return grouping;
			}
		}

		public bool Contains(TKey key)
		{
			return this._map.ContainsKey(key);
		}

		public IEnumerable<TResult> ApplyResultSelector<TResult>(
		  Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			if (resultSelector == null)
				throw new ArgumentNullException(nameof(resultSelector));
			foreach (KeyValuePair<TKey, IGrouping<TKey, TElement>> keyValuePair in this._map)
				yield return resultSelector(keyValuePair.Key, keyValuePair.Value);
		}

		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
		{
			return this._map.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}