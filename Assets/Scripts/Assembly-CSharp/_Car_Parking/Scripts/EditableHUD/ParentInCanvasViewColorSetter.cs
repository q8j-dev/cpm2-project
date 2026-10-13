using System;
using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ParentInCanvasViewColorSetter : ParentInCanvasViewBase
	{
		private Color DefaultColorElement => Color.white;

		protected override void Start()
		{
			base.Start();
			ElementParentInCanvasEditorWindow parentInCanvasEditorWindow = ParentInCanvasEditorWindow;
			parentInCanvasEditorWindow.OnSelectUpdated = (Action<ElementParentInCanvasEditorWindow, bool>)Delegate.Combine(parentInCanvasEditorWindow.OnSelectUpdated, new Action<ElementParentInCanvasEditorWindow, bool>(SelectedUpdatedCallback));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ElementParentInCanvasEditorWindow parentInCanvasEditorWindow = ParentInCanvasEditorWindow;
			parentInCanvasEditorWindow.OnSelectUpdated = (Action<ElementParentInCanvasEditorWindow, bool>)Delegate.Remove(parentInCanvasEditorWindow.OnSelectUpdated, new Action<ElementParentInCanvasEditorWindow, bool>(SelectedUpdatedCallback));
		}

		private void SelectedUpdatedCallback(ElementParentInCanvasEditorWindow parentEditorWindow, bool isSelect)
		{
			SetColor(isSelect ? ParentInCanvasEditorWindow.Config.EditorSettings.SelectedColor : DefaultColorElement);
		}

		private void SetColor(Color color)
		{
			CanvasRenderer[] componentsInChildren = GetComponentsInChildren<CanvasRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetColor(color);
			}
		}
	}
}
