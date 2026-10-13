using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.PoliceLights;
using _Car_Parking.Scripts.Infrastructure;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class LightsComposition
	{
		private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

		private readonly VehicleData _vehicle;

		private readonly LightController _lightController;

		private bool _separateRearBrakeFound;

		private bool _rearLightFound;

		private Material _diodMat;

		private Material _diodMat2;

		private Material _diodMat3;

		public Material DiodMat => _diodMat;

		public Material DiodMat2 => _diodMat2;

		public Material DiodMat3 => _diodMat3;

		public PoliceLightControl PoliceLights { get; private set; }

		public bool IsPolice { get; set; }

		public bool IsTaxi { get; set; }

		public GameObject Light { get; set; }

		public LightsComposition(VehicleData vehicle)
		{
			_vehicle = vehicle;
			_lightController = _vehicle.GetComponent<LightController>();
		}

		public void SetupHDLights()
		{
			if ((bool)_lightController)
			{
				_lightController.SetupHDRenderers();
			}
		}

		public void ApplyCurrentLightState()
		{
			if ((bool)_lightController)
			{
				_lightController.ApplyLightStateToBodyKitMaterials();
			}
		}

		public void Init(Transform bodyTransform)
		{
			if (bodyTransform.TryGetComponent<CarBodyReferences>(out var component))
			{
				SetDiod(component.Diods ? component.Diods.gameObject : null, component.TurnLeftDiod ? component.TurnLeftDiod.gameObject : null, component.TurnRightDiod ? component.TurnRightDiod.gameObject : null);
				SetRearBrake(component.RearBrake ? component.RearBrake.gameObject : null);
				SetRearLight(component.RearLight ? component.RearLight.gameObject : null);
				SetReverseLight(component.ReverseLight ? component.ReverseLight.gameObject : null);
				SetLeftTurn(component.TurnLeft ? component.TurnLeft.gameObject : null);
				SetRightTurn(component.TurnRight ? component.TurnRight.gameObject : null);
				SetTurnLeft2(component.TurnLeft2 ? component.TurnLeft2.gameObject : null);
				SetTurnRight2(component.TurnRight2 ? component.TurnRight2.gameObject : null);
				SetTurnLeft3(component.TurnLeft3 ? component.TurnLeft3.gameObject : null);
				SetTurnRight3(component.TurnRight3 ? component.TurnRight3.gameObject : null);
				SetTurnLeft4(component.TurnLeft4 ? component.TurnLeft4.gameObject : null);
				SetTurnRight4(component.TurnRight4 ? component.TurnRight4.gameObject : null);
				SetLights2(component.Lights2 ? component.Lights2.gameObject : null);
				SetHeadLight(component.FrontLight ? component.FrontLight.gameObject : null);
				ResetLights();
			}
		}

		private void ResetLights()
		{
			if ((bool)_lightController)
			{
				_lightController.ResetAllLights();
			}
		}

		public void SetHeadLight(GameObject headLight)
		{
			if ((bool)headLight)
			{
				Material material = headLight.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.HeadlightsMats = material;
			}
		}

		public void SetRearBrake(GameObject rearBrake)
		{
			if ((bool)rearBrake)
			{
				Material material = rearBrake.GetComponent<MeshRenderer>().material;
				_lightController.BrakeMat2.Add(material);
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				material.color = Color.red;
				_separateRearBrakeFound = true;
				if (_rearLightFound && !_separateRearBrakeFound)
				{
					_lightController.BrakeIsLight = true;
				}
			}
		}

		public void SetRearLight(GameObject rearLight)
		{
			if ((bool)rearLight)
			{
				Material material = rearLight.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.rearLightsMats.Add(material);
				material.color = Color.red;
				_rearLightFound = true;
				if (_rearLightFound && !_separateRearBrakeFound)
				{
					_lightController.BrakeIsLight = true;
				}
			}
		}

		public void SetDiod(GameObject diod, GameObject diod2, GameObject diod3)
		{
			if ((bool)diod)
			{
				Material material = diod.GetComponent<MeshRenderer>().material;
				diod.GetComponent<MeshRenderer>().material = new Material(GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights"));
				_diodMat = diod.GetComponent<MeshRenderer>().material;
				_diodMat.SetTexture(BaseMap, material.GetTexture(BaseMap));
				_diodMat.color = PlayerDatas.GetColor($"Diod_C_{_vehicle.CarId}", Color.white);
				if (_vehicle.gameObject == V.Car)
				{
					PlayerDatas.SetDiod(diod, _diodMat.color);
				}
				_lightController.indicatorColorDiod = _diodMat.color;
				_lightController.IndicatorDiodMat = _diodMat;
			}
			if ((bool)diod2)
			{
				Material material2 = diod2.GetComponent<MeshRenderer>().material;
				diod2.GetComponent<MeshRenderer>().material = new Material(GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights"));
				_diodMat2 = diod2.GetComponent<MeshRenderer>().material;
				if (material2.HasProperty(BaseMap))
				{
					_diodMat2.SetTexture(BaseMap, material2.GetTexture(BaseMap));
				}
				_diodMat2.color = PlayerDatas.GetColor($"Diod_C_{_vehicle.CarId}", Color.white);
				if (_vehicle.gameObject == V.Car)
				{
					PlayerDatas.SetDiod(diod2, _diodMat2.color);
				}
				_lightController.indicatorColorDiod = _diodMat2.color;
				_lightController.turnLeftDiodMat = _diodMat2;
			}
			if ((bool)diod3)
			{
				Material material3 = diod3.GetComponent<MeshRenderer>().material;
				diod3.GetComponent<MeshRenderer>().material = new Material(GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights"));
				_diodMat3 = diod3.GetComponent<MeshRenderer>().material;
				if (material3.HasProperty(BaseMap))
				{
					_diodMat3.SetTexture(BaseMap, material3.GetTexture(BaseMap));
				}
				_diodMat3.color = PlayerDatas.GetColor($"Diod_C_{_vehicle.CarId}", Color.white);
				if (_vehicle.gameObject == V.Car)
				{
					PlayerDatas.SetDiod(diod3, _diodMat3.color);
				}
				_lightController.indicatorColorDiod = _diodMat3.color;
				_lightController.turnRightDiodMat = _diodMat3;
			}
		}

		public void SetLights(GameObject lights)
		{
			if ((bool)lights)
			{
				Material material = lights.GetComponent<MeshRenderer>().material;
				if (_lightController.lightsMat == null)
				{
					material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
					_lightController.lightsMat = material;
				}
				else
				{
					lights.GetComponent<MeshRenderer>().material = _lightController.lightsMat;
				}
			}
		}

		public void SetLeftTurn(GameObject turn)
		{
			if ((bool)turn)
			{
				MeshRenderer component = turn.GetComponent<MeshRenderer>();
				if (!_lightController.LeftIndicatorMat2)
				{
					component.material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
					_lightController.LeftIndicatorMat2 = component.material;
				}
				else
				{
					component.material = _lightController.LeftIndicatorMat2;
				}
			}
		}

		public void SetRightTurn(GameObject turn)
		{
			if ((bool)turn)
			{
				MeshRenderer component = turn.GetComponent<MeshRenderer>();
				Material material = component.material;
				if (!_lightController.RightIndicatorMat2)
				{
					material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
					_lightController.RightIndicatorMat2 = material;
				}
				else
				{
					component.material = _lightController.RightIndicatorMat2;
				}
			}
		}

		public void SetReverseLight(GameObject reverseLight)
		{
			if ((bool)reverseLight)
			{
				Material material = reverseLight.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.reverseMat = material;
			}
		}

		private void SetTurnLeft2(GameObject turn)
		{
			if ((bool)turn)
			{
				Material material = turn.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.LeftIndicatorMat3 = material;
			}
		}

		private void SetTurnRight2(GameObject turn)
		{
			if ((bool)turn)
			{
				Material material = turn.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.RightIndicatorMat3 = material;
			}
		}

		private void SetTurnLeft3(GameObject turn)
		{
			if ((bool)turn)
			{
				Material material = turn.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.LeftIndicatorMat4 = material;
			}
		}

		private void SetTurnRight3(GameObject turn)
		{
			if ((bool)turn)
			{
				Material material = turn.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.RightIndicatorMat4 = material;
			}
		}

		private void SetTurnLeft4(GameObject turn)
		{
			if ((bool)turn)
			{
				Material material = turn.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.LeftIndicatorMat5 = material;
			}
		}

		private void SetTurnRight4(GameObject turn)
		{
			if ((bool)turn)
			{
				Material material = turn.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.RightIndicatorMat5 = material;
			}
		}

		private void SetLights2(GameObject lights)
		{
			if ((bool)lights)
			{
				Material material = lights.GetComponent<MeshRenderer>().material;
				material.shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				_lightController.lightsMat2 = material;
			}
		}

		public void TryDisableLight()
		{
			if ((SceneManager.GetActiveScene().buildIndex == 1 || V.CoreGameManager.isFreeDrive) && (bool)Light)
			{
				Light.SetActive(value: false);
			}
		}

		public void Dispose()
		{
			CommonExtensions.DestroyMat(ref _diodMat);
			CommonExtensions.DestroyMat(ref _diodMat2);
			CommonExtensions.DestroyMat(ref _diodMat3);
		}

		public void TryFindJolPolLights()
		{
			if ((bool)PoliceLights || _vehicle.isTraffiCar)
			{
				return;
			}
			PoliceLights = _vehicle.GetComponentInChildren<PoliceLightControl>();
			if ((bool)PoliceLights)
			{
				PoliceLights.Initialize();
				if (ObscuredPrefs.GetInt($"CarPoliceSet{V.CarId}") == 1 && _vehicle.gameObject == V.Car)
				{
					MakePoliceCar();
				}
				else
				{
					DisablePolice();
				}
			}
		}

		public void MakePoliceCar()
		{
			if (!IsPolice)
			{
				if (PoliceLights == null)
				{
					TryFindJolPolLights();
				}
				IsPolice = true;
				PoliceLights.UpdateStateSiren();
			}
			if ((bool)PoliceLights)
			{
				PoliceLights.gameObject.SetActive(!IsTaxi);
			}
		}

		public void DisablePolice()
		{
			IsPolice = false;
			if ((bool)PoliceLights)
			{
				PoliceLights.gameObject.SetActive(value: false);
			}
		}
	}
}
