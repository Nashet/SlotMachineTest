using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Contracts.Views;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.SlotMachineNetTest.Scripts.Universal.Views;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.SlotMachineNetTest.Scripts.Views
{
	public class ExtraBonusWindowView : MonoView, IExtraBonusWindowView
	{
		[SerializeField] private Text prizeText;
		private string prizeTextTemplate => gameplayData.prizeWindowTextTemplate;
		private float openingDelay => gameplayData.openingExtraPrizeWindowDelay;
		[SerializeField] private CanvasGroup canvasGroup;
		private IExtraBonusWindowViewModel extraBonusWindowViewModel;
		private GameplayData gameplayData;

		[Inject]
		private void Construct(GameplayData gameplayData, IExtraBonusWindowViewModel extraBonusWindowViewModel)
		{
			this.extraBonusWindowViewModel = extraBonusWindowViewModel;
			this.gameplayData = gameplayData;
			SubscribeTo(extraBonusWindowViewModel);
		}

		private void OnDestroy()
		{
			UnSubscribeFrom(extraBonusWindowViewModel);
		}

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
