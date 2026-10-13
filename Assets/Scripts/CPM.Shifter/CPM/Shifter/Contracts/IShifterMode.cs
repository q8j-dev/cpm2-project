namespace CPM.Shifter.Contracts
{
	public interface IShifterMode
	{
		bool IsTouching { get; }

		void Activate();

		void Deactivate();

		void SyncToCurrentGear();

		void ResetToNeutral();

		void SetPosByGear(int gear);

		void SetGearTraceAlpha(float a);

		void SetGearLabelText(string text);

		void PlayDriverShiftAnimation();
	}
}
