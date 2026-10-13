using System;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class DeltaDirectionTracker
	{
		private readonly float _pressDistance;

		private readonly double _releaseSeconds;

		private float _x;

		private float _y;

		private double _lastMove = double.NegativeInfinity;

		private double _lastPressedMove;

		private DeltaDirection? _pressed;

		public DeltaDirection? Pressed => _pressed;

		public DeltaDirectionTracker(float pressDistance, double releaseSeconds)
		{
			_pressDistance = pressDistance;
			_releaseSeconds = releaseSeconds;
		}

		public bool Feed(float x, float y, double time)
		{
			if (x == 0f && y == 0f)
			{
				return false;
			}
			_lastMove = time;
			if (_pressed.HasValue)
			{
				if (Along(_pressed.Value, x, y) > 0f)
				{
					_lastPressedMove = time;
				}
				return false;
			}
			_x += x;
			_y += y;
			float num = Math.Abs(_x);
			float num2 = Math.Abs(_y);
			if (Math.Max(num, num2) < _pressDistance)
			{
				return false;
			}
			_pressed = ((!(num >= num2)) ? ((!(_y > 0f)) ? DeltaDirection.Down : DeltaDirection.Up) : ((_x > 0f) ? DeltaDirection.Right : DeltaDirection.Left));
			_lastPressedMove = time;
			return true;
		}

		public bool Tick(double time, out DeltaDirection released)
		{
			released = DeltaDirection.Up;
			if (!_pressed.HasValue)
			{
				if (time - _lastMove >= _releaseSeconds)
				{
					_x = (_y = 0f);
				}
				return false;
			}
			if (time - _lastPressedMove < _releaseSeconds)
			{
				return false;
			}
			released = _pressed.Value;
			Reset();
			return true;
		}

		public void Reset()
		{
			_pressed = null;
			_x = (_y = 0f);
		}

		public static string NameOf(DeltaDirection direction)
		{
			return direction switch
			{
				DeltaDirection.Up => "up", 
				DeltaDirection.Down => "down", 
				DeltaDirection.Left => "left", 
				_ => "right", 
			};
		}

		private static float Along(DeltaDirection direction, float x, float y)
		{
			return direction switch
			{
				DeltaDirection.Up => y, 
				DeltaDirection.Down => 0f - y, 
				DeltaDirection.Left => 0f - x, 
				_ => x, 
			};
		}
	}
}
