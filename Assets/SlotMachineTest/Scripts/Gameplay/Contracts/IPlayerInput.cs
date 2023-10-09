using System;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface IPlayerInput
	{
		Action OnSpinButtonClicked { get; set; }
	}
}