using Assets.Nashet.Scripts.Contracts.Models;
using Assets.Nashet.Scripts.Data.Configs;
using Assets.Nashet.Scripts.Universal.Contracts.Patterns;

namespace Assets.Nashet.Scripts.Contracts.ViewModels
{
	public interface IReelViewModel : IPropertyChangeNotifier<IReelViewModel>, ISubscriber<IReelModel>
	{
		SymbolData decorativeSymbol { get; }
		SymbolData selectedSymbol { get; }
	}
}