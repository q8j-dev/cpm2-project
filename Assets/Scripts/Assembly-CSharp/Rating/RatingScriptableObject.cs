using System.Collections.Generic;
using UnityEngine;

namespace Rating
{
	[CreateAssetMenu(fileName = "Data", menuName = "Scriptable/Rating", order = 1)]
	public class RatingScriptableObject : ScriptableObject
	{
		public Texture icon;

		[SerializeField]
		private string _name;

		[SerializeField]
		private string mainDescription;

		public List<Data> data;

		public string Name
		{
			get
			{
				return _name;
			}
			private set
			{
				_name = value;
			}
		}

		public (string name, string description, string score, Texture icon, string rang, string mainDescription) Get(int i)
		{
			return (name: _name, description: data[i].description, score: data[i].score, icon: icon, rang: i.ToString() ?? "", mainDescription: mainDescription);
		}
	}
}
