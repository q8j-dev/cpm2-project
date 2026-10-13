using System;
using Unity.Sentis;
using UnityEngine;

namespace _Car_Parking.Scripts.VinylsAi
{
	public class PreWarmAI
	{
		private static bool _wasSentisPrewarmed;

		public static void TryPreWarm()
		{
			try
			{
				if (!_wasSentisPrewarmed)
				{
					_wasSentisPrewarmed = true;
					TextureConverter.ToTensor(Texture2D.whiteTexture)?.Dispose();
				}
			}
			catch (Exception arg)
			{
				Debug.LogError($"PreWarmAI error: {arg}");
			}
		}
	}
}
