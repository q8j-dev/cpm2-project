using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Racing.Scripts.UIGrid;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

namespace Racing.Scripts.UserColors
{
	public class SavedColorUIElement : MonoBehaviour, IGridElement<SavedColorUIElement>, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		[SerializeField]
		private Button button;

		[SerializeField]
		private GameObject background;

		[SerializeField]
		private Image image;

		[SerializeField]
		private GameObject plusIcon;

		[SerializeField]
		private GameObject removeIcon;

		private SavedColorUIHandler _gridController;

		private SavedColorCollection _savedColorCollection;

		private ColorPicker _colorPicker;

		private const float LongPressThreshold = 1f;

		private bool _isPointerDown;

		private ManagerUI _managerUI;

		private bool _isSelected;

		private IDisposable _timer;

		private bool _isAddButton;

		private Tweener _scaler;

		private Tweener _shaker;

		public int ID { get; set; }

		[Inject]
		public void Construct(SavedColorCollection savedColorCollection, ManagerUI managerUI)
		{
			_savedColorCollection = savedColorCollection;
			_managerUI = managerUI;
		}

		public void Init(GridController<SavedColorUIElement> gridController)
		{
			_gridController = (SavedColorUIHandler)gridController;
			_colorPicker = _gridController.colorPicker;
		}

		public void Populate(int id, bool isInteractable = true, bool ignoreEmptySlots = false)
		{
			button.onClick.RemoveAllListeners();
			plusIcon.SetActive(value: false);
			removeIcon.SetActive(value: false);
			_isSelected = false;
			_isAddButton = false;
			ID = id;
			background.gameObject.SetActive(value: false);
			image.color = _savedColorCollection[id];
			button.interactable = isInteractable;
			button.onClick.AddListener(() =>
			{
				_colorPicker.Init(_savedColorCollection[id]);
				_gridController.OnElementSelect(this);
			});
		}

		public void InitAddButton()
		{
			button.onClick.RemoveAllListeners();
			plusIcon.SetActive(value: true);
			removeIcon.SetActive(value: false);
			image.color = Color.clear;
			background.gameObject.SetActive(value: true);
			button.onClick.AddListener(() =>
			{
				_savedColorCollection.AddColor(_colorPicker.color);
			});
			button.interactable = true;
			_isAddButton = true;
		}

		public void SetSelected(bool isSelected)
		{
			_isSelected = isSelected;
			if (!_isSelected)
			{
				_isPointerDown = false;
				ResetEffects();
			}
		}

		public void DeleteElement()
		{
			image.color = Color.clear;
			background.gameObject.SetActive(value: true);
			plusIcon.gameObject.SetActive(value: false);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!_isAddButton)
			{
				_isPointerDown = false;
				ResetEffects();
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_isAddButton)
			{
				return;
			}
			_isPointerDown = true;
			ResetEffects();
			_scaler = base.transform.DOScale(2f, 1f).OnComplete(() =>
			{
				base.transform.localScale = Vector3.one;
			});
			_shaker = base.transform.DOShakeRotation(1f, new Vector3(0f, 0f, 20f), 50).SetLoops(-1, LoopType.Yoyo);
			_timer = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				if (_isPointerDown)
				{
					_gridController.VisualTuningWindow.ShowUI(state: false);
					_managerUI.ShowConfirmWindow(() =>
					{
						_gridController.VisualTuningWindow.ShowUI(state: true);
						_savedColorCollection.RemoveColor(ID);
					}, new LocalizedString("SavedColorUIElement.RemoveColor"), new LocalizedString("SavedColorUIElement.DoYouWantToRemoveThisColor"), () =>
					{
						_gridController.VisualTuningWindow.ShowUI(state: true);
					}).Forget();
				}
			}).AddTo(this);
		}

		private void ResetEffects()
		{
			_timer?.Dispose();
			_scaler?.Kill();
			_shaker?.Kill();
			Transform obj = base.transform;
			obj.localScale = Vector3.one;
			obj.localRotation = Quaternion.identity;
		}
	}
}
