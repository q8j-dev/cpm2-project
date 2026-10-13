using System;
using CPM.Scripts.Garage;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement.CircularMenu;

namespace _Car_Parking.Scripts.Free_drive
{
	[RequireComponent(typeof(Image), typeof(RectTransform))]
	public class ToRoadIconUpdater : MonoBehaviour
	{
		[SerializeField]
		private Sprite ToRoadIcon;

		[SerializeField]
		private Vector2 SizeToRoadIcon = new Vector2(90f, 80f);

		[SerializeField]
		private Sprite ResetToSlotPositionIcon;

		[SerializeField]
		private Vector2 SizeResetToSlotPositionIcon = new Vector2(80f, 80f);

		private GarageController _garageController;

		private OptionalDependency<GarageController> _garageControllerDep;

		private RectTransform _rectTransform;

		private Image _image;

		private bool _isMenuIcon;

		[Inject]
		public void Construct(OptionalDependency<GarageController> garageController)
		{
			_garageControllerDep = garageController;
		}

		private void Awake()
		{
			_garageController = _garageControllerDep?.Value;
			CircularMenuItem component = GetComponent<CircularMenuItem>();
			_isMenuIcon = (bool)component && (bool)component.Icon;
			_image = (_isMenuIcon ? component.Icon : GetComponent<Image>());
			_rectTransform = (_image ? _image.rectTransform : GetComponent<RectTransform>());
			if ((bool)_garageController)
			{
				_garageController.OnGarageInited += UpdateIconEventHandler;
				_garageController.OnGarageClosed += UpdateIconEventHandler;
			}
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Combine(V.OnInGarageFreeChanged, new Action<bool>(OnInGarageFreeChanged));
		}

		public void UpdateIconEventHandler()
		{
			if ((bool)ToRoadIcon && (bool)ResetToSlotPositionIcon && (bool)_image && (bool)_rectTransform)
			{
				if (FlagsStorage.InGarageFree)
				{
					_image.sprite = ResetToSlotPositionIcon;
					SetSize(SizeResetToSlotPositionIcon);
				}
				else
				{
					_image.sprite = ToRoadIcon;
					SetSize(SizeToRoadIcon);
				}
			}
		}

		private void SetSize(Vector2 size)
		{
			if (!_isMenuIcon)
			{
				_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}
		}

		private void OnDestroy()
		{
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Remove(V.OnInGarageFreeChanged, new Action<bool>(OnInGarageFreeChanged));
			if ((bool)_garageController)
			{
				_garageController.OnGarageInited -= UpdateIconEventHandler;
				_garageController.OnGarageClosed -= UpdateIconEventHandler;
			}
		}

		private void OnInGarageFreeChanged(bool isFree)
		{
			UpdateIconEventHandler();
		}
	}
}
