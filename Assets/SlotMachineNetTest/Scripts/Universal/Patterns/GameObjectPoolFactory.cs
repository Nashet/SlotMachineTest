﻿using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Universal.Patterns
{
	/// <summary>
	/// That implementation of IGameObjectFactory uses some implementation of object pool pattern
	/// </summary>
	public class GameObjectPoolFactory : IGameObjectFactory
	{
		private readonly IGameObjectPool objectPool;

		public GameObjectPoolFactory()
		{
		}

		public GameObjectPoolFactory(IGameObjectPool pool)
		{
			objectPool = pool;
		}

		public GameObject CreateObject()
		{
			GameObject obj = objectPool.GetPooledObject();

			return obj;
		}

		public void DestroyObject(GameObject obj)
		{
			objectPool.ReturnToPool(obj);
		}
	}
}