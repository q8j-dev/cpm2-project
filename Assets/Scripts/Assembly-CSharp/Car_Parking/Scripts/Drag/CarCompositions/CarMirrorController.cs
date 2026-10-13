using UnityEngine;
using UnityEngine.Rendering.Universal;
using _Car_Parking;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Visual;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class CarMirrorController
	{
		public GameObject lMirror;

		public GameObject rMirror;

		public GameObject mMirror;

		public GameObject ReverseCamDisplay;

		public float SideMirrorView = 60f;

		public float MiddleMirrorView = 80f;

		public Material displayMat;

		private Material _blackMirrorMaterial;

		private bool _showingRearDisplayCamera;

		private readonly VehicleData _vehicle;

		private readonly MirrorComposition _mirrorComposition;

		private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

		private static readonly int MatSmooth = Shader.PropertyToID("_Smoothness");

		private static readonly int MatMetal = Shader.PropertyToID("_Metalness");

		public Material BlackMirrorMaterial => _blackMirrorMaterial;

		public CarMirrorController(VehicleData vehicle, MirrorComposition mirrorComposition)
		{
			_vehicle = vehicle;
			_mirrorComposition = mirrorComposition;
			_blackMirrorMaterial = new Material(Shader.Find("CPM/URP/Automotive/AutomotiveSimple"));
			_blackMirrorMaterial.color = Color.white;
			_blackMirrorMaterial.SetColor(EmissionColor, Color.black);
			_blackMirrorMaterial.SetFloat(MatSmooth, 1f);
			_blackMirrorMaterial.SetFloat(MatMetal, 1f);
		}

		public void Init(Transform bodyTransform)
		{
			if (bodyTransform.TryGetComponent<CarBodyReferences>(out var component))
			{
				if ((bool)component.Mmirror)
				{
					InitMirror(component.Mmirror.gameObject, out mMirror);
				}
				if ((bool)component.Lmirror)
				{
					InitMirror(component.Lmirror.gameObject, out lMirror);
				}
				if ((bool)component.Rmirror)
				{
					InitMirror(component.Rmirror.gameObject, out rMirror);
				}
			}
		}

		public void InitCameras()
		{
			CarChildsReferences carChildsReferences = _vehicle.CarChildsReferences;
			if ((bool)carChildsReferences)
			{
				float num = StripLegacyCamera(carChildsReferences.RCam);
				if (num > 0f)
				{
					SideMirrorView = num;
				}
				StripLegacyCamera(carChildsReferences.LCam);
				float num2 = StripLegacyCamera(carChildsReferences.MCam);
				if (num2 > 0f)
				{
					MiddleMirrorView = num2;
				}
			}
		}

		public static float StripLegacyCamera(Transform anchor)
		{
			if (!anchor || !anchor.TryGetComponent<Camera>(out var component))
			{
				return -1f;
			}
			float fieldOfView = component.fieldOfView;
			component.enabled = false;
			if (anchor.TryGetComponent<UniversalAdditionalCameraData>(out var component2))
			{
				Object.DestroyImmediate(component2);
			}
			Object.DestroyImmediate(component);
			return fieldOfView;
		}

		private void InitMirror(GameObject mirrorObj, out GameObject field)
		{
			field = mirrorObj;
			mirrorObj.GetComponent<MeshRenderer>().sharedMaterial = _blackMirrorMaterial;
			TrySetShortDistance(mirrorObj);
		}

		private void TrySetShortDistance(GameObject obj)
		{
			if (!obj.TryGetComponent<IgnoreChangeCarLayer>(out var _) && !FlagsStorage.IsMainMenu)
			{
				obj.layer = LayerMask.NameToLayer("short distance");
			}
		}

		public void SetMirror(bool val)
		{
			if (val)
			{
				_mirrorComposition.ActivateForCar(_vehicle).Forget();
				return;
			}
			if ((bool)rMirror)
			{
				rMirror.GetComponent<MeshRenderer>().sharedMaterial = _blackMirrorMaterial;
			}
			if ((bool)lMirror)
			{
				lMirror.GetComponent<MeshRenderer>().sharedMaterial = _blackMirrorMaterial;
			}
			if ((bool)ReverseCamDisplay && ReverseCamDisplay.activeSelf)
			{
				ShowRearCamDisplay();
				return;
			}
			_mirrorComposition.Deactivate(_vehicle);
			if ((bool)mMirror)
			{
				mMirror.GetComponent<MeshRenderer>().sharedMaterial = _blackMirrorMaterial;
			}
		}

		public void ShowRearCamDisplay()
		{
			if (!_showingRearDisplayCamera && FlagsStorage.ShowCarMirror && (bool)displayMat)
			{
				ReverseCamDisplay.GetComponent<MeshRenderer>().sharedMaterial = mMirror.GetComponent<MeshRenderer>().sharedMaterial;
				_showingRearDisplayCamera = true;
			}
		}

		public void HideRearCamDisplay()
		{
			if (_showingRearDisplayCamera && (bool)displayMat)
			{
				ReverseCamDisplay.GetComponent<MeshRenderer>().sharedMaterial = displayMat;
				_showingRearDisplayCamera = false;
			}
		}

		public void Dispose()
		{
			CommonExtensions.DestroyMat(ref _blackMirrorMaterial);
		}
	}
}
