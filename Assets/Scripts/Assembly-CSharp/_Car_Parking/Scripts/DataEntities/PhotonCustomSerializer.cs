using System;
using System.Collections.Generic;
using CPM.Database;
using CPMEngine;
using ExitGames.Client.Photon;
using UnityEngine;

namespace _Car_Parking.Scripts.DataEntities
{
	public class PhotonCustomSerializer
	{
		private const int ColorValueBytes = 255;

		public static readonly int TwoFloatsSize = 8;

		public static readonly int ThreeByteSize = 3;

		public static readonly int FourByteSize = 4;

		public static readonly List<Type> SerializedTypes = new List<Type>
		{
			typeof(TorqueCurveSample),
			typeof(TorqueCurveSample[]),
			typeof(Color),
			typeof(Color[]),
			typeof(ColorSerializableWithAlpha),
			typeof(Color)
		};

		private static short SerializeTorqueCurveSample(StreamBuffer outStream, object customObject)
		{
			TorqueCurveSample obj = (TorqueCurveSample)customObject;
			byte[] array = new byte[TwoFloatsSize];
			int targetOffset = 0;
			Protocol.Serialize(obj.Rpm, array, ref targetOffset);
			Protocol.Serialize(obj.Torque, array, ref targetOffset);
			outStream.Write(array, 0, TwoFloatsSize);
			return (short)TwoFloatsSize;
		}

		private static object DeserializeTorqueCurveSample(StreamBuffer inStream, short length)
		{
			TorqueCurveSample torqueCurveSample = default;
			byte[] array = new byte[TwoFloatsSize];
			inStream.Read(array, 0, TwoFloatsSize);
			int offset = 0;
			Protocol.Deserialize(out torqueCurveSample.Rpm, array, ref offset);
			Protocol.Deserialize(out torqueCurveSample.Torque, array, ref offset);
			return torqueCurveSample;
		}

		private static void Serialize(byte value, byte[] target, ref int targetOffset)
		{
			target[targetOffset++] = value;
		}

		private static void Deserialize(out byte value, byte[] source, ref int offset)
		{
			value = source[offset++];
		}

		private static short SerializeColor(StreamBuffer outStream, object customObject)
		{
			Color obj = (Color)customObject;
			byte[] array = new byte[ThreeByteSize];
			int targetOffset = 0;
			byte value = (byte)(obj.r * 255f);
			byte value2 = (byte)(obj.g * 255f);
			byte value3 = (byte)(obj.b * 255f);
			Serialize(value, array, ref targetOffset);
			Serialize(value2, array, ref targetOffset);
			Serialize(value3, array, ref targetOffset);
			outStream.Write(array, 0, ThreeByteSize);
			return (short)ThreeByteSize;
		}

		private static object DeserializeColor(StreamBuffer inStream, short length)
		{
			Color color = default;
			byte[] array = new byte[ThreeByteSize];
			inStream.Read(array, 0, ThreeByteSize);
			int offset = 0;
			Deserialize(out var value, array, ref offset);
			Deserialize(out var value2, array, ref offset);
			Deserialize(out var value3, array, ref offset);
			color.r = (float)(int)value / 255f;
			color.g = (float)(int)value2 / 255f;
			color.b = (float)(int)value3 / 255f;
			return color;
		}

		private static short SerializeColorSerializableWithAlpha(StreamBuffer outStream, object customObject)
		{
			ColorSerializableWithAlpha obj = (ColorSerializableWithAlpha)customObject;
			byte[] array = new byte[FourByteSize];
			int targetOffset = 0;
			Serialize(obj.R, array, ref targetOffset);
			Serialize(obj.G, array, ref targetOffset);
			Serialize(obj.B, array, ref targetOffset);
			Serialize(obj.A, array, ref targetOffset);
			outStream.Write(array, 0, FourByteSize);
			return (short)FourByteSize;
		}

		private static object DeserializeColorSerializableWithAlpha(StreamBuffer inStream, short length)
		{
			ColorSerializableWithAlpha colorSerializableWithAlpha = default;
			byte[] array = new byte[FourByteSize];
			inStream.Read(array, 0, FourByteSize);
			int offset = 0;
			Deserialize(out colorSerializableWithAlpha.R, array, ref offset);
			Deserialize(out colorSerializableWithAlpha.G, array, ref offset);
			Deserialize(out colorSerializableWithAlpha.B, array, ref offset);
			Deserialize(out colorSerializableWithAlpha.A, array, ref offset);
			return colorSerializableWithAlpha;
		}

		public static void Init()
		{
			PhotonPeer.RegisterType(typeof(TorqueCurveSample), 65, SerializeTorqueCurveSample, DeserializeTorqueCurveSample);
			PhotonPeer.RegisterType(typeof(Color), 67, SerializeColor, DeserializeColor);
			PhotonPeer.RegisterType(typeof(ColorSerializableWithAlpha), 68, SerializeColorSerializableWithAlpha, DeserializeColorSerializableWithAlpha);
		}
	}
}
