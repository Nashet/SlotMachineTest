using Nashet.SlotMachine.Gameplay.Contracts;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.Views
{
	/// <summary>
	/// The only purpose of that class is to play sounds
	/// </summary>
	public class PlayerSoundsView : MonoBehaviour, IPlayerSoundsView
	{
		[SerializeField] private AudioClip symbolCollected;

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

		public void CoinCollectedHandler(GameObject obj)
		{
			PlaySound(symbolCollected);
		}

		//public void PropertyChangedHandler(IPlayerViewModel sender, string propertyName)
		//{
		//	if (propertyName == nameof(IPlayerViewModel.playerMovementContext))
		//	{
		//		AudioClip onCollectedSound = sender.playerMovementContext.state.config.onCollectedSound;
		//		if (onCollectedSound != null)
		//		{
		//			PlaySound(onCollectedSound);
		//		}
		//	}
		//}
	}
}
