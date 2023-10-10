using Nashet.SlotMachine.Gameplay.Contracts;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Nashet.SlotMachine.Gameplay.Views
{
	public class ExtraBonusWindowView : MonoBehaviour, IExtraBonusWindowView
	{
		[SerializeField] private Text prizeText;
		[SerializeField] private string prizeTextTemplate;
		[SerializeField] private float openingDelay;

		public void PropertyChangedHandler(IExtraBonusWindowViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IExtraBonusWindowViewModel.extraBonusSum))
			{
				StartWithDelay(sender);
			}
		}

		private async void StartWithDelay(IExtraBonusWindowViewModel sender)
		{
			await Task.Delay((int)(openingDelay * 1000f));
			ExtraBonusHandler(sender);
		}

		private void ExtraBonusHandler(IExtraBonusWindowViewModel sender)
		{
			gameObject.SetActive(true);
			SetText(string.Format(prizeTextTemplate, sender.extraBonusSum));
		}

		public void RevealPrizeButtonClickedHandler()
		{
			gameObject.SetActive(false);
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
