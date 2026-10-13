using UnityEngine;

namespace _Car_Parking.Scripts.DataEntities.Car
{
	public interface IOneCarKeysToCollectionsAdapter
	{
		void SetVector(string key, Vector3 value);

		Vector3 GetVector(string key, Vector3 defaultValue);

		void SetFloat(string key, float value, bool saveNow = false);

		float GetFloat(string key, float defaultValue = 0f, bool ifZeroReturnDefault = false);

		int GetIntFromFloat(string key, int defaultValue = 0, bool ifZeroReturnDefault = false);

		void SetBoughtPart(string key, int value, bool saveNow = false);

		int GetBoughtPart(string key, int defaultValue = 0);

		bool HasBoughtPart(string key);

		bool IsBought(int i);

		void SetTypeToInstall(int keyInt, int value, bool saveNow = false);

		int GetTypeToInstall(int keyInt, int defaultValue = 0);

		void SetGears(int keyInt, float value, bool saveNow = false);

		void SetGears(string key, float value, bool saveNow = false);

		float GetGears(int index, float defaultValue = 0f);

		float GetGears(string key, float defaultValue = 0f);

		void SetString(string key, string value, bool saveNow = false);

		string GetString(string key, string defaultValue = "");

		void SetColor(string key, Color color, bool saveNow = false);

		Color GetColor(string key, Color defaultValue);

		void DeleteAllData();

		void Commit();

		void SetFlag(int lastSelectedFlagIndex, bool saveNow = false);

		int GetFlag(int i);

		int GetFsoIndex();

		void SetFsoIndex(int value, bool saveNow = false);

		void SetFsoPos(int value, bool saveNow = false);

		int GetFsoPos();

		void SetFsoColor(Color left, Color right, bool saveNow = false);

		Color GetFsoColorPrefsLeft();

		Color GetFsoColorPrefsRight();

		void DeleteKeyFso();

		void SetBoughtPartBuy(string key, int value, bool saveNow = false);
	}
}
