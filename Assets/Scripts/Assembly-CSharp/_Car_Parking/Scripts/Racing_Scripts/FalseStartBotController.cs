using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class FalseStartBotController
	{
		private DragRacingConfig _config;

		private int _falseStartInRow;

		public FalseStartBotController(DragRacingConfig config)
		{
			_config = config;
		}

		public bool GetNewIsFalseStart()
		{
			float chanceToFalseStart = _config.Bot.FalseStartData.GetChanceToFalseStart(_falseStartInRow);
			if (Random.value < chanceToFalseStart)
			{
				_falseStartInRow++;
				return true;
			}
			Clear();
			return false;
		}

		public void Clear()
		{
			_falseStartInRow = 0;
		}
	}
}
