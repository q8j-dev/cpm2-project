using Car_Parking.Scripts.Racing;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.UIElement.HUD;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	public class RacingProvider
	{
		public DragRacingController DragRacingController { get; set; }

		public RacingCameraController RacingCameraController { get; set; }

		public HudColorChanger HudColorChanger { get; set; }

		public RallyController RallyController { get; set; }

		public FreeTimeAttackController FreeTimeAttackController { get; set; }
	}
}
