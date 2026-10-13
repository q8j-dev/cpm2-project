using System;
using UnityEngine;
using UnityEngine.EventSystems;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementParentInCanvasEditorWindow : ElementParentInCanvasBase, IPointerDownHandler, IEventSystemHandler
	{
		public Action<ElementParentInCanvasEditorWindow> OnClick;

		public Action<ElementParentInCanvasEditorWindow, bool> OnSelectUpdated;

		[SerializeField]
		private CanvasGroup CanvasGroup;

		[SerializeField]
		private Transform MainRoot;

		[SerializeField]
		private DragUIController DragUIController;

		[SerializeField]
		private Transform EditorRoot;

		protected override Transform ElementsParent => MainRoot;

		protected override float SizeDeltaScale => base.Config.EditorSettings.TapZoneSize;

		public override bool IsVisible
		{
			get
			{
				if ((bool)element)
				{
					return element.IsVisible;
				}
				return false;
			}
		}

		public new void Init(EditableHUDElement element, FakeElement fakeElementPrefab, EditableHUDConfig config)
		{
			base.Init(element, fakeElementPrefab, config);
			OnHUDChangesUpdated(base.Element);
			if (base.Element.IsStatic)
			{
				DragUIController.enabled = false;
			}
			element.ShowEditorGraphics(EditorRoot);
		}

		public void OnPointerDown(PointerEventData pointerEventData)
		{
			OnClick(this);
		}

		public void SetSelect(bool isSelected)
		{
			OnSelectUpdated(this, isSelected);
		}

		protected override void OnHUDChangesUpdated(EditableHUDElement element)
		{
			base.OnHUDChangesUpdated(element);
			VisibleChanged(element.IsVisible);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			DragUIController dragUIController = DragUIController;
			dragUIController.OnEndDragAction = (Action<DragUIController>)Delegate.Combine(dragUIController.OnEndDragAction, new Action<DragUIController>(OnDragEnd));
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			DragUIController dragUIController = DragUIController;
			dragUIController.OnEndDragAction = (Action<DragUIController>)Delegate.Remove(dragUIController.OnEndDragAction, new Action<DragUIController>(OnDragEnd));
		}

		private void OnDragEnd(DragUIController dragUIController)
		{
			if ((bool)element)
			{
				element.ChangesPosition = element.CentreScreenPosition;
			}
		}
	}
}
