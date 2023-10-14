using System.Collections;
using UnityEngine;

namespace Nashet.Models
{
	public class CoroutineHelper : MonoBehaviour
	{
		public Coroutine MakeCoroutine(IEnumerator routine)
		{
			return StartCoroutine(routine);
		}
	}
}
