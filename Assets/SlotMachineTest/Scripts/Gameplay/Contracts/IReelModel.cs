using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Data.Configs;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface IReelModel : IPropertyChangeNotifier<IReelModel>
	{
		SymbolData decorativeSymbol { get; }
		SymbolData selectedSymbol { get; }
		void StartNewRound();
	}
}