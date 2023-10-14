using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Contracts.Views;
using Assets.SlotMachineNetTest.Scripts.Universal.Views;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SlotMachineNetTest.Scripts.Views
{
	public class ReelView : MonoView, IReelView
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

		public void SubscribeTo(IReelViewModel sender)
		{
			sender.OnPropertyChanged += PropertyChangedHandler;
		}

		public void SubscribeTo(ISlotMachineViewModel sender)
		{
			sender.OnPropertyChanged += PropertyChangedHandler;
		}

		public void UnSubscribeFrom(IReelViewModel sender)
		{
			sender.OnPropertyChanged -= PropertyChangedHandler;
		}

		public void UnSubscribeFrom(ISlotMachineViewModel sender)
		{
			sender.OnPropertyChanged -= PropertyChangedHandler;
		}
	}
}
