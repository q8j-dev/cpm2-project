using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.MenuUI.LanguagesServers;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class OpenWorldStartEvent : DefaultEvent<OpenWorldStartContext>
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
				"car_id",
				AnalyticsHelper.GetCurrentCarId()
			},
			{
				"map_id",
				V.PlaceToSpawn.ToString()
			},
			{
				"game_mode",
				base.Context.GameMode.ToString()
			},
			{
				"region",
				(base.Context.GameMode == OpenWorldType.Multiplayer) ? ((object)(LanguageIconsProvider.CpmLanguages)PlayerPrefs.GetInt("languageIndex", (int)Application.systemLanguage)) : "-1"
			},
			{
				"region_online",
				(base.Context.GameMode == OpenWorldType.Multiplayer) ? ((object)PhotonNetwork.CountOfPlayers) : "-1"
			},
			{
				"region_ping",
				(base.Context.GameMode == OpenWorldType.Multiplayer) ? PhotonNetwork.GetPing() : (-1)
			},
			{ "win_rate", WinRate },
			{ "race_number", RaceCount },
			{ "winner_count", RaceWin }
		};

		protected override string Name => "openworld_start";

		public OpenWorldStartEvent(CPMAnalytics analytics, OpenWorldStartContext context)
			: base(analytics, context)
		{
		}
	}
}
