using System.Collections.Generic;
using UnityEngine;

namespace CPM.DailyTasks.Daily
{
	[CreateAssetMenu(fileName = "textures", menuName = "Scriptable/BattlePass/Daily/Textures", order = 1)]
	public class TexturesSO : ScriptableObject
	{
		[SerializeField]
		private List<TextureData> textures = default;

		private Dictionary<string, Texture> _dict;

		public Dictionary<string, Texture> dict
		{
			get
			{
				if (_dict != null)
				{
					return _dict;
				}
				_dict = new Dictionary<string, Texture>();
				foreach (TextureData texture in textures)
				{
					_dict.Add(texture.name, texture.texture);
				}
				return _dict;
			}
		}
	}
}
