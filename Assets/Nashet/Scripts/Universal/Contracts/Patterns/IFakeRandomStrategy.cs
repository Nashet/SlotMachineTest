﻿namespace Assets.Nashet.Scripts.Universal.Contracts.Patterns
{
	public interface IFakeRandomStrategy<T>
	{
		bool IsFinished { get; }
		T Get();
		void Reset();
		bool IsInitialized { get; }
	}
}
