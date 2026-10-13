using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[CreateAssetMenu(menuName = "Scriptable/Arena/ArenaPointsConfig", fileName = "ArenaPoints")]
	public class ArenaPointsConfig : ScriptableObject
	{
		public ArenaLocation Type;

		public List<PositionsAndRotation> Points = new List<PositionsAndRotation>();
	}
}
