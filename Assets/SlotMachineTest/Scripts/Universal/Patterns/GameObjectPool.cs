using Nashet.Contracts.Patterns;
using System.Collections.Generic;
using UnityEngine;

namespace Nashet.Patterns
{
	/// <summary>
	/// Simple implementation of object pool pattern
	/// </summary>
	public class GameObjectPool : MonoBehaviour, IGameObjectPool
	{
		[SerializeField] private GameObject prefab;
		[SerializeField] private int initialPoolSize;

		private List<GameObject> pool;

		private void Awake()
		{
			pool = new List<GameObject>();

			for (int i = 0; i < initialPoolSize; i++)
			{
				CreatePooledObject();
			}
		}

		private GameObject CreatePooledObject()
		{
			GameObject obj = Instantiate(prefab, transform);
			obj.SetActive(false);
			pool.Add(obj);
			return obj;
		}

		public GameObject GetPooledObject()
		{
			for (int i = 0; i < pool.Count; i++)
			{
				if (!pool[i].activeInHierarchy)
				{
					return pool[i];
				}
			}

			// If no inactive object found, create a new one
			return CreatePooledObject();
		}

		public void ReturnToPool(GameObject obj)
		{
			obj.SetActive(false);
		}
	}
}