using Assets.SlotMachineNetTest.Scripts.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Data.Configs;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using Assets.SlotMachineNetTest.Scripts.Universal.Models;
using System.Collections.Generic;

namespace Assets.SlotMachineNetTest.Scripts.Models
{
	public class SlotMachineModel : Model, ISlotMachineModel
	{
		public event PropertyChangedEventHandler<ISlotMachineModel> OnPropertyChanged;

		public IList<IReelModel> reelModelsList { get; protected set; }

		public SymbolData symbolConfig => selectedSymbols[0];

		private List<SymbolData> selectedSymbols = new List<SymbolData>();
		private bool isSpinInProgress;
		private float _extraBonusSum;
		private int _lastSpinScores;
		private GameplayData gameplayData;

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

		public SlotMachineModel(GameplayData gameplayConfig, List<IReelModel> nestedObjects)
		{
			gameplayData = gameplayConfig;
			reelModelsList = nestedObjects;
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


			if (lastSpinScores > 0 && gameplayData.extraBonusSymbol.Contains(selectedSymbols[0]))
			{
				extraBonusSum = gameplayData.extraBonusMultiplier * selectedSymbols[0].prize3InRow;
			}

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
