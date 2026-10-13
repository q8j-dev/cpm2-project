using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ObstacleRaceSpectatorListView : MonoBehaviour
	{
		[SerializeField]
		private RectTransform Content = default;

		[SerializeField]
		private ObstacleRaceSpectatorRow RowPrefab = default;

		private readonly List<ObstacleRaceSpectatorRow> _rows = new List<ObstacleRaceSpectatorRow>();

		public RectTransform SelectedRow { get; private set; }

		public event Action<int> RowClicked;

		private void OnDestroy()
		{
			for (int i = 0; i < _rows.Count; i++)
			{
				if ((bool)_rows[i])
				{
					_rows[i].Clicked -= OnRowClicked;
				}
			}
		}

		public void SetRows(IReadOnlyList<SpectatorListRow> rows)
		{
			int num = rows?.Count ?? 0;
			Grow(num);
			SelectedRow = null;
			for (int i = 0; i < _rows.Count; i++)
			{
				bool flag = i < num;
				_rows[i].Show(flag);
				if (flag)
				{
					_rows[i].Bind(rows[i]);
					if (rows[i].Selected)
					{
						SelectedRow = (RectTransform)_rows[i].transform;
					}
				}
			}
		}

		private void Grow(int count)
		{
			if ((bool)RowPrefab && (bool)Content)
			{
				while (_rows.Count < count)
				{
					ObstacleRaceSpectatorRow obstacleRaceSpectatorRow = UnityEngine.Object.Instantiate(RowPrefab, Content);
					obstacleRaceSpectatorRow.Clicked += OnRowClicked;
					_rows.Add(obstacleRaceSpectatorRow);
				}
			}
		}

		private void OnRowClicked(int actor)
		{
			RowClicked?.Invoke(actor);
		}
	}
}
