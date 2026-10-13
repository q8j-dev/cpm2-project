using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Racing.Scripts.UserColors
{
	public class SavedColorCollection
	{
		public const int MaxColorCount = 10;

		private List<Color> _savedColors = new List<Color>();

		public Color this[int id] => _savedColors[id];

		public int Count => _savedColors.Count;

		public event Action<int> OnColorAdded;

		public event Action<int> OnColorChanged;

		public event Action<int> OnColorRemoved;

		public SavedColorCollection()
		{
			if (PlayerPrefs.HasKey("SavedColors"))
			{
				string value = PlayerPrefs.GetString("SavedColors");
				_savedColors = (from v in JsonConvert.DeserializeObject<List<Vector3>>(value)
					select v.ToColor()).ToList();
			}
		}

		public void AddColor(Color color)
		{
			if (_savedColors.Count >= 10)
			{
				Debug.LogError("Max color count reached");
				return;
			}
			_savedColors.Add(color);
			OnColorAdded?.Invoke(_savedColors.Count - 1);
			Save();
		}

		public void ChangeColor(int id, Color color)
		{
			_savedColors[id] = color;
			OnColorChanged?.Invoke(id);
			Save();
		}

		public void RemoveColor(int id)
		{
			_savedColors.Remove(_savedColors[id]);
			OnColorRemoved?.Invoke(id);
			Save();
		}

		private void Save()
		{
			if (_savedColors.Count > 0)
			{
				string value = JsonConvert.SerializeObject(_savedColors.Select((Color color) => color.ToVector3()).ToList(), new JsonSerializerSettings
				{
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				});
				PlayerPrefs.SetString("SavedColors", value);
			}
		}
	}
}
