using System;
using UnityEngine;

namespace _Car_Parking.Scripts.DiscordPresence
{
	[CreateAssetMenu(fileName = "DiscordPresenceConfig", menuName = "Scriptable/DiscordPresenceConfig")]
	public class DiscordPresenceConfigSO : ScriptableObject
	{
		[Serializable]
		public struct ActivityEntry
		{
			public DiscordActivityKind Kind;

			[Tooltip("Art Asset key from the Discord developer portal. Portal keys are always lower-case.")]
			public string LargeImageKey;

			[Tooltip("Hover text on the large image.")]
			public string LargeImageTooltip;

			[Tooltip("Top line when no location is known. Example: Circuit Racing")]
			public string Details;

			[Tooltip("Top line when a location is known. {0} is the track / level / city. Example: Circuit Racing - {0}")]
			public string DetailsWithLocation;

			[Tooltip("Bottom line when no lap info is available. Leave empty to hide the line.")]
			public string State;

			[Tooltip("Bottom line during a lap race. {0} is the current lap, {1} the total. Example: Lap {0}/{1}")]
			public string StateWithLaps;
		}

		[Header("Discord Application")]
		[SerializeField]
		[Tooltip("Numeric Application ID from the Discord developer portal (General Information). Public, not a secret.")]
		private string _applicationId;

		[Header("Behaviour")]
		[SerializeField]
		[Tooltip("Master switch. Turn off to keep the game off Discord profiles entirely.")]
		private bool _enabled = true;

		[SerializeField]
		[Tooltip("Seconds between presence updates. Discord silently drops anything faster than 15.")]
		private float _minUpdateIntervalSeconds = 15f;

		[Header("Shared Artwork")]
		[SerializeField]
		[Tooltip("Small corner image, shown for every activity.")]
		private string _smallImageKey = "logo_main";

		[SerializeField]
		private string _smallImageTooltip = "Car Parking Multiplayer 2";

		[SerializeField]
		[Header("Activities")]
		private ActivityEntry[] _entries = Array.Empty<ActivityEntry>();

		public string ApplicationId => _applicationId;

		public bool Enabled => _enabled;

		public string SmallImageKey => _smallImageKey;

		public string SmallImageTooltip => _smallImageTooltip;

		public float MinUpdateIntervalSeconds => Mathf.Max(15f, _minUpdateIntervalSeconds);

		public ActivityEntry GetEntry(DiscordActivityKind kind)
		{
			if (_entries == null)
			{
				return default;
			}
			for (int i = 0; i < _entries.Length; i++)
			{
				if (_entries[i].Kind == kind)
				{
					return _entries[i];
				}
			}
			return default;
		}
	}
}
