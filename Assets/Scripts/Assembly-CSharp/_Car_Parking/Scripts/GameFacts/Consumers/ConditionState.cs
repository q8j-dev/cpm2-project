using System;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.GameFacts.Conditions;

namespace _Car_Parking.Scripts.GameFacts.Consumers
{
	public sealed class ConditionState : MonoBehaviour
	{
		[SerializeField]
		private GameFactCondition Condition;

		[SerializeField]
		private bool DefaultState;

		private IGameFactManager _facts;

		private IContextCondition[] _context;

		public bool Current { get; private set; }

		public event Action<bool> OnStatusChanged;

		[Inject]
		public void Construct(IGameFactManager facts)
		{
			_facts = facts;
			Current = DefaultState;
		}

		private void Awake()
		{
			_context = GetComponents<IContextCondition>();
		}

		private void Start()
		{
			if (_facts == null)
			{
				Debug.LogWarning("ConditionState on '" + base.name + "' was not injected with IGameFactManager; status will not react to facts.", this);
				return;
			}
			_facts.OnChanged += OnFactChanged;
			for (int i = 0; i < _context.Length; i++)
			{
				if (_context[i] != null)
				{
					_context[i].Changed += OnContextChanged;
				}
			}
			Reevaluate();
		}

		private void OnDestroy()
		{
			if (_facts != null)
			{
				_facts.OnChanged -= OnFactChanged;
			}
			if (_context == null)
			{
				return;
			}
			for (int i = 0; i < _context.Length; i++)
			{
				if (_context[i] != null)
				{
					_context[i].Changed -= OnContextChanged;
				}
			}
		}

		private void OnFactChanged(GameFact _)
		{
			Reevaluate();
		}

		private void OnContextChanged()
		{
			Reevaluate();
		}

		private void Reevaluate()
		{
			bool flag = (bool)Condition && Condition.IsMet(_facts) && AllContextMet();
			if (flag != Current)
			{
				Current = flag;
				OnStatusChanged?.Invoke(Current);
			}
		}

		private bool AllContextMet()
		{
			for (int i = 0; i < _context.Length; i++)
			{
				if (_context[i] != null && !_context[i].IsTrue)
				{
					return false;
				}
			}
			return true;
		}
	}
}
