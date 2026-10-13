using NewUI.Scripts1.Elements;
using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class DealerWindowFilterSelectedClassView : MonoBehaviour
	{
		[SerializeField]
		private DealerWindowFiltersController DealerWindowFiltersController;

		[SerializeField]
		private TMP_Text ClassText;

		[SerializeField]
		private GameObject Root;

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

		private void OnEnable()
		{
			DealerWindowFiltersController.OnFilterChanged += FilterChangedCallback;
			FilterChangedCallback();
		}

		private void OnDisable()
		{
			DealerWindowFiltersController.OnFilterChanged -= FilterChangedCallback;
		}

		private void FilterChangedCallback()
		{
			NewCarsFilterElement isOnFilterElement = DealerWindowFiltersController.GetIsOnFilterElement(DealerWindow.FilterType.CarClass);
			Root.SetActive(isOnFilterElement);
			if ((bool)isOnFilterElement)
			{
				ClassText.text = isOnFilterElement.VehicleClass.ToString();
			}
		}
	}
}
