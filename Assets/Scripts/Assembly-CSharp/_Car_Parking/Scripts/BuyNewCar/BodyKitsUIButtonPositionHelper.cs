using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.BuyNewCar
{
	[Serializable]
	public class BodyKitsUIButtonPositionHelper
	{
		[Serializable]
		private class PositionData
		{
			[Serializable]
			public class OffsetData
			{
				private enum Direction
				{
					Left = 0,
					Right = 1,
					Up = 2,
					Down = 3,
					Forward = 4,
					Backward = 5
				}

				[SerializeField]
				private Direction Dir;

				[SerializeField]
				private float Length;

				public Vector3 GetOffset(Transform transform)
				{
					return Dir switch
					{
						Direction.Left => -transform.right * Length, 
						Direction.Right => transform.right * Length, 
						Direction.Up => transform.up * Length, 
						Direction.Down => -transform.up * Length, 
						Direction.Forward => transform.forward * Length, 
						Direction.Backward => -transform.forward * Length, 
						_ => Vector3.zero, 
					};
				}
			}

			public BodyKitType BodyKitType;

			public BoxColliderSidePositions.Side Side;

			public OffsetData[] Offsets;
		}

		[SerializeField]
		private PositionData[] Positions;

		public List<Vector3> GetPositions(BodyKitType bodyKitType, BoxCollider boxCollider, Transform transform)
		{
			List<Vector3> list = new List<Vector3>();
			PositionData[] positions = Positions;
			foreach (PositionData positionData in positions)
			{
				if (positionData.BodyKitType == bodyKitType)
				{
					Vector3 side = boxCollider.GetSide(positionData.Side);
					PositionData.OffsetData[] offsets = positionData.Offsets;
					foreach (PositionData.OffsetData offsetData in offsets)
					{
						side += offsetData.GetOffset(transform);
					}
					list.Add(side);
				}
			}
			return list;
		}
	}
}
