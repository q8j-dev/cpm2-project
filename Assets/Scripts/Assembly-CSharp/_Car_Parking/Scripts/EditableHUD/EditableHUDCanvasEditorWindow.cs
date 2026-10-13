using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditableHUDCanvasEditorWindow : EditableHUDCanvasBase
	{
		[SerializeField]
		private Button ButtonVisible = default;

		[SerializeField]
		private Button ButtonDefault = default;

		[SerializeField]
		private Button ButtonDefaultAll = default;

		[SerializeField]
		private Button ButtonSaveAndExit = default;

		[SerializeField]
		private Slider SliderScale = default;

		[SerializeField]
		private ElementParentInCanvasEditorWindow ElementParentInCanvasPrefab = default;

		[SerializeField]
		private RectTransform ElementsParent = default;

		private ElementParentInCanvasEditorWindow _elementActive;

		private List<ElementParentInCanvasEditorWindow> _elements = new List<ElementParentInCanvasEditorWindow>();

		public EditableHUDElement ActiveEditableElement
		{
			get
			{
				if (!_elementActive)
				{
					return null;
				}
				return _elementActive.Element;
			}
		}

		public event Action<EditableHUDCanvasEditorWindow, List<EditableHUDElement>> OnClose;

		public event Action<EditableHUDElement> OnActiveElementChangesChanged;

		public override void AddElement(EditableHUDElement element, FakeElement fakeElementPrefab, EditableHUDConfig config)
		{
			ElementParentInCanvasEditorWindow elementParentInCanvasEditorWindow = UnityEngine.Object.Instantiate(ElementParentInCanvasPrefab, ElementsParent);
			elementParentInCanvasEditorWindow.Init(element, fakeElementPrefab, config);
			elementParentInCanvasEditorWindow.OnClick = (Action<ElementParentInCanvasEditorWindow>)Delegate.Combine(elementParentInCanvasEditorWindow.OnClick, new Action<ElementParentInCanvasEditorWindow>(OnElementClick));
			_elements.Add(elementParentInCanvasEditorWindow);
		}

		public override void Clear()
		{
			_elements.Clear();
			_elementActive = null;
			ElementsParent.RemoveAllChild();
			OnActiveElementChangesUpdated(ActiveEditableElement);
		}

		private void OnEnable()
		{
			EditableHUDEditingSession.SetOpen(isOpen: true);
			ButtonVisible.onClick.AddListener(OnVisibleClick);
			ButtonDefault.onClick.AddListener(OnDefaultClick);
			ButtonDefaultAll.onClick.AddListener(OnDefaultAllClick);
			ButtonSaveAndExit.onClick.AddListener(OnSaveAndExitClick);
			SliderScale.onValueChanged.AddListener(OnSliderScaleChange);
		}

		private void OnDisable()
		{
			ButtonVisible.onClick.RemoveListener(OnVisibleClick);
			ButtonDefault.onClick.RemoveListener(OnDefaultClick);
			ButtonDefaultAll.onClick.RemoveListener(OnDefaultAllClick);
			ButtonSaveAndExit.onClick.RemoveListener(OnSaveAndExitClick);
			SliderScale.onValueChanged.RemoveListener(OnSliderScaleChange);
			RemoveElementListeners();
			_elementActive = null;
			_elements.Clear();
			EditableHUDEditingSession.SetOpen(isOpen: false);
		}

		private void AddElementListeners()
		{
			if ((bool)ActiveEditableElement)
			{
				ActiveEditableElement.OnChangesChanged += OnActiveElementChangesUpdated;
			}
		}

		private void RemoveElementListeners()
		{
			if ((bool)ActiveEditableElement)
			{
				ActiveEditableElement.OnChangesChanged -= OnActiveElementChangesUpdated;
			}
		}

		private void OnElementClick(ElementParentInCanvasEditorWindow elementParentInCanvas)
		{
			if ((bool)_elementActive)
			{
				_elementActive.SetSelect(isSelected: false);
			}
			RemoveElementListeners();
			_elementActive = elementParentInCanvas;
			AddElementListeners();
			if ((bool)_elementActive)
			{
				_elementActive.SetSelect(isSelected: true);
			}
			OnActiveElementChangesUpdated(ActiveEditableElement);
		}

		private void OnActiveElementChangesUpdated(EditableHUDElement element)
		{
			OnActiveElementChangesChanged?.Invoke(ActiveEditableElement);
		}

		private void OnVisibleClick()
		{
			if ((bool)ActiveEditableElement)
			{
				ActiveEditableElement.IsVisible = !ActiveEditableElement.IsVisible;
			}
		}

		private void OnDefaultClick()
		{
			if ((bool)ActiveEditableElement)
			{
				ActiveEditableElement.ClearChanges();
			}
		}

		private void OnDefaultAllClick()
		{
			for (int i = 0; i < _elements.Count; i++)
			{
				_elements[i].Element.ClearChanges();
			}
		}

		private void OnSaveAndExitClick()
		{
			if ((bool)_elementActive)
			{
				_elementActive.SetSelect(isSelected: false);
			}
			List<EditableHUDElement> list = new List<EditableHUDElement>();
			for (int i = 0; i < _elements.Count; i++)
			{
				if ((bool)_elements[i].Element)
				{
					_elements[i].Element.HideEditorGraphics();
					list.Add(_elements[i].Element);
				}
			}
			OnClose?.Invoke(this, list);
			base.gameObject.SetActive(value: false);
		}

		private void OnSliderScaleChange(float value)
		{
			if ((bool)ActiveEditableElement)
			{
				ActiveEditableElement.Scale = value;
			}
		}
	}
}
