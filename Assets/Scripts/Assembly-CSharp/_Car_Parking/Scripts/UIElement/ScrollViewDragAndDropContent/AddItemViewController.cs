using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class AddItemViewController : MonoBehaviour
	{
		public enum AddType
		{
			None = 0,
			Back = 1,
			Front = 2
		}

		[SerializeField]
		private GameObject AddItemBack;

		[SerializeField]
		private GameObject AddItemFront;

		public AddType CurrentAddType
		{
			get
			{
				if (AddItemBack.activeSelf)
				{
					return AddType.Back;
				}
				if (AddItemFront.activeSelf)
				{
					return AddType.Front;
				}
				return AddType.None;
			}
		}

		public void ShowGraphics(AddType addType)
		{
			HideGraphics();
			GameObject graphics = GetGraphics(addType);
			if ((bool)graphics)
			{
				graphics.SetActive(value: true);
			}
		}

		public void HideGraphics()
		{
			AddItemBack.SetActive(value: false);
			AddItemFront.SetActive(value: false);
		}

		private GameObject GetGraphics(AddType addType)
		{
			return addType switch
			{
				AddType.Front => AddItemFront, 
				AddType.Back => AddItemBack, 
				_ => null, 
			};
		}
	}
}
