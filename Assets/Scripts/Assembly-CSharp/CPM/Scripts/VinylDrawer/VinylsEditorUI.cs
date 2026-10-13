using DG.Tweening;
using Racing.CameraManagement;
using Racing.CameraManagement.SpecificTypes;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Vinyls.UIElements;

namespace CPM.Scripts.VinylDrawer
{
	public class VinylsEditorUI
	{
		public GameObject editPlane;

		public GameObject visualIcons;

		public Transform posCollider;

		public Transform rotCollider;

		public Transform scaleCollider;

		public Transform posIcon;

		public Transform rotIcon;

		public Transform scaleIcon;

		public Transform middlePoint;

		public Vector3 initScaleIconPos;

		public LineRenderer editLine1;

		public LineRenderer editLine2;

		private Transform _joystickSpriteHolder;

		private ResettingSlider _zoomSlider;

		private AnimationCurve _scaleCurve;

		private Transform _animatedSticker;

		private Image _animStickSprite;

		private Tweener _animStickAlpha;

		private Tweener _animStickScale;

		private Tweener _animStickPos;

		private VinylsEditor _editor;

		private float _getControlTimer;

		private CameraManager _cameraManager;

		private CinemachineCameraOffset _cameraOffset;

		private VinylJoystick _vinylJoystick;

		private ResettingSlider _scaleSlider;

		private ResettingSlider _scaleSliderX;

		private ResettingSlider _scaleSliderY;

		private ResettingSlider _rotationSlider;

		private FreeLookCameraHandler _vinylsCameraHandler;

		private AnimationCurve _buttonsScaleCurve;

		private CinemachineCameraOffset _benchmarkCamOffset;

		private PlateCameraMoveConfig _plateCameraMoveConfig;

		private Joystick _movePlateCameraJoystick;

		private FreeDriveProvider _freeDriveProvider;

		private const float SCALE_BY_DIST = 0.15f;

		public VinylsEditorUI(VinylsEditor editor, Material editLineMaterial, Transform joystickSpriteHolder, AnimationCurve scaleCurve, Transform animatedSticker, ResettingSlider zoomSlider, CameraManager cameraManager, VinylJoystick vinylJoystick, ResettingSlider scaleSlider, ResettingSlider scaleSliderX, ResettingSlider scaleSliderY, ResettingSlider rotationSlider, FreeLookCameraHandler vinylsCameraHandler, AnimationCurve buttonsScaleCurve, Joystick movePlateCameraJoystick, CinemachineCameraOffset benchmarkCamOffset, PlateCameraMoveConfig plateCameraMoveConfig, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			_buttonsScaleCurve = buttonsScaleCurve;
			_vinylsCameraHandler = vinylsCameraHandler;
			_scaleSlider = scaleSlider;
			_scaleSliderX = scaleSliderX;
			_scaleSliderY = scaleSliderY;
			_vinylJoystick = vinylJoystick;
			_rotationSlider = rotationSlider;
			_editor = editor;
			_joystickSpriteHolder = joystickSpriteHolder;
			_zoomSlider = zoomSlider;
			_scaleCurve = scaleCurve;
			_animatedSticker = animatedSticker;
			_cameraManager = cameraManager;
			_animStickSprite = _animatedSticker.GetComponent<Image>();
			_benchmarkCamOffset = benchmarkCamOffset;
			_plateCameraMoveConfig = plateCameraMoveConfig;
			_movePlateCameraJoystick = movePlateCameraJoystick;
			editLine1 = new GameObject().AddComponent<LineRenderer>();
			editLine1.material = editLineMaterial;
			editLine1.startWidth = (editLine1.endWidth = 0.02f);
			editLine1.gameObject.layer = 26;
			editLine2 = new GameObject().AddComponent<LineRenderer>();
			editLine2.material = editLineMaterial;
			editLine2.startWidth = (editLine2.endWidth = 0.02f);
			editLine2.gameObject.layer = 26;
		}

