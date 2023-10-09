using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
		[FormerlySerializedAs("collectableObjectTypes")]
		public List<SymbolConfig> availableSymbols;
	}
}

