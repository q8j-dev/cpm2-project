using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct ChordCapture
	{
		public readonly IReadOnlyList<string> Keys;

		public int Count => Keys.Count;

		public string Key
		{
			get
			{
				if (Count <= 0)
				{
					return null;
				}
				return Keys[Count - 1];
			}
		}

		public string Modifier
		{
			get
			{
				if (Count != 2)
				{
					return null;
				}
				return Keys[0];
			}
		}

		public ChordCapture(IReadOnlyList<string> keys)
		{
			Keys = keys ?? Array.Empty<string>();
		}
	}
}
