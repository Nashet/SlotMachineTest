namespace Assets.Nashet.Scripts.Universal.Contracts.Services
{
	/// <summary>
	/// Purpose of this service is to provide access to the configs.
	/// Configs can have all kind of information.
	/// Some implemenations can acquire configs from https or other sources.
	/// </summary>
	public interface IConfigService : IService
	{
		T GetConfig<T>() where T : class;
	}
}