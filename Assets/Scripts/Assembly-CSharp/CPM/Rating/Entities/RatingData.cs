using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace CPM.Rating.Entities
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class RatingData : IMemoryPackable<RatingData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class RatingDataFormatter : MemoryPackFormatter<RatingData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RatingData value)
			{
				RatingData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref RatingData value)
			{
				RatingData.Deserialize(ref reader, ref value);
			}
		}

		public float? t_distance;

		public float? time;

		public int? speed_banner;

		public int? gifts;

		public int? treasure;

		public int? cars;

		public int? race_win;

		public int? levels;

		public float? drift;

		public float? run;

		public int? police;

		public int? block_post;

		public int? real_estate;

		public float? fuel;

		public int? car_trade;

		public int? car_exchange;

		public int? burnt_tire;

		public int? car_fix;

		public int? car_wash;

		public float? offroad;

		public float? passanger_distance;

		public int? reactions;

		public float? drift_max;

		public float? taxi;

		public float? delivery;

		public float? cargo;

		public int? push_ups;

		public int? slicer_cut;

		public int? car_collided;

		public RatingData(float? t_distance = null, float? time = null, int? speed_banner = null, int? gifts = null, int? treasure = null, int? cars = null, int? race_win = null, int? levels = null, float? drift = null, float? run = null, int? police = null, int? block_post = null, int? real_estate = null, float? fuel = null, int? car_trade = null, int? car_exchange = null, int? burnt_tire = null, int? car_fix = null, int? car_wash = null, float? offroad = null, float? passanger_distance = null, int? reactions = null, float? drift_max = null, float? taxi = null, float? delivery = null, float? cargo = null, int? push_ups = null, int? slicer_cut = null, int? car_collided = null)
		{
			this.t_distance = t_distance;
			this.time = time;
			this.speed_banner = speed_banner;
			this.gifts = gifts;
			this.treasure = treasure;
			this.cars = cars;
			this.race_win = race_win;
			this.levels = levels;
			this.drift = drift;
			this.run = run;
			this.police = police;
			this.block_post = block_post;
			this.real_estate = real_estate;
			this.fuel = fuel;
			this.car_trade = car_trade;
			this.car_exchange = car_exchange;
			this.burnt_tire = burnt_tire;
			this.car_fix = car_fix;
			this.car_wash = car_wash;
			this.offroad = offroad;
			this.passanger_distance = passanger_distance;
			this.reactions = reactions;
			this.drift_max = drift_max;
			this.taxi = taxi;
			this.delivery = delivery;
			this.cargo = cargo;
			this.push_ups = push_ups;
			this.slicer_cut = slicer_cut;
			this.car_collided = car_collided;
		}

		public string ToJson()
		{
			string text = "{";
			List<string> obj = new List<string>
			{
				t_distance.HasValue ? ("\"t_distance\" : " + t_distance.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null,
				time.HasValue ? ("\"time\" : " + time.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null
			};
			object item;
			if (!speed_banner.HasValue)
			{
				item = null;
			}
			else
			{
				int? num = speed_banner;
				item = "\"speed_banner\" : " + num + ",";
			}
			obj.Add((string)item);
			object item2;
			if (!gifts.HasValue)
			{
				item2 = null;
			}
			else
			{
				int? num = gifts;
				item2 = "\"gifts\" : " + num + ",";
			}
			obj.Add((string)item2);
			object item3;
			if (!treasure.HasValue)
			{
				item3 = null;
			}
			else
			{
				int? num = treasure;
				item3 = "\"treasure\" : " + num + ",";
			}
			obj.Add((string)item3);
			object item4;
			if (!race_win.HasValue)
			{
				item4 = null;
			}
			else
			{
				int? num = race_win;
				item4 = "\"race_win\" : " + num + ",";
			}
			obj.Add((string)item4);
			object item5;
			if (!cars.HasValue)
			{
				item5 = null;
			}
			else
			{
				int? num = cars;
				item5 = "\"cars\" : " + num + ",";
			}
			obj.Add((string)item5);
			object item6;
			if (!levels.HasValue)
			{
				item6 = null;
			}
			else
			{
				int? num = levels;
				item6 = "\"levels\" : " + num + ",";
			}
			obj.Add((string)item6);
			obj.Add(drift.HasValue ? ("\"drift\" : " + drift.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			obj.Add(run.HasValue ? ("\"run\" : " + run.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			object item7;
			if (!police.HasValue)
			{
				item7 = null;
			}
			else
			{
				int? num = police;
				item7 = "\"police\" : " + num + ",";
			}
			obj.Add((string)item7);
			object item8;
			if (!block_post.HasValue)
			{
				item8 = null;
			}
			else
			{
				int? num = block_post;
				item8 = "\"block_post\" : " + num + ",";
			}
			obj.Add((string)item8);
			object item9;
			if (!real_estate.HasValue)
			{
				item9 = null;
			}
			else
			{
				int? num = real_estate;
				item9 = "\"real_estate\" : " + num + ",";
			}
			obj.Add((string)item9);
			obj.Add(fuel.HasValue ? ("\"fuel\" : " + fuel.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			object item10;
			if (!car_trade.HasValue)
			{
				item10 = null;
			}
			else
			{
				int? num = car_trade;
				item10 = "\"car_trade\" : " + num + ",";
			}
			obj.Add((string)item10);
			object item11;
			if (!car_exchange.HasValue)
			{
				item11 = null;
			}
			else
			{
				int? num = car_exchange;
				item11 = "\"car_exchange\" : " + num + ",";
			}
			obj.Add((string)item11);
			object item12;
			if (!burnt_tire.HasValue)
			{
				item12 = null;
			}
			else
			{
				int? num = burnt_tire;
				item12 = "\"burnt_tire\" : " + num + ",";
			}
			obj.Add((string)item12);
			object item13;
			if (!car_fix.HasValue)
			{
				item13 = null;
			}
			else
			{
				int? num = car_fix;
				item13 = "\"car_fix\" : " + num + ",";
			}
			obj.Add((string)item13);
			object item14;
			if (!car_wash.HasValue)
			{
				item14 = null;
			}
			else
			{
				int? num = car_wash;
				item14 = "\"car_wash\" : " + num + ",";
			}
			obj.Add((string)item14);
			obj.Add(offroad.HasValue ? ("\"offroad\" : " + offroad.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			obj.Add(passanger_distance.HasValue ? ("\"passanger_distance\" : " + passanger_distance.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			object item15;
			if (!reactions.HasValue)
			{
				item15 = null;
			}
			else
			{
				int? num = reactions;
				item15 = "\"reactions\" : " + num + ",";
			}
			obj.Add((string)item15);
			obj.Add(drift_max.HasValue ? ("\"drift_max\" : " + drift_max.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			obj.Add(taxi.HasValue ? ("\"taxi\" : " + taxi.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			obj.Add(delivery.HasValue ? ("\"delivery\" : " + delivery.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			obj.Add(cargo.HasValue ? ("\"cargo\" : " + cargo.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			obj.Add(push_ups.HasValue ? ("\"push_ups\" : " + push_ups.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			obj.Add(slicer_cut.HasValue ? ("\"slicer_cut\" : " + slicer_cut.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			obj.Add(car_collided.HasValue ? ("\"car_collided\" : " + car_collided.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) + ",") : null);
			foreach (string item16 in obj)
			{
				text += item16;
			}
			text = text.Remove(text.Length - 1);
			return text + "}";
		}

		public Dictionary<string, float> ToDictionary()
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			string text = ToJson().Remove(0, 1);
			string[] array = text.Remove(text.Length - 1, 1).Split(',');
			foreach (string text2 in array)
			{
				string[] separator = new string[1] { " : " };
				string[] array2 = text2.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				array2[0] = array2[0].Remove(0, 1);
				array2[0] = array2[0].Remove(array2[0].Length - 1, 1);
				dictionary.Add(array2[0], float.Parse(array2[1], CultureInfo.InvariantCulture));
			}
			return dictionary;
		}

		static RatingData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<RatingData>())
			{
				MemoryPackFormatterProvider.Register(new RatingDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<RatingData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<RatingData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<float?>())
			{
				MemoryPackFormatterProvider.Register(new NullableFormatter<float>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<int?>())
			{
				MemoryPackFormatterProvider.Register(new NullableFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RatingData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.DangerousWriteUnmanagedWithObjectHeader(29, in value.t_distance, in value.time, in value.speed_banner, in value.gifts, in value.treasure, in value.cars, in value.race_win, in value.levels, in value.drift, in value.run, in value.police, in value.block_post, in value.real_estate, in value.fuel, in value.car_trade);
			writer.DangerousWriteUnmanaged(in value.car_exchange, in value.burnt_tire, in value.car_fix, in value.car_wash, in value.offroad, in value.passanger_distance, in value.reactions, in value.drift_max, in value.taxi, in value.delivery, in value.cargo, in value.push_ups, in value.slicer_cut, in value.car_collided);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref RatingData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			float? value2;
			float? value3;
			int? value4;
			int? value5;
			int? value6;
			int? value7;
			int? value8;
			int? value9;
			float? value10;
			float? value11;
			int? value12;
			int? value13;
			int? value14;
			float? value15;
			int? value16;
			int? value17;
			int? value18;
			int? value19;
			int? value20;
			float? value21;
			float? value22;
			int? value23;
			float? value24;
			float? value25;
			float? value26;
			float? value27;
			int? value28;
			int? value29;
			int? value30;
			if (memberCount == 29)
			{
				if (value == null)
				{
					reader.DangerousReadUnmanaged<float?, float?, int?, int?, int?, int?, int?, int?, float?, float?, int?, int?, int?, float?, int?>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9, out value10, out value11, out value12, out value13, out value14, out value15, out value16);
					reader.DangerousReadUnmanaged<int?, int?, int?, int?, float?, float?, int?, float?, float?, float?, float?, int?, int?, int?>(out value17, out value18, out value19, out value20, out value21, out value22, out value23, out value24, out value25, out value26, out value27, out value28, out value29, out value30);
				}
				else
				{
					value2 = value.t_distance;
					value3 = value.time;
					value4 = value.speed_banner;
					value5 = value.gifts;
					value6 = value.treasure;
					value7 = value.cars;
					value8 = value.race_win;
					value9 = value.levels;
					value10 = value.drift;
					value11 = value.run;
					value12 = value.police;
					value13 = value.block_post;
					value14 = value.real_estate;
					value15 = value.fuel;
					value16 = value.car_trade;
					value17 = value.car_exchange;
					value18 = value.burnt_tire;
					value19 = value.car_fix;
					value20 = value.car_wash;
					value21 = value.offroad;
					value22 = value.passanger_distance;
					value23 = value.reactions;
					value24 = value.drift_max;
					value25 = value.taxi;
					value26 = value.delivery;
					value27 = value.cargo;
					value28 = value.push_ups;
					value29 = value.slicer_cut;
					value30 = value.car_collided;
					reader.DangerousReadUnmanaged<float?>(out value2);
					reader.DangerousReadUnmanaged<float?>(out value3);
					reader.DangerousReadUnmanaged<int?>(out value4);
					reader.DangerousReadUnmanaged<int?>(out value5);
					reader.DangerousReadUnmanaged<int?>(out value6);
					reader.DangerousReadUnmanaged<int?>(out value7);
					reader.DangerousReadUnmanaged<int?>(out value8);
					reader.DangerousReadUnmanaged<int?>(out value9);
					reader.DangerousReadUnmanaged<float?>(out value10);
					reader.DangerousReadUnmanaged<float?>(out value11);
					reader.DangerousReadUnmanaged<int?>(out value12);
					reader.DangerousReadUnmanaged<int?>(out value13);
					reader.DangerousReadUnmanaged<int?>(out value14);
					reader.DangerousReadUnmanaged<float?>(out value15);
					reader.DangerousReadUnmanaged<int?>(out value16);
					reader.DangerousReadUnmanaged<int?>(out value17);
					reader.DangerousReadUnmanaged<int?>(out value18);
					reader.DangerousReadUnmanaged<int?>(out value19);
					reader.DangerousReadUnmanaged<int?>(out value20);
					reader.DangerousReadUnmanaged<float?>(out value21);
					reader.DangerousReadUnmanaged<float?>(out value22);
					reader.DangerousReadUnmanaged<int?>(out value23);
					reader.DangerousReadUnmanaged<float?>(out value24);
					reader.DangerousReadUnmanaged<float?>(out value25);
					reader.DangerousReadUnmanaged<float?>(out value26);
					reader.DangerousReadUnmanaged<float?>(out value27);
					reader.DangerousReadUnmanaged<int?>(out value28);
					reader.DangerousReadUnmanaged<int?>(out value29);
					reader.DangerousReadUnmanaged<int?>(out value30);
				}
			}
			else
			{
				if (memberCount > 29)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RatingData), 29, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = null;
					value3 = null;
					value4 = null;
					value5 = null;
					value6 = null;
					value7 = null;
					value8 = null;
					value9 = null;
					value10 = null;
					value11 = null;
					value12 = null;
					value13 = null;
					value14 = null;
					value15 = null;
					value16 = null;
					value17 = null;
					value18 = null;
					value19 = null;
					value20 = null;
					value21 = null;
					value22 = null;
					value23 = null;
					value24 = null;
					value25 = null;
					value26 = null;
					value27 = null;
					value28 = null;
					value29 = null;
					value30 = null;
				}
				else
				{
					value2 = value.t_distance;
					value3 = value.time;
					value4 = value.speed_banner;
					value5 = value.gifts;
					value6 = value.treasure;
					value7 = value.cars;
					value8 = value.race_win;
					value9 = value.levels;
					value10 = value.drift;
					value11 = value.run;
					value12 = value.police;
					value13 = value.block_post;
					value14 = value.real_estate;
					value15 = value.fuel;
					value16 = value.car_trade;
					value17 = value.car_exchange;
					value18 = value.burnt_tire;
					value19 = value.car_fix;
					value20 = value.car_wash;
					value21 = value.offroad;
					value22 = value.passanger_distance;
					value23 = value.reactions;
					value24 = value.drift_max;
					value25 = value.taxi;
					value26 = value.delivery;
					value27 = value.cargo;
					value28 = value.push_ups;
					value29 = value.slicer_cut;
					value30 = value.car_collided;
				}
				if (memberCount != 0)
				{
					reader.DangerousReadUnmanaged<float?>(out value2);
					if (memberCount != 1)
					{
						reader.DangerousReadUnmanaged<float?>(out value3);
						if (memberCount != 2)
						{
							reader.DangerousReadUnmanaged<int?>(out value4);
							if (memberCount != 3)
							{
								reader.DangerousReadUnmanaged<int?>(out value5);
								if (memberCount != 4)
								{
									reader.DangerousReadUnmanaged<int?>(out value6);
									if (memberCount != 5)
									{
										reader.DangerousReadUnmanaged<int?>(out value7);
										if (memberCount != 6)
										{
											reader.DangerousReadUnmanaged<int?>(out value8);
											if (memberCount != 7)
											{
												reader.DangerousReadUnmanaged<int?>(out value9);
												if (memberCount != 8)
												{
													reader.DangerousReadUnmanaged<float?>(out value10);
													if (memberCount != 9)
													{
														reader.DangerousReadUnmanaged<float?>(out value11);
														if (memberCount != 10)
														{
															reader.DangerousReadUnmanaged<int?>(out value12);
															if (memberCount != 11)
															{
																reader.DangerousReadUnmanaged<int?>(out value13);
																if (memberCount != 12)
																{
																	reader.DangerousReadUnmanaged<int?>(out value14);
																	if (memberCount != 13)
																	{
																		reader.DangerousReadUnmanaged<float?>(out value15);
																		if (memberCount != 14)
																		{
																			reader.DangerousReadUnmanaged<int?>(out value16);
																			if (memberCount != 15)
																			{
																				reader.DangerousReadUnmanaged<int?>(out value17);
																				if (memberCount != 16)
																				{
																					reader.DangerousReadUnmanaged<int?>(out value18);
																					if (memberCount != 17)
																					{
																						reader.DangerousReadUnmanaged<int?>(out value19);
																						if (memberCount != 18)
																						{
																							reader.DangerousReadUnmanaged<int?>(out value20);
																							if (memberCount != 19)
																							{
																								reader.DangerousReadUnmanaged<float?>(out value21);
																								if (memberCount != 20)
																								{
																									reader.DangerousReadUnmanaged<float?>(out value22);
																									if (memberCount != 21)
																									{
																										reader.DangerousReadUnmanaged<int?>(out value23);
																										if (memberCount != 22)
																										{
																											reader.DangerousReadUnmanaged<float?>(out value24);
																											if (memberCount != 23)
																											{
																												reader.DangerousReadUnmanaged<float?>(out value25);
																												if (memberCount != 24)
																												{
																													reader.DangerousReadUnmanaged<float?>(out value26);
																													if (memberCount != 25)
																													{
																														reader.DangerousReadUnmanaged<float?>(out value27);
																														if (memberCount != 26)
																														{
																															reader.DangerousReadUnmanaged<int?>(out value28);
																															if (memberCount != 27)
																															{
																																reader.DangerousReadUnmanaged<int?>(out value29);
																																if (memberCount != 28)
																																{
																																	reader.DangerousReadUnmanaged<int?>(out value30);
																																	_ = 29;
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				_ = value;
			}
			value = new RatingData(value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12, value13, value14, value15, value16, value17, value18, value19, value20, value21, value22, value23, value24, value25, value26, value27, value28, value29, value30);
		}
	}
}
