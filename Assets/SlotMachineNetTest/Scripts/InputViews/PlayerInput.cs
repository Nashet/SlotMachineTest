using Assets.SlotMachineNetTest.Scripts.Contracts.InputViews;
using System;
using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.InputViews
{
	/// <summary>
	/// The only goal is to handle player input
	/// </summary>
	public class PlayerInput : MonoBehaviour, IPlayerInput
	{
		public Action OnSpinButtonClicked { get; set; }

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
