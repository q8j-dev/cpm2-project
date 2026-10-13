using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ParentInCanvasViewBase : MonoBehaviour
	{
		[SerializeField]
		protected ElementParentInCanvasEditorWindow ParentInCanvasEditorWindow;

		protected virtual void Start()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void SetRefs()
		{
			ParentInCanvasEditorWindow = GetComponent<ElementParentInCanvasEditorWindow>();
		}
	}
}
