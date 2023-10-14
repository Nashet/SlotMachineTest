using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns
{
	/// <summary>
	/// Represents factory pattern. Creates standart gameobjects
	/// </summary>
	public interface IGameObjectFactory
	{
		GameObject CreateObject();
		void DestroyObject(GameObject obj);
	}
}