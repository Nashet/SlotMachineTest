using System.Collections.Generic;
using UnityEngine;

namespace Assets.CommonNashet.Data
{
	/// <summary>
	/// The only purpose of this class is to hold all other configs
	/// </summary>
	[CreateAssetMenu(fileName = nameof(ConfigHolderData), menuName = "Solution/" + nameof(ConfigHolderData))]
	public class ConfigHolderData : ScriptableObject
	{
		public List<ScriptableObject> AllConfigs;
	}
}
