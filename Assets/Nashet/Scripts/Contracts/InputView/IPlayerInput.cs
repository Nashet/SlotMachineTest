using System;

namespace Nashet.Contracts.InputView
{
	public interface IPlayerInput
	{
		Action OnSpinButtonClicked { get; }
	}
}