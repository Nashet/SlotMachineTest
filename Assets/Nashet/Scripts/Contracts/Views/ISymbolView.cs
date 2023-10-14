using Nashet.Data.Configs;

namespace Nashet.Contracts.View
{
	/// <summary>
	/// Represents visualization of a symbol in slot machine
	/// </summary>
	public interface ISymbolView
	{
		SymbolData symbolConfig { get; set; }
	}
}