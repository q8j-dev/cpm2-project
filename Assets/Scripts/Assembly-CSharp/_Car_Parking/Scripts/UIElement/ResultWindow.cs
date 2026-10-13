using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public abstract class ResultWindow<TRow, TElement, TAction> : WindowScriptAsyncResult<TAction> where TRow : IResultRow where TElement : ResultRowElement<TRow>
	{
		[SerializeField]
		private Transform Content = default;

		[SerializeField]
		private TElement LeaderboardElementPrefab = default;

		[SerializeField]
		private TElement SelfLeaderboardElement = default;

		[SerializeField]
		[Tooltip("Give the local player a row of their own once they finish worse than this place. 0 keeps them inline in the table wherever they finished.")]
		private int PinSelfAfterPlace = default;

		private readonly List<TElement> _elements = new List<TElement>();

		public void SetRows(IReadOnlyList<TRow> rows)
		{
			int num = rows?.Count ?? 0;
			int num2 = ResultRowLayout.FindLocalIndex(rows);
			bool flag = ResultRowLayout.ShouldPinLocalRow(num, PinSelfAfterPlace, num2);
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!flag || i != num2)
				{
					TElement element = GetElement(num3);
					if (!element)
					{
						break;
					}
					element.Bind(i + 1, rows[i]);
					num3++;
				}
			}
			for (int j = num3; j < _elements.Count; j++)
			{
				if ((bool)_elements[j])
				{
					_elements[j].Show(visible: false);
				}
			}
			if ((bool)SelfLeaderboardElement)
			{
				SelfLeaderboardElement.Show(flag);
				if (flag)
				{
					SelfLeaderboardElement.Bind(num2 + 1, rows[num2]);
				}
			}
		}

		private TElement GetElement(int index)
		{
			if (index < _elements.Count)
			{
				TElement val = _elements[index];
				if ((bool)val)
				{
					val.Show(visible: true);
				}
				return val;
			}
			if (!LeaderboardElementPrefab || !Content)
			{
				return null;
			}
			TElement val2 = Object.Instantiate(LeaderboardElementPrefab, Content);
			_elements.Add(val2);
			return val2;
		}
	}
}
