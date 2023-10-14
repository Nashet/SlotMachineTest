using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Data.Configs;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface ISlotMachineModel : IPropertyChangeNotifier<ISlotMachineModel>
	{
		int lastSpinScores { get; }

		IList<IReelModel> reelModelsList { get; }
		SymbolData symbolConfig { get; }
		float extraBonusSum { get; }

		void HandleReelStop(SymbolData selectedSymbol);
		void StartNewRound();
	}
}