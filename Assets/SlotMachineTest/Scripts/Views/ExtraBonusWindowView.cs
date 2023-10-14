﻿using Nashet.Contracts.View;
using Nashet.Contracts.ViewModel;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nashet.Views
{
	public class ExtraBonusWindowView : MonoBehaviour, IExtraBonusWindowView
	{
		[SerializeField] private Text prizeText;
		[SerializeField] private string prizeTextTemplate;
		[SerializeField] private float openingDelay;
		[SerializeField] private CanvasGroup canvasGroup;

		public void PropertyChangedHandler(IExtraBonusWindowViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IExtraBonusWindowViewModel.extraBonusSum))
			{
				StartCoroutine(StartWithDelay(sender));
			}
		}

		private IEnumerator StartWithDelay(IExtraBonusWindowViewModel sender)
		{
			yield return new WaitForSeconds(openingDelay);
			ExtraBonusHandler(sender);
		}

		private void ExtraBonusHandler(IExtraBonusWindowViewModel sender)
		{
			canvasGroup.alpha = 1f;
			canvasGroup.blocksRaycasts = true;
			SetText(string.Format(prizeTextTemplate, sender.extraBonusSum));
		}

		public void RevealPrizeButtonClickedHandler()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = false;
		}

		private void SetText(string text)
		{
			prizeText.text = text;
		}

		public void SubscribeTo(IExtraBonusWindowViewModel sender)
		{
			sender.OnPropertyChanged += PropertyChangedHandler;
		}

		public void UnSubscribeFrom(IExtraBonusWindowViewModel sender)
		{
			sender.OnPropertyChanged -= PropertyChangedHandler;
		}
	}
}
