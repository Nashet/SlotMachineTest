using Nashet.Contracts.Model;
using Nashet.Contracts.Patterns;
using Nashet.Contracts.Services;
using Nashet.Contracts.ViewModel;
using Nashet.Data.Configs;
using System.Collections.Generic;

namespace Nashet.Models
{
	public class SlotMachineModel : ISlotMachineModel
	{
		public event PropertyChangedEventHandler<ISlotMachineModel> OnPropertyChanged;

		private GameplayData gameplayConfig;

		public IList<IReelModel> reelModelsList { get; protected set; }

		public SymbolData symbolConfig => selectedSymbols[0];

		private List<SymbolData> selectedSymbols = new List<SymbolData>();
		private bool isSpinInProgress;
		private float _extraBonusSum;
		private int _lastSpinScores;

		public int lastSpinScores
		{
			get => _lastSpinScores; protected set
			{
				_lastSpinScores = value;
				RiseOnPropertyChanged(nameof(lastSpinScores));
			}
		}
		public float extraBonusSum
		{
			get => _extraBonusSum;
			private set
			{
				_extraBonusSum = value;
				RiseOnPropertyChanged(nameof(extraBonusSum));
			}
		}

		public SlotMachineModel(GameplayData gameplayConfig, IEnumerable<IReelViewModel> reelVMList, ISocketClientService socketClient)
		{
			this.gameplayConfig = gameplayConfig;
			reelModelsList = new List<IReelModel>();

			var id = 0;
			foreach (var item in reelVMList)
			{
				var randomSymbolStrategy = new FakeRandomStrategy(gameplayConfig);

				reelModelsList.Add(new ReelModel(randomSymbolStrategy, gameplayConfig));
				id++;
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

		public void HandleReelStop(SymbolData selectedSymbol)
		{
			selectedSymbols.Add(selectedSymbol);
			var allReelsHaveStopped = selectedSymbols.Count == reelModelsList.Count;
			if (!allReelsHaveStopped)
			{
				return;
			}

			lastSpinScores = IsAllSymbolsSame() ? selectedSymbols[0].prize3InRow : 0;
			
			isSpinInProgress = false;
		}

		private bool IsAllSymbolsSame()
		{
			var firstSymbol = selectedSymbols[0];
			for (int i = 0; i < selectedSymbols.Count; i++)
			{
				SymbolData item = selectedSymbols[i];
				if (item.id != firstSymbol.id)
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
