using System;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class CustomCameraSettingsWindow : MonoBehaviour
	{
		[SerializeField]
		private SliderValueHandler Distance;

		[SerializeField]
		private SliderValueHandler Height;

		[SerializeField]
		private SliderValueHandler Pitch;

		[SerializeField]
		private SliderValueHandler Damp;

		[SerializeField]
		private SliderValueHandler FOV;

		[SerializeField]
		private Button SaveButton;

		[SerializeField]
		private Button SaveForThisCarButton;

		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private GameObject[] ToHide;

		private CoreGameManager _coreGameManager;

		private CarCameras _carCameras;

		private Camera _currentCamera;

		[Inject]
		public void Construct(CoreGameManager coreGameManager)
		{
			_coreGameManager = coreGameManager;
		}

		private void Start()
		{
			ResolveCameraReferences();
			SliderValueHandler distance = Distance;
			distance.OnPointerUpAction = (Action)Delegate.Combine(distance.OnPointerUpAction, new Action(OnDragEnd));
			SliderValueHandler height = Height;
			height.OnPointerUpAction = (Action)Delegate.Combine(height.OnPointerUpAction, new Action(OnDragEnd));
			SliderValueHandler pitch = Pitch;
			pitch.OnPointerUpAction = (Action)Delegate.Combine(pitch.OnPointerUpAction, new Action(OnDragEnd));
			SliderValueHandler damp = Damp;
			damp.OnPointerUpAction = (Action)Delegate.Combine(damp.OnPointerUpAction, new Action(OnDragEnd));
			SliderValueHandler fOV = FOV;
			fOV.OnPointerUpAction = (Action)Delegate.Combine(fOV.OnPointerUpAction, new Action(OnDragEnd));
			Distance.AddListener(OnDistanceChange);
			Height.AddListener(OnHeightChange);
			Pitch.AddListener(OnPitchChange);
			Damp.AddListener(OnDampChange);
			FOV.AddListener(OnFOVChange);
			CloseButton.onClick.AddListener(Close);
			SaveButton.onClick.AddListener(SaveGlobal);
			SaveForThisCarButton.onClick.AddListener(SaveForThisCar);
		}

		private void OnDestroy()
		{
			SliderValueHandler distance = Distance;
			distance.OnPointerUpAction = (Action)Delegate.Remove(distance.OnPointerUpAction, new Action(OnDragEnd));
			SliderValueHandler height = Height;
			height.OnPointerUpAction = (Action)Delegate.Remove(height.OnPointerUpAction, new Action(OnDragEnd));
			SliderValueHandler pitch = Pitch;
			pitch.OnPointerUpAction = (Action)Delegate.Remove(pitch.OnPointerUpAction, new Action(OnDragEnd));
			SliderValueHandler damp = Damp;
			damp.OnPointerUpAction = (Action)Delegate.Remove(damp.OnPointerUpAction, new Action(OnDragEnd));
			SliderValueHandler fOV = FOV;
			fOV.OnPointerUpAction = (Action)Delegate.Remove(fOV.OnPointerUpAction, new Action(OnDragEnd));
			Distance.RemoveAllListeners();
			Height.RemoveAllListeners();
			Pitch.RemoveAllListeners();
			Damp.RemoveAllListeners();
			FOV.RemoveAllListeners();
			CloseButton.onClick.RemoveAllListeners();
			SaveButton.onClick.RemoveAllListeners();
			SaveForThisCarButton.onClick.RemoveAllListeners();
		}

		private void ResolveCameraReferences()
		{
			if ((bool)_coreGameManager)
			{
				if (!_carCameras)
				{
					_carCameras = _coreGameManager.carCameras;
				}
				if (!_currentCamera)
				{
					_currentCamera = _coreGameManager.GetComponent<Camera>();
				}
			}
		}

		public void OpenCustomCarSettings()
		{
			ResolveCameraReferences();
			if ((bool)_carCameras && (bool)_currentCamera && (bool)Distance && (bool)Height && (bool)Pitch && (bool)Damp && (bool)FOV)
			{
				Distance.SetValueWithoutNotify(_carCameras.distance / 15f);
				Height.SetValueWithoutNotify(Mathf.InverseLerp(0f, 30f, _carCameras.height));
				Pitch.SetValueWithoutNotify((_carCameras.pitchAngle + 3f) / 5f);
				Damp.SetValueWithoutNotify(_carCameras.rotationDamping / 9f);
				FOV.SetValueWithoutNotify(Mathf.InverseLerp(15f, 80f, _currentCamera.fieldOfView));
				base.gameObject.SetActive(value: true);
			}
		}

		public void Close()
		{
			base.gameObject.SetActive(value: false);
		}

		private void OnDistanceChange(float value)
		{
			value = Mathf.Clamp(value, 0.01f, 1f);
			_carCameras.distance = value * 15f;
			OnDragStart(Distance);
		}

		private void OnHeightChange(float value)
		{
			_carCameras.height = Mathf.Lerp(0f, 30f, value);
			OnDragStart(Height);
		}

		private void OnPitchChange(float value)
		{
			value = Mathf.Clamp(value, 0.01f, 1f);
			_carCameras.pitchAngle = value * 5f - 3f;
			OnDragStart(Pitch);
		}

		private void OnDampChange(float value)
		{
			value = Mathf.Clamp(value, 0.01f, 1f);
			_carCameras.rotationDamping = ((value == 1f) ? 200f : (value * 9f));
			OnDragStart(Damp);
		}

		private void OnFOVChange(float value)
		{
			_currentCamera.fieldOfView = Mathf.Lerp(15f, 80f, value);
			OnDragStart(FOV);
		}

		private void OnDragStart(SliderValueHandler sliderValueHandler)
		{
			PlayerDatas.DisableEnableAll(isEnable: false, ToHide);
			sliderValueHandler.gameObject.SetActive(value: true);
		}

		private void OnDragEnd()
		{
			PlayerDatas.DisableEnableAll(isEnable: true, ToHide);
		}

		private void SaveGlobal()
		{
			if ((bool)V.Car)
			{
				int carId = V.Car.GetComponent<VehicleData>().CarId;
				ObscuredPrefs.DeleteKey("camDistance" + carId);
				ObscuredPrefs.DeleteKey("camHeight" + carId);
				ObscuredPrefs.DeleteKey("camPitch" + carId);
				ObscuredPrefs.DeleteKey("camDamp" + carId);
				ObscuredPrefs.DeleteKey("camField" + carId);
			}
			ObscuredPrefs.SetFloat("camDistance", _carCameras.distance);
			ObscuredPrefs.SetFloat("camHeight", _carCameras.height);
			ObscuredPrefs.SetFloat("camPitch", _carCameras.pitchAngle);
			ObscuredPrefs.SetFloat("camDamp", _carCameras.rotationDamping);
			ObscuredPrefs.SetFloat("camField", _currentCamera.fieldOfView);
			Close();
		}

		private void SaveForThisCar()
		{
			if ((bool)V.Car)
			{
				int carId = V.Car.GetComponent<VehicleData>().CarId;
				ObscuredPrefs.SetFloat("camDistance" + carId, _carCameras.distance);
				ObscuredPrefs.SetFloat("camHeight" + carId, _carCameras.height);
				ObscuredPrefs.SetFloat("camPitch" + carId, _carCameras.pitchAngle);
				ObscuredPrefs.SetFloat("camDamp" + carId, _carCameras.rotationDamping);
				ObscuredPrefs.SetFloat("camField" + carId, _currentCamera.fieldOfView);
			}
			Close();
		}
	}
}
