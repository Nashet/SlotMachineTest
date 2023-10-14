using Assets.Nashet.Scripts.Data.Configs;

namespace Assets.Nashet.Scripts.Contracts.Views
{
	/// <summary>
	/// Represents visualization of a symbol in slot machine
	/// </summary>
	public interface ISymbolView
	{
		SymbolData symbolConfig { get; set; }
	}
}