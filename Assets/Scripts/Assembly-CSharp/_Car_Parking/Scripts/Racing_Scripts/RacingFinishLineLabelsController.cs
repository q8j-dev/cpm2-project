using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RacingFinishLineLabelsController : MonoBehaviour
	{
		public const string OneMileInMetres = "1600m";

		public const string QuoterMileInMetres = "402m";

		public const string OneMile = "1 mile";

		public const string QuoterMile = "1/4 mile";

		[SerializeField]
		private List<TextMesh> QuoterMileLabels;

		[SerializeField]
		private List<TextMesh> OneMileLabels;

		private void Start()
		{
			UpdateLabels(FlagsStorage.Kph);
		}

		private void OnEnable()
		{
			GlobalEventSystem.OnSpeedUnitChanged += UpdateLabels;
		}

		private void OnDisable()
		{
			GlobalEventSystem.OnSpeedUnitChanged -= UpdateLabels;
		}

		private void UpdateLabels(bool isKph)
		{
			string text = (isKph ? "1600m" : "1 mile");
			string text2 = (isKph ? "402m" : "1/4 mile");
			foreach (TextMesh oneMileLabel in OneMileLabels)
			{
				if ((bool)oneMileLabel)
				{
					oneMileLabel.text = text;
				}
			}
			foreach (TextMesh quoterMileLabel in QuoterMileLabels)
			{
				if ((bool)quoterMileLabel)
				{
					quoterMileLabel.text = text2;
				}
			}
		}
	}
}
