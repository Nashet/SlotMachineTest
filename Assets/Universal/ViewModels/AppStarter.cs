using Assets.CommonNashet.Contracts.Models;
using Assets.CommonNashet.Patterns;
using Zenject;

namespace Assets.CommonNashet.ViewModels
{
	/// <summary>
	/// The only goal is to provide ticks for non unity classes
	/// </summary>
	public class AppStarter : MonoSingleton<AppStarter> //todo better use Zenject to avoid seconfd creation of that object
	{
		private IApplicationStatePattern application;

		//private void Awake()
		//{
		//	gameObject.AddComponent<Zenject.SceneContext>();
		//}

		[Inject]
		public void Construct(IApplicationStatePattern application)
		{
			this.application = application;
		}

		private void Start()
		{
			DontDestroyOnLoad(this);
		}
		//todo zenject tickables

		private void Update()
		{
			application.Update();
		}
	}
}

