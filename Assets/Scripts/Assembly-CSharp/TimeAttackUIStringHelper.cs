using CPMEngine;
using Car_Parking.Scripts.Racing;

public class TimeAttackUIStringHelper
{
	public static string GetFormattedBestTime(RaceBestData bestData)
	{
		switch (bestData.TimeAttackType)
		{
		case TimeAttackMode.Drift:
			return $"{bestData.BestResult:F0} DP";
		case TimeAttackMode.TimeAttack:
			if (!(bestData.BestResult > 0f))
			{
				return "--:--:--";
			}
			return Utils.FormatTime(bestData.BestResult);
		default:
			return "";
		}
	}

	public static string GetFormattedFinishTime(float finishTime, TimeAttackMode timeAttackMode)
	{
		string result = "";
		switch (timeAttackMode)
		{
		case TimeAttackMode.TimeAttack:
		case TimeAttackMode.Rally:
			result = Utils.ConvertToTimeMiliSecs(finishTime);
			break;
		case TimeAttackMode.Drift:
			result = $"{finishTime:0000} DP";
			break;
		}
		return result;
	}
}
