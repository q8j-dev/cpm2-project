using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;

namespace _Car_Parking.Scripts.EngineTuning
{
	[CreateAssetMenu(fileName = "EngineKitOffer", menuName = "CarConfigs/EngineKitOffer", order = 2)]
	public class EngineKitOffer : ScriptableObject
	{
		[Tooltip("Shared EngineItem (block, head, internals). Compatibility checks key off its EngineId.")]
		[Header("Base engine")]
		public EngineItem Base;

		[Tooltip("Parts layered on top of Base. Empty = NA. [TurboCharger, Intercooler] = turbo variant of the same engine family.\n\nApplied ONCE at acquisition time — new car purchase (CarInfoService.CreateDefault) and engine swap in the market (DefaultEngineMarketWindow). After that the result lives in the car's per-instance EngineKitData save and is authoritative for that car. Editing Extras here does NOT retroactively patch cars players already own — by design. If a kit needs to reach existing saves, do an explicit save migration, not a CheckForNulls hook.")]
		[Header("Stock extras")]
		public PartItem[] Extras;

		[Tooltip("Shop name. Localization key. Auto-filled from Base.GetName by the EngineKitOffer generators; override in inspector for variants.")]
		[Header("Shop display")]
		public string Name;

		[TextArea]
		[Tooltip("Shop description. Localization key. Auto-filled from Base.Description by the EngineKitOffer generators; override in inspector for variants.")]
		public string Description;

		[Tooltip("Hide this offer from the engine market without affecting Base availability.")]
		[Header("Shop availability")]
		public bool IsNotForSale;

		[Tooltip("Pre-tuned ECU for this kit. Null = market falls back to a universal ECU at buy time.")]
		[Header("ECU")]
		public ECUPart ECU;

		[HideInInspector]
		public int GeneratedSchemaVersion;
	}
}
