using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class NewYearSnowMenBanner : EventView
	{
		[SerializeField]
		private TextMeshProUGUI ProgressText;

		public override void Init(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			base.Init(@event);
			UpdateTimer();
			base.ClaimedPanel.SetActive(value: false);
			SetPanelsActive();
			SetupBanner().Forget();
		}

		private void SetPanelsActive()
		{
			if (base.Event.Status == EventStatus.Finished)
			{
				base.ClaimButtonAnimationParent.gameObject.SetActive(value: true);
			}
			else
			{
				base.ClaimButtonAnimationParent.gameObject.SetActive(value: false);
			}
		}

		protected override UniTask SetProgress(int cachedProgress, CancellationToken token)
		{
			ProgressText.text = $"{base.Event.Progress}/{base.Event.MaxProgress}";
			return base.SetProgress(cachedProgress, token);
		}
	}
}
