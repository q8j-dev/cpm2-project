using System;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class UtilsRank
	{
		public enum RankType
		{
			Beginner = 0,
			Driver = 1,
			Racer = 2,
			Expert = 3,
			Professional = 4,
			Supreme = 5,
			King = 6,
			Developer = 7,
			YouTuber = 8,
			TikToker = 9,
			IGBlogger = 10
		}

		private const string Path = "icons/Rating/Rank_0";

		public static RankType GetRank()
		{
			return (RankType)ObscuredPrefs.GetInt("rank" + DateTime.Now.Day);
		}

		public static void SetRank(RankType rank)
		{
			ObscuredPrefs.SetInt("rank" + DateTime.Now.Day, (int)rank);
		}

		public static string GetMyRankName()
		{
			return GetRankName(GetRank());
		}

		public static string GetRankName(RankType rankType)
		{
			return Utils.GetRankNameByIndex((int)rankType);
		}

		public static Texture2D LoadMyRankTextureFromResources()
		{
			return LoadRankTextureFromResources(GetRank());
		}

		public static Texture2D LoadRankTextureFromResources(RankType rankType)
		{
			return Resources.Load<Texture2D>(string.Format("{0}{1}", "icons/Rating/Rank_0", (int)rankType));
		}

		public static Material LoadMyRankMaterialFromResources()
		{
			return LoadRankMaterialFromResources(GetRank());
		}

		public static Material LoadRankMaterialFromResources(RankType rankType)
		{
			return Resources.Load<Material>(string.Format("{0}{1}mat", "icons/Rating/Rank_0", (int)rankType));
		}

		public static Sprite LoadMyRankSpriteFromResources()
		{
			return LoadRankSpriteFromResources(GetRank());
		}

		public static Sprite LoadRankSpriteFromResources(RankType rankType)
		{
			return Resources.Load<Sprite>(string.Format("{0}{1}", "icons/Rating/Rank_0", (int)rankType));
		}
	}
}
