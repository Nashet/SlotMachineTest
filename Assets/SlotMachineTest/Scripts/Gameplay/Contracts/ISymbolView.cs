using Nashet.SlotMachine.Data.Configs;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	/// <summary>
	/// Represents visualization of a symbol in slot machine
	/// </summary>
	public interface ISymbolView
	{
		SymbolData symbolConfig { get; set; }
	}
}