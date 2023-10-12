using Nashet.Contracts.Patterns;
using Nashet.Contracts.Services;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using Socket.WebSocket4Net.System.Linq;
using System.Collections.Generic;

namespace Nashet.SlotMachine.Gameplay.Models
{
	public class SlotMachineModel : ISlotMachineModel
	{
		public event PropertyChangedEventHandler<ISlotMachineModel> OnPropertyChanged;

		private GameplayConfig gameplayConfig;

		public IList<IReelModel> reelModelsList { get; protected set; }

		private NetworkSymbolsModel networkSymbols;

		public SymbolConfig symbolConfig => selectedSymbols[0];

		private List<SymbolConfig> selectedSymbols = new List<SymbolConfig>();
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

		public SlotMachineModel(GameplayConfig gameplayConfig, IEnumerable<IReelViewModel> reelVMList, ISocketClientService socketClient)
		{
			this.gameplayConfig = gameplayConfig;
			reelModelsList = new List<IReelModel>();

			networkSymbols = new NetworkSymbolsModel(gameplayConfig, socketClient, reelVMList.Count((x) => true));

			var id = 0;
			foreach (var item in reelVMList)
			{
				var randomSymbolStrategy = new NetworkRandomStrategy(gameplayConfig, id, networkSymbols);

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
			if (networkSymbols != null)
				networkSymbols.Prepare();
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

			lastSpinScores = IsAllSymbolsSame() ? selectedSymbols[0].prize3InRow : 0;

			if (networkSymbols.bonusPrize != 0)
			{
				extraBonusSum = networkSymbols.bonusPrize;
			}
			isSpinInProgress = false;
		}

		private bool IsAllSymbolsSame()
		{
			var firstSymbol = selectedSymbols[0];
			for (int i = 0; i < selectedSymbols.Count; i++)
			{
				SymbolConfig item = selectedSymbols[i];
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
