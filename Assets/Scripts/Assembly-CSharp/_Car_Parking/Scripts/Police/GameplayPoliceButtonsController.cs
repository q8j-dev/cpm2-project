using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	public class GameplayPoliceButtonsController : MonoBehaviour
	{
		[SerializeField]
		private GameObject Root = default;

		private CancellationTokenSource _updateRootCTS;

		private PoliceButtonsControllerRegistry _registry;

		public PoliceSirenSoundEnableButton PoliceSirenSound;

		public PoliceSirenStatusUpdateButton PoliceSirenStatus;

		[Inject]
		public void Construct(PoliceButtonsControllerRegistry registry)
		{
			_registry = registry;
		}

		private void OnEnable()
		{
			_registry?.Register(this);
			UpdateRoot();
			V.OnCarEntered += OnCarEntered;
			V.OnCarExit += OnCarExit;
		}

		private void OnDisable()
		{
			_registry?.Unregister(this);
			V.OnCarEntered -= OnCarEntered;
			V.OnCarExit -= OnCarExit;
		}

		private void OnCarEntered(GameObject car, VehicleData vehicleData)
		{
			UpdateRoot();
		}

		private void UpdateRoot()
		{
			CommonExtensions.CancelAndDisposeToken(ref _updateRootCTS);
			_updateRootCTS = new CancellationTokenSource();
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_updateRootCTS.Token, base.destroyCancellationToken);
			UpdateRootAsync(cancellationTokenSource.Token).Forget();
		}

		private void OnCarExit(GameObject car, VehicleData vehicleData)
		{
			CommonExtensions.CancelAndDisposeToken(ref _updateRootCTS);
			Root.SetActive(value: false);
		}

		private async UniTask UpdateRootAsync(CancellationToken cancellationToken)
		{
			Root.SetActive(value: false);
			await UniTask.WaitUntil(() => V.Car != null, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (!cancellationToken.IsCancellationRequested && (bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var vehicleData))
			{
				await UniTask.WaitUntil(() => !vehicleData || vehicleData.CarInfoProvider.IsReady, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				if ((bool)vehicleData && !cancellationToken.IsCancellationRequested)
				{
					CarInfo carInfo = vehicleData.CarInfoProvider.CarInfo;
					Root.SetActive(carInfo != null && carInfo.AdditionalData != null && carInfo.AdditionalData.Police != null && carInfo.AdditionalData.Police.Installed);
				}
			}
		}

		public bool GetPoliceButtonsActiveState()
		{
			return Root.activeSelf;
		}
	}
}
