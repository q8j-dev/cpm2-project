namespace CPM.Shifter.Contracts
{
	public interface IShifterControl
	{
		bool IsTouching { get; }

		void SetGearbox(bool isAutomatic);

		void UpdateCar();

		void ShowObjects(bool fromControlUi);

		void HideObjects();

		void SetActiveObjects(bool state);

		void ResetPos();

		void OnClutchReleased();

		void SetNeutral();

		void SetPosByGearJoyStick(int gear);

		void SetGearTraceAlpha(float a);

		void SetGearLabelText(string text);

		void SetAutoMatic();

		void SetManual();

		void SetAutoMode(int selectorState);

		void PlayDriverShiftAnimation();
	}
}
