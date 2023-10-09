using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface ISlotMachineModel
	{
		int LastSpinScores { get; }

		event PropertyChangedEventHandler<ISlotMachineModel> OnPropertyChanged;
		IList<IReelModel> reelModelsList { get; }

		void HandleReelStop(SymbolConfig selectedSymbol);
		void RiseOnPropertyChanged(string propertyName);
		void StartNewRound();
	}
}