using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class TimeAttackNurbBanner : TimeAttackBannerBase
	{
		private const string RequiredCarClassColorTag = "yellow";

		[SerializeField]
		private Image[] DotIcons;

		[SerializeField]
		private Color DefaultDotColor;

		[SerializeField]
		private Color ReadyDotColor;

		protected override async UniTask SetProgress(int cachedProgress, CancellationToken token)
		{
			TimeAttackNurburgEventData obj = (TimeAttackNurburgEventData)base.Event.Data;
			bool flag = base.Event.Status == EventStatus.Claimed;
			List<int> listFromBitMask = BitMaskToListConverter.GetListFromBitMask(obj.WonCarClasses, DotIcons.Length);
			for (int i = 0; i < listFromBitMask.Count; i++)
			{
				bool flag2 = flag || listFromBitMask[i] == 1;
				DotIcons[i].color = (flag2 ? ReadyDotColor : DefaultDotColor);
			}
		}

		protected override void UIInitialize()
		{
			TimeAttackNurburgEventData timeAttackNurburgEventData = (TimeAttackNurburgEventData)base.Event.Data;
			string text = new LocalizedString("Newcarswindow.Class");
			List<VehicleClass> vehicleClasses = base._eventRestrictionService.GetVehicleClasses(base.Event.Type);
			if (vehicleClasses == null)
			{
				return;
			}
			for (int i = 0; i < vehicleClasses.Count; i++)
			{
				if (timeAttackNurburgEventData.MaxLapTimeSet.Length > i)
				{
					string text2 = Utils.FormatTime(timeAttackNurburgEventData.MaxLapTimeSet[i]);
					Points[i].text = string.Format("{0} <color={1}>{2}</color> ≤ {3}", text, "yellow", vehicleClasses[i], text2);
				}
			}
		}
	}
}
