using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.Police;

namespace _Car_Parking.Scripts.MenuUI
{
	public class GarageUIRootInitializer : MonoBehaviour
	{
		[SerializeField]
		private Image LeftIndicatorArrowTC;

		[SerializeField]
		private Image RightIndicatorArrowTC;

		[SerializeField]
		private Image EmergencyTC;

		[SerializeField]
		private Image HeadLampIconUI;

		[SerializeField]
		private PoliceSirenStatusUpdateButton _policeSirenButton;

		[SerializeField]
		private SteamUIParentChanger steamUIParentChanger;

		private MenuLevelParent _menuLevelParent;

		[Inject]
		public void Construct(MenuLevelParent menuLevelParent)
		{
			_menuLevelParent = menuLevelParent;
		}

		private void OnEnable()
		{
			VehicleData vehicleData = (V.Car ? V.Car.GetComponent<VehicleData>() : null);
			bool flag = !vehicleData || !CarIdStorage.IsCarWithoutTurnSignals(vehicleData.CarId);
			LeftIndicatorArrowTC.gameObject.SetActive(flag);
			RightIndicatorArrowTC.gameObject.SetActive(flag);
			EmergencyTC.gameObject.SetActive(flag);
			if (flag)
			{
				_menuLevelParent.LeftIndicatorArrowTC = LeftIndicatorArrowTC;
				_menuLevelParent.RightIndicatorArrowTC = RightIndicatorArrowTC;
				_menuLevelParent.EmergencyTC = EmergencyTC;
			}
			_menuLevelParent.HeadLampIconUI = HeadLampIconUI;
			_menuLevelParent.makePoliceButton = _policeSirenButton.gameObject;
		}

		private void Start()
		{
			if ((bool)steamUIParentChanger)
			{
				steamUIParentChanger.ApplyAllChanges();
			}
		}
	}
}
