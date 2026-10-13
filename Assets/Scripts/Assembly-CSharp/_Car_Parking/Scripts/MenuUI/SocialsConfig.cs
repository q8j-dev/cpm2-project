using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace _Car_Parking.Scripts.MenuUI
{
	[CreateAssetMenu(fileName = "SocialsConfig", menuName = "Scriptable/SocialsConfig", order = 20)]
	public class SocialsConfig : ScriptableObject
	{
		[Serializable]
		public class SocialData
		{
			public Texture2D buttonTexture;

			public string url;

			public string prefsKey;
		}

		[Header("Config for Socials on main menu")]
		public float timeToShowButtonAgain = 5f;

		public ObscuredInt reward = 5000;

		public List<SocialData> socials = new List<SocialData>();
	}
}
