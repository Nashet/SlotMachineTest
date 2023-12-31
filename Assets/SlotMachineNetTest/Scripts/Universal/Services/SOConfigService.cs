﻿using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Services;
using Assets.SlotMachineNetTest.Scripts.Universal.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Services
{
	/// <summary>
	/// That implementation of IConfigService is used to get configs from ScriptableObjects.
	/// </summary>
	public class SOConfigService : IConfigService
	{
		private readonly Dictionary<Type, object> allConfigs = new Dictionary<Type, object>();

		public SOConfigService(string configHolderName)
		{
			if (string.IsNullOrEmpty(configHolderName))
			{
				throw new ArgumentNullException(nameof(configHolderName));
			}
			var configHolder = Resources.Load<ConfigHolderData>(configHolderName);

			foreach (var item in configHolder.AllConfigs)
			{
				AddConfig(item);
			}
		}

		public virtual T GetConfig<T>() where T : class
		{
			return (T)allConfigs[typeof(T)];
		}

		private void AddConfig(ScriptableObject config)
		{
			allConfigs[config.GetType()] = config;
		}
	}
}