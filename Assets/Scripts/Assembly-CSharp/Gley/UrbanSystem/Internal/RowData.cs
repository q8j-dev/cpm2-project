using System;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public class RowData
	{
		[SerializeField]
		private CellData[] _row;

		public CellData[] Row
		{
			get
			{
				return _row;
			}
			set
			{
				_row = value;
			}
		}

		public RowData(CellData[] row)
		{
			_row = row;
		}
	}
}
