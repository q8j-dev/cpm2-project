using System;
using CPM.Fallout;
using _Car_Parking.Scripts.GameActivity;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaRaceFinishReporter
	{
		private readonly FalloutRaceSession _session;

		private readonly ArenaModeType _modeType;

		private long _timeSpent = -1L;

		private bool _reported;

		public ArenaRaceFinishReporter(FalloutRaceSession session, ArenaModeType modeType)
		{
			_session = session;
			_modeType = modeType;
		}

		public void CaptureTime(double elapsed)
		{
			if (_timeSpent < 0)
			{
				_timeSpent = ((elapsed > 0.0) ? ((long)elapsed) : 0);
			}
		}

		public void TryReport(IArenaContext ctx, Func<int> resolvePlace)
		{
			if (_reported || _timeSpent < 0 || string.IsNullOrEmpty(ctx.RaceId))
			{
				return;
			}
			int num = resolvePlace();
			if (num > 0 && ArenaActivityMap.TryGetActivity(_modeType, out var activityType))
			{
				_reported = true;
				float delaySeconds = 3f;
				switch (num)
				{
				case 1:
					delaySeconds = 0f;
					break;
				case 2:
					delaySeconds = 1f;
					break;
				case 3:
					delaySeconds = 2f;
					break;
				}
				_session.ReportFinish(activityType, ctx.RaceId, _timeSpent * 1000, num, delaySeconds);
			}
		}

		public void Reset()
		{
			_timeSpent = -1L;
			_reported = false;
		}
	}
}
