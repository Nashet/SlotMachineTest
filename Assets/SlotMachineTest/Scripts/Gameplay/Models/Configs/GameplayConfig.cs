﻿using System.Collections.Generic;
using UnityEngine;

namespace Nashet.SlotMachine.Configs
{
	/// <summary>
	/// The purpose of this class is to store configs related to general gameplay.
	/// </summary>
	[CreateAssetMenu(fileName = nameof(GameplayConfig), menuName = "Solution/" + nameof(GameplayConfig))]
	public class GameplayConfig : ScriptableObject
	{
		public int amountOfDecorateSymbolsPerSpin;
		public int randomAmountOfDecorateSymbolsPerSpin;

		public float oneSymbolSpinTime;
		public List<SymbolConfig> availableSymbols;
		public List<SymbolConfig> extraBonusSymbol;
		public float extraBonusMultiplier;
		public string WSURL = "http://localhost:3000";
	}
}
