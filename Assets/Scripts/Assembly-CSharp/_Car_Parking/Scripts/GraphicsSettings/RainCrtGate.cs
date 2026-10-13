using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class RainCrtGate : MonoBehaviour
	{
		[Tooltip("CRT дождя: ScreenRaindrops, Raindrops, RainStreaks, RipplesRT и т.п. (Assets/ThirdPartyPackages/GraphicsRND/Rain/...)")]
		public List<CustomRenderTexture> RainTextures = new List<CustomRenderTexture>();

		[Tooltip("Дождь активен на старте (false = CRT в OnDemand, кадр не тратится)")]
		public bool RainActiveOnStart;

		private bool _initialized;

		private void OnEnable()
		{
			SetRainActive(RainActiveOnStart);
		}

		public void SetRainActive(bool active)
		{
			_initialized = true;
			for (int i = 0; i < RainTextures.Count; i++)
			{
				CustomRenderTexture customRenderTexture = RainTextures[i];
				if (!(customRenderTexture == null))
				{
					customRenderTexture.updateMode = (active ? CustomRenderTextureUpdateMode.Realtime : CustomRenderTextureUpdateMode.OnDemand);
					if (active)
					{
						customRenderTexture.Update();
					}
				}
			}
		}

		public void ForceOff()
		{
			if (_initialized)
			{
				SetRainActive(active: false);
			}
		}
	}
}
