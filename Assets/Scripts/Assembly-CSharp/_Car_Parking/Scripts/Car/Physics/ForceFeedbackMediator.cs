using System.Runtime.InteropServices;

namespace _Car_Parking.Scripts.Car.Physics
{
	public static class ForceFeedbackMediator
	{
		[DllImport("user32")]
		public static extern int GetForegroundWindow();

		[DllImport("UnityForceFeedback")]
		public static extern int InitDirectInput(int HWND);

		[DllImport("UnityForceFeedback")]
		public static extern int Aquire();

		[DllImport("UnityForceFeedback")]
		public static extern int SetDeviceForcesXY(int x, int y);

		[DllImport("UnityForceFeedback")]
		public static extern int StartEffect();

		[DllImport("UnityForceFeedback")]
		public static extern int StopEffect();

		[DllImport("UnityForceFeedback")]
		public static extern int SetAutoCenter(bool autoCenter);

		[DllImport("UnityForceFeedback")]
		public static extern void FreeDirectInput();
	}
}
