using Nashet.Contracts;
using Nashet.Contracts.InputView;
using System;
using UnityEngine;

namespace Nashet.InputView
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
