using Nashet.Contracts.Model;
using Nashet.Contracts.Patterns;
using Nashet.Data.Configs;

namespace Nashet.Contracts.ViewModel
{
	public interface IReelViewModel : IPropertyChangeNotifier<IReelViewModel>, ISubscriber<IReelModel>
	{
		SymbolData decorativeSymbol { get; }
		SymbolData selectedSymbol { get; }
	}
}