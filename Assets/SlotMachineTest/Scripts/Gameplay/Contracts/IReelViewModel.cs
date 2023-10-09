﻿using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;

namespace Nashet.SlotMachine.Gameplay.Contracts
{
	public interface IReelViewModel : IPropertyChangeNotifier<IReelViewModel>
	{
		SymbolConfig decorativeSymbol { get; }
		SymbolConfig selectedSymbol { get; }

		void StartNewRound();
	}
}