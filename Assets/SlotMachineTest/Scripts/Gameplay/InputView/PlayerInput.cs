using Nashet.SlotMachine.Gameplay.Contracts;
using System;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.InputView
{
	/// <summary>
	/// The only goal is to handle player input
	/// </summary>
	public class PlayerInput : MonoBehaviour, IPlayerInput
	{
		public Action OnSpinButtonClicked;

		Action IPlayerInput.OnSpinButtonClicked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void RiseOnPropertyChanged()
		{
			OnSpinButtonClicked?.Invoke();
		}

		public void OnSpinButtonClickedHandler()
		{
			RiseOnPropertyChanged();
		}
	}
}
