using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class CharacterCreationEvent : DefaultEvent<CharacterCreationContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"status",
				base.Context.Status
			},
			{
				"preset_id",
				base.Context.PresetId
			}
		};

		protected override string Name => "character_creation";

		public CharacterCreationEvent(CPMAnalytics analytics, CharacterCreationContext context)
			: base(analytics, context)
		{
		}
	}
}
