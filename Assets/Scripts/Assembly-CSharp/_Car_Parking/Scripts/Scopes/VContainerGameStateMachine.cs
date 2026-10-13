using CodeBase.Infrastructure.States;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Scopes
{
	public class VContainerGameStateMachine : IGameStateMachine
	{
		private readonly IObjectResolver _resolver;

		private IExitableState _activeState;

		[Inject]
		public VContainerGameStateMachine(IObjectResolver resolver)
		{
			_resolver = resolver;
		}

		public void Enter<TState>() where TState : class, IState
		{
			TState val = ChangeState<TState>();
			Debug.Log($"<color=orange>Enter to {typeof(TState)} </color>");
			val.Enter();
		}

		public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
		{
			TState val = ChangeState<TState>();
			Debug.Log($"<color=orange>Enter to: {typeof(TState)} with payload: {payload}</color>");
			val.Enter(payload);
		}

		private TState ChangeState<TState>() where TState : class, IExitableState
		{
			_activeState?.Exit();
			return (TState)(_activeState = _resolver.Resolve<TState>());
		}
	}
}
