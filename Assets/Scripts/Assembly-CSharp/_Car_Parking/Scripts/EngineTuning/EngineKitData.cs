#nullable enable annotations
using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

namespace _Car_Parking.Scripts.EngineTuning
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class EngineKitData : IMemoryPackable<EngineKitData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EngineKitDataFormatter : MemoryPackFormatter<EngineKitData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EngineKitData value)
			{
				EngineKitData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EngineKitData value)
			{
				EngineKitData.Deserialize(ref reader, ref value);
			}
		}

		[Header("Parts")]
		public PartData EngineId;

		[Header("EngineParts")]
		public PartData Block;

		public PartData CylinderHead;

		public PartData IntakeManifold;

		public PartData AirFilter;

		public PartData Charger;

		public PartData ExhaustHeader;

		public PartData ExhaustPipe;

		[Header("Block parts")]
		public PartData Crankshaft;

		public PartData ConnectionRod;

		public PartData Pistons;

		public PartData Flywheel;

		[Header("Cylinder head")]
		public PartData Camshaft1;

		public PartData Camshaft2;

		[Header("Electrical")]
		public PartData FuelRail;

		public PartData FuelInjectors;

		public PartData ThrottleValve;

		public PartData FuelPump;

		[Header("Cooling")]
		public PartData Radiator;

		public PartData Intercooler;

		[Header("Clutch")]
		public PartData Clutch;

		[Header("Wankel parts")]
		public PartData RotorHousing;

		public PartData Rotors;

		public PartData RotorTrackingGear;

		public PartData EccentricShaft;

		static EngineKitData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EngineKitData>())
			{
				MemoryPackFormatterProvider.Register(new EngineKitDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EngineKitData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EngineKitData>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EngineKitData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(25);
			writer.WritePackable(in value.EngineId);
			writer.WritePackable(in value.Block);
			writer.WritePackable(in value.CylinderHead);
			writer.WritePackable(in value.IntakeManifold);
			writer.WritePackable(in value.AirFilter);
			writer.WritePackable(in value.Charger);
			writer.WritePackable(in value.ExhaustHeader);
			writer.WritePackable(in value.ExhaustPipe);
			writer.WritePackable(in value.Crankshaft);
			writer.WritePackable(in value.ConnectionRod);
			writer.WritePackable(in value.Pistons);
			writer.WritePackable(in value.Flywheel);
			writer.WritePackable(in value.Camshaft1);
			writer.WritePackable(in value.Camshaft2);
			writer.WritePackable(in value.FuelRail);
			writer.WritePackable(in value.FuelInjectors);
			writer.WritePackable(in value.ThrottleValve);
			writer.WritePackable(in value.FuelPump);
			writer.WritePackable(in value.Radiator);
			writer.WritePackable(in value.Intercooler);
			writer.WritePackable(in value.Clutch);
			writer.WritePackable(in value.RotorHousing);
			writer.WritePackable(in value.Rotors);
			writer.WritePackable(in value.RotorTrackingGear);
			writer.WritePackable(in value.EccentricShaft);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EngineKitData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			PartData value2;
			PartData value3;
			PartData value4;
			PartData value5;
			PartData value6;
			PartData value7;
			PartData value8;
			PartData value9;
			PartData value10;
			PartData value11;
			PartData value12;
			PartData value13;
			PartData value14;
			PartData value15;
			PartData value16;
			PartData value17;
			PartData value18;
			PartData value19;
			PartData value20;
			PartData value21;
			PartData value22;
			PartData value23;
			PartData value24;
			PartData value25;
			PartData value26;
			if (memberCount == 25)
			{
				if (value != null)
				{
					value2 = value.EngineId;
					value3 = value.Block;
					value4 = value.CylinderHead;
					value5 = value.IntakeManifold;
					value6 = value.AirFilter;
					value7 = value.Charger;
					value8 = value.ExhaustHeader;
					value9 = value.ExhaustPipe;
					value10 = value.Crankshaft;
					value11 = value.ConnectionRod;
					value12 = value.Pistons;
					value13 = value.Flywheel;
					value14 = value.Camshaft1;
					value15 = value.Camshaft2;
					value16 = value.FuelRail;
					value17 = value.FuelInjectors;
					value18 = value.ThrottleValve;
					value19 = value.FuelPump;
					value20 = value.Radiator;
					value21 = value.Intercooler;
					value22 = value.Clutch;
					value23 = value.RotorHousing;
					value24 = value.Rotors;
					value25 = value.RotorTrackingGear;
					value26 = value.EccentricShaft;
					reader.ReadPackable(ref value2);
					reader.ReadPackable(ref value3);
					reader.ReadPackable(ref value4);
					reader.ReadPackable(ref value5);
					reader.ReadPackable(ref value6);
					reader.ReadPackable(ref value7);
					reader.ReadPackable(ref value8);
					reader.ReadPackable(ref value9);
					reader.ReadPackable(ref value10);
					reader.ReadPackable(ref value11);
					reader.ReadPackable(ref value12);
					reader.ReadPackable(ref value13);
					reader.ReadPackable(ref value14);
					reader.ReadPackable(ref value15);
					reader.ReadPackable(ref value16);
					reader.ReadPackable(ref value17);
					reader.ReadPackable(ref value18);
					reader.ReadPackable(ref value19);
					reader.ReadPackable(ref value20);
					reader.ReadPackable(ref value21);
					reader.ReadPackable(ref value22);
					reader.ReadPackable(ref value23);
					reader.ReadPackable(ref value24);
					reader.ReadPackable(ref value25);
					reader.ReadPackable(ref value26);
					goto IL_0550;
				}
				value2 = reader.ReadPackable<PartData>();
				value3 = reader.ReadPackable<PartData>();
				value4 = reader.ReadPackable<PartData>();
				value5 = reader.ReadPackable<PartData>();
				value6 = reader.ReadPackable<PartData>();
				value7 = reader.ReadPackable<PartData>();
				value8 = reader.ReadPackable<PartData>();
				value9 = reader.ReadPackable<PartData>();
				value10 = reader.ReadPackable<PartData>();
				value11 = reader.ReadPackable<PartData>();
				value12 = reader.ReadPackable<PartData>();
				value13 = reader.ReadPackable<PartData>();
				value14 = reader.ReadPackable<PartData>();
				value15 = reader.ReadPackable<PartData>();
				value16 = reader.ReadPackable<PartData>();
				value17 = reader.ReadPackable<PartData>();
				value18 = reader.ReadPackable<PartData>();
				value19 = reader.ReadPackable<PartData>();
				value20 = reader.ReadPackable<PartData>();
				value21 = reader.ReadPackable<PartData>();
				value22 = reader.ReadPackable<PartData>();
				value23 = reader.ReadPackable<PartData>();
				value24 = reader.ReadPackable<PartData>();
				value25 = reader.ReadPackable<PartData>();
				value26 = reader.ReadPackable<PartData>();
			}
			else
			{
				if (memberCount > 25)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EngineKitData), 25, memberCount);
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
				}
				else
				{
					value2 = value.EngineId;
					value3 = value.Block;
					value4 = value.CylinderHead;
					value5 = value.IntakeManifold;
					value6 = value.AirFilter;
					value7 = value.Charger;
					value8 = value.ExhaustHeader;
					value9 = value.ExhaustPipe;
					value10 = value.Crankshaft;
					value11 = value.ConnectionRod;
					value12 = value.Pistons;
					value13 = value.Flywheel;
					value14 = value.Camshaft1;
					value15 = value.Camshaft2;
					value16 = value.FuelRail;
					value17 = value.FuelInjectors;
					value18 = value.ThrottleValve;
					value19 = value.FuelPump;
					value20 = value.Radiator;
					value21 = value.Intercooler;
					value22 = value.Clutch;
					value23 = value.RotorHousing;
					value24 = value.Rotors;
					value25 = value.RotorTrackingGear;
					value26 = value.EccentricShaft;
				}
				if (memberCount != 0)
				{
					reader.ReadPackable(ref value2);
					if (memberCount != 1)
					{
						reader.ReadPackable(ref value3);
						if (memberCount != 2)
						{
							reader.ReadPackable(ref value4);
							if (memberCount != 3)
							{
								reader.ReadPackable(ref value5);
								if (memberCount != 4)
								{
									reader.ReadPackable(ref value6);
									if (memberCount != 5)
									{
										reader.ReadPackable(ref value7);
										if (memberCount != 6)
										{
											reader.ReadPackable(ref value8);
											if (memberCount != 7)
											{
												reader.ReadPackable(ref value9);
												if (memberCount != 8)
												{
													reader.ReadPackable(ref value10);
													if (memberCount != 9)
													{
														reader.ReadPackable(ref value11);
														if (memberCount != 10)
														{
															reader.ReadPackable(ref value12);
															if (memberCount != 11)
															{
																reader.ReadPackable(ref value13);
																if (memberCount != 12)
																{
																	reader.ReadPackable(ref value14);
																	if (memberCount != 13)
																	{
																		reader.ReadPackable(ref value15);
																		if (memberCount != 14)
																		{
																			reader.ReadPackable(ref value16);
																			if (memberCount != 15)
																			{
																				reader.ReadPackable(ref value17);
																				if (memberCount != 16)
																				{
																					reader.ReadPackable(ref value18);
																					if (memberCount != 17)
																					{
																						reader.ReadPackable(ref value19);
																						if (memberCount != 18)
																						{
																							reader.ReadPackable(ref value20);
																							if (memberCount != 19)
																							{
																								reader.ReadPackable(ref value21);
																								if (memberCount != 20)
																								{
																									reader.ReadPackable(ref value22);
																									if (memberCount != 21)
																									{
																										reader.ReadPackable(ref value23);
																										if (memberCount != 22)
																										{
																											reader.ReadPackable(ref value24);
																											if (memberCount != 23)
																											{
																												reader.ReadPackable(ref value25);
																												if (memberCount != 24)
																												{
																													reader.ReadPackable(ref value26);
																													_ = 25;
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
				if (value != null)
				{
					goto IL_0550;
				}
			}
			value = new EngineKitData
			{
				EngineId = value2,
				Block = value3,
				CylinderHead = value4,
				IntakeManifold = value5,
				AirFilter = value6,
				Charger = value7,
				ExhaustHeader = value8,
				ExhaustPipe = value9,
				Crankshaft = value10,
				ConnectionRod = value11,
				Pistons = value12,
				Flywheel = value13,
				Camshaft1 = value14,
				Camshaft2 = value15,
				FuelRail = value16,
				FuelInjectors = value17,
				ThrottleValve = value18,
				FuelPump = value19,
				Radiator = value20,
				Intercooler = value21,
				Clutch = value22,
				RotorHousing = value23,
				Rotors = value24,
				RotorTrackingGear = value25,
				EccentricShaft = value26
			};
			return;
			IL_0550:
			value.EngineId = value2;
			value.Block = value3;
			value.CylinderHead = value4;
			value.IntakeManifold = value5;
			value.AirFilter = value6;
			value.Charger = value7;
			value.ExhaustHeader = value8;
			value.ExhaustPipe = value9;
			value.Crankshaft = value10;
			value.ConnectionRod = value11;
			value.Pistons = value12;
			value.Flywheel = value13;
			value.Camshaft1 = value14;
			value.Camshaft2 = value15;
			value.FuelRail = value16;
			value.FuelInjectors = value17;
			value.ThrottleValve = value18;
			value.FuelPump = value19;
			value.Radiator = value20;
			value.Intercooler = value21;
			value.Clutch = value22;
			value.RotorHousing = value23;
			value.Rotors = value24;
			value.RotorTrackingGear = value25;
			value.EccentricShaft = value26;
		}
	}
}
