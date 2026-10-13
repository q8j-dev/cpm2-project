using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public abstract class EditorWindowViewBase : MonoBehaviour
	{
		[SerializeField]
		private EditableHUDCanvasEditorWindow EditorWindow;

		protected virtual void OnEnable()
		{
			EditorWindow.OnActiveElementChangesChanged += OnActiveElementChanged;
			OnActiveElementChanged(EditorWindow.ActiveEditableElement);
		}

		protected void OnDisable()
		{
			EditorWindow.OnActiveElementChangesChanged -= OnActiveElementChanged;
		}

		protected virtual void OnActiveElementChanged(EditableHUDElement activeElement)
		{
		}

		protected virtual void SetRefs()
		{
			EditorWindow = GetComponentInParent<EditableHUDCanvasEditorWindow>();
		}
	}
}
