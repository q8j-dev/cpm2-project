using System;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public class Lane
	{
		public LaneConnectors laneEdges;

		public bool laneDirection;

		public int laneSpeed;

		public bool[] allowedCars;

		public Lane(int nrOfCars, bool laneDirection, int laneSpeed)
		{
			laneEdges = default;
			this.laneDirection = laneDirection;
			this.laneSpeed = laneSpeed;
			allowedCars = new bool[nrOfCars];
			for (int i = 0; i < allowedCars.Length; i++)
			{
				allowedCars[i] = true;
			}
		}

		public void UpdateAllowedCars(int nrOfCars)
		{
			if (allowedCars.Length < nrOfCars)
			{
				bool[] array = new bool[nrOfCars];
				for (int i = 0; i < nrOfCars; i++)
				{
					array[i] = true;
				}
				for (int j = 0; j < allowedCars.Length; j++)
				{
					array[j] = allowedCars[j];
				}
				allowedCars = array;
			}
		}
	}
}
