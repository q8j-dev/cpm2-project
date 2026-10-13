using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CPMEngine
{
	public class CPMVehicleCore : MonoBehaviour
	{
		public bool UseMultithreading;

		public int CarBufferSize = 20;

		private HashSet<Powertrain> _vehicles = new HashSet<Powertrain>();

		private volatile bool _quitApp;

		private static readonly Predicate<Powertrain> s_isDeadPowertrain = (Powertrain p) => !p;

		private PhysWorker[] _workers;

		private Thread[] _workerThreads;

		private CountdownEvent _countdown;

		private int _numWorkers = -1;

		public static Powertrain[] CarBuffer;

		public static CarPhysData[] CarDataBuffer;

		private void Awake()
		{
			_quitApp = false;
			_vehicles = new HashSet<Powertrain>();
			CarBuffer = new Powertrain[CarBufferSize];
			CarDataBuffer = new CarPhysData[CarBufferSize];
			_numWorkers = Environment.ProcessorCount;
		}

		private void OnDestroy()
		{
			_quitApp = true;
			StopWorkerThreads();
			_countdown?.Dispose();
		}

		private void StopWorkerThreads()
		{
			if (_workers != null)
			{
				PhysWorker[] workers = _workers;
				for (int i = 0; i < workers.Length; i++)
				{
					workers[i].Stop();
				}
				Thread[] workerThreads = _workerThreads;
				for (int i = 0; i < workerThreads.Length; i++)
				{
					workerThreads[i].Join(200);
				}
				_workers = null;
				_workerThreads = null;
			}
		}

		private void EnsureWorkers(int count)
		{
			if (_workers == null || _workers.Length < count)
			{
				StopWorkerThreads();
				_workers = new PhysWorker[count];
				_workerThreads = new Thread[count];
				for (int i = 0; i < count; i++)
				{
					PhysWorker physWorker = new PhysWorker();
					_workers[i] = physWorker;
					Thread thread = new Thread(physWorker.WorkerLoop)
					{
						Name = $"CPM Physics {i}",
						IsBackground = true,
						Priority = System.Threading.ThreadPriority.AboveNormal
					};
					_workerThreads[i] = thread;
					thread.Start();
				}
			}
		}

		public void AddCar(Powertrain powertrain)
		{
			if (!_quitApp)
			{
				_vehicles.Add(powertrain);
			}
		}

		public void RemoveCar(Powertrain powertrain)
		{
			if (!_quitApp)
			{
				_vehicles.Remove(powertrain);
			}
		}

		private void SingleThreadUpdate(float dt, int carCount)
		{
			for (int i = 0; i < carCount; i++)
			{
				if (CarBuffer[i].TrafficMode)
				{
					CarBuffer[i].Phys.UpdateTraffic(dt);
				}
				else if (CarBuffer[i].RaceBotMode)
				{
					CarBuffer[i].Phys.UpdateRaceBot(dt);
				}
				else
				{
					CarBuffer[i].Phys.PreUpdate(dt, CarBuffer[i].PhysSteps);
					CarBuffer[i].Phys.ApplyWheels(dt, CarBuffer[i].PhysSteps);
					CarBuffer[i].Phys.PostUpdate(dt);
				}
				CarBuffer[i] = null;
			}
		}

		private void FillCarDataBuffer(int carCount)
		{
			for (int i = 0; i < carCount; i++)
			{
				CarDataBuffer[i] = new CarPhysData
				{
					Phys = CarBuffer[i].Phys,
					TrafficMode = CarBuffer[i].TrafficMode,
					RaceBot = CarBuffer[i].RaceBotMode,
					PhysSteps = CarBuffer[i].PhysSteps
				};
				CarBuffer[i] = null;
			}
		}

		private void ProcessCarSlice(float dt, int from, int to)
		{
			for (int i = from; i < to; i++)
			{
				ref CarPhysData reference = ref CarDataBuffer[i];
				if (reference.TrafficMode)
				{
					reference.Phys.UpdateTraffic(dt);
					continue;
				}
				if (reference.RaceBot)
				{
					reference.Phys.UpdateRaceBot(dt);
					continue;
				}
				reference.Phys.PreUpdate(dt, reference.PhysSteps);
				reference.Phys.ApplyWheels(dt, reference.PhysSteps);
				reference.Phys.PostUpdate(dt);
			}
		}

		private void MultiThreadUpdate(float dt, int carCount)
		{
			if (carCount == 0)
			{
				return;
			}
			int num = Mathf.Min(_numWorkers, carCount);
			int num2 = num - 1;
			PhysicsThreadCache.TimeScale = Time.timeScale;
			FillCarDataBuffer(carCount);
			if (num2 > 0)
			{
				EnsureWorkers(num2);
				if (_countdown == null)
				{
					_countdown = new CountdownEvent(num2);
				}
				else
				{
					_countdown.Reset(num2);
				}
				for (int i = 0; i < num2; i++)
				{
					int min = i * carCount / num;
					int max = (i + 1) * carCount / num;
					_workers[i].Setup(min, max, dt, _countdown);
					_workers[i].Signal();
				}
			}
			int num3 = num2 * carCount / num;
			ProcessCarSlice(dt, num3, carCount);
			if (num2 <= 0)
			{
				return;
			}
			_countdown.Wait();
			for (int j = 0; j < num2; j++)
			{
				if (_workers[j].Exception != null)
				{
					Debug.LogError($"[CPMVehicleCore] Thread {j}: {_workers[j].Exception.Message}\n{_workers[j].Exception.StackTrace}");
				}
			}
		}

		private void PhysUpdate(float dt)
		{
			int num = 0;
			foreach (Powertrain vehicle in _vehicles)
			{
				if ((bool)vehicle && !vehicle.PhysicsAsleep)
				{
					CarBuffer[num++] = vehicle;
					if (num == CarBufferSize)
					{
						UpdateCars(dt, num);
						num = 0;
					}
				}
			}
			UpdateCars(dt, num);
		}

		private void UpdateCars(float dt, int count)
		{
			if (UseMultithreading)
			{
				MultiThreadUpdate(dt, count);
			}
			else
			{
				SingleThreadUpdate(dt, count);
			}
		}

		private void FixedUpdate()
		{
			float deltaTime = Time.deltaTime;
			foreach (Powertrain vehicle in _vehicles)
			{
				if ((bool)vehicle)
				{
					vehicle.UpdatePhysicsSleep();
				}
			}
			foreach (Powertrain vehicle2 in _vehicles)
			{
				if ((bool)vehicle2 && !vehicle2.PhysicsAsleep)
				{
					vehicle2.PreSimulation(deltaTime);
				}
			}
			PhysUpdate(deltaTime);
			foreach (Powertrain vehicle3 in _vehicles)
			{
				if ((bool)vehicle3)
				{
					vehicle3.Phys.FlushPendingEvents();
				}
			}
			foreach (Powertrain vehicle4 in _vehicles)
			{
				if ((bool)vehicle4 && !vehicle4.PhysicsAsleep && !vehicle4.OnLineCar)
				{
					vehicle4.PostSimulation();
				}
			}
		}

		private void Update()
		{
			foreach (Powertrain vehicle in _vehicles)
			{
				if ((bool)vehicle)
				{
					vehicle.UpdateWheels();
					if (!vehicle.PhysicsAsleep)
					{
						vehicle.UpdateChassis();
					}
				}
			}
		}

		private void LateUpdate()
		{
			_vehicles.RemoveWhere(s_isDeadPowertrain);
		}
	}
}
