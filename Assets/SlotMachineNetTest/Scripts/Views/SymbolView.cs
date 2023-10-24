using Assets.SlotMachineNetTest.Scripts.Contracts.Views;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.CommonNashet.Views;

namespace Assets.SlotMachineNetTest.Scripts.Views
{
	/// <summary>
	/// The purpose of this class is to handle visuals of symbols
	/// and to keep connection with the config of that symbol
	/// </summary>
	public class SymbolView : MonoView, ISymbolView
	{
		public SymbolData symbolConfig
		{
			get
			{
				return symbolConfig;
			}
			set
			{
				symbolConfig = value;
			}
		}
	}
}
