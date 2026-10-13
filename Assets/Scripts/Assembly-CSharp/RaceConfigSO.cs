using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[CreateAssetMenu(fileName = "RaceConfig", menuName = "Scriptable/RaceConfig")]
public class RaceConfigSO : ScriptableObject
{
	public Spline Spline;

	public Spline ReverseSpline;

	public int AiCarsCount;

	public int LapsCount = 3;

	public int QualificationLapsCount = 1;

	public int WaitAfterFinish = 15;

	public float PenaltyCrashSecs = 2f;

	public float PenaltyOutOfRoadDistance = 2f;

	public float HalfTrackWidth = 11f;

	public float LapDistance;

	public float TargetSpeedAfterFinish = 30f;

	public float MinTargetCrashValue = 20f;

	public int Direction;

	public string TrackName;

	public float PenaltySecondsForToRoad;

	public bool IsLeman;

	public int LemansMinutesCount;

	public int BonusLapCoeficentSeconds;

	public float PenaltyMetersForToRoad;

	[Header("Baked Track Data")]
	public BakedTrajectory BakedTrajectory;

	public BakedTrajectory PitStopTrajectory;

	[Header("Win prices")]
	public MoneyType[] Rewards;

	[ContextMenu("CreateReversed Spline")]
	public void CreateReversedSpline()
	{
		BezierKnot[] array = Spline.Knots.ToArray();
		BezierKnot[] array2 = new BezierKnot[array.Length];
		int num = 0;
		int num2 = array.Length;
		while (num2-- > 0)
		{
			array2[num] = array[num2];
			num++;
		}
		ReverseSpline.Knots = array2;
	}

	public int GetBonusSecsCount()
	{
		return BonusLapCoeficentSeconds;
	}

	public Spline ReversedSpline()
	{
		if (ReverseSpline.Knots.Count() < 1)
		{
			CreateReversedSpline();
		}
		return ReverseSpline;
	}
}
