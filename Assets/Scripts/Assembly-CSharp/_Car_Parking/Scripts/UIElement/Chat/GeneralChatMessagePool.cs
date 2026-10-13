using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class GeneralChatMessagePool
	{
		private readonly Stack<TextMessageView> _textPool = new Stack<TextMessageView>();

		private readonly Stack<SmileMessageView> _smilePool = new Stack<SmileMessageView>();

		private readonly IPrefabFactory _prefabFactory;

		private readonly TextMessageView _textPrefab;

		private readonly SmileMessageView _smilePrefab;

		public GeneralChatMessagePool(IPrefabFactory prefabFactory, TextMessageView textPrefab, SmileMessageView smilePrefab)
		{
			_prefabFactory = prefabFactory;
			_textPrefab = textPrefab;
			_smilePrefab = smilePrefab;
		}

		public TextMessageView GetTextMessage(Transform parent)
		{
			if (_textPool.Count > 0)
			{
				TextMessageView textMessageView = _textPool.Pop();
				textMessageView.transform.SetParent(parent, worldPositionStays: false);
				textMessageView.gameObject.SetActive(value: true);
				return textMessageView;
			}
			return _prefabFactory.Instantiate(_textPrefab, parent);
		}

		public SmileMessageView GetSmileMessage(Transform parent)
		{
			if (_smilePool.Count > 0)
			{
				SmileMessageView smileMessageView = _smilePool.Pop();
				smileMessageView.transform.SetParent(parent, worldPositionStays: false);
				smileMessageView.gameObject.SetActive(value: true);
				return smileMessageView;
			}
			return _prefabFactory.Instantiate(_smilePrefab, parent);
		}

		public void Return(PlayerNameColoredMessageView view)
		{
			view.gameObject.SetActive(value: false);
			if (!(view is TextMessageView item))
			{
				if (view is SmileMessageView item2)
				{
					_smilePool.Push(item2);
				}
				else
				{
					Object.Destroy(view.gameObject);
				}
			}
			else
			{
				_textPool.Push(item);
			}
		}
	}
}
