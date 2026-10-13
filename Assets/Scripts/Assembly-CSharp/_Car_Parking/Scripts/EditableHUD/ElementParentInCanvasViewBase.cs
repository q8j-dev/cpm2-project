using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementParentInCanvasViewBase : MonoBehaviour
	{
		[SerializeField]
		private ElementParentInCanvasBase ElementParentInCanvasBase;

		protected EditableHUDConfig Config => ElementParentInCanvasBase.Config;

		protected virtual void Start()
		{
			ElementParentInCanvasBase.OnPositionChanged += OnPositionChanged;
			ElementParentInCanvasBase.OnScaleChanged += OnScaleChanged;
			ElementParentInCanvasBase.OnVisibleChanged += OnVisibleChanged;
			if ((bool)ElementParentInCanvasBase.Element)
			{
				OnPositionChanged(ElementParentInCanvasBase, ElementParentInCanvasBase.Element, ElementParentInCanvasBase.Position);
				OnScaleChanged(ElementParentInCanvasBase, ElementParentInCanvasBase.Element, ElementParentInCanvasBase.Scale);
				OnVisibleChanged(ElementParentInCanvasBase, ElementParentInCanvasBase.Element, ElementParentInCanvasBase.IsVisible);
			}
		}

		protected virtual void OnDestroy()
		{
			ElementParentInCanvasBase.OnPositionChanged -= OnPositionChanged;
			ElementParentInCanvasBase.OnScaleChanged -= OnScaleChanged;
			ElementParentInCanvasBase.OnVisibleChanged -= OnVisibleChanged;
		}

		protected virtual void OnPositionChanged(ElementParentInCanvasBase elementParent, EditableHUDElement element, Vector3 position)
		{
		}

		protected virtual void OnScaleChanged(ElementParentInCanvasBase elementParent, EditableHUDElement element, float scale)
		{
		}

		protected virtual void OnVisibleChanged(ElementParentInCanvasBase elementParent, EditableHUDElement element, bool visible)
		{
		}

		protected virtual void SetRefs()
		{
			ElementParentInCanvasBase = GetComponent<ElementParentInCanvasBase>();
		}
	}
}
