using System;
using System.Threading;

namespace CPMEngine
{
	public class PhysWorker
	{
		private int _min;

		private int _max;

		private float _dt;

		private CountdownEvent _countdown;

		private Exception _exception;

		private readonly ManualResetEventSlim _startEvent = new ManualResetEventSlim(initialState: false);

		private volatile bool _running = true;

		internal Exception Exception => _exception;

		internal void Setup(int min, int max, float dt, CountdownEvent countdown)
		{
			_min = min;
			_max = max;
			_dt = dt;
			_countdown = countdown;
			_exception = null;
		}

		internal void Signal()
		{
			_startEvent.Set();
		}

		internal void Stop()
		{
			_running = false;
			_startEvent.Set();
		}

		internal void WorkerLoop()
		{
			while (_running)
			{
				_startEvent.Wait();
				_startEvent.Reset();
				if (!_running)
				{
					break;
				}
				try
				{
					for (int i = _min; i < _max; i++)
					{
						ref CarPhysData reference = ref CPMVehicleCore.CarDataBuffer[i];
						if (reference.TrafficMode)
						{
							reference.Phys.UpdateTraffic(_dt);
							continue;
						}
						if (reference.RaceBot)
						{
							reference.Phys.UpdateRaceBot(_dt);
							continue;
						}
						reference.Phys.PreUpdate(_dt, reference.PhysSteps);
						reference.Phys.ApplyWheels(_dt, reference.PhysSteps);
						reference.Phys.PostUpdate(_dt);
					}
				}
				catch (Exception exception)
				{
					_exception = exception;
				}
				finally
				{
					_countdown?.Signal();
				}
			}
		}
	}
}
