using System;
using UnityEngine;

namespace TouchControlsKit
{
	[Serializable]
	public sealed class Axes
	{
		public bool enabled = true;

		public bool inverse;

		[SerializeField]
		private string name = string.Empty;

		internal float value { get; private set; }

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (!(name == value))
				{
					if (value == string.Empty)
					{
						Debug.LogError("ERROR: name for this axis cannot be empty!");
					}
					else
					{
						name = value;
					}
				}
			}
		}

		public Axes(string m_Name)
		{
			name = ((name == string.Empty) ? m_Name : name);
		}

		internal void SetValue(float m_value)
		{
			value = m_value;
		}
	}
}
