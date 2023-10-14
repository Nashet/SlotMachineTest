using Nashet.Contracts.Patterns;
using Nashet.Data.Configs;

namespace Nashet.Contracts.Model
{
	public interface IReelModel : IPropertyChangeNotifier<IReelModel>
	{
		SymbolData decorativeSymbol { get; }
		SymbolData selectedSymbol { get; }
		void StartNewRound();
	}
}