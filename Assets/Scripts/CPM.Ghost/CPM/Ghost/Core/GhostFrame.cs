using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace CPM.Ghost.Core
{
	[UnityEngine.Scripting.Preserve]
	[MemoryPackable(GenerateType.Object)]
	public struct GhostFrame : IMemoryPackable<GhostFrame>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class GhostFrameFormatter : MemoryPackFormatter<GhostFrame>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GhostFrame value)
			{
				GhostFrame.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref GhostFrame value)
			{
				GhostFrame.Deserialize(ref reader, ref value);
			}
		}

		public float Time;

		public float PosX;

		public float PosY;

		public float PosZ;

		public float RotX;

		public float RotY;

		public float RotZ;

		public float RotW;

		[MemoryPackIgnore]
		public Vector3 Position => new Vector3(PosX, PosY, PosZ);

		[MemoryPackIgnore]
		public Quaternion Rotation => new Quaternion(RotX, RotY, RotZ, RotW);

		public static GhostFrame Create(float time, Vector3 pos, Quaternion rot)
		{
			return new GhostFrame
			{
				Time = time,
				PosX = pos.x,
				PosY = pos.y,
				PosZ = pos.z,
				RotX = rot.x,
				RotY = rot.y,
				RotZ = rot.z,
				RotW = rot.w
			};
		}

		static GhostFrame()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<GhostFrame>())
			{
				MemoryPackFormatterProvider.Register(new GhostFrameFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<GhostFrame[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<GhostFrame>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GhostFrame value) where TBufferWriter : class, IBufferWriter<byte>
		{
			writer.WriteUnmanaged(in value);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref GhostFrame value)
		{
			reader.ReadUnmanaged<GhostFrame>(out value);
		}
	}
}
