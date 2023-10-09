using System.Collections.Generic;
using UnityEngine;

namespace Nashet.SlotMachine.Configs
{
	/// <summary>
	/// The only purpose of this class is to hold all other configs
	/// </summary>
	[CreateAssetMenu(fileName = nameof(ConfigHolder), menuName = "Solution/" + nameof(ConfigHolder))]
	public class ConfigHolder : ScriptableObject
	{
		public List<ScriptableObject> AllConfigs;
	}
}
