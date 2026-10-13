using System;
using CPM.Database;
using CPM.Database.Constructors;
using CPM.Scripts.VinylDrawer;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class PlatesTuningScope : VisualTuningScope
	{
		private VinylDrawer _vinylDrawer;

		private VehicleData _vehicleData;

		public int currentPlateIndex = 1;

		public Material PlateMat;

		private RenderTexture _plateTempRT;

		public Transform plateCreatorTableTr;

		public GameObject[] Plates = new GameObject[2];

		public Material originPlateMaterial;

		private readonly Color _colorWhite = Color.white;

		private readonly Color _colorGreen = Color.green;

		private bool _changingPlate;

		private Transform lastSelectedPlateTransform;

		private ICarInfoService _carInfoService;

		[Inject]
		public void Construct(VinylDrawer vinylDrawer, ICarInfoService carInfoService)
		{
			_carInfoService = carInfoService;
			_vinylDrawer = vinylDrawer;
		}

		public void SelectRear(bool enable)
		{
			Observable.Timer(TimeSpan.FromSeconds(0.25)).Subscribe((long _) =>
			{
				if (enable)
				{
					int num = PlayerPrefs.GetInt($"plate_{currentPlateIndex}_isRear", -1);
					if (num != -1)
					{
						PlayerPrefs.SetInt($"{num}_hasRearPlate", 0);
					}
					PlayerPrefs.SetInt($"{_vehicleData.CarId}_hasRearPlate", currentPlateIndex);
					PlayerPrefs.SetInt($"plate_{currentPlateIndex}_isRear", _vehicleData.CarId);
				}
				else
				{
					PlayerPrefs.DeleteKey($"{_vehicleData.CarId}_hasRearPlate");
					PlayerPrefs.DeleteKey($"plate_{currentPlateIndex}_isRear");
				}
			});
		}

		public void SelectFront(bool enable)
		{
			Observable.Timer(TimeSpan.FromSeconds(0.25)).Subscribe((long _) =>
			{
				if (enable)
				{
					int num = PlayerPrefs.GetInt($"plate_{currentPlateIndex}_isFront", -1);
					if (num != -1)
					{
						PlayerPrefs.SetInt($"{num}_hasFrontPlate", 0);
					}
					PlayerPrefs.SetInt($"{_vehicleData.CarId}_hasFrontPlate", currentPlateIndex);
					PlayerPrefs.SetInt($"plate_{currentPlateIndex}_isFront", _vehicleData.CarId);
				}
				else
				{
					PlayerPrefs.DeleteKey($"{_vehicleData.CarId}_hasFrontPlate");
					PlayerPrefs.DeleteKey($"plate_{currentPlateIndex}_isFront");
				}
			});
		}

		private async UniTaskVoid OnPlateChanged()
		{
			bool state = _vinylDrawer.Camera.enabled;
			_vinylDrawer.Camera.enabled = false;
			await UniTask.Delay(TimeSpan.FromMilliseconds(50.0));
			_vinylDrawer.DeleteVinylContext(VinylsType.PlateVinyls);
			if (PlateMat != null)
			{
				PlateMat.mainTexture = null;
			}
			UnityEngine.Object.Destroy(_plateTempRT);
			_plateTempRT = null;
			await UniTask.Delay(TimeSpan.FromMilliseconds(50.0));
			_vinylDrawer.Camera.enabled = state;
			_plateTempRT = _vinylDrawer.CreateVinylContext(VinylsType.PlateVinyls);
			if (currentPlateIndex > 0)
			{
				GameObject obj = Plates[(currentPlateIndex - 1) % 2];
				obj.SetActive(value: true);
				Renderer component = obj.GetComponent<Renderer>();
				PlateMat = component.material;
				_vinylDrawer.ChangePlaneState(originPlateMaterial.mainTexture, _colorWhite);
			}
			if (PlateMat != null)
			{
				PlateMat.mainTexture = _plateTempRT;
			}
			_changingPlate = false;
		}

		public override void Init()
		{
			_vehicleData = V.Car.GetComponent<VehicleData>();
			GameObject obj = Plates[(currentPlateIndex - 1) % 2];
			obj.SetActive(value: true);
			Renderer component = obj.GetComponent<Renderer>();
			PlateMat = component.material;
		}

		private void Update()
		{
			_vinylDrawer.SetCamState(isActive: false);
			_vinylDrawer.DeleteVinylContext(VinylsType.PlateVinyls);
			PlateConstructor.SetPlates(_vehicleData, _carInfoService.GetFromSlot(V.SlotID));
			plateCreatorTableTr.gameObject.SetActive(value: false);
			WindowVinylsTuningScope.SwitchToCarVinyls(_vinylDrawer, _vinylDrawer.GetCancellationTokenOnDestroy()).Forget();
		}

		public override void Apply()
		{
		}

		public override void Revert()
		{
		}

		public override void Select(int index)
		{
			if (!_changingPlate)
			{
				_changingPlate = true;
				Plates[0].SetActive(value: false);
				Plates[1].SetActive(value: false);
				if ((bool)lastSelectedPlateTransform)
				{
					lastSelectedPlateTransform.GetChild(0).GetComponent<Image>().color = Color.black;
				}
				currentPlateIndex = ++index;
				OnPlateChanged().Forget();
			}
		}
	}
}
