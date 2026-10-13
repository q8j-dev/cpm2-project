using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using MVPFree.MapIcons.ScrObjs.Scripts;

namespace _Car_Parking.Scripts.GameActivity.Providers
{
	public sealed class TimeAttackActivityProvider : IGameActivityProvider
	{
		private readonly TimeAttackDataSO _data;

		public TimeAttackActivityProvider(TimeAttackDataSO data)
		{
			_data = data;
		}

		public bool Handles(GameActivityType type)
		{
			if (type != GameActivityType.Rally && type != GameActivityType.Drift)
			{
				return type == GameActivityType.TimeAttack;
			}
			return true;
		}

		public IReadOnlyList<GameActivityEntry> GetActivities(GameActivityType type)
		{
			List<GameActivityEntry> list = new List<GameActivityEntry>();
			if (!Handles(type) || !_data || _data.TimeAttackDatas == null)
			{
				return list;
			}
			TimeAttackMode timeAttackMode = ToMode(type);
			TimeAttackData[] timeAttackDatas = _data.TimeAttackDatas;
			foreach (TimeAttackData timeAttackData in timeAttackDatas)
			{
				if (timeAttackData.TimeAttackMode == timeAttackMode)
				{
					list.Add(new GameActivityEntry(type, timeAttackData.TrackIndex, timeAttackData.Name, timeAttackData.Description, timeAttackData.IconType, null));
				}
			}
			return list;
		}

		private static TimeAttackMode ToMode(GameActivityType type)
		{
			return type switch
			{
				GameActivityType.Rally => TimeAttackMode.Rally, 
				GameActivityType.Drift => TimeAttackMode.Drift, 
				_ => TimeAttackMode.TimeAttack, 
			};
		}
	}
}
