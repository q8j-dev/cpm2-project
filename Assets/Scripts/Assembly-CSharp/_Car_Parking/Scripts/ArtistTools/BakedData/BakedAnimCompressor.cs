using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using _Car_Parking.Scripts.ArtistTools.Enums;
using _Car_Parking.Scripts.Multiplayer.DataPack;

namespace _Car_Parking.Scripts.ArtistTools.BakedData
{
	public static class BakedAnimCompressor
	{
		private static readonly RangedVectorPacker _vectorPacker = new RangedVectorPacker((Precision: 21, x: -80f, y: 80f), (Precision: 21, x: -30f, y: 30f), (Precision: 21, x: -80f, y: 80f));

		private static readonly QuaternionPacker _quaternionPacker = new QuaternionPacker(8, 8, 8, 7);

		private static bool IsReallyLittleEndian => BitConverter.IsLittleEndian;

		[Pure]
		public static async UniTask<List<BakedDirectionPoint>> UnpackAsync(byte[] bin)
		{
			byte[] decompressed = await DecompressBrotliAsync(bin);
			int index = 0;
			List<BakedDirectionPoint> bakedPoints = new List<BakedDirectionPoint>();
			int countForDelay = 0;
			for (int i = 0; i < 24; i++)
			{
				BakedDirectionPoint point = new BakedDirectionPoint();
				byte[] subArray = decompressed[index..(index + 4)];
				point.SpeedType = (BakedAnimSpeedType)FromBytesInLittleEndianToInt32(subArray);
				index += 4;
				byte[] subArray2 = decompressed[index..(index + 4)];
				point.Direction = (HitDirection)FromBytesInLittleEndianToInt32(subArray2);
				index += 4;
				byte[] subArray3 = decompressed[index..(index + 4)];
				int count = FromBytesInLittleEndianToInt32(subArray3);
				index += 4;
				countForDelay++;
				point.BakedPoints = new List<BakedPointData>();
				for (int j = 0; j < count; j++)
				{
					countForDelay++;
					long current = FromBytesInLittleEndianToInt64(decompressed[index..(index + 8)]);
					index += 8;
					int num = FromBytesInLittleEndianToInt32(decompressed[index..(index + 4)]);
					index += 4;
					long current2 = num;
					Vector3 position = _vectorPacker.Unpack(ref current);
					Quaternion rotation = _quaternionPacker.Unpack(ref current2);
					point.BakedPoints.Add(new BakedPointData
					{
						Position = position,
						Rotation = rotation
					});
					if (countForDelay % 5000 == 4999)
					{
						await UniTask.Yield();
					}
				}
				bakedPoints.Add(point);
			}
			return bakedPoints;
		}

		private static long FromBytesInLittleEndianToInt64(byte[] t4)
		{
			return BitConverter.ToInt64(IsReallyLittleEndian ? t4 : t4.Reverse().ToArray());
		}

		private static int FromBytesInLittleEndianToInt32(byte[] p0)
		{
			return BitConverter.ToInt32(IsReallyLittleEndian ? p0 : p0.Reverse().ToArray());
		}

		private static byte[] DecompressBrotli(byte[] compressedData)
		{
			using MemoryStream stream = new MemoryStream(compressedData);
			using BrotliStream brotliStream = new BrotliStream(stream, CompressionMode.Decompress);
			using MemoryStream memoryStream = new MemoryStream();
			brotliStream.CopyTo(memoryStream);
			return memoryStream.ToArray();
		}

		private static async UniTask<byte[]> DecompressBrotliAsync(byte[] compressedData)
		{
			using MemoryStream input = new MemoryStream(compressedData);
			byte[] result;
			await using (BrotliStream brotliStream = new BrotliStream(input, CompressionMode.Decompress))
			{
				using MemoryStream output = new MemoryStream();
				await brotliStream.CopyToAsync(output);
				result = output.ToArray();
			}
			return result;
		}
	}
}
