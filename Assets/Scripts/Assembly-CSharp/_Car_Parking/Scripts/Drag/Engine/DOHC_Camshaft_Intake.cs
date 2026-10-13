using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "DOHC_Camshaft_Intake", menuName = "CarPartsTuning/EngineParts/Camshafts/DOHC_Camshaft Intake", order = 21)]
	public class DOHC_Camshaft_Intake : CamshaftPart
	{
		public DOHC_Camshaft_Intake()
		{
			TypeId = PartType.DOHC_Camshaft_Intake;
		}
	}
}
