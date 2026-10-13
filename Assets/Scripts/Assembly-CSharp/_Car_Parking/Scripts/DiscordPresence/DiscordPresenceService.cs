using System;
using System.Runtime.InteropServices;
using Discord.Sdk;
using UnityEngine;
using VContainer.Unity;
using _Car_Parking.Scripts.Steam.Perf;

namespace _Car_Parking.Scripts.DiscordPresence
{
	public sealed class DiscordPresenceService : IDiscordPresenceService, IStartable, ITickable, IDisposable
	{
		private static class NativeProbe
		{
			[DllImport("discord_partner_sdk", EntryPoint = "Discord_ResetCallbacks")]
			public static extern void ResetCallbacks();
		}

		private readonly DiscordPresenceConfigSO _config;

		private readonly Client.UpdateRichPresenceCallback _onPresenceUpdated;

		private readonly Client.LogCallback _onSdkLog;

		private Client _client;

		private DiscordActivityKind _kind;

		private DiscordActivityContext _context;

		private ulong _startTimeMs;

		private bool _dirty;

		private bool _lastUpdateFailed;

		private float _lastSendTime = float.NegativeInfinity;

		private bool IsRunning => _client != null;

		public DiscordPresenceService(DiscordPresenceConfigSO config)
		{
			_config = config;
			_onPresenceUpdated = OnPresenceUpdated;
			_onSdkLog = OnSdkLog;
		}

		public void Start()
		{
			if (SteamPerfFlags.IsOn("discord") && !(_config == null) && _config.Enabled && DiscordPresenceText.TryParseApplicationId(_config.ApplicationId, out var applicationId))
			{
				_client = TryCreateClient(applicationId);
				_startTimeMs = NowMs();
				_dirty = IsRunning;
			}
		}

		public void Tick()
		{
			if (IsRunning && _dirty && !(Time.unscaledTime - _lastSendTime < _config.MinUpdateIntervalSeconds))
			{
				Flush();
			}
		}

		public void SetActivity(DiscordActivityKind kind)
		{
			if (_kind != kind)
			{
				_kind = kind;
				_context = default;
				_startTimeMs = NowMs();
				_dirty = true;
			}
		}

		public void SetContext(in DiscordActivityContext context)
		{
			if (!_context.Equals(in context))
			{
				_context = context;
				_dirty = true;
			}
		}

		public void Dispose()
		{
			if (IsRunning)
			{
				_client.Dispose();
				_client = null;
			}
		}

		private Client TryCreateClient(ulong applicationId)
		{
			Client client = null;
			try
			{
				NativeProbe.ResetCallbacks();
				client = new Client();
				client.AddLogCallback(_onSdkLog, LoggingSeverity.Error);
				client.SetApplicationId(applicationId);
				return client;
			}
			catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException || ex is BadImageFormatException || ex is TypeInitializationException)
			{
				client?.Dispose();
				return null;
			}
		}

		private void Flush()
		{
			_dirty = false;
			_lastSendTime = Time.unscaledTime;
			DiscordPresenceConfigSO.ActivityEntry entry = _config.GetEntry(_kind);
			using Activity activity = new Activity();
			using ActivityAssets activityAssets = new ActivityAssets();
			using ActivityTimestamps activityTimestamps = new ActivityTimestamps();
			activity.SetType(ActivityTypes.Playing);
			activity.SetDetails(DiscordPresenceText.ToField(DiscordPresenceText.FormatDetails(entry.Details, entry.DetailsWithLocation, _context.LocationName)));
			activity.SetState(DiscordPresenceText.ToField(DiscordPresenceText.FormatState(entry.State, entry.StateWithLaps, _context.Lap, _context.TotalLaps)));
			activityAssets.SetLargeImage(DiscordPresenceText.ToField(entry.LargeImageKey));
			activityAssets.SetLargeText(DiscordPresenceText.ToField(entry.LargeImageTooltip));
			activityAssets.SetSmallImage(DiscordPresenceText.ToField(_config.SmallImageKey));
			activityAssets.SetSmallText(DiscordPresenceText.ToField(_config.SmallImageTooltip));
			activity.SetAssets(activityAssets);
			activityTimestamps.SetStart(_startTimeMs);
			activity.SetTimestamps(activityTimestamps);
			_client.UpdateRichPresence(activity, _onPresenceUpdated);
		}

		private void OnPresenceUpdated(ClientResult result)
		{
			if (result.Successful())
			{
				_lastUpdateFailed = false;
				return;
			}
			_ = _lastUpdateFailed;
			_lastUpdateFailed = true;
			_dirty = true;
		}

		private static void OnSdkLog(string message, LoggingSeverity severity)
		{
		}

		private static ulong NowMs()
		{
			return (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		void IDiscordPresenceService.SetContext(in DiscordActivityContext context)
		{
			SetContext(in context);
		}
	}
}
