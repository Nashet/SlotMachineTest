using Assets.SlotMachineNetTest.Scripts.Contracts.InputViews;
using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Contracts.Views;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.SlotMachineNetTest.Scripts.InputViews;
using Assets.SlotMachineNetTest.Scripts.Models;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using Assets.SlotMachineNetTest.Scripts.Universal.Services;
using Assets.SlotMachineNetTest.Scripts.ViewModels;
using Assets.SlotMachineNetTest.Scripts.Views;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.SlotMachineNetTest.Scripts.Installers
{
	public class ZenjectInstaller : MonoInstaller
	{
		[SerializeField] private List<ReelViewModel> reelVMlList = new List<ReelViewModel>();
		[SerializeField] private string configHolderName;
		[SerializeField] private PlayerSoundsView playerSoundsView;
		[SerializeField] private PlayerInput playerInput;

		public override void InstallBindings()
		{
			// In this example there is only one 'installer' but in larger projects you
			// will likely end up with many different re-usable installers
			// that you'll want to use in several different scenes
			//
			// There are several ways to do this.  You can store your installer as a prefab,
			// a scriptable object, a component within the scene, etc.  Or, if you don't
			// need your installer to be a MonoBehaviour then you can just simply call
			// Container.Install
			//
			// See here for more details:
			// https://github.com/modesttree/zenject#installers
			//
			//Container.Install<MyOtherInstaller>();			

			var configService = new SOConfigService(configHolderName);

			var gameplayConfig = configService.GetConfig<GameplayData>();
			var amountsOfReels = reelVMlList.Count;


			Container.Bind<GameplayData>().FromInstance(gameplayConfig);

			Container.Bind<IFakeRandomStrategy<SymbolData>>().To<FakeRandomStrategy>().AsTransient();
			Container.Bind<IPlayerSoundsView>().FromInstance(playerSoundsView).AsSingle();
			Container.Bind<IPlayerInput>().FromInstance(playerInput);

			InstallBindingsFromInstance(gameplayConfig, amountsOfReels);
		}

		private void InstallBindingsFromInstance(GameplayData gameplayConfig, int amountsOfReels)
		{
			var reelsModels = new List<IReelModel>(amountsOfReels);
			for (int i = 0; i < amountsOfReels; i++)
			{
				reelsModels.Add(new ReelModel(Container.Resolve<IFakeRandomStrategy<SymbolData>>(), gameplayConfig));
			}

			var slotMachineModel = new SlotMachineModel(gameplayConfig, reelsModels);


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

