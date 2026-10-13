using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	[RequireComponent(typeof(RectTransformInCanvasSpace))]
	public abstract class ScrollViewItemBase : MonoBehaviour
	{
		[SerializeField]
		private RectTransformInCanvasSpace RectTransformInCanvasSpace;

		public Rect RectInCanvasSpace
		{
			get
			{
				if (!RectTransformInCanvasSpace)
				{
					return Rect.zero;
				}
				return RectTransformInCanvasSpace.CanvasRect;
			}
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}
	}
}
