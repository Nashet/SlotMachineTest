using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.CommonNashet.Contracts.Patterns;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.Models
{
	public interface IReelModel : IPropertyChangeNotifier<IReelModel>
	{
		SymbolData decorativeSymbol { get; }
		SymbolData selectedSymbol { get; }
		void StartNewRound();
	}
}