using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion.Stamina
{
	public class StaminaService
	{
		private readonly StaminaConfig _config;

		private readonly PersonLocomotionConfig _locomotionConfig;

		private readonly ReactiveProperty<float> _normalized = new ReactiveProperty<float>(1f);

		private float _current;

		private float _regenDelayTimer;

		private bool _exhausted;

		private bool Infinite
		{
			get
			{
				if ((bool)_locomotionConfig)
				{
					return _locomotionConfig.InfiniteStamina;
				}
				return false;
			}
		}

		public float Current => _current;

		public float Max => _config.MaxStamina;

		public bool IsExhausted => _exhausted;

		public IReadOnlyReactiveProperty<float> Normalized => _normalized;

		public bool CanStartSprint
		{
			get
			{
				if (Infinite)
				{
					return true;
				}
				if (_config.ZeroStaminaBehaviour == ZeroStaminaBehaviour.AllowButNoRegenPenalty)
				{
					return true;
				}
				if (_exhausted)
				{
					return _current >= _config.ExhaustedRecoveryThreshold;
				}
				return _current >= _config.MinStaminaToSprint;
			}
		}

		public bool CanContinueSprint
		{
			get
			{
				if (Infinite)
				{
					return true;
				}
				if (_config.ZeroStaminaBehaviour == ZeroStaminaBehaviour.AllowButNoRegenPenalty)
				{
					return true;
				}
				if (!_exhausted)
				{
					return _current > 0f;
				}
				return false;
			}
		}

		public bool CanJump
		{
			get
			{
				if (Infinite)
				{
					return true;
				}
				return _current >= _config.MinStaminaToJump;
			}
		}

		public bool CanSlide
		{
			get
			{
				if (Infinite)
				{
					return true;
				}
				return _current >= _config.MinStaminaToSlide;
			}
		}

		public StaminaService(StaminaConfig config, PersonLocomotionConfig locomotionConfig)
		{
			_config = config;
			_locomotionConfig = locomotionConfig;
			_current = config.MaxStamina;
			PublishNormalized();
		}

		public bool TryConsumeJump()
		{
			if (Infinite)
			{
				return true;
			}
			if (!CanJump)
			{
				return false;
			}
			Consume(_config.JumpCost);
			return true;
		}

		public bool TryConsumeSlide()
		{
			if (Infinite)
			{
				return true;
			}
			if (!CanSlide)
			{
				return false;
			}
			Consume(_config.SlideCost);
			return true;
		}

		public void DrainSprint(float deltaTime)
		{
			if (!Infinite)
			{
				Consume(_config.SprintDrainPerSecond * deltaTime);
			}
		}

		public void Reset()
		{
			_current = _config.MaxStamina;
			_regenDelayTimer = 0f;
			_exhausted = false;
			PublishNormalized();
		}

		public void Tick(float deltaTime, bool usingStamina)
		{
			if (Infinite)
			{
				if (_current < _config.MaxStamina)
				{
					_current = _config.MaxStamina;
					PublishNormalized();
				}
				_exhausted = false;
				return;
			}
			if (usingStamina)
			{
				_regenDelayTimer = _config.RegenDelayAfterUse;
				return;
			}
			if (_regenDelayTimer > 0f)
			{
				_regenDelayTimer -= deltaTime;
				return;
			}
			if (_current < _config.MaxStamina)
			{
				_current = Mathf.Min(_config.MaxStamina, _current + _config.RegenPerSecond * deltaTime);
				PublishNormalized();
			}
			if (_exhausted && _current >= _config.ExhaustedRecoveryThreshold)
			{
				_exhausted = false;
			}
		}

		private void Consume(float amount)
		{
			_current = Mathf.Max(0f, _current - amount);
			_regenDelayTimer = _config.RegenDelayAfterUse;
			if (_current <= 0f)
			{
				_exhausted = true;
			}
			PublishNormalized();
		}

		private void PublishNormalized()
		{
			_normalized.Value = ((_config.MaxStamina > 0f) ? (_current / _config.MaxStamina) : 0f);
		}
	}
}
