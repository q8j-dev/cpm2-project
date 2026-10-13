using CodeStage.AntiCheat.Storage;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	public class AllTimeGamePlayUtil
	{
		public static int Get()
		{
			return (int)ObscuredPrefs.GetFloat("AllTimeGamePlay");
		}

		public static void Set(float value)
		{
			ObscuredPrefs.SetFloat("AllTimeGamePlay", value);
		}
	}
}
