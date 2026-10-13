using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class ChartListView : MonoBehaviour
	{
		[SerializeField]
		private ChartListElementView[] Rows = default;

		[Header("Header (optional — assign only what the prefab has)")]
		[SerializeField]
		private TextMeshProUGUI PlaceText = default;

		[SerializeField]
		private TextMeshProUGUI AllPlaceCount = default;

		[SerializeField]
		private TextMeshProUGUI LapText = default;

		[SerializeField]
		private TextMeshProUGUI AllLapCount = default;

		[SerializeField]
		private GameObject Panel = default;

		private readonly List<int> _visible = new List<int>();

		private void Start()
		{
			if ((bool)PlaceText)
			{
				PlaceText.ForceMeshUpdate();
			}
			if ((bool)AllPlaceCount)
			{
				AllPlaceCount.ForceMeshUpdate();
			}
			if ((bool)LapText)
			{
				LapText.ForceMeshUpdate();
			}
			if ((bool)AllLapCount)
			{
				AllLapCount.ForceMeshUpdate();
			}
		}

		public void SetRows(IReadOnlyList<ChartListRowData> standings)
		{
			SelectVisibleIndices(standings, Rows.Length, _visible);
			for (int i = 0; i < Rows.Length; i++)
			{
				bool flag = i < _visible.Count;
				Rows[i].Show(flag);
				if (flag)
				{
					Rows[i].Bind(standings[_visible[i]]);
				}
			}
		}

		public void SetProgress(string place, string placesCount)
		{
			if ((bool)PlaceText)
			{
				PlaceText.text = place;
			}
			if ((bool)AllPlaceCount)
			{
				AllPlaceCount.text = placesCount;
			}
		}

		public void SetLap(string lap, string lapsCount)
		{
			if ((bool)LapText)
			{
				LapText.text = lap;
			}
			if ((bool)AllLapCount)
			{
				AllLapCount.text = lapsCount;
			}
		}

		public void SetPanelActive(bool active)
		{
			if ((bool)Panel)
			{
				Panel.SetActive(active);
			}
		}

		private static void SelectVisibleIndices(IReadOnlyList<ChartListRowData> standings, int slots, List<int> result)
		{
			result.Clear();
			int num = standings?.Count ?? 0;
			if (num <= 0 || slots <= 0)
			{
				return;
			}
			if (num <= slots)
			{
				for (int i = 0; i < num; i++)
				{
					result.Add(i);
				}
				return;
			}
			int num2 = FindPlayerIndex(standings);
			if (num2 < 0 || num2 < slots)
			{
				for (int j = 0; j < slots; j++)
				{
					result.Add(j);
				}
				return;
			}
			result.Add(0);
			int num3 = slots - 1;
			int num4 = num2 - (num3 - 1) / 2;
			int num5 = num4 + num3 - 1;
			if (num5 > num - 1)
			{
				num5 = num - 1;
				num4 = num5 - (num3 - 1);
			}
			if (num4 < 1)
			{
				num4 = 1;
				num5 = num4 + num3 - 1;
			}
			for (int k = num4; k <= num5; k++)
			{
				result.Add(k);
			}
		}

		private static int FindPlayerIndex(IReadOnlyList<ChartListRowData> standings)
		{
			for (int i = 0; i < standings.Count; i++)
			{
				if (standings[i].IsUser)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
