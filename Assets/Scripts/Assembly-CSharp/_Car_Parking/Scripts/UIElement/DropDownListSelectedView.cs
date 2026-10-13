using NewUI.Scripts1.Elements;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class DropDownListSelectedView : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroupStateTweener CanvasGroupStateTweener;

		public void UpdateVisible(bool isVisible)
		{
			CanvasGroupStateTweener.ChangeState(isVisible);
		}

		private void Awake()
		{
			CanvasGroupStateTweener.DurationFadeEnd = (CanvasGroupStateTweener.DurationFadeStart = 0.5f);
		}
	}
}
