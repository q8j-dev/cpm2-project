using System;
using System.Collections.Generic;
using MemoryPack;
using _Car_Parking.Scripts.Other.Helpers;

namespace CPM.Rating.Entities
{
	public class RatingDataFormatter : MemoryPackFormatter<RatingData>
	{
		private const int ElementCount = 29;

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RatingData value)
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			List<int?> list = ToList(value);
			int value2 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				value2 = value2.SetBit(i % 32, list[i].HasValue);
				if (i == list.Count - 1 || (i + 1) % 32 == 0)
				{
					writer.WriteValue(in value2);
					value2 = 0;
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].HasValue)
				{
					writer.WriteValue<int>(list[j].Value);
				}
			}
		}

		public override void Deserialize(ref MemoryPackReader reader, ref RatingData value)
		{
			if (reader.PeekIsNull())
			{
				reader.Advance(1);
				value = null;
				return;
			}
			value = new RatingData();
			int value2 = reader.ReadValue<int>();
			for (int i = 0; i < 29; i++)
			{
				if (value2.IsBitSetTo1(i))
				{
					switch (i)
					{
					case 0:
						value.t_distance = reader.ReadValue<float>();
						break;
					case 1:
						value.time = reader.ReadValue<float>();
						break;
					case 2:
						value.speed_banner = reader.ReadValue<int>();
						break;
					case 3:
						value.gifts = reader.ReadValue<int>();
						break;
					case 4:
						value.treasure = reader.ReadValue<int>();
						break;
					case 5:
						value.cars = reader.ReadValue<int>();
						break;
					case 6:
						value.race_win = reader.ReadValue<int>();
						break;
					case 7:
						value.levels = reader.ReadValue<int>();
						break;
					case 8:
						value.drift = reader.ReadValue<float>();
						break;
					case 9:
						value.run = reader.ReadValue<float>();
						break;
					case 10:
						value.police = reader.ReadValue<int>();
						break;
					case 11:
						value.block_post = reader.ReadValue<int>();
						break;
					case 12:
						value.real_estate = reader.ReadValue<int>();
						break;
					case 13:
						value.fuel = reader.ReadValue<float>();
						break;
					case 14:
						value.car_trade = reader.ReadValue<int>();
						break;
					case 15:
						value.car_exchange = reader.ReadValue<int>();
						break;
					case 16:
						value.burnt_tire = reader.ReadValue<int>();
						break;
					case 17:
						value.car_fix = reader.ReadValue<int>();
						break;
					case 18:
						value.car_wash = reader.ReadValue<int>();
						break;
					case 19:
						value.offroad = reader.ReadValue<float>();
						break;
					case 20:
						value.passanger_distance = reader.ReadValue<float>();
						break;
					case 21:
						value.reactions = reader.ReadValue<int>();
						break;
					case 22:
						value.drift_max = reader.ReadValue<float>();
						break;
					case 23:
						value.taxi = reader.ReadValue<float>();
						break;
					case 24:
						value.delivery = reader.ReadValue<float>();
						break;
					case 25:
						value.cargo = reader.ReadValue<float>();
						break;
					case 26:
						value.push_ups = reader.ReadValue<int>();
						break;
					case 27:
						value.slicer_cut = reader.ReadValue<int>();
						break;
					case 28:
						value.car_collided = reader.ReadValue<int>();
						break;
					}
				}
			}
		}

		private List<int?> ToList(RatingData value)
		{
			return new List<int?>
			{
				FloatToInt(value.t_distance),
				FloatToInt(value.time),
				value.speed_banner,
				value.gifts,
				value.treasure,
				value.cars,
				value.race_win,
				value.levels,
				FloatToInt(value.drift),
				FloatToInt(value.run),
				value.police,
				value.block_post,
				value.real_estate,
				FloatToInt(value.fuel),
				value.car_trade,
				value.car_exchange,
				value.burnt_tire,
				value.car_fix,
				value.car_wash,
				FloatToInt(value.offroad),
				FloatToInt(value.passanger_distance),
				value.reactions,
				FloatToInt(value.drift_max),
				FloatToInt(value.taxi),
				FloatToInt(value.delivery),
				FloatToInt(value.cargo),
				value.push_ups,
				value.slicer_cut,
				value.car_collided
			};
		}

		private int? FloatToInt(float? value)
		{
			if (value.HasValue)
			{
				return BitConverter.ToInt32(BitConverter.GetBytes(value.Value), 0);
			}
			return null;
		}
	}
}
