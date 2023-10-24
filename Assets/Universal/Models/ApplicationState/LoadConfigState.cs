using Assets.CommonNashet.Contracts.Models;
using Assets.CommonNashet.Contracts.Services;
using System.Threading.Tasks;

namespace Assets.SlotMachineNetTest.Scripts.Models.ApplicationState
{
	public class LoadConfigState : IApplicationState
	{
		private bool isFailed;
		private IConfigService configService;

		public LoadConfigState(IConfigService configService)
		{
			this.configService = configService;
		}

		public bool IsFinished { get; protected set; }

		public void EnteredStateHandler()
		{
			isFailed = false;
			IsFinished = false;
			LoadConfigs();
		}

		public bool IsFailed() => isFailed;

		public void Update()
		{
		}

		private async void LoadConfigs()
		{

			await Task.Delay(200);
			configService.LoadConfigs();
			//var random = Random.Range(0, 15);
			//if (random != 0)
			//{
			//	Debug.LogError("config loading is failed");
			//	isFailed = true;
			//}
			//else
			//{
			//	Debug.Log("Config loaded succesfull");
			//	isFailed = false;
			//}

			IsFinished = true;
		}
	}
}

