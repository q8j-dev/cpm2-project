using UnityEngine;

namespace Service.Delivery
{
	[CreateAssetMenu(fileName = "DeliveryItems", menuName = "Scriptable/DeliveryItems", order = 1)]
	public class ItemScriptables : ScriptableObject
	{
		[SerializeField]
		private GameObject items = default;

		public GameObject GetItems(bool isMine = true)
		{
			return items;
		}
	}
}
