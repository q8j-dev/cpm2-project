using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Steam.Perf
{
	public static class SteamPerfFlags
	{
		public const string ReflectionCube = "reflCube";

		public const string Minimap = "minimap";

		public const string MaxDelta = "maxDelta";

		public const string MirrorShadows = "mirrorShadows";

		public const string LodBias = "lodBias";

		public const string NoCrossFade = "noCrossFade";

		public const string MirrorClip = "mirrorClip";

		public const string MirrorLod = "mirrorLod";

		public const string MirrorRate = "mirrorRate";

		public const string MirrorInterior = "mirrorInterior";

		public const string SceneSlicing = "sceneSlicing";

		public const string CleverAsync = "cleverAsync";

		public const string MirrorExterior = "mirrorExterior";

		public const string PedAvoidance = "pedAvoidance";

		public const string PedAlwaysWalk = "pedAlwaysWalk";

		public const string PedDodge = "pedDodge";

		public const string Discord = "discord";

		public const string CloudShadow = "cloudShadow";

		public const string PedAnim = "pedAnim";

		public const string TrafficShadow = "trafficShadow";

		public const string TrafficCull = "trafficCull";

		public const string TrafficPhysics = "trafficPhysics";

		public const string TrafficLean = "trafficLean";

		public static readonly string[] Experiments = new string[23]
		{
			"reflCube", "minimap", "maxDelta", "mirrorShadows", "lodBias", "noCrossFade", "mirrorClip", "mirrorLod", "mirrorRate", "mirrorInterior",
			"sceneSlicing", "cleverAsync", "mirrorExterior", "pedAvoidance", "pedAlwaysWalk", "pedDodge", "discord", "cloudShadow", "pedAnim", "trafficShadow",
			"trafficCull", "trafficPhysics", "trafficLean"
		};

		private static readonly HashSet<string> DefaultOn = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"reflCube", "lodBias", "minimap", "maxDelta", "mirrorShadows", "mirrorClip", "mirrorLod", "mirrorRate", "mirrorInterior", "sceneSlicing",
			"cleverAsync", "mirrorExterior", "pedAvoidance", "pedAlwaysWalk", "pedDodge", "discord", "cloudShadow", "pedAnim", "trafficPhysics", "trafficLean"
		};

		private const string OffPrefix = "-perf-off:";

		private const string OnPrefix = "-perf-on:";

		public static event Action Changed;

		public static bool IsOn(string name)
		{
			return DefaultOn.Contains(name);
		}
	}
}
