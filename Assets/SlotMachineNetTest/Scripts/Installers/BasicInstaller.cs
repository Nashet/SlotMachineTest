using Assets.SlotMachineNetTest.Scripts.Models.ApplicationState;
using Assets.CommonNashet.Contracts.Models;
using Assets.CommonNashet.Contracts.Services;
using Assets.CommonNashet.Services;
using UnityEngine;
using Zenject;

namespace Assets.SlotMachineNetTest.Scripts.Installers
{
	public class BasicInstaller : MonoInstaller
	{
		[SerializeField] private string configHolderName;
		[SerializeField] private bool StartWithInstantGameplay;

		protected IConfigService configService;

		public override void InstallBindings()
		{
			InstallBasics();

			InstallApplicationStates();
		}

		private void InstallBasics()
		{
			configService = new SOConfigService(configHolderName);
			Container.Bind<IConfigService>().FromInstance(configService).NonLazy();
		}

		protected virtual void InstallApplicationStates()
		{
			Container.Bind<IApplicationStatePattern>().To<ApplicationStatePattern>().AsSingle();

			Container.Bind<LoadConfigState>().To<LoadConfigState>().AsSingle();
			Container.Bind<DefaultApplicationState>().To<DefaultApplicationState>().AsSingle();
			Container.Bind<AbortApplicationState>().To<AbortApplicationState>().AsSingle();
			Container.Bind<GameplayState>().To<GameplayState>().AsSingle();

			if (StartWithInstantGameplay)
				Container.Bind<MainMenuState>().To<NoMainMenuState>().AsSingle();
			else
				Container.Bind<MainMenuState>().To<MainMenuState>().AsSingle();
		}
	}
}

