using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class DriftTimeAttackBanner : TimeAttackBannerBase
	{
		private const string ColorKey = "yellow";

		[SerializeField]
		private Image[] DotIcons;

		[SerializeField]
		private Color DefaultDotColor;

		[SerializeField]
		private Color ReadyDotColor;

		[SerializeField]
		private TextMeshProUGUI TargetPointsText;

		protected override async UniTask SetProgress(int cachedProgress, CancellationToken token)
		{
			bool flag = cachedProgress == base.Event.MaxProgress;
			for (int i = 0; i < DotIcons.Length; i++)
			{
				DotIcons[i].color = (flag ? ReadyDotColor : DefaultDotColor);
			}
			float points = ((DriftAttackEventData)base.Event.Data).Points;
			TargetPointsText.text = string.Format("{0} <color={1}> {2} DP", TargetPointsText.text, "yellow", points);
		}
	}
}
