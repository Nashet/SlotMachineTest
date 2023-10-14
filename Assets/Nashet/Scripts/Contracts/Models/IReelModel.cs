using Assets.Nashet.Scripts.Data.Configs;
using Assets.Nashet.Scripts.Universal.Contracts.Patterns;

namespace Assets.Nashet.Scripts.Contracts.Models
{
	public interface IReelModel : IPropertyChangeNotifier<IReelModel>
	{
		SymbolData decorativeSymbol { get; }
		SymbolData selectedSymbol { get; }
		void StartNewRound();
	}
}