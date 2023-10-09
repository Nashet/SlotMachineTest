﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Nashet.SlotMachine.Configs
{
	/// <summary>
	/// The only purpose of this class is to hold configs for symbols
	/// </summary>
	[CreateAssetMenu(fileName = nameof(SymbolConfig), menuName = "Solution/" + nameof(SymbolConfig))]
	public class SymbolConfig : ScriptableObject
	{
		public Sprite sprite;
		[FormerlySerializedAs("onCollectedSound")]
		public AudioClip winSound;
		public int prize3InRow;
	}
}