		public void CreateEditIcons(Vector3 positionClicked, GameObject currentVinyl)
		{
			if (!(positionClicked == Vector3.one * 1000f) && !(positionClicked == Vector3.zero) && (bool)currentVinyl)
			{
				if (editPlane == null)
				{
					(GameObject, GameObject) tuple = _editor.InstantiateEditIcons();
					editPlane = tuple.Item1;
					visualIcons = tuple.Item2;
					Transform transform = editPlane.transform;
					Transform transform2 = visualIcons.transform;
					transform.SetPositionAndRotation(positionClicked, transform2.rotation);
					posCollider = transform.Find("P");
					rotCollider = transform.Find("R");
					scaleCollider = transform.Find("S");
					editPlane.GetComponent<AllwaysLookAtCam>().FixZ = true;
					visualIcons.GetComponent<AllwaysLookAtCam>().FixZ = true;
					posIcon = transform2.Find("P");
					rotIcon = transform2.Find("R");
					scaleIcon = transform2.Find("S");
					middlePoint = transform2.Find("M");
					initScaleIconPos = scaleIcon.transform.localPosition;
					V.CoreGameManager.menuControl.tempTarget.position = transform.position;
					_freeDriveProvider.CarCameras.Target = V.CoreGameManager.menuControl.tempTarget;
				}
				else
				{
					editPlane.transform.position = positionClicked;
					editPlane.gameObject.SetActive(value: true);
					visualIcons.transform.position = positionClicked;
					visualIcons.gameObject.SetActive(value: true);
				}
				if (_editor.CurrentVinylsType == VinylsType.PlateVinyls && !_cameraOffset && _cameraManager.CurrentCameraType == RacingCameraType.VinylBenchmark)
				{
					_cameraOffset = ((MonoBehaviour)_cameraManager.CurrentCamera).GetComponent<CinemachineCameraOffset>();
				}
			}
		}

		public void UpdateEditControl()
		{
			if (_zoomSlider.touchingSlider)
			{
				OnZoomChanged();
			}
			if (_movePlateCameraJoystick.isDrag)
			{
				Vector3 offset = _benchmarkCamOffset.m_Offset + new Vector3(_movePlateCameraJoystick.Direction.x * _plateCameraMoveConfig.VelocityByFOV.GetValue(_editor.CurrentFOV) * Time.deltaTime, _movePlateCameraJoystick.Direction.y * _plateCameraMoveConfig.VelocityByFOV.GetValue(_editor.CurrentFOV) * Time.deltaTime);
				offset.x = Mathf.Clamp(offset.x, 0f - _plateCameraMoveConfig.ClampXPosition, _plateCameraMoveConfig.ClampXPosition);
				offset.y = Mathf.Clamp(offset.y, _plateCameraMoveConfig.PosYMin, _plateCameraMoveConfig.PosYMax);
				_benchmarkCamOffset.m_Offset = offset;
			}
			if ((bool)editPlane && (bool)visualIcons)
			{
				Vector3 position = editPlane.transform.position;
				Vector3 position2 = visualIcons.transform.position;
				visualIcons.transform.position = position;
				float value = 0.15f * Vector3.Distance(_freeDriveProvider.CarCameras.transform.position, position);
				value = Mathf.Clamp(value, 0.4f, 0.5f);
				Vector3 vector = new Vector3(value, value, value);
				Transform transform = posCollider.transform;
				Transform transform2 = rotCollider.transform;
				Vector3 vector2 = (scaleCollider.transform.localScale = vector);
				Vector3 localScale = (transform2.localScale = vector2);
				transform.localScale = localScale;
				if (_editor.CurrentVinylsType == VinylsType.PlateVinyls)
				{
					value = (_cameraOffset ? Mathf.Clamp(0.5f - _cameraOffset.m_Offset.z, 0.4f, 0.6f) : 0.4f);
					value -= Mathf.Lerp(0f, 0.35f, Mathf.InverseLerp(50f, 10f, _editor.CurrentFOV));
				}
				else
				{
					float a = Mathf.InverseLerp(_vinylsCameraHandler.MinZoomDistance, 0f, _vinylsCameraHandler.ZoomDistance);
					float b = Mathf.InverseLerp(10f, 50f, _editor.CurrentFOV);
					value = _buttonsScaleCurve.Evaluate(Mathf.Lerp(a, b, 0.65f));
				}
				visualIcons.transform.localScale = new Vector3(value, value, value);
				editPlane.transform.localScale = new Vector3(value, value, 0f - value);
				DrawLine(position2, scaleIcon.transform.position, editLine1);
				DrawLine(position2, rotIcon.transform.position, editLine2);
			}
		}

