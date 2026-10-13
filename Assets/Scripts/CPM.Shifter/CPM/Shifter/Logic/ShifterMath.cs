using System.Collections.Generic;
using UnityEngine;

namespace CPM.Shifter.Logic
{
	public static class ShifterMath
	{
		public const int MinGears = 3;

		public const int MaxGears = 7;

		public static IReadOnlyDictionary<int, SlotCoord> LayoutToSlotPositions(int numGears, ReversePosition reversePos, bool doglegFirstGear)
		{
			int num = ((numGears < 3) ? 3 : ((numGears > 7) ? 7 : numGears));
			Dictionary<int, SlotCoord> dictionary = new Dictionary<int, SlotCoord>(num + 1);
			int num2 = ((reversePos == ReversePosition.TopLeft) ? 1 : 0);
			if (doglegFirstGear)
			{
				dictionary[1] = new SlotCoord(num2, 1);
				int num3 = 2;
				int num4 = num2 + 1;
				while (num3 <= num)
				{
					dictionary[num3++] = new SlotCoord(num4, 0);
					if (num3 > num)
					{
						break;
					}
					dictionary[num3++] = new SlotCoord(num4, 1);
					num4++;
				}
			}
			else
			{
				int num5 = 1;
				int num6 = num2;
				while (num5 <= num)
				{
					dictionary[num5++] = new SlotCoord(num6, 0);
					if (num5 > num)
					{
						break;
					}
					dictionary[num5++] = new SlotCoord(num6, 1);
					num6++;
				}
			}
			int num7 = LastForwardColumn(dictionary);
			SlotCoord value;
			switch (reversePos)
			{
			case ReversePosition.TopLeft:
			case ReversePosition.DoglegTopLeft:
				value = new SlotCoord(0, 0);
				break;
			case ReversePosition.TopRight:
				value = new SlotCoord(num7 + 1, 0);
				break;
			default:
				value = ((!doglegFirstGear && num % 2 == 1) ? new SlotCoord(num7, 1) : new SlotCoord(num7 + 1, 1));
				break;
			}
			dictionary[-1] = value;
			return dictionary;
		}

		public static int NearestSlot(Vector2 knobAnchoredPos, IReadOnlyDictionary<int, Vector2> slotPixelPositions, float neutralBandHalfHeight)
		{
			if (slotPixelPositions == null || slotPixelPositions.Count == 0)
			{
				return 0;
			}
			int result = 0;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			foreach (KeyValuePair<int, Vector2> slotPixelPosition in slotPixelPositions)
			{
				float num4 = slotPixelPosition.Value.x - knobAnchoredPos.x;
				float num5 = slotPixelPosition.Value.y - knobAnchoredPos.y;
				float num6 = num4 * num4 + num5 * num5;
				if (num6 < num)
				{
					num = num6;
					result = slotPixelPosition.Key;
				}
				if (slotPixelPosition.Value.y < num2)
				{
					num2 = slotPixelPosition.Value.y;
				}
				if (slotPixelPosition.Value.y > num3)
				{
					num3 = slotPixelPosition.Value.y;
				}
			}
			float num7 = (num2 + num3) * 0.5f;
			if (Mathf.Abs(knobAnchoredPos.y - num7) <= neutralBandHalfHeight)
			{
				return 0;
			}
			return result;
		}

		private static int LastForwardColumn(Dictionary<int, SlotCoord> slots)
		{
			int num = 0;
			foreach (KeyValuePair<int, SlotCoord> slot in slots)
			{
				if (slot.Key != -1 && slot.Value.Col > num)
				{
					num = slot.Value.Col;
				}
			}
			return num;
		}

