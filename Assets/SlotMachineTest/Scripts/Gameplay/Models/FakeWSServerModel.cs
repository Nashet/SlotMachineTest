﻿using Nashet.Common.Services;
using Nashet.Contracts.Services;
using Nashet.SlotMachine.Gameplay.ViewModels;
using Socket.Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Nashet.SlotMachine.Gameplay.Models
{
	public class FakeWSServerModel : MonoBehaviour
	{
		[SerializeField] private List<string> fakeRespond = new List<string>();
		[SerializeField] private SlotMachineViewModel slotMachineViewModel;

		private ISocketClientService socketService;
		private static System.Random random;
		private const string roomName = "symbols";
		private const string orderSymbolsCommand = "gime new symbols plz";

		private void Start()
		{
			socketService = slotMachineViewModel.socketService;
			socketService.Connect("http://localhost:3000");
			socketService.Subscribe(roomName);
			socketService.OnMessageReceived += MessageReceivedHandler;
			random = new System.Random(Time.frameCount);
		}

		private void MessageReceivedHandler(string socketName, SocketData data)
		{
			if (socketName != roomName)
			{
				return;
			}

			if (data.msg == orderSymbolsCommand)
			{
				var randomRespond = new List<List<string>> { GetRandomCollection(fakeRespond), GetRandomCollection(fakeRespond), GetRandomCollection(fakeRespond) };

				AddBonusPrize(randomRespond);

				Send("serversymbols", JsonConvert.SerializeObject(randomRespond));
			}
		}

		private void AddBonusPrize(List<List<string>> randomRespond)
		{
			var chance = random.Next(2);
			if (chance == 1)
			{
				foreach (var item in randomRespond)
				{
					item.Add("2"); //just to fake extra bonus
				}
				var bonusPrize = 10000;
				Send("bonus", bonusPrize.ToString());
			}
		}

		private void Send(string type, string message)
		{
			var data = new SocketData
			{
				id = type,
				msg = message
			};
			socketService.Send(roomName, data);
		}

		private void OnDestroy()
		{
			socketService.Dispose();
		}

		private static List<string> GetRandomCollection(List<string> sourceList)
		{

			int collectionSize = random.Next(3, sourceList.Count + 4);
			List<string> randomCollection = new List<string>();

			for (int i = 0; i < collectionSize; i++)
			{
				int randomIndex = random.Next(sourceList.Count);
				string randomElement = sourceList[randomIndex];
				randomCollection.Add(randomElement);
			}

			return randomCollection;
		}
	}
}