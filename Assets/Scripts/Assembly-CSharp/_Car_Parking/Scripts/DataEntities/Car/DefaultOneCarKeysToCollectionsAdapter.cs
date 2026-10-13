using UnityEngine;

namespace _Car_Parking.Scripts.DataEntities.Car
{
	public class DefaultOneCarKeysToCollectionsAdapter : IOneCarKeysToCollectionsAdapter
	{
		public void SetVector(string key, Vector3 value)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public Vector3 GetVector(string key, Vector3 defaultValue)
		{
			return defaultValue;
		}

		public void SetFloat(string key, float value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values. key: " + key);
		}

		public float GetFloat(string key, float defaultValue = 0f, bool ifZeroReturnDefault = false)
		{
			return defaultValue;
		}

		public int GetIntFromFloat(string key, int defaultValue = 0, bool ifZeroReturnDefault = false)
		{
			return defaultValue;
		}

		public void SetBoughtPart(string key, int value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public int GetBoughtPart(string key, int defaultValue = 0)
		{
			return defaultValue;
		}

		public bool HasBoughtPart(string key)
		{
			return false;
		}

		public bool IsBought(int i)
		{
			return false;
		}

		public void SetTypeToInstall(int keyInt, int value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public int GetTypeToInstall(int keyInt, int defaultValue = 0)
		{
			return defaultValue;
		}

		public void SetGears(int keyInt, float value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public void SetGears(string key, float value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public float GetGears(int index, float defaultValue = 0f)
		{
			return defaultValue;
		}

		public float GetGears(string key, float defaultValue = 0f)
		{
			return defaultValue;
		}

		public void SetString(string key, string value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public string GetString(string key, string defaultValue = "")
		{
			return defaultValue;
		}

		public void SetColor(string key, Color color, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public Color GetColor(string key, Color defaultValue)
		{
			return defaultValue;
		}

		public void DeleteAllData()
		{
			Debug.LogError("Default one car adapter shouldn't be used to delete any data");
		}

		public void Commit()
		{
			Debug.LogError("Default one car adapter shouldn't be used to commit any data");
		}

		public void SetFlag(int lastSelectedFlagIndex, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public int GetFlag(int i)
		{
			return i;
		}

		public int GetFsoIndex()
		{
			return -1;
		}

		public void SetFsoIndex(int value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public void SetFsoPos(int value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public int GetFsoPos()
		{
			return 0;
		}

		public void SetFsoColor(Color left, Color right, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public Color GetFsoColorPrefsLeft()
		{
			return Color.white;
		}

		public Color GetFsoColorPrefsRight()
		{
			return Color.white;
		}

		public void DeleteKeyFso()
		{
			Debug.LogError("Default one car adapter shouldn't be used to set any values");
		}

		public void SetBoughtPartBuy(string key, int value, bool saveNow = false)
		{
			Debug.LogError("Default one car adapter shouldn't be SetBoughtPartBuy");
		}
	}
}
