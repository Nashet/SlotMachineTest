using UnityEngine;

namespace Assets.Nashet.Scripts.Data.Configs
{
	[CreateAssetMenu(fileName = nameof(SoundData), menuName = "Solution/" + nameof(SoundData))]
	public class SoundData : ScriptableObject
	{
		public AudioClip clip;
		public int priority;
	}
}
