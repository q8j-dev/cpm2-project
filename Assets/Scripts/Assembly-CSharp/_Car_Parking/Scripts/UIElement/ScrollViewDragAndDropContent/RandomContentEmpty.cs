using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class RandomContentEmpty : MonoBehaviour
	{
		private void Start()
		{
			UpdateContentAsync().Forget();
		}

		private async UniTaskVoid UpdateContentAsync()
		{
			await UniTask.Yield();
			ItemContentDraggable[] componentsInChildren = GetComponentsInChildren<ItemContentDraggable>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SelectVisibleUpdate(visible: false);
				if (Random.value > 0.35f)
				{
					componentsInChildren[i].ContentVisibleUpdate(visible: true);
					componentsInChildren[i].EmptyVisibleUpdate(visible: false);
				}
				else
				{
					componentsInChildren[i].ContentVisibleUpdate(visible: false);
					componentsInChildren[i].EmptyVisibleUpdate(visible: true);
				}
			}
		}
	}
}
