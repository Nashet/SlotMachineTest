using Nashet.SlotMachine.Configs;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	/// <summary>
	/// Represents visualization of a symbol in slot machine
	/// </summary>
	public interface ISymbolView
	{
		SymbolConfig SymbolConfig { get; set; }
	}
}
a