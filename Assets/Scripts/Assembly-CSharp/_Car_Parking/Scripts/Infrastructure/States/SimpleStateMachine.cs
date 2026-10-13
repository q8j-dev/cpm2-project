using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.States;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	public class SimpleStateMachine : IGameStateMachine
	{
		private Dictionary<Type, IExitableState> _states = new Dictionary<Type, IExitableState>();

		public IExitableState ActiveState { get; set; }

		public SimpleStateMachine(params IExitableState[] states)
		{
			foreach (IExitableState exitableState in states)
			{
				_states.Add(exitableState.GetType(), exitableState);
			}
		}

		public void Enter<TState>() where TState : class, IState
		{
			ChangeState<TState>().Enter();
		}

		public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
		{
			ChangeState<TState>().Enter(payload);
		}

		private TState ChangeState<TState>() where TState : class, IExitableState
		{
			ActiveState?.Exit();
			return (TState)(ActiveState = GetState<TState>());
		}

		private TState GetState<TState>() where TState : class, IExitableState
		{
			return (TState)_states[typeof(TState)];
		}
	}
}
