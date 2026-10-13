using System;
using UnityEngine;

namespace _Car_Parking.Scripts.CpmCustomFeatures.Benchmarking
{
	public sealed class BenchmarkReport
	{
		public DateTime Start;

		public DateTime End;

		public FpsData FpsData;

		public string Name;

		public BenchmarkReport(string name, DateTime start, DateTime end)
		{
			Name = name;
			Start = start;
			End = end;
		}

		public override string ToString()
		{
			return "Name: " + Name + "\n" + $"Duration: {(End - Start).TotalSeconds} seconds\n" + "Device: " + SystemInfo.deviceModel + "\nDevice Id: " + SystemInfo.deviceUniqueIdentifier + "\nSystem: " + SystemInfo.operatingSystem + "\n" + $"{FpsData}";
		}
	}
}
