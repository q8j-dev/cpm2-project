using MVPFree.MapIcons.Enum;
using UnityEngine;
using _Car_Parking.Scripts.EventSystem;

namespace MVPFree.MapIcons.Views
{
	public class IconMapView : MonoBehaviour
	{
		private class OrderDefaultData
		{
			public int SpriteRendererOrder;

			public int SpriteRendererBackgroundOrder;

			public int BGCircleSpriteRendererOrder;

			public OrderDefaultData(SpriteRenderer spriteRenderer, SpriteRenderer spriteRendererBackground, SpriteRenderer bgCircleSpriteRenderer)
			{
				SpriteRendererOrder = (spriteRenderer ? spriteRenderer.sortingOrder : 0);
				SpriteRendererBackgroundOrder = (spriteRendererBackground ? spriteRendererBackground.sortingOrder : 0);
				BGCircleSpriteRendererOrder = (bgCircleSpriteRenderer ? bgCircleSpriteRenderer.sortingOrder : 0);
			}
		}

		private const int TopOrder = 1000;

		[SerializeField]
		private GameObject Root;

		[SerializeField]
		private GameObject BGCircle;

		[SerializeField]
		private SpriteRenderer BGCircleSpriteRenderer;

		private OrderDefaultData _orderDefaultData;

		public int Index;

		public IconType IconType;

		public GameObject TimeIcon;

		public GameObject NewIcon;

		public _Car_Parking.Scripts.EventSystem.EventType EventType;

		[field: SerializeField]
		public SpriteRenderer SpriteRenderer { get; private set; }

		[field: SerializeField]
		public SpriteRenderer SpriteRendererBackground { get; private set; }

		public bool IsVisible
		{
			get
			{
				if (base.gameObject.activeSelf && Root.activeSelf)
				{
					if (!SpriteRenderer)
					{
						return true;
					}
					return SpriteRenderer.color.a > 0.01f;
				}
				return false;
			}
		}

		public float BigMapRootScale { get; set; } = 1f;

		public float MiniMapRootScale { get; set; } = 1f;

		public void Init(int index, IconType iconType)
		{
			Index = index;
			IconType = iconType;
		}

		public void SetRootScale(float scale)
		{
			if ((bool)Root)
			{
				Root.transform.localScale = Vector3.one * scale;
			}
		}

		public void ApplyMapRootScale(bool bigMap)
		{
			SetRootScale(bigMap ? BigMapRootScale : MiniMapRootScale);
		}

		public void UpdateRootVisible(bool visible)
		{
			if ((bool)base.gameObject && (bool)Root)
			{
				Root.SetActive(visible);
			}
		}

		public void ShowTimeIcon(bool state)
		{
			TimeIcon.SetActive(state);
		}

		public void ShowNewIcon(bool state)
		{
			NewIcon.SetActive(state);
		}

		public void UpdateIsSelected(bool isSelected, bool updateBGCircle = true)
		{
			if ((bool)BGCircle & updateBGCircle)
			{
				BGCircle.SetActive(isSelected);
			}
			SetOrder(isSelected ? 1000 : 0);
		}

		public void UpdateAlpha(float alpha)
		{
			if ((bool)SpriteRenderer)
			{
				SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alpha);
			}
			if ((bool)SpriteRendererBackground)
			{
				SpriteRendererBackground.color = new Color(SpriteRendererBackground.color.r, SpriteRendererBackground.color.g, SpriteRendererBackground.color.b, (alpha > 0.99f) ? 1f : (alpha * 0.5f));
			}
		}

		private void SetOrder(int offset)
		{
			SaveOrderDefault();
			if ((bool)SpriteRenderer)
			{
				SpriteRenderer.sortingOrder = _orderDefaultData.SpriteRendererOrder + offset;
			}
			if ((bool)SpriteRendererBackground)
			{
				SpriteRendererBackground.sortingOrder = _orderDefaultData.SpriteRendererBackgroundOrder + offset;
			}
			if ((bool)BGCircleSpriteRenderer)
			{
				BGCircleSpriteRenderer.sortingOrder = _orderDefaultData.BGCircleSpriteRendererOrder + offset;
			}
		}

		private void SaveOrderDefault()
		{
			if (_orderDefaultData == null)
			{
				_orderDefaultData = new OrderDefaultData(SpriteRenderer, SpriteRendererBackground, BGCircleSpriteRenderer);
			}
		}

		public void Close()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
