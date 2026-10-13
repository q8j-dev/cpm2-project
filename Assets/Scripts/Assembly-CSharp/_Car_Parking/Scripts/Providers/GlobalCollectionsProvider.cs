using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.ArtistTools.Trackables;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	public class GlobalCollectionsProvider
	{
		private readonly List<RenderTexture> _allRenderTextures = new List<RenderTexture>();

		private readonly List<qualitySet> _qualitySets = new List<qualitySet>();

		private readonly List<Transform> _allIconOnMap = new List<Transform>();

		private readonly List<GasPistolMove> _gasPistolMoves = new List<GasPistolMove>();

		private readonly List<GameObject> _npcPeople = new List<GameObject>();

		private readonly List<ParkingFreeSpaceSensor> _freeSpaceSensors = new List<ParkingFreeSpaceSensor>();

		private readonly List<TrackableWithAspects> _trackables = new List<TrackableWithAspects>();

		public GlobalCollectionsProvider()
		{
			ResetAllCollections();
		}

		public void RegisterRenderTexture(RenderTexture texture)
		{
			if ((bool)texture)
			{
				_allRenderTextures.Add(texture);
			}
		}

		public void UnregisterRenderTexture(RenderTexture texture)
		{
			if ((bool)texture)
			{
				_allRenderTextures.Remove(texture);
			}
		}

		public void ResetRenderTextures()
		{
			foreach (RenderTexture allRenderTexture in _allRenderTextures)
			{
				CommonExtensions.DeleteRenderTex(allRenderTexture);
			}
			_allRenderTextures.Clear();
		}

		public void ActivateQualitySets()
		{
			foreach (qualitySet item in _qualitySets.Where((qualitySet qualitySet2) => qualitySet2))
			{
				item.Start();
			}
		}

		public void RegisterQualitySet(qualitySet value)
		{
			if ((bool)value)
			{
				_qualitySets.Add(value);
			}
		}

		public void UnregisterQualitySet(qualitySet value)
		{
			if ((bool)value)
			{
				_qualitySets.Remove(value);
			}
		}

		private void ResetQualitySets()
		{
			CommonExtensions.ClearGOCollection(_qualitySets);
		}

		public void RegisterMapIcon(Transform icon)
		{
			if ((bool)icon)
			{
				_allIconOnMap.Add(icon);
			}
		}

		public void UnregisterMapIcon(Transform icon)
		{
			if ((bool)icon)
			{
				_allIconOnMap.Remove(icon);
			}
		}

		public List<Transform> GetAllMapIcons()
		{
			_allIconOnMap.RemoveAll((Transform x) => !x);
			return _allIconOnMap;
		}

		public void ResetMapIcons()
		{
			CommonExtensions.ClearGOCollection(_allIconOnMap);
		}

		public void RegisterGasPistolMove(GasPistolMove gasPistolMove)
		{
			if ((bool)gasPistolMove)
			{
				_gasPistolMoves.Add(gasPistolMove);
			}
		}

		public void UnregisterGasPistolMove(GasPistolMove gasPistolMove)
		{
			if ((bool)gasPistolMove)
			{
				_gasPistolMoves.Remove(gasPistolMove);
			}
		}

		public List<GasPistolMove> GetAllGasPistolMoves()
		{
			_gasPistolMoves.RemoveAll((GasPistolMove x) => !x);
			return _gasPistolMoves;
		}

		public void ResetGasPistolMoves()
		{
			CommonExtensions.ClearGOCollection(_gasPistolMoves);
		}

		public void RegisterNpc(GameObject npc)
		{
			if ((bool)npc)
			{
				_npcPeople.Add(npc);
			}
		}

		public void UnregisterNpc(GameObject npc)
		{
			if ((bool)npc)
			{
				_npcPeople.Remove(npc);
			}
		}

		public List<GameObject> GetAllNpcPeople()
		{
			_npcPeople.RemoveAll((GameObject x) => !x);
			return _npcPeople;
		}

		private void ResetNpcPeople()
		{
			CommonExtensions.ClearGOCollection(_npcPeople);
		}

		public void RegisterFreeSpaceSensors(ParkingFreeSpaceSensor sensor)
		{
			if ((bool)sensor)
			{
				_freeSpaceSensors.Add(sensor);
			}
		}

		public void UnregisterFreeSpaceSensors(ParkingFreeSpaceSensor sensor)
		{
			if ((bool)sensor)
			{
				_freeSpaceSensors.Remove(sensor);
			}
		}

		public List<ParkingFreeSpaceSensor> GetAllFreeSpaceSensors()
		{
			return _freeSpaceSensors.Where((ParkingFreeSpaceSensor x) => x).ToList();
		}

		private void ResetFreeSpaceSensors()
		{
			CommonExtensions.ClearGOCollection(_freeSpaceSensors);
		}

		public void RegisterTrackable(TrackableWithAspects trackable)
		{
			if ((bool)trackable)
			{
				_trackables.Add(trackable);
			}
		}

		public void UnregisterTrackable(TrackableWithAspects trackable)
		{
			if ((bool)trackable)
			{
				_trackables.Remove(trackable);
			}
		}

		public List<TrackableWithAspects> GetTrackables()
		{
			return _trackables;
		}

		private void ResetTrackables()
		{
			CommonExtensions.ClearGOCollection(_trackables);
		}

		public void ResetAllCollections(bool resetRenderTex = true)
		{
			ResetMapIcons();
			ResetNpcPeople();
			ResetQualitySets();
			ResetGasPistolMoves();
			ResetFreeSpaceSensors();
			if (resetRenderTex)
			{
				ResetRenderTextures();
			}
			ResetTrackables();
		}
	}
}
