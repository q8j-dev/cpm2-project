using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace CodeBase.Infrastructure.States
{
	public class GameStateMachine : IGameStateMachine
	{
		private readonly IObjectResolver _container;

		private IExitableState _activeState;

		private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

		[Inject]
		public GameStateMachine(IObjectResolver container)
		{
			_container = container;
		}

		public void Enter<TState>() where TState : class, IState
		{
			TState val = ChangeState<TState>();
			Debug.Log("<color=orange>Enter to " + typeof(TState).ToString() + " </color>");
			val.Enter();
		}

		public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
		{
			TState val = ChangeState<TState>();
			Debug.Log("<color=orange>Enter to: " + typeof(TState).ToString() + " with payload: " + payload.ToString() + "</color>");
			val.Enter(payload);
		}

		private TState ChangeState<TState>() where TState : class, IExitableState
		{
			_activeState?.Exit();
			return (TState)(_activeState = GetState<TState>());
		}

		private TState GetState<TState>() where TState : class, IExitableState
		{
			return _container.Resolve<TState>();
		}
	}
}
