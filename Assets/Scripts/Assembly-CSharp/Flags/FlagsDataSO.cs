using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Flags
{
	[CreateAssetMenu(fileName = "FlagsData", menuName = "Scriptable/FlagsData")]
	public class FlagsDataSO : ScriptableObject
	{
		public List<Texture> textures;

		public List<FlagItem> flags;

		public Texture[] languageTextures;

		[Preserve]
		public FlagsDataSO()
		{
		}

		private void OnValidate()
		{
			flags.Clear();
			for (int i = 0; i < textures.Count; i++)
			{
				flags.Add(new FlagItem(i, textures[i]));
			}
		}
	}
}
