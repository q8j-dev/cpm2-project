using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class RectTransformInCanvasSpace : MonoBehaviour
	{
		[SerializeField]
		private RectTransform Root;

		[SerializeField]
		private float ExtraSizeTop;

		[SerializeField]
		private float ExtraSizeBottom;

		private RectTransform _canvasRectTransform;

		public float ExtraTopSize => ExtraSizeTop;

		public float ExtraBottomSize => ExtraSizeBottom;

		public RectTransform CanvasRectTransform
		{
			get
			{
				Init();
				return _canvasRectTransform;
			}
		}

		public Rect CanvasRect
		{
			get
			{
				Init();
				if (!_canvasRectTransform || !Root)
				{
					return Rect.zero;
				}
				Vector3[] array = new Vector3[4];
				Root.GetWorldCorners(array);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = _canvasRectTransform.InverseTransformPoint(array[i]);
				}
				Vector3 vector = array[0];
				Vector3 vector2 = array[2];
				Rect result = new Rect(vector + new Vector3(_canvasRectTransform.rect.width * 0.5f, _canvasRectTransform.rect.height * 0.5f), vector2 - vector);
				result.height += ExtraSizeTop + ExtraSizeBottom;
				result.y -= ExtraSizeBottom;
				return result;
			}
		}

		private void Init()
		{
			if (!_canvasRectTransform)
			{
				Canvas componentInParent = GetComponentInParent<Canvas>();
				if ((bool)componentInParent)
				{
					_canvasRectTransform = componentInParent.GetComponent<RectTransform>();
				}
			}
		}
	}
}
