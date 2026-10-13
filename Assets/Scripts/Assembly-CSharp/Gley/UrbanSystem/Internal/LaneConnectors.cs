using System;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public struct LaneConnectors
	{
		public WaypointSettingsBase inConnector;

		public WaypointSettingsBase outConnector;

		public LaneConnectors(WaypointSettingsBase inConnector, WaypointSettingsBase outConnector)
		{
			this.inConnector = inConnector;
			this.outConnector = outConnector;
		}
	}
}
