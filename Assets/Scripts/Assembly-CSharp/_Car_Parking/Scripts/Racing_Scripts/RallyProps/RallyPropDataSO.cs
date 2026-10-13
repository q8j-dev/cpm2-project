using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[CreateAssetMenu(menuName = "Rally/Rally Prop Data")]
	public class RallyPropDataSO : ScriptableObject
	{
		public List<RallyPropData> Props = new List<RallyPropData>();
	}
}
