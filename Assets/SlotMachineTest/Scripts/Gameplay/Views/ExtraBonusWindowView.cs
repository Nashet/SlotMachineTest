using Nashet.SlotMachine.Gameplay.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace Nashet.SlotMachine.Gameplay.Views
{
	public class ExtraBonusWindowView : MonoBehaviour, IExtraBonusWindowView
	{
		[SerializeField] private Text prizeText;
		[SerializeField] private string prizeTextTemplate;

		public void PropertyChangedHandler(IExtraBonusWindowViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IExtraBonusWindowViewModel.extraBonusSum))
			{
				ExtraBonusHandler(sender);
			}
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
	}
}
