using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace _Car_Parking.Scripts.ArtistTools.SO
{
	[CreateAssetMenu(fileName = "VfxModel", menuName = "ScriptableObjects/VfxHandlerModel", order = 0)]
	public class VfxHandlerSO : ScriptableObject
	{
		[SerializeField]
		public List<VfxModel> Models;

		public void Register(string id, ParticleSystem reference)
		{
			if (id.IsNullOrWhitespace() || Models.Any((VfxModel x) => x.ID == id))
			{
				return;
			}
			if (Models.Any((VfxModel x) => x.ReferenceVFX == reference))
			{
				Models.FirstOrDefault((VfxModel x) => x.ReferenceVFX == reference);
				return;
			}
			VfxModel item = new VfxModel
			{
				ID = id,
				ReferenceVFX = reference
			};
			Models.Add(item);
		}
	}
}
