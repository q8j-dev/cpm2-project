using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class DealerWindowFilterSelectedFilterView : MonoBehaviour
	{
		[SerializeField]
		private DealerWindowFiltersController DealerWindowFiltersController;

		[SerializeField]
		private TMP_Text ClassText;

		[SerializeField]
		private GameObject Root;

		[SerializeField]
		private DealerWindow.FilterType FilterType;

		private void Start()
		{
			DealerWindowFiltersController.OnFilterChanged += FilterChangedCallback;
			FilterChangedCallback();
		}

		private void OnDestroy()
		{
			DealerWindowFiltersController.OnFilterChanged -= FilterChangedCallback;
		}

		private void FilterChangedCallback()
		{
			NewCarsFilterElement isOnFilterElement = DealerWindowFiltersController.GetIsOnFilterElement(FilterType);
			Root.SetActive(isOnFilterElement);
			if ((bool)isOnFilterElement)
			{
				ClassText.text = isOnFilterElement.VehicleClass.ToString();
			}
		}
	}
}
