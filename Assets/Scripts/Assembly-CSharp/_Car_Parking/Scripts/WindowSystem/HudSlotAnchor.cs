using UnityEngine;

namespace _Car_Parking.Scripts.WindowSystem
{
	[RequireComponent(typeof(RectTransform))]
	public sealed class HudSlotAnchor : MonoBehaviour
	{
		[SerializeField]
		private HudSlot Slot;

		private RectTransform _rect;

		private void Awake()
		{
			_rect = (RectTransform)base.transform;
		}

		private void OnEnable()
		{
			HudSlotRegistry.Register(Slot, _rect);
		}

		private void OnDisable()
		{
			HudSlotRegistry.Unregister(Slot, _rect);
		}
	}
}
