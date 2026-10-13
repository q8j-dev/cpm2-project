using System.Threading;
using CPM.Checkpoints;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ObstacleRaceCourseAssembler
	{
		public static async UniTask<(ObstacleRaceTrack track, CheckpointCourse course)> AssembleAsync(IArenaContext ctx, ObstacleRaceGameModeData data, IPrefabFactory prefabFactory, CancellationToken ct)
		{
			int? num = await ObstacleRaceSeed.ResolveAsync(ctx, ct);
			if (!num.HasValue)
			{
				return default;
			}
			ObstacleRaceConfig obstacleRaceConfig = ctx.Config as ObstacleRaceConfig;
			ObstacleRaceZoneInfo[] pool = (data.ZonePool ? data.ZonePool.BuildZoneInfos() : null);
			int[] indices = ObstacleRaceTrackPlan.Pick(num.Value, pool, obstacleRaceConfig.GetBlocks());
			Transform finishRoot = (data.FinishRoot ? data.FinishRoot : data.FinishTrigger.transform);
			ObstacleRaceTrack obstacleRaceTrack = ObstacleRaceTrackBuilder.Build(data.ZonePool, indices, data.TrackOrigin, finishRoot, prefabFactory, num.Value);
			if ((bool)data.FallingZone)
			{
				data.FallingZone.Init(obstacleRaceConfig.FallingZone);
			}
			CheckpointCourse item = new CheckpointCourse(obstacleRaceTrack?.Checkpoints, (GameObject go) => go == V.DriverPerson);
			return (obstacleRaceTrack, item);
		}
	}
}
