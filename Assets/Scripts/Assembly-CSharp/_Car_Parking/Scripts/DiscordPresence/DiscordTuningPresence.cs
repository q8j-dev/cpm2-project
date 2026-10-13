using System;
using System.Collections.Generic;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.DiscordPresence
{
	public sealed class DiscordTuningPresence : IStartable, IDisposable
	{
		private static readonly HashSet<string> TuningWindowTypeNames = new HashSet<string> { "CarTuningWindow", "CarTuningPartsUIWindow", "SuspensionTuningWindow", "VisualTuningWindow", "EngineUIWindow" };

		private readonly IObjectResolver _resolver;

		private readonly IDiscordPresenceService _presence;

		private readonly DiscordActivityKind _idleKind;

		private ManagerUI _managerUI;

		private string _openTuningWindow;

		public DiscordTuningPresence(IObjectResolver resolver, IDiscordPresenceService presence, DiscordActivityKind idleKind)
		{
			_resolver = resolver;
			_presence = presence;
			_idleKind = idleKind;
		}

		public void Start()
		{
			if (_resolver.TryResolve<ManagerUI>(out _managerUI) && !(_managerUI == null))
			{
				_managerUI.OnCurrentWindowUpdated += OnWindowOpened;
				_managerUI.OnWindowClosed += OnWindowClosed;
			}
		}

		public void Dispose()
		{
			if (!(_managerUI == null))
			{
				_managerUI.OnCurrentWindowUpdated -= OnWindowOpened;
				_managerUI.OnWindowClosed -= OnWindowClosed;
				_managerUI = null;
			}
		}

		private void OnWindowOpened(WindowScript window)
		{
			if (!(window == null) && TuningWindowTypeNames.Contains(window.GetType().Name))
			{
				_openTuningWindow = Normalize(window.GUID);
				_presence.SetActivity(DiscordActivityKind.CarTuning);
			}
		}

		private void OnWindowClosed(string windowName)
		{
			if (_openTuningWindow != null && !(Normalize(windowName) != _openTuningWindow))
			{
				_openTuningWindow = null;
				_presence.SetActivity(_idleKind);
			}
		}

		private static string Normalize(string windowName)
		{
			if (!string.IsNullOrEmpty(windowName))
			{
				return windowName.Replace("(Clone)", string.Empty);
			}
			return string.Empty;
		}
	}
}
