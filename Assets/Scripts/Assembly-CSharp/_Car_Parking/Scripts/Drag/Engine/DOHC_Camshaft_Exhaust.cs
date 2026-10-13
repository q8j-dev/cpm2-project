using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "DOHC_Camshaft_Exhaust", menuName = "CarPartsTuning/EngineParts/Camshafts/DOHC_Camshaft_Exhaust", order = 21)]
	public class DOHC_Camshaft_Exhaust : CamshaftPart
	{
		public DOHC_Camshaft_Exhaust()
		{
			TypeId = PartType.DOHC_Camshaft_Exhaust;
		}
	}
}
