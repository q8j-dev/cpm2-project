using System;
using UnityEngine;

namespace Rating
{
	[Serializable]
	public class Data
	{
		[SerializeField]
		public string _description;

		[SerializeField]
		public string _score;

		public string score
		{
			get
			{
				return _score;
			}
			private set
			{
				_score = score;
			}
		}

		public string description
		{
			get
			{
				return _description;
			}
			private set
			{
				_description = value;
			}
		}
	}
}
