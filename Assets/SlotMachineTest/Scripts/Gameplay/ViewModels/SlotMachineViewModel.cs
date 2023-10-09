using Nashet.Common.Services;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using Nashet.SlotMachine.Gameplay.InputView;
using System.Collections.Generic;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.ViewModels
{
	/// <summary>
	/// Purpose of that class is to setup nested view models and own model/view
	/// </summary>
	public class SlotMachineViewModel : MonoBehaviour, ISlotMachineViewModel
	{
		[SerializeField] private PlayerInput playerInput;
		[SerializeField] private List<ReelViewModel> reelVMlList = new();
		[SerializeField] private string configHolderName;

		private void Awake()
		{
			var configService = new SOConfigService(configHolderName);
			Initialize(configService.GetConfig<GameplayConfig>());
		}
		public void Initialize(GameplayConfig gameplayConfig)
		{
			playerInput.OnSpinButtonClicked += OnSpinButtonClickedHandler;

			var randomStrategy = new FakeRandomStrategy(gameplayConfig);
			for (int i = 0; i < reelVMlList.Count; i++)
			{
				var item = reelVMlList[i];
				item.Initialize(gameplayConfig, randomStrategy);
			}
		}

		private void OnSpinButtonClickedHandler()
		{
			StartNewRound();
		}

		private void StartNewRound()
		{
			foreach (var item in reelVMlList)
			{
				item.StartNewRound();
			}
		}
	}
}
