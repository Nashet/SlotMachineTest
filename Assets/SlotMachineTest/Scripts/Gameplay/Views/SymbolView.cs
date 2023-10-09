using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.Views
{
	/// <summary>
	/// The purpose of this class is to handle visuals of symbols
	/// and to keep connection with the config of that symbol
	/// </summary>
	public class SymbolView : MonoBehaviour, ISymbolView
	{
		public SymbolConfig symbolConfig
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
