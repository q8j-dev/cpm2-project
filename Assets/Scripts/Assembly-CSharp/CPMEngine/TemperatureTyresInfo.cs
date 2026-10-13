using System.Collections.Generic;
using UnityEngine;

namespace CPMEngine
{
	[CreateAssetMenu(fileName = "TemperatureTyresInfo", menuName = "CPMInstruments/TemperatureTyresInfo")]
	public class TemperatureTyresInfo : ScriptableObject
	{
		public List<TCurveTyre> TCurveTyres = new List<TCurveTyre>();

		public TCurveTyre GetTyreInfo(TyreType type)
		{
			return TCurveTyres.Find((TCurveTyre x) => x.Type == type);
		}

		public float GetTreadThickness(TyreType type)
		{
			return GetTyreInfo(type)?.TreadThickness ?? 0.01f;
		}
	}
}
