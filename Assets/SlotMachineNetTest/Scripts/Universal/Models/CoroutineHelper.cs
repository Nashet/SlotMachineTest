using System.Collections;
using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Models
{
	public class CoroutineHelper : MonoBehaviour
	{
		public Coroutine MakeCoroutine(IEnumerator routine)
		{
			return StartCoroutine(routine);
		}
	}
}
