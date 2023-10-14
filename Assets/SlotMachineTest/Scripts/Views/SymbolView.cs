using Nashet.Contracts;
using Nashet.Contracts.View;
using Nashet.Data.Configs;
using UnityEngine;

namespace Nashet.Views
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
