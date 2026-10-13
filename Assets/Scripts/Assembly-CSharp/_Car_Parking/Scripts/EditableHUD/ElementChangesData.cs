using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementChangesData
	{
		public Vector2 Position;

		public float Scale = 1f;

		public bool Visible = true;

		public ElementChangesData(Vector2 position)
		{
			Position = position;
		}

		public void Save(string key)
		{
			PlayerDatas.SetVector3(GetPositionKey(key), Position);
			PlayerPrefs.SetFloat(GetScaleKey(key), Scale);
			PlayerPrefs.SetInt(GetVisibleKey(key), Visible ? 1 : 0);
		}

		public void Load(string key)
		{
			Position = PlayerDatas.GetVector3(GetPositionKey(key), Position);
			Scale = PlayerPrefs.GetFloat(GetScaleKey(key), Scale);
			Visible = PlayerPrefs.GetInt(GetVisibleKey(key), 1) == 1;
		}

		public void ClearPlayerPrefs(string key)
		{
			PlayerDatas.DeletePlayerPrefsVector3(GetPositionKey(key));
			PlayerPrefs.DeleteKey(GetScaleKey(key));
			PlayerPrefs.DeleteKey(GetVisibleKey(key));
		}

		private string GetPositionKey(string key)
		{
			return key + "Position";
		}

		private string GetScaleKey(string key)
		{
			return key + "Scale";
		}

		private string GetVisibleKey(string key)
		{
			return key + "Visible";
		}
	}
}
