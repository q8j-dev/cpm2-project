using System;

namespace UnityEngine.Purchasing.Security
{
	public class GooglePlayTangle
	{
		private static byte[] data = Convert.FromBase64String("9N8/Iqa3aQ6SEGGrFXiw5jrcP8qKftBwhd2jQDVOwhRw7bN5FlpstdVWWFdn1VZdVdVWVlfGhLzTghDGy+O7PoiMxB/udxzE/5poSoYdnH9DmXRF3d9zlnx0L5uuptcHNZrfzywdch9kzBd5nzZisC2QA4r+03VUZ9VWdWdaUV590R/RoFpWVlZSV1Q5hLyXWYYMS4Jhh5ubbfHGjC56iqCeNqukcvMqlI+RfVEOuZc4Gb6To2piNBf7Fw71ZztdXCMVEwWk/TIo2XeX/kKUyC2sgO35jqJOnHHbls52KNoApCVt90Uf4U7r3Q3jtmGe12ZL/gUaWQ0k7UYNYVQO+NPN/jpZXFDs6eMyt68V1f4kytyp06F3Mg4FW9Jv6rHqylVUVldW");

		private static int[] order = new int[15]
		{
			2, 7, 7, 5, 12, 13, 7, 10, 9, 10,
			10, 13, 12, 13, 14
		};

		private static int key = 87;

		public static readonly bool IsPopulated = true;

		public static byte[] Data()
		{
			if (!IsPopulated)
			{
				return null;
			}
			return Obfuscator.DeObfuscate(data, order, key);
		}
	}
}
