using Nashet.Contracts.Patterns;
using Nashet.SlotMachine.Configs;
using Nashet.SlotMachine.Gameplay.Contracts;
using System.Threading.Tasks;

namespace Nashet.SlotMachine.Gameplay.Models
{
	public class ReelModel : IReelModel
	{
		public event PropertyChangedEventHandler<IReelModel> OnPropertyChanged;

		public SymbolConfig selectedSymbol
		{
			get => _selectedSymbol;
			private set
			{
				_selectedSymbol = value;
				RiseOnPropertyChanged(nameof(selectedSymbol));
			}
		}
		public SymbolConfig decorativeSymbol
		{
			get => _decorativeSymbol;
			private set
			{
				_decorativeSymbol = value;
				RiseOnPropertyChanged(nameof(decorativeSymbol));
			}
		}

		private GameplayConfig gameplayConfig;
		private IFakeRandomStrategy<SymbolConfig> randomStrategy;
		private SymbolConfig _selectedSymbol;
		private SymbolConfig _decorativeSymbol;

		public ReelModel(IFakeRandomStrategy<SymbolConfig> randomStrategy, GameplayConfig gameplayConfig)
		{
			this.gameplayConfig = gameplayConfig;
			this.randomStrategy = randomStrategy;
		}

		public void StartNewRound()
		{
			SpinReel();
		}
		private async void SpinReel()
		{
			randomStrategy.Reset();

			while (!randomStrategy.IsFinished)
			{
				decorativeSymbol = randomStrategy.Get();
				await Task.Delay((int)(gameplayConfig.oneSymbolSpinTime * 1000));
			}

			selectedSymbol = decorativeSymbol;
		}

		public void RiseOnPropertyChanged(string propertyName)
		{
			OnPropertyChanged?.Invoke(this, propertyName);
		}
	}
}
