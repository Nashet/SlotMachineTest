using UnityEngine;

namespace Nashet.SlotMachine.Configs
{
	/// <summary>
	/// The only purpose of this class is to hold configs for symbols
	/// </summary>
	[CreateAssetMenu(fileName = nameof(SymbolConfig), menuName = "Solution/" + nameof(SymbolConfig))]
	public class SymbolConfig : ScriptableObject
	{
		public Sprite sprite;
		public AudioClip onCollectedSound;
		public int prize3InRow;
	}
}
