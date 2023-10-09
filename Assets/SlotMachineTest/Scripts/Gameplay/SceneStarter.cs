using Nashet.Common.Services;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.InputView;
using Nashet.SlotMachine.Gameplay.Views;
using UnityEngine;

namespace Nashet.SlotMachine
{
	/// <summary>Injects dependencies into the scene.
	/// Here you can choose which implementation you are going to use
	/// </summary>
	public class SceneStarter : MonoBehaviour
	{
		[SerializeField] private string configHolderName;
		[SerializeField] private PlayerInput playerInput;
		[SerializeField] private PlayerSoundsView playerSoundsView;

		private void Start()
		{
			var configService = new SOConfigService(configHolderName);
			var gameplayConfig = configService.GetConfig<GameplayConfig>();
			//new SlotMachineViewModel( gameplayConfig);

			//todo delete
		}
	}
}
