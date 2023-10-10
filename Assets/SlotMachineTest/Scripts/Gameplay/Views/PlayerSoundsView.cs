using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using System.Collections.Generic;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.Views
{
	/// <summary>
	/// The only purpose of that class is to play sounds
	/// </summary>
	public class PlayerSoundsView : MonoBehaviour, IPlayerSoundsView
	{
		//todo put it in gameplay config:
		[SerializeField] private SoundConfig threeInARow;
		[SerializeField] private SoundConfig reelTick;
		[SerializeField] private SoundConfig extraBonus;

		private AudioSource audioSource;
		private List<SoundConfig> soundsQueue = new();

		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void AddInQueue(SoundConfig sound)
		{
			soundsQueue.Add(sound);
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
					AddInQueue(threeInARow);
				}
			}
			if (propertyName == nameof(ISlotMachineViewModel.extraBonusSum))
			{
				AddInQueue(extraBonus);
			}
		}

		public void PropertyChangedHandler(IReelViewModel sender, string propertyName)
		{
			if (propertyName == nameof(IReelViewModel.decorativeSymbol))
			{
				if (reelTick != null)
				{
					AddInQueue(reelTick);
				}
			}
		}

		private void Update()
		{
			if (soundsQueue.Count > 0)
			{
				soundsQueue.Sort((x, y) => y.priority.CompareTo(x.priority));
				PlaySound(soundsQueue[0].clip);
				soundsQueue.Clear();
			}
		}

		public void SubscribeTo(ISlotMachineViewModel sender)
		{
			sender.OnPropertyChanged += PropertyChangedHandler;
		}

		public void SubscribeTo(IReelViewModel sender)
		{
			sender.OnPropertyChanged += PropertyChangedHandler;
		}

		public void UnSubscribeFrom(ISlotMachineViewModel sender)
		{
			sender.OnPropertyChanged -= PropertyChangedHandler;
		}

		public void UnSubscribeFrom(IReelViewModel sender)
		{
			sender.OnPropertyChanged -= PropertyChangedHandler;
		}
	}
}
