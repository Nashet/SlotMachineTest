using Nashet.SlotMachine.Gameplay.Contracts;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nashet.SlotMachine.Gameplay.Views
{
	public class ReelView : MonoBehaviour, IReelView
	{
		[SerializeField] private Image image;
		[SerializeField] private float transitionDuration;
		[SerializeField] private Vector3 targetColor;

		private WaitForSeconds waitForSeconds;

		private void Awake()
		{
			waitForSeconds = new WaitForSeconds(transitionDuration);
		}

		public void PropertyChangedHandler(IReelViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IReelViewModel.decorativeSymbol))
				image.sprite = sender.decorativeSymbol.sprite;
		}

		public void PropertyChangedHandler(ISlotMachineViewModel sender, string propertyName)
		{
			if (propertyName == nameof(ISlotMachineViewModel.extraBonusSum))
			{
				StartCoroutine(TransitionColor());
			}
		}

		private IEnumerator TransitionColor()
		{
			yield return waitForSeconds;
			float elapsedTime = 0f;
			while (elapsedTime < transitionDuration)
			{
				float t = elapsedTime / transitionDuration;
				transform.localScale = Vector3.Lerp(Vector3.one, targetColor, t);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			// Ensure the final scale matches the target scale exactly
			transform.localScale = Vector3.one;
		}
	}
}
