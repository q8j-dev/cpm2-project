using System.IO;
using System.IO.Compression;
using MemoryPack;

namespace CPM.Ghost.Core
{
	public static class GhostSerializer
	{
		public static byte[] Serialize(GhostRecording rec)
		{
			return Compress(MemoryPackSerializer.Serialize(in rec));
		}

		public static GhostRecording Deserialize(byte[] packed)
		{
			return MemoryPackSerializer.Deserialize<GhostRecording>(Decompress(packed));
		}

		private static byte[] Compress(byte[] raw)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using (BrotliStream brotliStream = new BrotliStream(memoryStream, CompressionLevel.Optimal))
			{
				brotliStream.Write(raw, 0, raw.Length);
			}
			return memoryStream.ToArray();
		}

		private static byte[] Decompress(byte[] packed)
		{
			using MemoryStream stream = new MemoryStream(packed);
			using BrotliStream brotliStream = new BrotliStream(stream, CompressionMode.Decompress);
			using MemoryStream memoryStream = new MemoryStream();
			brotliStream.CopyTo(memoryStream);
			return memoryStream.ToArray();
		}
	}
}
