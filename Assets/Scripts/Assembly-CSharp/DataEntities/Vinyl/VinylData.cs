#nullable enable annotations
using System;
using System.Buffers;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts;

namespace DataEntities.Vinyl
{
	[Serializable]
	[UnityEngine.Scripting.Preserve]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	public class VinylData : IMemoryPackable<VinylData>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class VinylDataFormatter : MemoryPackFormatter<VinylData>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref VinylData value)
			{
				VinylData.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref VinylData value)
			{
				VinylData.Deserialize(ref reader, ref value);
			}
		}

		[Vector3Formatter.MemoryPackCustomFormatterAttributeVector3]
		public Vector3 position;

		[Vector3Formatter.MemoryPackCustomFormatterAttributeVector3]
		public Vector3 scaleRotation;

		[Vector3Formatter.MemoryPackCustomFormatterAttributeVector3]
		public Vector3 iconPosition;

		public string text = "";

		public uint color;

		public long packedData;

		private static readonly IMemoryPackFormatter<Vector3> __positionFormatter;

		private static readonly IMemoryPackFormatter<Vector3> __scaleRotationFormatter;

		private static readonly IMemoryPackFormatter<Vector3> __iconPositionFormatter;

		static VinylData()
		{
			__positionFormatter = typeof(VinylData).GetField("position", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<Vector3Formatter.MemoryPackCustomFormatterAttributeVector3>().GetFormatter();
			__scaleRotationFormatter = typeof(VinylData).GetField("scaleRotation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<Vector3Formatter.MemoryPackCustomFormatterAttributeVector3>().GetFormatter();
			__iconPositionFormatter = typeof(VinylData).GetField("iconPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<Vector3Formatter.MemoryPackCustomFormatterAttributeVector3>().GetFormatter();
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<VinylData>())
			{
				MemoryPackFormatterProvider.Register(new VinylDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<VinylData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<VinylData>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref VinylData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(6);
			writer.WriteValueWithFormatter(__positionFormatter, in value.position);
			writer.WriteValueWithFormatter(__scaleRotationFormatter, in value.scaleRotation);
			writer.WriteValueWithFormatter(__iconPositionFormatter, in value.iconPosition);
			writer.WriteString(value.text);
			writer.WriteUnmanaged(in value.color, in value.packedData);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref VinylData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Vector3 value2;
			Vector3 value3;
			Vector3 value4;
			uint value5;
			long value6;
			string text;
			if (memberCount == 6)
			{
				if (value != null)
				{
					value2 = value.position;
					value3 = value.scaleRotation;
					value4 = value.iconPosition;
					text = value.text;
					value5 = value.color;
					value6 = value.packedData;
					reader.ReadValueWithFormatter(__positionFormatter, ref value2);
					reader.ReadValueWithFormatter(__scaleRotationFormatter, ref value3);
					reader.ReadValueWithFormatter(__iconPositionFormatter, ref value4);
					text = reader.ReadString();
					reader.ReadUnmanaged<uint>(out value5);
					reader.ReadUnmanaged<long>(out value6);
					goto IL_019a;
				}
				value2 = reader.ReadValueWithFormatter<IMemoryPackFormatter<Vector3>, Vector3>(__positionFormatter);
				value3 = reader.ReadValueWithFormatter<IMemoryPackFormatter<Vector3>, Vector3>(__scaleRotationFormatter);
				value4 = reader.ReadValueWithFormatter<IMemoryPackFormatter<Vector3>, Vector3>(__iconPositionFormatter);
				text = reader.ReadString();
				reader.ReadUnmanaged<uint, long>(out value5, out value6);
			}
			else
			{
				if (memberCount > 6)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(VinylData), 6, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = default;
					value3 = default;
					value4 = default;
					text = null;
					value5 = 0u;
					value6 = 0L;
				}
				else
				{
					value2 = value.position;
					value3 = value.scaleRotation;
					value4 = value.iconPosition;
					text = value.text;
					value5 = value.color;
					value6 = value.packedData;
				}
				if (memberCount != 0)
				{
					reader.ReadValueWithFormatter(__positionFormatter, ref value2);
					if (memberCount != 1)
					{
						reader.ReadValueWithFormatter(__scaleRotationFormatter, ref value3);
						if (memberCount != 2)
						{
							reader.ReadValueWithFormatter(__iconPositionFormatter, ref value4);
							if (memberCount != 3)
							{
								text = reader.ReadString();
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<uint>(out value5);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<long>(out value6);
										_ = 6;
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_019a;
				}
			}
			value = new VinylData
			{
				position = value2,
				scaleRotation = value3,
				iconPosition = value4,
				text = text,
				color = value5,
				packedData = value6
			};
			return;
			IL_019a:
			value.position = value2;
			value.scaleRotation = value3;
			value.iconPosition = value4;
			value.text = text;
			value.color = value5;
			value.packedData = value6;
		}
	}
}
