using System.Collections;
using UnityEngine;

namespace Assets.CommonNashet.Models
{
	public class CoroutineHelper : MonoBehaviour
	{
		public Coroutine MakeCoroutine(IEnumerator routine)
		{
			return StartCoroutine(routine);
		}
	}
}
