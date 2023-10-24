using Assets.SlotMachineNetTest.Scripts.Contracts.InputViews;
using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Contracts.Views;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.SlotMachineNetTest.Scripts.InputViews;
using Assets.SlotMachineNetTest.Scripts.Models;
using Assets.CommonNashet.Contracts.Patterns;
using Assets.CommonNashet.Contracts.Services;
using Assets.SlotMachineNetTest.Scripts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Views;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.SlotMachineNetTest.Scripts.Installers
{
	public class GameplayInstaller : MonoInstaller
	{
		[SerializeField] private List<ReelViewModel> reelVMlList = new List<ReelViewModel>();
		[SerializeField] private PlayerSoundsView playerSoundsView;
		[SerializeField] private PlayerInput playerInput;

		private IConfigService configService;

		[Inject]
		public void Construct(IConfigService configService)
		{
			this.configService = configService;
			if (IsGameplayStartedAlone(configService))
				PrepareGameplayAloneStart(configService);
		}

		private static void PrepareGameplayAloneStart(IConfigService configService)
		{
			configService.LoadConfigs();
		}

		private static bool IsGameplayStartedAlone(IConfigService configService)
		{
			return !configService.IsReady;
		}

		public override void InstallBindings()
		{
			var amountsOfReels = reelVMlList.Count;
			InstallViews();

			InstallBindingsFromInstance(amountsOfReels);
		}

		private void InstallViews()
		{
			Container.Bind<IFakeRandomStrategy<SymbolData>>().To<FakeRandomStrategy>().AsTransient();
			Container.Bind<IPlayerSoundsView>().FromInstance(playerSoundsView).AsSingle();
			Container.Bind<IPlayerInput>().FromInstance(playerInput);
		}

		private void InstallBindingsFromInstance(int amountsOfReels)
		{
			//todo zenject and colections
			//todo git rename
			//todo git move
			var reelsModels = new List<IReelModel>(amountsOfReels);
			for (int i = 0; i < amountsOfReels; i++)
			{
				reelsModels.Add(new ReelModel(Container.Resolve<IFakeRandomStrategy<SymbolData>>(), configService));
			}

			var slotMachineModel = new SlotMachineModel(configService, reelsModels);


			var convertedReelsVM = reelVMlList.ConvertAll<IReelViewModel>(r => r);

			Container.Bind<IList<IReelViewModel>>().FromInstance(convertedReelsVM);

			Container.Bind<ISlotMachineModel>().FromInstance(slotMachineModel).AsSingle();


			var extraBonusWindowViewModel = new ExtraBonusWindowViewModel(slotMachineModel);
			Container.Bind(typeof(IExtraBonusWindowViewModel), typeof(IDisposable)).FromInstance(extraBonusWindowViewModel);

			var slotMachineViewModel = new SlotMachineViewModel(playerSoundsView, slotMachineModel, playerInput, convertedReelsVM);
			Container.Bind(typeof(ISlotMachineViewModel), typeof(IDisposable)).FromInstance(slotMachineViewModel);
		}
	}
}

