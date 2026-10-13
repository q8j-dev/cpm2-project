using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class TabIconsController : MonoBehaviour
	{
		[SerializeField]
		private GameObjectStateUpdaterContainer TabIconPrefab;

		private List<GameObjectStateUpdaterContainer> _tabIcons = new List<GameObjectStateUpdaterContainer>();

		public int Count => _tabIcons.Count;

		public void Init(int amount)
		{
			base.transform.RemoveAllChild();
			_tabIcons.Clear();
			for (int i = 0; i < amount; i++)
			{
				_tabIcons.Add(Object.Instantiate(TabIconPrefab, base.transform));
			}
		}

		public void AddTabIcon()
		{
			_tabIcons.Add(Object.Instantiate(TabIconPrefab, base.transform));
		}

		public void SetActive(int index)
		{
			for (int i = 0; i < _tabIcons.Count; i++)
			{
				if ((bool)_tabIcons[i])
				{
					_tabIcons[i].UpdateState(i == index);
				}
			}
		}
	}
}
