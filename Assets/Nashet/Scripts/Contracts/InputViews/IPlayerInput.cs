using System;

namespace Assets.Nashet.Scripts.Contracts.InputViews
{
	public interface IPlayerInput
	{
		Action OnSpinButtonClicked { get; }
	}
}