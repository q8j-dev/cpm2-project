using System;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.BuyNewCar
{
	public class BuyCarBodyKitsUI : MonoBehaviour
	{
		[SerializeField]
		private Canvas Canvas;

		[SerializeField]
		private BuyCarUIBodyKitIndexUpdateButtons ButtonPrefab;

		[SerializeField]
		private FaderCanvas FaderCanvas;

		private bool _tempCameraRotationDisablerPressed;

		private bool _buyCarBodyKitsUIPressed;

		public event Action<BodyKitType, int> OnClicked;

		public void AddButton(Vector3 position, BodyKitType bodyKitType, int index)
		{
			BuyCarUIBodyKitIndexUpdateButtons buyCarUIBodyKitIndexUpdateButtons = UnityEngine.Object.Instantiate(ButtonPrefab, position, Quaternion.identity, Canvas.transform);
			buyCarUIBodyKitIndexUpdateButtons.name = "Button_" + bodyKitType;
			buyCarUIBodyKitIndexUpdateButtons.Init(bodyKitType, index);
			buyCarUIBodyKitIndexUpdateButtons.OnClicked += OnBodyKitButtonClicked;
		}

		private void Start()
		{
			Canvas.worldCamera = Camera.main;
		}

		private void OnEnable()
		{
			TempCameraRotationDisabler.OnPointerDownAction = (Action)Delegate.Combine(TempCameraRotationDisabler.OnPointerDownAction, new Action(OnBackgroundPress));
			TempCameraRotationDisabler.OnPointerUpAction = (Action)Delegate.Combine(TempCameraRotationDisabler.OnPointerUpAction, new Action(OnBackgroundRelease));
		}

		private void OnDisable()
		{
			TempCameraRotationDisabler.OnPointerDownAction = (Action)Delegate.Remove(TempCameraRotationDisabler.OnPointerDownAction, new Action(OnBackgroundPress));
			TempCameraRotationDisabler.OnPointerUpAction = (Action)Delegate.Remove(TempCameraRotationDisabler.OnPointerUpAction, new Action(OnBackgroundRelease));
		}

		private void OnBackgroundPress()
		{
			_tempCameraRotationDisablerPressed = true;
			_buyCarBodyKitsUIPressed = false;
			UpdateFade();
		}

		private void OnBackgroundRelease()
		{
			_tempCameraRotationDisablerPressed = false;
			UpdateFade();
		}

		private void OnBodyKitButtonClicked(BodyKitType bodyKitType, int addIndex)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.TuningSelectCarbodySwitch, base.gameObject);
			_buyCarBodyKitsUIPressed = true;
			OnClicked?.Invoke(bodyKitType, addIndex);
			UpdateFade();
		}

		private void UpdateFade()
		{
			if (_buyCarBodyKitsUIPressed || !_tempCameraRotationDisablerPressed)
			{
				FaderCanvas.Fade(1f, 0.5f);
			}
			else
			{
				FaderCanvas.Fade(0f, 0.5f);
			}
		}
	}
}
