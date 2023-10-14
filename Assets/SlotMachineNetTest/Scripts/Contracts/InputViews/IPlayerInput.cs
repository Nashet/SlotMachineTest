using System;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.InputViews
{
	public interface IPlayerInput
	{
		Action OnSpinButtonClicked { get; }
	}
}