﻿using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;

namespace Assets.SlotMachineNetTest.Scripts.Contracts.ViewModels
{
	public interface IReelViewModel : IPropertyChangeNotifier<IReelViewModel>, IPropertyChangeSubscriber<IReelModel>
	{
		SymbolData decorativeSymbol { get; }
		SymbolData selectedSymbol { get; }
		void InitializeOld(IReelModel reelModel);
	}
}