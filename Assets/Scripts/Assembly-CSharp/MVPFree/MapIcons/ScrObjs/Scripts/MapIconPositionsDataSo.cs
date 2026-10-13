using System;
using MVPFree.MapIcons.Enum;
using UnityEngine;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[CreateAssetMenu(fileName = "MapIconPositionsDataSo", menuName = "Scriptable/MapIconPositionsDataSo")]
	public class MapIconPositionsDataSo : ScriptableObject
	{
		[Serializable]
		public class IconPositionData
		{
			public string SceneName;

			public int Id;

			public Vector3 WorldPosition;

			public IconType IconType;
		}

		public IconPositionData[] IconPositions;
	}
}
