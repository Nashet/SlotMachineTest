using Assets.SlotMachineNetTest.Scripts.Data.Configs;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.Views
{
	/// <summary>
	/// Represents visualization of a symbol in slot machine
	/// </summary>
	public interface ISymbolView
	{
		SymbolData symbolConfig { get; set; }
	}
}