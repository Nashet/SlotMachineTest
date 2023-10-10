using System.Collections;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.Models
{
	public class CoroutineHelper : MonoBehaviour
	{
		public Coroutine MakeCoroutine(IEnumerator routine)
		{
			return StartCoroutine(routine);
		}
	}
}
