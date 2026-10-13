using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem
{
	public sealed class HudMenuAutoHide : IDisposable
	{
		private static readonly HashSet<string> NonBlockingWindows = new HashSet<string> { "ChatWindow", "WaitingWindow", "WaitingWindowNotTransparent", "UserBalanceWindow", "UserBalanceOnlyWindow" };

		private readonly HashSet<string> _openMenus = new HashSet<string>();

		private readonly WindowScript _owner;

		private readonly ManagerUI _managerUI;

		private readonly CanvasGroup _group;

		public HudMenuAutoHide(WindowScript owner, ManagerUI managerUI)
		{
			_owner = owner;
			_managerUI = managerUI;
			_group = owner.gameObject.EnsureComponent<CanvasGroup>();
			if ((bool)_managerUI)
			{
				_managerUI.OnCurrentWindowUpdated += HandleWindowOpened;
				_managerUI.OnWindowClosed += HandleWindowClosed;
			}
		}

		public void Dispose()
		{
			if ((bool)_managerUI)
			{
				_managerUI.OnCurrentWindowUpdated -= HandleWindowOpened;
				_managerUI.OnWindowClosed -= HandleWindowClosed;
			}
		}

		private void HandleWindowOpened(WindowScript window)
		{
			if ((bool)window && !(window == _owner) && !string.IsNullOrEmpty(window.GUID) && !NonBlockingWindows.Contains(window.GUID) && _openMenus.Add(window.GUID))
			{
				Refresh();
			}
		}

		private void HandleWindowClosed(string guid)
		{
			if (_openMenus.Remove(guid))
			{
				Refresh();
			}
		}

		private void Refresh()
		{
			bool flag = _openMenus.Count == 0;
			_group.alpha = (flag ? 1f : 0f);
			_group.blocksRaycasts = flag;
		}
	}
}
