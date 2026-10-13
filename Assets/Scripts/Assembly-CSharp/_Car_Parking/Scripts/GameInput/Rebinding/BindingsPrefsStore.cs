using UnityEngine;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class BindingsPrefsStore
	{
		public const string PrefsKey = "SteamControls.Bindings";

		public const string WheelSetsKey = "SteamControls.Bindings.Wheels";

		public string Load()
		{
			return PlayerPrefs.GetString("SteamControls.Bindings", string.Empty);
		}

		public string LoadWheelSets()
		{
			return PlayerPrefs.GetString("SteamControls.Bindings.Wheels", string.Empty);
		}

		public void SaveWheelSets(string wheelSetsJson)
		{
			PlayerPrefs.SetString("SteamControls.Bindings.Wheels", wheelSetsJson ?? string.Empty);
			PlayerPrefs.Save();
		}

		public void Save(string overridesJson)
		{
			PlayerPrefs.SetString("SteamControls.Bindings", overridesJson ?? string.Empty);
			PlayerPrefs.Save();
		}

		public void Clear()
		{
			PlayerPrefs.DeleteKey("SteamControls.Bindings");
			PlayerPrefs.Save();
		}
	}
}
