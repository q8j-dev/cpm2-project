using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using CodeBase.Infrastructure.States;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Multiplayer.PhotonCustom;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	public class GameStateBootstrapFree : MonoBehaviour
	{
		private IGameStateMachine _gameStateMachine;

		private IObjectResolver _diContainer;

		[Inject]
		public void Construct(IGameStateMachine gameStateMachine, IObjectResolver diContainer)
		{
			_diContainer = diContainer;
			_gameStateMachine = gameStateMachine;
		}

		private void Awake()
		{
			VContainerPrefabPool pool = (VContainerPrefabPool)(PhotonNetwork.PrefabPool = new VContainerPrefabPool(_diContainer));
			if (FlagsStorage.IsMultiFree && RamDevice.AllowHeavyPreload())
			{
				WarmShellsAsync(pool).Forget();
			}
		}

		private async UniTaskVoid WarmShellsAsync(VContainerPrefabPool pool)
		{
			AllCarsInfo allCarsInfo = Resources.Load<AllCarsInfo>("Configs/AllCarsInfo");
			if (!allCarsInfo || allCarsInfo.AllCars == null)
			{
				return;
			}
			List<string> ids = new List<string>(allCarsInfo.AllCars.Count + 2);
			foreach (VehicleStockTuningData allCar in allCarsInfo.AllCars)
			{
				ids.Add("car" + allCar.Serial.ToString());
			}
			ids.Add("person0");
			ids.Add("person1");
			CancellationToken ct = this.GetCancellationTokenOnDestroy();
			string firstCar = ((allCarsInfo.AllCars.Count > 0) ? ("car" + allCarsInfo.AllCars[0].Serial.ToString()) : null);
			List<string> list = new List<string>(2);
			if (firstCar != null)
			{
				list.Add(firstCar);
			}
			list.Add("person0");
			await pool.WarmShellsAsync(list, ct);
			if (!ct.IsCancellationRequested)
			{
				pool.WarmInjectionGraph(firstCar);
				pool.WarmInjectionGraph("person0");
				await pool.WarmShellsAsync(ids, ct);
			}
		}

		private void Start()
		{
			_gameStateMachine.Enter<InitFreeDriveState>();
		}
	}
}
