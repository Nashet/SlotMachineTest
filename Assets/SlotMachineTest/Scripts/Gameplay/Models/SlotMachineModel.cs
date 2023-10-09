using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using Nashet.SlotMachine.Gameplay.ViewModels;
using System.Collections.Generic;


namespace Nashet.SlotMachine.Gameplay.Models
{
	public class SlotMachineModel : ISlotMachineModel, IPropertyChangeNotifier<ISlotMachineModel>
	{
		public event PropertyChangedEventHandler<ISlotMachineModel> OnPropertyChanged;

		public IList<IReelModel> reelModelsList { get; protected set; }

		private List<SymbolConfig> selectedSymbols = new List<SymbolConfig>();
		private bool isSpinInProgress;

		public int LastSpinScores { get; protected set; }


		public SlotMachineModel(GameplayConfig gameplayConfig, IEnumerable<IReelViewModel> reelVMList)
		{
			reelModelsList = new List<IReelModel>();
			foreach (var item in reelVMList)
			{
				var randomSymbolStrategy = new FakeRandomStrategy(gameplayConfig);
				reelModelsList.Add(new ReelModel(randomSymbolStrategy, gameplayConfig));
			}
		}

		public void StartNewRound()
		{
			if (isSpinInProgress)
				return;
			isSpinInProgress = true;
			selectedSymbols.Clear();
			foreach (var item in reelModelsList)
			{
				item.StartNewRound();
			}
		}

		public void HandleReelStop(SymbolConfig selectedSymbol)
		{
			selectedSymbols.Add(selectedSymbol);
			var allReelsHaveStopped = selectedSymbols.Count == reelModelsList.Count;
			if (!allReelsHaveStopped)
			{
				return;
			}

			LastSpinScores = IsAllSymbolsSame() ? selectedSymbols[0].prize3InRow : 0;
			isSpinInProgress = false;
			RiseOnPropertyChanged(nameof(LastSpinScores));
		}

		private bool IsAllSymbolsSame()
		{
			var firstSymbol = selectedSymbols[0];
			for (int i = 0; i < selectedSymbols.Count; i++)
			{
				SymbolConfig item = selectedSymbols[i];
				if (item != firstSymbol)
					return false;
			}
			return true;
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}
	}
}