		public void UpdateJoystickControl(Transform editTransform)
		{
			bool flag = false;
			if ((bool)editTransform)
			{
				_vinylJoystick.transform.localRotation = editTransform.localRotation;
				Vector3 localScale = editTransform.localScale;
				Vector2 vector = new Vector2(localScale.x, localScale.y);
				vector = Vector3.ClampMagnitude(vector, 0.8f);
				vector.x = 0f - Mathf.Abs(vector.x);
				vector.y = 0f - Mathf.Abs(vector.y);
				_joystickSpriteHolder.localScale = new Vector3(vector.x, vector.y, 1f);
				if (_rotationSlider.touchingSlider)
				{
					float num = _rotationSlider.slider.value - 0.5f;
					Vector3 localEulerAngles = editTransform.localEulerAngles;
					localEulerAngles.z -= num;
					editTransform.localRotation = Quaternion.Euler(localEulerAngles);
					_editor.IsVinylsChanged = true;
					_editor.UpdateVinylAngleInfo();
				}
				if (_vinylJoystick.isDrag)
				{
					flag = true;
					OnJoystickMove(editTransform);
				}
				if (_scaleSliderX.touchingSlider)
				{
					OnScaleChange(_scaleSliderX, editTransform);
					_editor.UpdateVinylWidthInfo();
				}
				if (_scaleSliderY.touchingSlider)
				{
					OnScaleChange(_scaleSliderY, editTransform);
					_editor.UpdateVinylHeightInfo();
				}
				if (_scaleSlider.touchingSlider)
				{
					OnScaleChange(_scaleSlider, editTransform);
					_editor.UpdateVinylWidthInfo();
					_editor.UpdateVinylHeightInfo();
				}
				if (flag)
				{
					_getControlTimer = 0.5f;
					UpdateAllEditIcons(isEnabled: false, includeEditPlane: false);
					_editor.UseJoystickControl = true;
				}
				else if (_getControlTimer > 0f)
				{
					_getControlTimer -= Time.deltaTime;
				}
				else
				{
					_editor.UseJoystickControl = false;
				}
			}
		}

		private void OnScaleChange(ResettingSlider uiSlider, Transform editTransform)
		{
			float scale = uiSlider.slider.value - 0.5f;
			Vector3 localScale = editTransform.localScale;
			localScale.x = Mathf.Abs(localScale.x);
			scale = GetValueScale(scale);
			if (uiSlider == _scaleSlider || uiSlider == _scaleSliderX || _editor.IsGrouping)
			{
				localScale.x += scale;
				if (localScale.x < 0.1f)
				{
					localScale.x = 0.1f;
				}
				if (_editor.IsGrouping)
				{
					localScale.x *= _editor.GroupXSign;
				}
			}
			if (uiSlider == _scaleSlider || uiSlider == _scaleSliderY || _editor.IsGrouping)
			{
				localScale.y += scale;
				if (localScale.y < 0.1f)
				{
					localScale.y = 0.1f;
				}
			}
			editTransform.localScale = localScale;
			_editor.IsVinylsChanged = true;
		}

		private void OnJoystickMove(Transform editTransform)
		{
			Vector3 localPosition = editTransform.localPosition;
			int num = 1;
			if (_vinylJoystick.InputDirection.x < 0f)
			{
				num = -1;
			}
			int num2 = 1;
			if (_vinylJoystick.InputDirection.y < 0f)
			{
				num2 = -1;
			}
			localPosition -= new Vector3(_scaleCurve.Evaluate(Mathf.Abs(_vinylJoystick.InputDirection.x)) * (float)num, _scaleCurve.Evaluate(Mathf.Abs(_vinylJoystick.InputDirection.y)) * (float)num2, 0f) * 0.001f;
			editTransform.localPosition = localPosition;
			_editor.IsVinylsChanged = true;
		}

		private void OnZoomChanged()
		{
			_editor.ResetFocusPos();
			float num = (_zoomSlider.slider.value - 0.5f) * 0.5f;
			_editor.UpdateCamFOV((0f - num) * 1.5f);
		}

