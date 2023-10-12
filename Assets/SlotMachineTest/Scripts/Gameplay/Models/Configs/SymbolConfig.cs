using UnityEngine;

namespace Nashet.SlotMachine.Configs
{
	/// <summary>
	/// The only purpose of this class is to hold configs for symbols
	/// </summary>
	[CreateAssetMenu(fileName = nameof(SymbolConfig), menuName = "Solution/" + nameof(SymbolConfig))]
	public class SymbolConfig : ScriptableObject
	{
		public string id; //should be uniqe, might be used for serialization
		public Sprite sprite;
		public int prize3InRow;
	}
}
