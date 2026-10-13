using System;
using System.Buffers;
using CPMEngine;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

[Serializable]
[MemoryPackable(GenerateType.Object)]
[GenerateTypeScript]
public class DynamicTuningSettings : IMemoryPackable<DynamicTuningSettings>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class DynamicTuningSettingsFormatter : MemoryPackFormatter<DynamicTuningSettings>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DynamicTuningSettings value)
		{
			DynamicTuningSettings.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DynamicTuningSettings value)
		{
			DynamicTuningSettings.Deserialize(ref reader, ref value);
		}
	}

	[Header("Engine")]
	public FuelType FuelType = FuelType.None;

	public float Boost = -1f;

	public float IdleRpm = -1f;

	public float RedlineRpm = -1f;

	public float MixtureRatio = -1f;

	[Header("Gearbox")]
	public float LaunchTime = -1f;

	public float LaunchRpm = -1f;

	public float TorqueSplit = -1f;

	public float[] Gears = new float[14]
	{
		-1f, -1f, -1f, -1f, -1f, -1f, -1f, -1f, -1f, -1f,
		-1f, -1f, -1f, -1f
	};

	public float FinalDrive = -1f;

	[Header("Front Differential")]
	public float FrontAccelBias = -1f;

	public float FrontBrakeBias = -1f;

	public float FrontMaxBias = -1f;

	public float FrontTorqueBiasRatio = -1f;

	public float FrontLsdLock = -1f;

	[Header("Rear Differential")]
	public float RearAccelBias = -1f;

	public float RearBrakeBias = -1f;

	public float RearMaxBias = -1f;

	public float RearTorqueBiasRatio = -1f;

	public float RearLsdLock = -1f;

	[Header("Steering")]
	public int MaxSteeringAngle = 33;

	[Header("Suspension")]
	public float MinFrontSuspensionHeight = -1f;

	public float MaxFrontSuspensionHeight = -1f;

	public float MinRearSuspensionHeight = -1f;

	public float MaxRearSuspensionHeight = -1f;

	public float HeightAdjustSpeed = -1f;

	public float CurrentFrontSuspensionHeight = -1f;

	public float CurrentRearSuspensionHeight = -1f;

	static DynamicTuningSettings()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DynamicTuningSettings>())
		{
			MemoryPackFormatterProvider.Register(new DynamicTuningSettingsFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DynamicTuningSettings[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<DynamicTuningSettings>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<FuelType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<FuelType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<float[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<float>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DynamicTuningSettings? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader(28, in value.FuelType, in value.Boost, in value.IdleRpm, in value.RedlineRpm, in value.MixtureRatio, in value.LaunchTime, in value.LaunchRpm, in value.TorqueSplit);
		writer.WriteUnmanagedArray(value.Gears);
		writer.WriteUnmanaged(in value.FinalDrive, in value.FrontAccelBias, in value.FrontBrakeBias, in value.FrontMaxBias, in value.FrontTorqueBiasRatio, in value.FrontLsdLock, in value.RearAccelBias, in value.RearBrakeBias, in value.RearMaxBias, in value.RearTorqueBiasRatio, in value.RearLsdLock, in value.MaxSteeringAngle, in value.MinFrontSuspensionHeight, in value.MaxFrontSuspensionHeight, in value.MinRearSuspensionHeight);
		writer.WriteUnmanaged(in value.MaxRearSuspensionHeight, in value.HeightAdjustSpeed, in value.CurrentFrontSuspensionHeight, in value.CurrentRearSuspensionHeight);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DynamicTuningSettings? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		FuelType value2;
		float value3;
		float value4;
		float value5;
		float value6;
		float value7;
		float value8;
		float value9;
		float[] value10;
		float value11;
		float value12;
		float value13;
		float value14;
		float value15;
		float value16;
		float value17;
		float value18;
		float value19;
		float value20;
		float value21;
		int value22;
		float value23;
		float value24;
		float value25;
		float value26;
		float value27;
		float value28;
		float value29;
		if (memberCount == 28)
		{
			if (value != null)
			{
				value2 = value.FuelType;
				value3 = value.Boost;
				value4 = value.IdleRpm;
				value5 = value.RedlineRpm;
				value6 = value.MixtureRatio;
				value7 = value.LaunchTime;
				value8 = value.LaunchRpm;
				value9 = value.TorqueSplit;
				value10 = value.Gears;
				value11 = value.FinalDrive;
				value12 = value.FrontAccelBias;
				value13 = value.FrontBrakeBias;
				value14 = value.FrontMaxBias;
				value15 = value.FrontTorqueBiasRatio;
				value16 = value.FrontLsdLock;
				value17 = value.RearAccelBias;
				value18 = value.RearBrakeBias;
				value19 = value.RearMaxBias;
				value20 = value.RearTorqueBiasRatio;
				value21 = value.RearLsdLock;
				value22 = value.MaxSteeringAngle;
				value23 = value.MinFrontSuspensionHeight;
				value24 = value.MaxFrontSuspensionHeight;
				value25 = value.MinRearSuspensionHeight;
				value26 = value.MaxRearSuspensionHeight;
				value27 = value.HeightAdjustSpeed;
				value28 = value.CurrentFrontSuspensionHeight;
				value29 = value.CurrentRearSuspensionHeight;
				reader.ReadUnmanaged<FuelType>(out value2);
				reader.ReadUnmanaged<float>(out value3);
				reader.ReadUnmanaged<float>(out value4);
				reader.ReadUnmanaged<float>(out value5);
				reader.ReadUnmanaged<float>(out value6);
				reader.ReadUnmanaged<float>(out value7);
				reader.ReadUnmanaged<float>(out value8);
				reader.ReadUnmanaged<float>(out value9);
				reader.ReadUnmanagedArray(ref value10);
				reader.ReadUnmanaged<float>(out value11);
				reader.ReadUnmanaged<float>(out value12);
				reader.ReadUnmanaged<float>(out value13);
				reader.ReadUnmanaged<float>(out value14);
				reader.ReadUnmanaged<float>(out value15);
				reader.ReadUnmanaged<float>(out value16);
				reader.ReadUnmanaged<float>(out value17);
				reader.ReadUnmanaged<float>(out value18);
				reader.ReadUnmanaged<float>(out value19);
				reader.ReadUnmanaged<float>(out value20);
				reader.ReadUnmanaged<float>(out value21);
				reader.ReadUnmanaged<int>(out value22);
				reader.ReadUnmanaged<float>(out value23);
				reader.ReadUnmanaged<float>(out value24);
				reader.ReadUnmanaged<float>(out value25);
				reader.ReadUnmanaged<float>(out value26);
				reader.ReadUnmanaged<float>(out value27);
				reader.ReadUnmanaged<float>(out value28);
				reader.ReadUnmanaged<float>(out value29);
				goto IL_05c6;
			}
			reader.ReadUnmanaged<FuelType, float, float, float, float, float, float, float>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9);
			value10 = reader.ReadUnmanagedArray<float>();
			reader.ReadUnmanaged<float, float, float, float, float, float, float, float, float, float, float, int, float, float, float>(out value11, out value12, out value13, out value14, out value15, out value16, out value17, out value18, out value19, out value20, out value21, out value22, out value23, out value24, out value25);
			reader.ReadUnmanaged<float, float, float, float>(out value26, out value27, out value28, out value29);
		}
		else
		{
			if (memberCount > 28)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DynamicTuningSettings), 28, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = FuelType.Gas95;
				value3 = 0f;
				value4 = 0f;
				value5 = 0f;
				value6 = 0f;
				value7 = 0f;
				value8 = 0f;
				value9 = 0f;
				value10 = null;
				value11 = 0f;
				value12 = 0f;
				value13 = 0f;
				value14 = 0f;
				value15 = 0f;
				value16 = 0f;
				value17 = 0f;
				value18 = 0f;
				value19 = 0f;
				value20 = 0f;
				value21 = 0f;
				value22 = 0;
				value23 = 0f;
				value24 = 0f;
				value25 = 0f;
				value26 = 0f;
				value27 = 0f;
				value28 = 0f;
				value29 = 0f;
			}
			else
			{
				value2 = value.FuelType;
				value3 = value.Boost;
				value4 = value.IdleRpm;
				value5 = value.RedlineRpm;
				value6 = value.MixtureRatio;
				value7 = value.LaunchTime;
				value8 = value.LaunchRpm;
				value9 = value.TorqueSplit;
				value10 = value.Gears;
				value11 = value.FinalDrive;
				value12 = value.FrontAccelBias;
				value13 = value.FrontBrakeBias;
				value14 = value.FrontMaxBias;
				value15 = value.FrontTorqueBiasRatio;
				value16 = value.FrontLsdLock;
				value17 = value.RearAccelBias;
				value18 = value.RearBrakeBias;
				value19 = value.RearMaxBias;
				value20 = value.RearTorqueBiasRatio;
				value21 = value.RearLsdLock;
				value22 = value.MaxSteeringAngle;
				value23 = value.MinFrontSuspensionHeight;
				value24 = value.MaxFrontSuspensionHeight;
				value25 = value.MinRearSuspensionHeight;
				value26 = value.MaxRearSuspensionHeight;
				value27 = value.HeightAdjustSpeed;
				value28 = value.CurrentFrontSuspensionHeight;
				value29 = value.CurrentRearSuspensionHeight;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<FuelType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<float>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<float>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<float>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<float>(out value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<float>(out value7);
									if (memberCount != 6)
									{
										reader.ReadUnmanaged<float>(out value8);
										if (memberCount != 7)
										{
											reader.ReadUnmanaged<float>(out value9);
											if (memberCount != 8)
											{
												reader.ReadUnmanagedArray(ref value10);
												if (memberCount != 9)
												{
													reader.ReadUnmanaged<float>(out value11);
													if (memberCount != 10)
													{
														reader.ReadUnmanaged<float>(out value12);
														if (memberCount != 11)
														{
															reader.ReadUnmanaged<float>(out value13);
															if (memberCount != 12)
															{
																reader.ReadUnmanaged<float>(out value14);
																if (memberCount != 13)
																{
																	reader.ReadUnmanaged<float>(out value15);
																	if (memberCount != 14)
																	{
																		reader.ReadUnmanaged<float>(out value16);
																		if (memberCount != 15)
																		{
																			reader.ReadUnmanaged<float>(out value17);
																			if (memberCount != 16)
																			{
																				reader.ReadUnmanaged<float>(out value18);
																				if (memberCount != 17)
																				{
																					reader.ReadUnmanaged<float>(out value19);
																					if (memberCount != 18)
																					{
																						reader.ReadUnmanaged<float>(out value20);
																						if (memberCount != 19)
																						{
																							reader.ReadUnmanaged<float>(out value21);
																							if (memberCount != 20)
																							{
																								reader.ReadUnmanaged<int>(out value22);
																								if (memberCount != 21)
																								{
																									reader.ReadUnmanaged<float>(out value23);
																									if (memberCount != 22)
																									{
																										reader.ReadUnmanaged<float>(out value24);
																										if (memberCount != 23)
																										{
																											reader.ReadUnmanaged<float>(out value25);
																											if (memberCount != 24)
																											{
																												reader.ReadUnmanaged<float>(out value26);
																												if (memberCount != 25)
																												{
																													reader.ReadUnmanaged<float>(out value27);
																													if (memberCount != 26)
																													{
																														reader.ReadUnmanaged<float>(out value28);
																														if (memberCount != 27)
																														{
																															reader.ReadUnmanaged<float>(out value29);
																															_ = 28;
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
			if (value != null)
			{
				goto IL_05c6;
			}
		}
		value = new DynamicTuningSettings
		{
			FuelType = value2,
			Boost = value3,
			IdleRpm = value4,
			RedlineRpm = value5,
			MixtureRatio = value6,
			LaunchTime = value7,
			LaunchRpm = value8,
			TorqueSplit = value9,
			Gears = value10,
			FinalDrive = value11,
			FrontAccelBias = value12,
			FrontBrakeBias = value13,
			FrontMaxBias = value14,
			FrontTorqueBiasRatio = value15,
			FrontLsdLock = value16,
			RearAccelBias = value17,
			RearBrakeBias = value18,
			RearMaxBias = value19,
			RearTorqueBiasRatio = value20,
			RearLsdLock = value21,
			MaxSteeringAngle = value22,
			MinFrontSuspensionHeight = value23,
			MaxFrontSuspensionHeight = value24,
			MinRearSuspensionHeight = value25,
			MaxRearSuspensionHeight = value26,
			HeightAdjustSpeed = value27,
			CurrentFrontSuspensionHeight = value28,
			CurrentRearSuspensionHeight = value29
		};
		return;
		IL_05c6:
		value.FuelType = value2;
		value.Boost = value3;
		value.IdleRpm = value4;
		value.RedlineRpm = value5;
		value.MixtureRatio = value6;
		value.LaunchTime = value7;
		value.LaunchRpm = value8;
		value.TorqueSplit = value9;
		value.Gears = value10;
		value.FinalDrive = value11;
		value.FrontAccelBias = value12;
		value.FrontBrakeBias = value13;
		value.FrontMaxBias = value14;
		value.FrontTorqueBiasRatio = value15;
		value.FrontLsdLock = value16;
		value.RearAccelBias = value17;
		value.RearBrakeBias = value18;
		value.RearMaxBias = value19;
		value.RearTorqueBiasRatio = value20;
		value.RearLsdLock = value21;
		value.MaxSteeringAngle = value22;
		value.MinFrontSuspensionHeight = value23;
		value.MaxFrontSuspensionHeight = value24;
		value.MinRearSuspensionHeight = value25;
		value.MaxRearSuspensionHeight = value26;
		value.HeightAdjustSpeed = value27;
		value.CurrentFrontSuspensionHeight = value28;
		value.CurrentRearSuspensionHeight = value29;
	}
}
