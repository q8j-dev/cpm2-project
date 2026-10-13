using System;
using ChartAndGraph;
using UnityEngine;

public class MonthWeekDayYear : MonoBehaviour
{
	public GapEnum Gap;

	private double mStartPosition;

	private double mEndPostion;

	private GapEnum? mCurrent;

	private void Start()
	{
	}

	private void OnValidate()
	{
	}

	private void SetConstantGap(int days)
	{
		HorizontalAxis component = GetComponent<HorizontalAxis>();
		if (!(component == null))
		{
			if (component.SubDivisions.Total != 0)
			{
				component.SubDivisions.Total = 0;
			}
			if (component.MainDivisions.Messure != ChartDivisionInfo.DivisionMessure.DataUnits)
			{
				component.MainDivisions.Messure = ChartDivisionInfo.DivisionMessure.DataUnits;
			}
			float num = (float)((double)days * TimeSpan.FromDays(1.0).TotalSeconds);
			if (component.MainDivisions.UnitsPerDivision != num)
			{
				component.MainDivisions.UnitsPerDivision = num;
			}
		}
	}

	private void FixDivisions()
	{
	}

	private void Regenrate()
	{
		ScrollableAxisChart component = GetComponent<ScrollableAxisChart>();
		HorizontalAxis component2 = GetComponent<HorizontalAxis>();
		if (!(component == null) && !(component2 == null))
		{
			if (component2.SubDivisions.Total != 0)
			{
				component2.SubDivisions.Total = 0;
			}
			if (component2.MainDivisions.Messure != ChartDivisionInfo.DivisionMessure.TotalDivisions)
			{
				component2.MainDivisions.Messure = ChartDivisionInfo.DivisionMessure.TotalDivisions;
			}
			if (component2.MainDivisions.Total != 1)
			{
				component2.MainDivisions.Total = 1;
			}
			component.ScrollableData.RestoreDataValues(0);
			double num = component.ScrollableData.HorizontalViewOrigin + component.HorizontalScrolling;
			double num2 = component.ScrollableData.HorizontalViewSize + num;
			if (num2 < num)
			{
				double num3 = num;
				num = num2;
				num2 = num3;
			}
			double num4 = Math.Abs(component.ScrollableData.HorizontalViewSize * 0.5);
			mStartPosition = num - num4;
			mEndPostion = num2 + num4;
			if (Gap == GapEnum.Month)
			{
				RegenrateMonth();
			}
			else if (Gap == GapEnum.Year)
			{
				RegenarateYear();
			}
		}
	}

	private void RegenarateYear()
	{
		ScrollableAxisChart component = GetComponent<ScrollableAxisChart>();
		if (!(component == null))
		{
			component.ClearHorizontalCustomDivisions();
			DateTime dateTime = ChartDateUtility.ValueToDate(mStartPosition);
			DateTime dateTime2 = ChartDateUtility.ValueToDate(mEndPostion);
			DateTime dateTime3 = ChartDateUtility.ValueToDate(component.ScrollableData.HorizontalViewOrigin);
			int num = dateTime.Year - dateTime3.Year;
			DateTime dateTime4 = dateTime3.AddYears(num);
			while (dateTime4 < dateTime2)
			{
				component.AddHorizontalAxisDivision(ChartDateUtility.DateToValue(dateTime4));
				num++;
				dateTime4 = dateTime3.AddYears(num);
			}
		}
	}

	private void RegenrateMonth()
	{
		ScrollableAxisChart component = GetComponent<ScrollableAxisChart>();
		if (!(component == null))
		{
			component.ClearHorizontalCustomDivisions();
			DateTime dateTime = ChartDateUtility.ValueToDate(mStartPosition);
			DateTime dateTime2 = ChartDateUtility.ValueToDate(mEndPostion);
			DateTime dateTime3 = ChartDateUtility.ValueToDate(component.ScrollableData.HorizontalViewOrigin);
			int value = dateTime.Year - dateTime3.Year;
			int num = dateTime.AddYears(value).Month - dateTime3.Month;
			DateTime dateTime4 = dateTime3.AddYears(value).AddMonths(num);
			while (dateTime4 < dateTime2)
			{
				component.AddHorizontalAxisDivision(ChartDateUtility.DateToValue(dateTime4));
				num++;
				dateTime4 = dateTime3.AddYears(value).AddMonths(num);
			}
		}
	}

	private bool IsViewInside()
	{
		ScrollableAxisChart component = GetComponent<ScrollableAxisChart>();
		if (component == null)
		{
			return false;
		}
		double num = component.ScrollableData.HorizontalViewOrigin + component.HorizontalScrolling;
		double num2 = component.ScrollableData.HorizontalViewSize + num;
		if (num2 < num)
		{
			double num3 = num;
			num = num2;
			num2 = num3;
		}
		if (num < mStartPosition)
		{
			return false;
		}
		if (num2 > mEndPostion)
		{
			return false;
		}
		return true;
	}

	private void CheckGenerate()
	{
		switch (Gap)
		{
		case GapEnum.Day:
			SetConstantGap(1);
			break;
		case GapEnum.Week:
			SetConstantGap(7);
			break;
		case GapEnum.Month:
			if (!IsViewInside() || !mCurrent.HasValue || mCurrent.Value != GapEnum.Month)
			{
				Regenrate();
			}
			break;
		case GapEnum.Year:
			if (!IsViewInside() || !mCurrent.HasValue || mCurrent.Value != GapEnum.Year)
			{
				Regenrate();
			}
			break;
		}
		mCurrent = Gap;
	}

	private void Update()
	{
		CheckGenerate();
	}
}
