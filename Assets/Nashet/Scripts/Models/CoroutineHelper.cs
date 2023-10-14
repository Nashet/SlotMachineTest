using System.Collections;
using UnityEngine;

namespace Assets.Nashet.Scripts.Models
{
	public class CoroutineHelper : MonoBehaviour
	{
		public Coroutine MakeCoroutine(IEnumerator routine)
		{
			return StartCoroutine(routine);
		}
	}
}
