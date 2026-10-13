using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ObstacleRaceSelectedRowHintAnchor : MonoBehaviour
	{
		[SerializeField]
		private ObstacleRaceSpectatorListView List;

		[Tooltip("Spawner target the hints are spawned into; moved and resized to cover the selected row.")]
		[SerializeField]
		private RectTransform Frame;

		[SerializeField]
		[Tooltip("The list's scroll viewport; the hints hide while the selected row is outside it.")]
		private RectTransform Viewport;

		private static readonly Vector2 Center = new Vector2(0.5f, 0.5f);

		private readonly Vector3[] _corners = new Vector3[4];

		private void Awake()
		{
			if ((bool)Frame)
			{
				Frame.anchorMin = Center;
				Frame.anchorMax = Center;
				Frame.pivot = Center;
			}
		}

		private void LateUpdate()
		{
			if ((bool)Frame)
			{
				RectTransform rectTransform = (((bool)List && List.isActiveAndEnabled) ? List.SelectedRow : null);
				bool flag = (bool)rectTransform && rectTransform.gameObject.activeInHierarchy && IsInsideViewport(rectTransform);
				if (Frame.gameObject.activeSelf != flag)
				{
					Frame.gameObject.SetActive(flag);
				}
				if (flag)
				{
					CoverRow(rectTransform);
				}
			}
		}

		private bool IsInsideViewport(RectTransform row)
		{
			if (!Viewport)
			{
				return true;
			}
			Vector3 point = Viewport.InverseTransformPoint(row.TransformPoint(row.rect.center));
			return Viewport.rect.Contains(point);
		}

		private void CoverRow(RectTransform row)
		{
			Transform parent = Frame.parent;
			row.GetWorldCorners(_corners);
			Vector3 vector = parent.InverseTransformPoint(_corners[0]);
			Vector3 vector2 = parent.InverseTransformPoint(_corners[2]);
			Vector3 vector3 = (vector + vector2) * 0.5f;
			Vector2 vector4 = vector2 - vector;
			if (Frame.localPosition != vector3)
			{
				Frame.localPosition = vector3;
			}
			if (Frame.sizeDelta != vector4)
			{
				Frame.sizeDelta = vector4;
			}
		}
	}
}
