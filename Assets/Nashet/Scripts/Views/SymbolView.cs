using Assets.Nashet.Scripts.Contracts.Views;
using Assets.Nashet.Scripts.Data.Configs;
using UnityEngine;

namespace Assets.Nashet.Scripts.Views
{
	/// <summary>
	/// The purpose of this class is to handle visuals of symbols
	/// and to keep connection with the config of that symbol
	/// </summary>
	public class SymbolView : MonoBehaviour, ISymbolView
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
