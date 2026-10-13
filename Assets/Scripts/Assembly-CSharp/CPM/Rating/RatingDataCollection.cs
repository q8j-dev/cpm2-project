using System.Collections.Generic;
using Rating;
using UnityEngine;

namespace CPM.Rating
{
	[CreateAssetMenu(menuName = "Scriptable/Data/RatingDataCollection", fileName = "RatingDataCollection")]
	public class RatingDataCollection : ScriptableObject
	{
		[SerializeField]
		private List<RatingScriptableObject> data;

		public Dictionary<string, RatingScriptableObject> achievs;

		public void Initialize()
		{
			achievs = new Dictionary<string, RatingScriptableObject>();
			foreach (RatingScriptableObject datum in data)
			{
				achievs.Add(datum.name, datum);
			}
		}
	}
}
