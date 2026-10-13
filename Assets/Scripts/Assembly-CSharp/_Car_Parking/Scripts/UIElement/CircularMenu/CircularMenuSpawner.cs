using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.UIElement.CircularMenu
{
	public class CircularMenuSpawner : MonoBehaviour
	{
		[SerializeField]
		[Header("Circular Menus")]
		private List<GameObject> _circularMenus = new List<GameObject>();

		private IPrefabFactory _prefabFactory;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
		}

		private void Start()
		{
			for (int i = 0; i < _circularMenus.Count; i++)
			{
				GameObject gameObject = _circularMenus[i];
				if ((bool)gameObject)
				{
					_prefabFactory.Instantiate(gameObject, base.transform).transform.SetAsFirstSibling();
				}
			}
		}
	}
}
