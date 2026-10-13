using CodeStage.AntiCheat.Storage;

namespace _Car_Parking.Scripts.EngineTuning
{
	public class PlayerTotalDistance
	{
		public static int Get()
		{
			return (int)ObscuredPrefs.GetFloat("PlayerTotalDistance");
		}

		public static void Set(float value)
		{
			ObscuredPrefs.SetFloat("PlayerTotalDistance", value);
		}
	}
}
