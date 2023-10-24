using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Models;
using Assets.SlotMachineNetTest.Scripts.Universal.Contracts.Patterns;
using Assets.SlotMachineNetTest.Scripts.Universal.Models;
using System;
using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Models.ApplicationState
{
	/// <summary>
	/// The only goal is switch between application state
	/// </summary>
	public class ApplicationStatePattern : Model, IApplicationStatePattern
	{
		public IApplicationState State => state;

		public event StateChangedDelegate<IApplicationState> OnStateChanged;

		private IApplicationState state;
		private LoadConfigState loadConfigState;
		private MainMenuState mainMenuState;
		private GameplayState gameplayState;
		private AbortApplicationState abortApplicationState;

		public ApplicationStatePattern(DefaultApplicationState defaultApplicationState, LoadConfigState loadConfigState, MainMenuState mainMenuState, AbortApplicationState abortApplicationState, GameplayState gameplayState)
		{
			this.loadConfigState = loadConfigState;
			this.mainMenuState = mainMenuState;
			this.abortApplicationState = abortApplicationState;
			this.gameplayState = gameplayState;
			ChangeStateTo(defaultApplicationState);
		}

		public void ChangeStateTo(IApplicationState state)
		{
			if (state == null)
			{
				Debug.Log($"Skipping null state");
				return;
			}

			Debug.Log($"Changing state to {state}");

			this.state = state;
			state.EnteredStateHandler();
			OnStateChanged?.Invoke(state);
		}

		public void Update()
		{
			state.Update();
			if (!state.IsFinished)
			{
				return;
			}
			var nextState = GetNextStage(state);
			ChangeStateTo(nextState);
		}

		private IApplicationState GetNextStage(IApplicationState currentState)
		{
			IApplicationState result;

			if (currentState.IsFailed())
			{
				return abortApplicationState;
			}
			result = currentState switch
			{
				DefaultApplicationState => loadConfigState,
				LoadConfigState => mainMenuState,
				MainMenuState => gameplayState,
				AbortApplicationState => null,
				GameplayState => null,
				_ => throw new InvalidOperationException($"Invalid state encountered - {currentState}"),
			};
			return result;
		}
	}
}

