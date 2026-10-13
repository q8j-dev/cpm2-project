using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Other.Helpers;

public class CarLODControl : MonoBehaviour
{
	private VehicleData _vehicleData;

	public bool IsActiveOnLoaded { get; set; } = true;

	private void Start()
	{
		_vehicleData = GetComponent<VehicleData>();
		LoadCarLOD().Forget();
	}

	private void OnEnable()
	{
		SceneManager.activeSceneChanged += OnActiveSceneChanged;
	}

	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= OnActiveSceneChanged;
	}

	private void OnActiveSceneChanged(Scene scene01, Scene scene02)
	{
		if ((bool)_vehicleData && ScenesHelper.IsLapRacingSceneActive() && (bool)V.LastUsedCar && V.LastUsedCar == base.gameObject)
		{
			_vehicleData.BodyLoadingComposition.ShowMainModelOnly();
			base.enabled = false;
		}
	}

	private async UniTaskVoid LoadCarLOD()
	{
		await UniTask.NextFrame();
		if ((bool)_vehicleData)
		{
			_vehicleData.BodyLoadingComposition.StartLODLoad(immediately: true, IsActiveOnLoaded).Forget();
		}
	}

	private void LateUpdate()
	{
		if (Time.frameCount % 5 != 0 || !_vehicleData || !_vehicleData.BodyLoadingComposition.LODModel)
		{
			return;
		}
		if (_vehicleData.racingState == RacingState.OnVisualTuning)
		{
			if (_vehicleData.BodyLoadingComposition.LODModel.gameObject.activeSelf)
			{
				_vehicleData.BodyLoadingComposition.ShowMainModelOnly();
			}
		}
		else if ((bool)V.CoreGameManager)
		{
			float sqrMagnitude = (V.PlayerPointLOD.position - base.transform.position).sqrMagnitude;
			float num = Mathf.Clamp(V.DetailDistance, 5f, 80f);
			if (sqrMagnitude < num * num)
			{
				_vehicleData.BodyLoadingComposition.ShowMainModelOnly();
				return;
			}
			_vehicleData.BodyLoadingComposition.ShowLod(val: true);
			_vehicleData.BodyLoadingComposition.SetLodWheel(showLODWheel: true);
		}
	}
}
