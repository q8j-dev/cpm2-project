using System.Collections.Generic;
using UnityEngine;

namespace Service.Cargo
{
	[CreateAssetMenu(fileName = "DeliveryItems", menuName = "Scriptable/Trailers", order = 1)]
	public class TrailerScriptable : ScriptableObject
	{
		public List<string> trailers;
	}
}