		public static IReadOnlyList<Rect> BuildAllowedZones(IReadOnlyDictionary<int, Vector2> slotPixelPositions, float cellWidth, float _cellHeight, float channelThickness)
		{
			List<Rect> list = new List<Rect>();
			if (slotPixelPositions == null || slotPixelPositions.Count == 0)
			{
				return list;
			}
			float num = channelThickness * 0.5f;
			Dictionary<float, List<Vector2>> dictionary = new Dictionary<float, List<Vector2>>();
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MaxValue;
			float num5 = float.MinValue;
			foreach (KeyValuePair<int, Vector2> slotPixelPosition in slotPixelPositions)
			{
				float x = slotPixelPosition.Value.x;
				float y = slotPixelPosition.Value.y;
				if (!dictionary.TryGetValue(x, out var value))
				{
					value = (dictionary[x] = new List<Vector2>());
				}
				value.Add(slotPixelPosition.Value);
				if (x < num2)
				{
					num2 = x;
				}
				if (x > num3)
				{
					num3 = x;
				}
				if (y < num4)
				{
					num4 = y;
				}
				if (y > num5)
				{
					num5 = y;
				}
			}
			float num6 = (num4 + num5) * 0.5f;
			foreach (KeyValuePair<float, List<Vector2>> item in dictionary)
			{
				float key = item.Key;
				float num7 = float.MaxValue;
				float num8 = float.MinValue;
				foreach (Vector2 item2 in item.Value)
				{
					if (item2.y < num7)
					{
						num7 = item2.y;
					}
					if (item2.y > num8)
					{
						num8 = item2.y;
					}
				}
				if (num6 < num7)
				{
					num7 = num6;
				}
				if (num6 > num8)
				{
					num8 = num6;
				}
				list.Add(new Rect(key - num, num7, channelThickness, num8 - num7));
			}
			list.Add(new Rect(num2, num6 - num, num3 - num2, channelThickness));
			return list;
		}

		public static bool IsOutsideZonesBoundingBox(Vector2 point, IReadOnlyList<Rect> zones)
		{
			if (zones == null || zones.Count == 0)
			{
				return true;
			}
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			foreach (Rect zone in zones)
			{
				if (zone.xMin < num)
				{
					num = zone.xMin;
				}
				if (zone.xMax > num3)
				{
					num3 = zone.xMax;
				}
				if (zone.yMin < num2)
				{
					num2 = zone.yMin;
				}
				if (zone.yMax > num4)
				{
					num4 = zone.yMax;
				}
			}
			if (!(point.x < num) && !(point.x > num3) && !(point.y < num2))
			{
				return point.y > num4;
			}
			return true;
		}

		public static Vector2 ClampToZonesBoundingBox(Vector2 point, IReadOnlyList<Rect> zones)
		{
			if (zones == null || zones.Count == 0)
			{
				return point;
			}
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			foreach (Rect zone in zones)
			{
				if (zone.xMin < num)
				{
					num = zone.xMin;
				}
				if (zone.xMax > num3)
				{
					num3 = zone.xMax;
				}
				if (zone.yMin < num2)
				{
					num2 = zone.yMin;
				}
				if (zone.yMax > num4)
				{
					num4 = zone.yMax;
				}
			}
			return new Vector2(Mathf.Clamp(point.x, num, num3), Mathf.Clamp(point.y, num2, num4));
		}

		public static Vector2 ClampToAllowedZones(Vector2 point, IReadOnlyList<Rect> zones)
		{
			if (zones == null || zones.Count == 0)
			{
				return point;
			}
			foreach (Rect zone in zones)
			{
				if (zone.Contains(point))
				{
					return point;
				}
			}
			Vector2 result = point;
			float num = float.MaxValue;
			foreach (Rect zone2 in zones)
			{
				float num2 = Mathf.Clamp(point.x, zone2.xMin, zone2.xMax);
				float num3 = Mathf.Clamp(point.y, zone2.yMin, zone2.yMax);
				float num4 = num2 - point.x;
				float num5 = num3 - point.y;
				float num6 = num4 * num4 + num5 * num5;
				if (num6 < num)
				{
					num = num6;
					result = new Vector2(num2, num3);
				}
			}
			return result;
		}
	}
}
