using Assets.CommonNashet.Contracts.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.SlotMachineNetTest.Scripts.Models.ApplicationState
{
	public class GameplayState : IApplicationState
	{
		private string gameplaySceneName = "Gameplay"; //todo use zenject?

		public bool IsFinished { get; protected set; }

		public GameplayState()
		{
		}

		public void EnteredStateHandler()
		{
			LoadScene();
		}
		private bool IsSceneLoaded()
		{
			Scene scene = SceneManager.GetSceneByName(gameplaySceneName);
			return scene.IsValid() && scene.isLoaded;
		}

		private void LoadScene()
		{
			if (IsSceneLoaded())
			{
				return;
			}

			var loading = SceneManager.LoadSceneAsync(gameplaySceneName, LoadSceneMode.Single);
			loading.completed += LoadingCompletedHandler;
		}

		private void LoadingCompletedHandler(AsyncOperation operation)
		{
			IsFinished = true;
		}

		public bool IsFailed() => false;


		public void Update()
		{
		}
	}
}

