using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Data.Configs;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface IReelViewModel : IPropertyChangeNotifier<IReelViewModel>, ISubscriber<IReelModel>
	{
		SymbolData decorativeSymbol { get; }
		SymbolData selectedSymbol { get; }
	}
}