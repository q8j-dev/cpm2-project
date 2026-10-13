using System;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.GameFacts.Consumers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaIdContextCondition : MonoBehaviour, IContextCondition
	{
		[SerializeField]
		private ArenaId TargetArenaId;

		private ArenaFlowController _arena;

		public bool IsTrue { get; private set; }

		public event Action Changed;

		[Inject]
		public void Construct(ArenaFlowController arena)
		{
			_arena = arena;
		}

		private void Start()
		{
			if (_arena == null)
			{
				Debug.LogWarning("ArenaIdContextCondition on '" + base.name + "' was not injected with ArenaFlowController; condition stays false.", this);
				return;
			}
			_arena.OnPhaseChanged += OnPhaseChanged;
			Reevaluate();
		}

		private void OnDestroy()
		{
			if (_arena != null)
			{
				_arena.OnPhaseChanged -= OnPhaseChanged;
			}
		}

		private void OnPhaseChanged(ArenaPhase _)
		{
			Reevaluate();
		}

		private void Reevaluate()
		{
			bool flag = ArenaPhaseFacts.IsActive(_arena.Phase) && _arena.ArenaId == TargetArenaId;
			if (flag != IsTrue)
			{
				IsTrue = flag;
				Changed?.Invoke();
			}
		}
	}
}
