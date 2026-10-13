using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	[CreateAssetMenu(fileName = "DragItemAnimation", menuName = "Configs/UI/ScrollView/DragItemAnimation")]
	public class ItemContentDraggableAnimationConfig : ScriptableObject
	{
		public float ItemBackVelocity;

		public float DestroyAnimationTime;
	}
}
