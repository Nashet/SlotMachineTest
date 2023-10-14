using Assets.Nashet.Scripts.Data.Configs;
using Assets.Nashet.Scripts.Universal.Contracts.Patterns;
using System.Collections.Generic;

namespace Assets.Nashet.Scripts.Contracts.Models
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