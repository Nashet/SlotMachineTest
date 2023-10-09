using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface IReelModel
	{
		SymbolConfig decorativeSymbol { get; }
		SymbolConfig selectedSymbol { get; }

		event PropertyChangedEventHandler<IReelModel> OnPropertyChanged;

		void RiseOnPropertyChanged(string propertyName);
		void StartNewRound();
	}
}