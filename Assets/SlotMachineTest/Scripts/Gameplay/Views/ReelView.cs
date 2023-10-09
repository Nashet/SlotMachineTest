using Nashet.SlotMachine.Gameplay.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace Nashet.SlotMachine.Gameplay.Views
{
	public class ReelView : MonoBehaviour, IReelView
	{
		[SerializeField] private Image image;
		public void PropertyChangedHandler(IReelViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IReelViewModel.currentSymbol))
				image.sprite = sender.currentSymbol.sprite;
		}
	}
}
