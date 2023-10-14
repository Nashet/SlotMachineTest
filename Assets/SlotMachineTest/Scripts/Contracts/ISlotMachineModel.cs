using Nashet.Contracts.Patterns;
using Nashet.Data.Configs;
using System.Collections.Generic;

namespace Nashet.Contracts.Model
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