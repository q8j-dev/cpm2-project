using System.Threading;
using CPM.Fallout;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.GameActivity;

namespace _Car_Parking.Scripts.UIElement
{
	public sealed class FalloutLimitsView : MonoBehaviour
	{
		[SerializeField]
		private GameActivityType ActivityType;

		[SerializeField]
		private TextMeshProUGUI CoinText;

		[SerializeField]
		private TextMeshProUGUI MoneyText;

		private FalloutLimitsCache _limits;

		[Inject]
		public void Construct(FalloutLimitsCache limits)
		{
			_limits = limits;
		}

		private void OnEnable()
		{
			if (_limits != null)
			{
				_limits.Updated += OnLimitsUpdated;
				if (_limits.TryGet(ActivityType, out var limits))
				{
					Render(limits);
				}
				Refresh(base.destroyCancellationToken).Forget();
			}
		}

		private void OnDisable()
		{
			if (_limits != null)
			{
				_limits.Updated -= OnLimitsUpdated;
			}
		}

		private async UniTaskVoid Refresh(CancellationToken ct)
		{
			Result<FalloutLimits> result = await _limits.GetLimits(ActivityType, ct);
			if (!ct.IsCancellationRequested && result.IsSuccess)
			{
				Render(result.Value);
			}
		}

		private void OnLimitsUpdated(GameActivityType activityType, FalloutLimits limits)
		{
			if (activityType == ActivityType)
			{
				Render(limits);
			}
		}

		private void Render(FalloutLimits limits)
		{
			if (limits != null)
			{
				if ((bool)CoinText)
				{
					CoinText.text = $"{limits.CoinsEarned}/{limits.CoinLimit}";
				}
				if ((bool)MoneyText)
				{
					MoneyText.text = $"{limits.MoneyEarned}/{limits.MoneyLimit}";
				}
			}
		}
	}
}
