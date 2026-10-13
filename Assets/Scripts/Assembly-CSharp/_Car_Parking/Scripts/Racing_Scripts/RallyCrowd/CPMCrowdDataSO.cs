using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[CreateAssetMenu(menuName = "Rally/Rally Crowd Data")]
	public class CPMCrowdDataSO : ScriptableObject
	{
		public List<CPMCrowdNpcData> Npcs = new List<CPMCrowdNpcData>();
	}
}
