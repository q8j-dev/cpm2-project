using System;
using CPM.Scripts.Garage;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class CarSelectorByTouch : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		[SerializeField]
		private float MaxTapMovement = 20f;

		private GarageController _garageController;

		private Vector2 _pointerDownPos;

		private bool _isPointerDown;

		private Camera _mainCamera;

		public event Action Selected;

		[Inject]
		public void Construct(GarageController garageController)
		{
			_garageController = garageController;
		}

		private void Awake()
		{
			_mainCamera = Camera.main;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_pointerDownPos = eventData.position;
			_isPointerDown = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_isPointerDown && !FlagsStorage.OutOfCar)
			{
				if (Vector2.Distance(_pointerDownPos, eventData.position) <= MaxTapMovement)
				{
					TrySelectCar(eventData.position);
				}
				_isPointerDown = false;
			}
		}

		private void TrySelectCar(Vector2 screenPosition)
		{
			RaycastHit hitInfo;
			if (_mainCamera == null)
			{
				Debug.LogError("Camera not assigned.");
			}
			else if (Physics.Raycast(_mainCamera.ScreenPointToRay(screenPosition), out hitInfo))
			{
				Transform parent = hitInfo.collider.transform.parent;
				if ((bool)parent && parent.TryGetComponent<VehicleData>(out var component))
				{
					OnCarSelected(component);
				}
			}
		}

		private void OnCarSelected(VehicleData car)
		{
			int slotId = car.slotId;
			_garageController.FocusOnCarInMenu(slotId);
			Selected?.Invoke();
		}
	}
}
