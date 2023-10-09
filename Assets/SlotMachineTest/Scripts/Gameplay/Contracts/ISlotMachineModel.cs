using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface ISlotMachineModel : IPropertyChangeNotifier<ISlotMachineModel>
	{
		int lastSpinScores { get; }

		IList<IReelModel> reelModelsList { get; }
		SymbolConfig symbolConfig { get; }
		float extraBonusSum { get; }

		void HandleReelStop(SymbolConfig selectedSymbol);
		void StartNewRound();
	}
}