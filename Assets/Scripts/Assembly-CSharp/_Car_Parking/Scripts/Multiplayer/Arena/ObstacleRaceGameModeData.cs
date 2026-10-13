using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public class ObstacleRaceGameModeData : MonoBehaviour
	{
		[Tooltip("Start trigger . need to change music")]
		public Collider StartTrigger;

		[Tooltip("Collapse-phase tile trigger. Unused now the mode is a plain race; kept for a possible rollback.")]
		public FallingZone FallingZone;

		[Tooltip("Finish trigger. First touch ends that player's run and claims them a place.")]
		public Collider FinishTrigger;

		[Tooltip("Collapse group the builder moves onto the last zone's exit. Author CollapseObjects and the FallingZone under it so both still ride to the course exit, even though nothing drops them anymore. Left empty only when the trigger has no dressing.")]
		public Transform FinishRoot;

		[Tooltip("Objects the collapse phase used to disable to drop players into the FallingZone. Unused now the mode is a plain race; kept for a possible rollback.")]
		public GameObject[] CollapseObjects;

		[Tooltip("Safe teleport point (with floor under it) for finishers. Fixed, outside the course.")]
		public PodiumOnline PodiumOnline;

		[Tooltip("Scene-authored start grid. Not part of the assembled track.")]
		public SpawnLayoutProvider SpawnLayoutProvider;

		[Tooltip("Zones the track is assembled from.")]
		public ObstacleRaceZonePool ZonePool;

		[Tooltip("Pose the zone chain grows from. Must live under a global parent (floating origin).")]
		public Transform TrackOrigin;

		private ArenaModeRegistry _registry;

		[Inject]
		public void Construct(ArenaModeRegistry arenaModeRegistry)
		{
			_registry = arenaModeRegistry;
		}

		private void Start()
		{
			if (_registry.TryGet(ArenaModeType.ObstacleRace, out var controller))
			{
				((ObstacleRaceArenaMode)controller).SetData(this);
			}
		}
	}
}
