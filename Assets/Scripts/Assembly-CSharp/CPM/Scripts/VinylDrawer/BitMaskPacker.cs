using System;

namespace CPM.Scripts.VinylDrawer
{
	public static class BitMaskPacker
	{
		public enum PackedDataMask : long
		{
			IsTextMask = 1L,
			ItalicMask = 2L,
			BoldMask = 4L,
			FlipXMask = 8L,
			FontIndexMask = 496L,
			StickerIndexMask = 261632L,
			XAngleMask = 133955584L,
			YAngleMask = 68585259008L
		}

		private const int DetailsShortDataCount = 7;

		private const int DetailStateMaxValue = 3;

		public static int PackTuningData(int[] values)
		{
			int num = 0;
			for (int i = 0; i < 7; i++)
			{
				if (values[i] < 0 || values[i] > 3)
				{
					throw new ArgumentException("Each value must be in the range [0, 3].");
				}
				num |= values[i] << i * 2;
			}
			return num;
		}

		public static int[] UnpackTuningData(int packedValue)
		{
			int[] array = new int[7];
			for (int i = 0; i < 7; i++)
			{
				array[i] = (packedValue >> i * 2) & 3;
			}
			return array;
		}

		public static long Pack(bool isText, bool italic, bool bold, bool flipX, int fontIndex, int stickerIndex, int xAngle, int yAngle)
		{
			return (long)(0uL | (ulong)(isText ? 1 : 0) | (ulong)(italic ? 2 : 0) | (ulong)(bold ? 4 : 0) | (ulong)(flipX ? 8 : 0) | (((ulong)fontIndex & 0x1FuL) << 4) | (((ulong)stickerIndex & 0x1FFuL) << 9) | (((ulong)xAngle & 0x1FFuL) << 18) | (((ulong)yAngle & 0x1FFuL) << 27));
		}

		public static void Unpack(long bitmask, out bool isText, out bool italic, out bool bold, out bool flipX, out int fontIndex, out int stickerIndex, out int xAngle, out int yAngle)
		{
			isText = (bitmask & 1) != 0;
			italic = (bitmask & 2) != 0;
			bold = (bitmask & 4) != 0;
			flipX = (bitmask & 8) != 0;
			fontIndex = (int)((bitmask & 0x1F0) >> 4);
			stickerIndex = (int)((bitmask & 0x3FE00) >> 9);
			xAngle = (int)((bitmask & 0x7FC0000) >> 18);
			yAngle = (int)((bitmask & 0xFF8000000L) >> 27);
		}

		public static void GetValueFromMask(long bitmask, PackedDataMask mask, out int number, out bool check)
		{
			switch (mask)
			{
			case PackedDataMask.IsTextMask:
			case PackedDataMask.ItalicMask:
			case PackedDataMask.BoldMask:
			case PackedDataMask.FlipXMask:
				number = 0;
				check = ((ulong)bitmask & (ulong)mask) != 0;
				break;
			case PackedDataMask.FontIndexMask:
				number = (int)((bitmask & 0x1F0) >> 4);
				check = false;
				break;
			case PackedDataMask.StickerIndexMask:
				number = (int)((bitmask & 0x3FE00) >> 9);
				check = false;
				break;
			case PackedDataMask.XAngleMask:
				number = (int)((bitmask & 0x7FC0000) >> 18);
				check = false;
				break;
			case PackedDataMask.YAngleMask:
				number = (int)((bitmask & 0xFF8000000L) >> 27);
				check = false;
				break;
			default:
				throw new ArgumentOutOfRangeException("mask", mask, null);
			}
		}
	}
}
