using UnityEngine;

namespace Nashet.Data.Configs
{
	/// <summary>
	/// The only purpose of this class is to hold configs for symbols
	/// </summary>
	[CreateAssetMenu(fileName = nameof(SymbolData), menuName = "Solution/" + nameof(SymbolData))]
	public class SymbolData : ScriptableObject
	{
		public string id; //should be uniqe, might be used for serialization
		public Sprite sprite;
		public int prize3InRow;
	}
}
