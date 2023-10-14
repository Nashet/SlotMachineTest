using System.Collections.Generic;
using UnityEngine;

namespace Nashet.Data.Configs
{
	/// <summary>
	/// The purpose of this class is to store configs related to general gameplay.
	/// </summary>
	[CreateAssetMenu(fileName = nameof(GameplayData), menuName = "Solution/" + nameof(GameplayData))]
	public class GameplayData : ScriptableObject
	{
		public int amountOfDecorateSymbolsPerSpin;
		public int randomAmountOfDecorateSymbolsPerSpin;

		public float oneSymbolSpinTime;
		public List<SymbolData> availableSymbols;
		public List<SymbolData> extraBonusSymbol;
		public float extraBonusMultiplier;
		public string WSURL = "http://localhost:3000";
	}
}
//todo remove project name from namespace
////todo  add basic class for yeach type?
//todo add rective property
//todo add assemblies
//todocheck non event communictaiein
