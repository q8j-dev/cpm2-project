using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class SlotIsBusySetter : MonoBehaviour
	{
		private const float DisabledSlotAlpha = 0.5f;

		[SerializeField]
		private CanvasGroup CarCanvasGroup = default;

		[SerializeField]
		private CanvasGroup SlotCanvasGroup = default;

		[SerializeField]
		private GameObject LoadingGameObject = default;

		public void LockSlot(bool state)
		{
			CarCanvasGroup.alpha = (state ? 0.5f : 1f);
			SlotCanvasGroup.interactable = !state;
			SlotCanvasGroup.blocksRaycasts = !state;
			LoadingGameObject.SetActive(state);
		}
	}
}
