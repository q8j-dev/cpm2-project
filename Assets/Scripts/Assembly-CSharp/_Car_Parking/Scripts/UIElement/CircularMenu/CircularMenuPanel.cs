using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.UIElement.CircularMenu
{
	public class CircularMenuPanel : MonoBehaviour
	{
		[Header("UI Layout")]
		public RadialLayout parentRadialLayout;

		public CanvasGroup contentParentCanvasGroup;

		public LocalizedString panelNameID;

		[Header("Selection Settings")]
		public float deadZone = 0.1f;

		public float selectionThreshold = 0.5f;

		public float selectorRadius = 200f;

		public float itemSelectionAngle = 45f;

		public float mouseSensitivity = 0.01f;

		[Header("Animation")]
		public float showDuration = 0.2f;

		public float hideDuration = 0.15f;

		[Header("Button Bindings")]
		[SerializeField]
		private List<CircularItemBinding> _buttonBindings = new List<CircularItemBinding>();

		[Header("Availability")]
		[Tooltip("Panel is skipped in scroll when the current car has no transfer case (non-4WD). Use for the transfer-case / diff-lock panel.")]
		[SerializeField]
		private bool _requiresTransferCase;

		private CircularMenuItem[] _cachedItems;

		public IReadOnlyList<CircularItemBinding> Bindings => _buttonBindings;

		public bool RequiresTransferCase => _requiresTransferCase;

		private void Awake()
		{
			contentParentCanvasGroup.alpha = 0f;
			base.transform.localScale = Vector3.zero;
		}

		public CircularMenuItem[] GetItems()
		{
			if (_cachedItems == null)
			{
				_cachedItems = GetComponentsInChildren<CircularMenuItem>(includeInactive: false);
			}
			return _cachedItems;
		}

		public void InvalidateItemsCache()
		{
			_cachedItems = null;
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
			if (!active)
			{
				InvalidateItemsCache();
			}
		}
	}
}
