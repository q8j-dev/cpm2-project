using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	public class ContentChildNavigationDisabler : MonoBehaviour
	{
		private void OnEnable()
		{
			DisableChildNavigation();
		}

		private void OnTransformChildrenChanged()
		{
			DisableChildNavigation();
		}

		private void DisableChildNavigation()
		{
			Selectable[] componentsInChildren = GetComponentsInChildren<Selectable>(includeInactive: true);
			foreach (Selectable selectable in componentsInChildren)
			{
				Navigation navigation = selectable.navigation;
				if (navigation.mode != Navigation.Mode.None)
				{
					navigation.mode = Navigation.Mode.None;
					selectable.navigation = navigation;
				}
			}
		}
	}
}
