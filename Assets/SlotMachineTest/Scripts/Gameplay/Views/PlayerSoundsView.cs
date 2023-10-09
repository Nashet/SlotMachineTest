using Nashet.SlotMachine.Gameplay.Contracts;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.Views
{
	/// <summary>
	/// The only purpose of that class is to play sounds
	/// </summary>
	public class PlayerSoundsView : MonoBehaviour, IPlayerSoundsView
	{
		[SerializeField] private AudioClip threeInARow;
		[SerializeField] private AudioClip reelTick;

		private AudioSource audioSource;

		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void PlaySound(AudioClip soundClip)
		{
			audioSource.clip = soundClip;
			audioSource.Play();
		}

		public void PropertyChangedHandler(ISlotMachineViewModel sender, string propertyName)
		{
			if (propertyName == nameof(ISlotMachineViewModel.lastSpinScores))
			{
				if (threeInARow != null && sender.lastSpinScores != 0)
				{
					PlaySound(threeInARow);
				}
			}
		}

		public void PropertyChangedHandler(IReelViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IReelViewModel.decorativeSymbol))
			{
				if (reelTick != null)
				{
					PlaySound(reelTick);
				}
			}
		}
	}
}
