using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class CanvasNavigator
	{
		private readonly Dictionary<string, NavCanvas> _canvases = new Dictionary<string, NavCanvas>();

		private readonly Stack<NavCanvas> _history = new Stack<NavCanvas>();

		private NavCanvas _current;

		public bool CanBack => _history.Count > 0;

		public string CurrentTitle => _current.Title;

		public event Action<bool> BackAvailabilityChanged;

		public event Action<string> TitleChanged;

		public CanvasNavigator(IEnumerable<NavCanvas> canvases, NavCanvas initial)
		{
			foreach (NavCanvas canvase in canvases)
			{
				canvase.Bind(this);
				_canvases[canvase.Id] = canvase;
				canvase.Hide();
			}
			_current = initial;
			_current.Show();
			RaiseCurrentChanged();
		}

		public void OpenWindow(string id, object data = null)
		{
			if (!_canvases.TryGetValue(id, out var value))
			{
				Debug.LogError("[CanvasNavigator] Unknown window id: " + id);
				return;
			}
			value.Open(data);
			SwitchTo(value);
		}

		public void Back()
		{
			if (_history.Count != 0)
			{
				_current.Hide();
				_current = _history.Pop();
				_current.Show();
				RaiseCurrentChanged();
			}
		}

		private void SwitchTo(NavCanvas next)
		{
			if (!(next == _current))
			{
				_current.Hide();
				_history.Push(_current);
				_current = next;
				_current.Show();
				RaiseCurrentChanged();
			}
		}

		private void RaiseCurrentChanged()
		{
			BackAvailabilityChanged?.Invoke(CanBack);
			TitleChanged?.Invoke(CurrentTitle);
		}
	}
}
