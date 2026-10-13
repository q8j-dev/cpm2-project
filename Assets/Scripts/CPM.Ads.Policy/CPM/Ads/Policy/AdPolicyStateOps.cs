namespace CPM.Ads.Policy
{
	public static class AdPolicyStateOps
	{
		public const int MaxContextsPerPlacement = 64;

		public static PlacementEntry GetOrAddPlacement(AdPolicyState state, AdPlacement placement)
		{
			for (int i = 0; i < state.Placements.Count; i++)
			{
				if (state.Placements[i].Placement == placement)
				{
					return state.Placements[i];
				}
			}
			PlacementEntry placementEntry = new PlacementEntry
			{
				Placement = placement
			};
			state.Placements.Add(placementEntry);
			return placementEntry;
		}

		public static ContextEntry GetOrAddContext(PlacementEntry entry, string contextId)
		{
			for (int i = 0; i < entry.Contexts.Count; i++)
			{
				if (entry.Contexts[i].Id == contextId)
				{
					return entry.Contexts[i];
				}
			}
			ContextEntry contextEntry = new ContextEntry
			{
				Id = contextId
			};
			entry.Contexts.Add(contextEntry);
			while (entry.Contexts.Count > 64)
			{
				entry.Contexts.RemoveAt(0);
			}
			return contextEntry;
		}
	}
}
