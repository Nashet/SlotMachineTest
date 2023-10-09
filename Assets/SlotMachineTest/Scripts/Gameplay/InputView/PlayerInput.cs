using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Gameplay.Contracts;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.InputView
{
	/// <summary>
	/// The only goal is to handle player input
	/// </summary>
	public class PlayerInput : MonoBehaviour, IPlayerInput
	{
		public event PropertyChangedEventHandler<IPlayerInput> OnPropertyChanged;

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}

		private void Update()
		{
			if (Input.anyKeyDown)
			{
				RiseOnPropertyChanged(nameof(PlayerInput));
			}
		}
	}
}
