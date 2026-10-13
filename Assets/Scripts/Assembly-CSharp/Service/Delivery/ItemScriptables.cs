using UnityEngine;

namespace Service.Delivery
{
	[CreateAssetMenu(fileName = "DeliveryItems", menuName = "Scriptable/DeliveryItems", order = 1)]
	public class ItemScriptables : ScriptableObject
	{
		[SerializeField]
		private GameObject items;

		public GameObject GetItems(bool isMine = true)
		{
			return items;
		}
	}
}
