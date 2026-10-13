using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _Car_Parking.Scripts.Drag
{
	public class UiNotificationsManager : IDisposable
	{
		private readonly List<PopupData> _popupWindows = new List<PopupData>();

		private IPopupWindow _currentWindow;

		private ManagerUI _managerUI;

		private bool _isDestroyed;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		public void ShowPopup(PopupData popupData)
		{
			AddPopupWindow(popupData);
		}

		public void StopPopupQueue()
		{
			_popupWindows.Clear();
			_currentWindow?.Close();
			_currentWindow = null;
		}

		private void AddPopupWindow(PopupData popupData)
		{
			if (!string.IsNullOrEmpty(popupData.PopupKey))
			{
				_popupWindows.Add(popupData);
				if (_popupWindows.Count == 1)
				{
					UpdatePlayList().Forget();
				}
			}
		}

		public void Remove(PopupData popupData)
		{
			_popupWindows.Remove(popupData);
			_currentWindow?.Close();
			_currentWindow = null;
			UpdatePlayList().Forget();
		}

		public async UniTaskVoid UpdatePlayList()
		{
			if (_popupWindows.Count < 1)
			{
				return;
			}
			PopupData popupData = _popupWindows.First();
			await UniTask.Delay(TimeSpan.FromSeconds(popupData.Delay));
			_currentWindow = await _managerUI.CreateWindowGeneric<IPopupWindow>(popupData.PopupKey, onTop: false);
			if (_isDestroyed)
			{
				return;
			}
			if (_currentWindow != null)
			{
				_currentWindow.Init(popupData);
				return;
			}
			_popupWindows.Remove(popupData);
			if (_popupWindows.Count > 0)
			{
				UpdatePlayList().Forget();
			}
		}

		public void Dispose()
		{
			_popupWindows.Clear();
			_isDestroyed = true;
			_currentWindow = null;
		}
	}
}
