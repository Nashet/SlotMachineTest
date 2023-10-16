using System.Collections.Generic;
using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Data.Configs
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
		public float openingExtraPrizeWindowDelay = 1f;
		public string prizeWindowTextTemplate = "Norm, mo money for me {0}";
	}
}

//todo add zenject 
//todo add rective property
//todo add assemblies
//todo check non event communictaiein
