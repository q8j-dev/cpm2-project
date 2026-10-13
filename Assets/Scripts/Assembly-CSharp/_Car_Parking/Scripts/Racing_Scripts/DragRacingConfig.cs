using System;
using UnityEngine;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[CreateAssetMenu(fileName = "DragRacingConfig", menuName = "Configs/DragRacing/DragRacingConfig")]
	public class DragRacingConfig : ScriptableObject
	{
		[Serializable]
		public class BotData
		{
			[Serializable]
			public class FalseStartBotData
			{
				[SerializeField]
				[Range(0f, 1f)]
				private float ChanceToFalseStartMax = 0.4f;

				[SerializeField]
				[Range(0f, 1f)]
				private float ChanceToFalseStartMin = 0.05f;

				[SerializeField]
				private int ChanceToFalseStartInRow = 3;

				public float GetChanceToFalseStart(int falseStartInRow)
				{
					return Mathf.Lerp(ChanceToFalseStartMax, ChanceToFalseStartMin, Mathf.InverseLerp(0f, ChanceToFalseStartInRow, falseStartInRow));
				}
			}

			public RangeFloat DelayToStartBot;

			public FalseStartBotData FalseStartData;
		}

		public float DelayToTurnTree = 0.5f;

		public BotData Bot;
	}
}
