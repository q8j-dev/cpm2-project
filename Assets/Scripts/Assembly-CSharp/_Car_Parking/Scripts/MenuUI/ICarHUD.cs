using UnityEngine;

namespace _Car_Parking.Scripts.MenuUI
{
	public interface ICarHUD
	{
		bool IsDoorShown { get; set; }

		void ShowDoor();

		void HideDoor();

		void ShowLock();

		void ShowUnlock();

		void HideBothLock();

		void HideResetTrailer();

		void ShowResetTrailer(RaycastHit hit);

		void ChangeCarLightMask(GameObject car, int i);

		void SetGearSelf(int value);

		void PreSetValues();

		void ShowHideHood();
	}
}
