using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public class EmoteLockerGrid : MonoBehaviour
	{
		[SerializeField]
		private Transform Content;

		[SerializeField]
		private EmoteLockerItem ItemPrefab;

		private readonly List<EmoteLockerItem> _cards = new List<EmoteLockerItem>();

		private int _used;

		public GameObject ContentRoot
		{
			get
			{
				if (!Content)
				{
					return null;
				}
				return Content.gameObject;
			}
		}

		public RectTransform ContentRect => Content as RectTransform;

		public event Action<EmoteLockerItem> CardTapped;

		public void BeginBuild()
		{
			_used = 0;
		}

		public EmoteLockerItem AddCard(int id, Sprite sprite)
		{
			if (!ItemPrefab || !Content)
			{
				return null;
			}
			EmoteLockerItem emoteLockerItem;
			if (_used < _cards.Count)
			{
				emoteLockerItem = _cards[_used];
			}
			else
			{
				emoteLockerItem = UnityEngine.Object.Instantiate(ItemPrefab, Content);
				emoteLockerItem.Tapped += OnTapped;
				_cards.Add(emoteLockerItem);
			}
			_used++;
			emoteLockerItem.gameObject.SetActive(value: true);
			emoteLockerItem.transform.SetAsLastSibling();
			emoteLockerItem.Init(id, sprite);
			return emoteLockerItem;
		}

		public void EndBuild()
		{
			for (int i = _used; i < _cards.Count; i++)
			{
				_cards[i].gameObject.SetActive(value: false);
			}
		}

		public EmoteLockerItem FindCard(int id)
		{
			for (int i = 0; i < _used; i++)
			{
				if (_cards[i].Id == id)
				{
					return _cards[i];
				}
			}
			return null;
		}

		private void OnTapped(EmoteLockerItem item)
		{
			CardTapped?.Invoke(item);
		}
	}
}
