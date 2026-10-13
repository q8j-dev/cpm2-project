using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.BuyNewCar
{
	public class BuyBodyKitsUIButtonPositionGetter : MonoBehaviour
	{
		[Serializable]
		private class BodyKitPositionData
		{
			public BodyKitType BodyKitType;

			public Transform[] Transforms;

			public List<Vector3> GetPositions()
			{
				List<Vector3> list = new List<Vector3>();
				for (int i = 0; i < Transforms.Length; i++)
				{
					if ((bool)Transforms[i] && Transforms[i].gameObject.activeSelf)
					{
						list.Add(Transforms[i].position);
					}
				}
				return list;
			}
		}

		[SerializeField]
		private BodyKitPositionData[] BodyKitPositionDatas;

		public void SavePosition(BodyKitType bodyKitType, int index, Vector3 position)
		{
			Vector3 position2 = base.transform.InverseTransformPoint(position);
			SaveLocalPosition(bodyKitType, index, position2);
			position2.x = 0f - position2.x;
			SaveLocalPosition(bodyKitType, (index == 0) ? 1 : 0, position2);
		}

		public List<Vector3> GetPositions(BodyKitType bodyKitType)
		{
			BodyKitPositionData bodyKitPositionData = GetBodyKitPositionData(bodyKitType);
			if (bodyKitPositionData != null)
			{
				return bodyKitPositionData.GetPositions();
			}
			return new List<Vector3>();
		}

		private BodyKitPositionData GetBodyKitPositionData(BodyKitType bodyKitType)
		{
			for (int i = 0; i < BodyKitPositionDatas.Length; i++)
			{
				if (BodyKitPositionDatas[i].BodyKitType == bodyKitType)
				{
					return BodyKitPositionDatas[i];
				}
			}
			return null;
		}

		private void SaveLocalPosition(BodyKitType bodyKitType, int index, Vector3 position)
		{
			BodyKitPositionData bodyKitPositionData = GetBodyKitPositionData(bodyKitType);
			if (bodyKitPositionData != null && index >= 0 && index < bodyKitPositionData.Transforms.Length)
			{
				bodyKitPositionData.Transforms[index].gameObject.SetActive(value: true);
				bodyKitPositionData.Transforms[index].localPosition = position;
			}
		}
	}
}
