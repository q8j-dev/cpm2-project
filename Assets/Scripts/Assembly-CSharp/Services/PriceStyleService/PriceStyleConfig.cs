using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace Services.PriceStyleService
{
	[Preserve]
	[CreateAssetMenu(fileName = "PriceStyleConfig", menuName = "Scriptable/PriceStyleConfig")]
	public class PriceStyleConfig : ScriptableObject
	{
		[SerializeField]
		private List<PriceStyleData> _priceStyleData = default;

		[Preserve]
		public PriceStyleConfig()
		{
		}

		public PriceStyleData GetByType(CostType costType)
		{
			return _priceStyleData.First((PriceStyleData x) => x.CostType == costType);
		}
	}
}