		private float GetValueScale(float scale)
		{
			if (scale == 0f)
			{
				return 0f;
			}
			float num = scale;
			int num2 = 1;
			if (num < 0f)
			{
				num2 = -1;
			}
			num = Mathf.Abs(num);
			num = _scaleCurve.Evaluate(num / 0.5f);
			return num * (float)num2 * 0.2f;
		}

		private void DrawLine(Vector3 start, Vector3 end, LineRenderer lineRend)
		{
			lineRend.transform.position = start;
			lineRend.SetPosition(0, start);
			lineRend.SetPosition(1, end);
		}

		public void AnimateSticker(Sprite sprite, Color c, bool save)
		{
			_animatedSticker.gameObject.SetActive(value: true);
			_animStickSprite.sprite = sprite;
			_animStickSprite.color = c;
			_animatedSticker.localScale = Vector3.one * 0.5f;
			_animatedSticker.localPosition = Vector3.zero;
			_animStickAlpha = _animStickSprite.DOFade(0f, 1f);
			_animStickScale = _animatedSticker.DOScale(Vector3.one, 1f);
			_animStickPos = _animatedSticker.DOLocalMove(new Vector3(save ? 1000f : 0f, 300f, 0f), 1f).OnComplete(() =>
			{
				_animatedSticker.gameObject.SetActive(value: false);
			});
		}

		public void FinishAllTweens()
		{
			_animStickAlpha?.Kill();
			_animStickScale?.Kill();
			_animStickPos?.Kill();
		}

		public void HideSomeEditIcons(VinylsEditorOperation operation, bool all = false)
		{
			if (all)
			{
				UpdateAllEditIcons(isEnabled: false);
				return;
			}
			switch (operation)
			{
			case VinylsEditorOperation.Create:
				UpdateAllEditIcons(isEnabled: true);
				if ((bool)middlePoint)
				{
					middlePoint.gameObject.SetActive(value: false);
				}
				break;
			case VinylsEditorOperation.Move:
				UpdateAllEditIcons(isEnabled: false);
				if ((bool)_editor)
				{
					_editor.IsVinylsChanged = true;
				}
				break;
			case VinylsEditorOperation.Rotate:
				UpdateAllEditIcons(isEnabled: false);
				if ((bool)rotIcon)
				{
					rotIcon.gameObject.SetActive(value: true);
				}
				if ((bool)editLine2)
				{
					editLine2.gameObject.SetActive(value: true);
				}
				if ((bool)middlePoint)
				{
					middlePoint.gameObject.SetActive(value: true);
				}
				if ((bool)_editor)
				{
					_editor.IsVinylsChanged = true;
				}
				break;
			case VinylsEditorOperation.Scale:
				UpdateAllEditIcons(isEnabled: false);
				if ((bool)scaleIcon)
				{
					scaleIcon.gameObject.SetActive(value: true);
				}
				if ((bool)middlePoint)
				{
					middlePoint.gameObject.SetActive(value: true);
				}
				if ((bool)editLine1)
				{
					editLine1.gameObject.SetActive(value: true);
				}
				if ((bool)_editor)
				{
					_editor.IsVinylsChanged = true;
				}
				break;
			}
			if (operation != VinylsEditorOperation.None)
			{
				if ((bool)editPlane)
				{
					editPlane.gameObject.SetActive(value: true);
				}
				if ((bool)visualIcons)
				{
					visualIcons.gameObject.SetActive(value: true);
				}
			}
		}

		public void UpdateAllEditIcons(bool isEnabled, bool includeEditPlane = true)
		{
			if ((bool)editLine1)
			{
				editLine1.gameObject.SetActive(isEnabled);
			}
			if ((bool)editLine2)
			{
				editLine2.gameObject.SetActive(isEnabled);
			}
			if ((bool)posIcon)
			{
				posIcon.gameObject.SetActive(isEnabled);
			}
			if ((bool)scaleIcon)
			{
				scaleIcon.gameObject.SetActive(isEnabled);
			}
			if ((bool)rotIcon)
			{
				rotIcon.gameObject.SetActive(isEnabled);
			}
			if ((bool)middlePoint)
			{
				middlePoint.gameObject.SetActive(isEnabled);
			}
			if ((bool)editPlane & includeEditPlane)
			{
				editPlane.gameObject.SetActive(isEnabled);
			}
		}
	}
}
