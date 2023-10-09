using UnityEngine;

namespace Nashet.SlotMachine.Configs
{
	[CreateAssetMenu(fileName = nameof(SoundConfig), menuName = "Solution/" + nameof(SoundConfig))]
	public class SoundConfig : ScriptableObject
	{
		public AudioClip clip;
		public int priority;
	}
}
