using UnityEngine;

namespace CPM.Shifter.Logic
{
	public class GamepadStickGrabFsm
	{
		public enum StickEventKind
		{
			None = 0,
			Begin = 1,
			Drag = 2,
			End = 3
		}

		public readonly struct StickEvent
		{
			public readonly StickEventKind Kind;

			public readonly Vector2 Delta;

			public static readonly StickEvent None = new StickEvent(StickEventKind.None, Vector2.zero);

			public StickEvent(StickEventKind kind, Vector2 delta)
			{
				Kind = kind;
				Delta = delta;
			}

			public static StickEvent Begin()
			{
				return new StickEvent(StickEventKind.Begin, Vector2.zero);
			}

			public static StickEvent Drag(Vector2 d)
			{
				return new StickEvent(StickEventKind.Drag, d);
			}

			public static StickEvent End()
			{
				return new StickEvent(StickEventKind.End, Vector2.zero);
			}
		}

		private readonly float _enterDeadzone;

		private readonly float _exitDeadzone;

		private readonly float _pxRange;

		private bool _isGrabbing;

		private Vector2 _accumulated;

		public bool IsGrabbing => _isGrabbing;

		public GamepadStickGrabFsm(float enterDeadzone, float exitDeadzone, float pxRange)
		{
			_enterDeadzone = enterDeadzone;
			_exitDeadzone = exitDeadzone;
			_pxRange = pxRange;
		}

		public StickEvent Tick(Vector2 stick)
		{
			float magnitude = stick.magnitude;
			if (!_isGrabbing)
			{
				if (magnitude < _enterDeadzone)
				{
					return StickEvent.None;
				}
				_isGrabbing = true;
				_accumulated = Vector2.zero;
				return StickEvent.Begin();
			}
			if (magnitude < _exitDeadzone)
			{
				_isGrabbing = false;
				_accumulated = Vector2.zero;
				return StickEvent.End();
			}
			Vector2 vector = ScaledTarget(stick, magnitude);
			Vector2 d = vector - _accumulated;
			if (d.sqrMagnitude < 0.0001f)
			{
				return StickEvent.None;
			}
			_accumulated = vector;
			return StickEvent.Drag(d);
		}

		private Vector2 ScaledTarget(Vector2 stick, float mag)
		{
			float num = Mathf.Clamp01((mag - _exitDeadzone) / (1f - _exitDeadzone));
			return stick / mag * num * _pxRange;
		}

		public void ForceEnd()
		{
			_isGrabbing = false;
			_accumulated = Vector2.zero;
		}
	}
}
