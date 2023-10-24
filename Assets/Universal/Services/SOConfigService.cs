using Assets.CommonNashet.Contracts.Services;
using Assets.CommonNashet.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CommonNashet.Services
{
	/// <summary>
	/// That implementation of IConfigService is used to get configs from ScriptableObjects.
	/// </summary>
	public class SOConfigService : IConfigService
	{
		private readonly Dictionary<Type, object> allConfigs = new Dictionary<Type, object>();
		private string configHolderName;

		public SOConfigService(string configHolderName)
		{
			if (string.IsNullOrEmpty(configHolderName))
			{
				throw new ArgumentNullException(nameof(configHolderName));
			}
			this.configHolderName = configHolderName;
		}

		public bool IsReady { get; private set; }

		public virtual T GetConfig<T>() where T : class
		{
			return (T)allConfigs[typeof(T)];
		}

		public bool LoadConfigs()
		{
			Debug.Log("Loading configs..");
			var configHolder = Resources.Load<ConfigHolderData>(configHolderName);

			foreach (var item in configHolder.AllConfigs)
			{
				AddConfig(item);
			}
			IsReady = true;
			return true;
		}

		private void AddConfig(ScriptableObject config)
		{
			allConfigs[config.GetType()] = config;
		}
	}
}