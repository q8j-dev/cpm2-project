using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using Photon.Pun;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class OpenWorldFinishEvent : DefaultEvent<OpenWorldFinishContext>
	{
		private int RaceWin => ObscuredPrefs.GetInt("RaceWin");

		private int RaceLose => ObscuredPrefs.GetInt("RaceLose");

		private int RaceCount => RaceWin + RaceLose;

		private float WinRate
		{
			get
			{
				float result = 0f;
				if (RaceCount != 0)
				{
					result = (float)RaceWin / (float)RaceCount;
				}
				return result;
			}
		}

		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"region_ping",
				(PhotonNetwork.GetPing() != 0) ? PhotonNetwork.GetPing() : (-1)
			},
			{
				"car_id",
				AnalyticsHelper.GetCurrentCarId()
			},
			{
				"map_id",
				V.CurrentEnvironment.ToString()
			},
			{ "win_rate", WinRate },
			{ "race_number", RaceCount },
			{ "winner_count", RaceWin },
			{
				"mode_duration",
				base.Context.ModeDuration
			},
			{
				"taxi_orders_completed",
				base.Context.TaxiOrdersCompleted
			},
			{
				"taxi_orders_earned",
				base.Context.TaxiOrdersEarned
			},
			{
				"eggs",
				CountingEggs()
			},
			{
				"checkpoint_earned",
				base.Context.CheckpointEarned
			},
			{
				"exit_reason",
				base.Context.ExitReason
			}
		};

		protected override string Name => "openworld_finish";

		public OpenWorldFinishEvent(CPMAnalytics analytics, OpenWorldFinishContext context)
			: base(analytics, context)
		{
		}

		private int CountingEggs()
		{
			int num = 0;
			for (int i = 0; i < 100; i++)
			{
				if (ObscuredPrefs.GetInt("egg" + i) == 1)
				{
					num++;
				}
			}
			return num;
		}
	}
}
