using UnityEngine;

namespace MVPFree.MapIcons
{
	public class GPSNavigationCrossView : MonoBehaviour
	{
		[SerializeField]
		private GameObject CentreCircle = default;

		public Vector2 CentreCircleScreenPosition
		{
			get
			{
				if (!CentreCircle)
				{
					return new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
				}
				Canvas componentInParent = CentreCircle.GetComponentInParent<Canvas>();
				return RectTransformUtility.WorldToScreenPoint(((bool)componentInParent && componentInParent.renderMode != RenderMode.ScreenSpaceOverlay) ? componentInParent.worldCamera : null, CentreCircle.transform.position);
			}
		}

		public void SetCentreCircleVisible(bool isVisible)
		{
			if ((bool)CentreCircle)
			{
				CentreCircle.SetActive(isVisible);
			}
		}
	}
}
