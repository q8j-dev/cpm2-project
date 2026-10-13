using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RacingScenePropsController : MonoBehaviour
	{
		public List<GameObject> listGO;

		public List<GameObject> listGO2;

		public List<GameObject> listGO3;

		[SerializeField]
		private string otherDistanceTreeName;

		private FreeDriveProvider _freeDriveProvider;

		[Inject]
		private void Construct(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		private void Start()
		{
			GlobalEventSystem.OnDragRacingPropsActivate += HandlePropsActivate;
			if (FlagsStorage.RacingGameMode && _freeDriveProvider.DragRacingControlMultiplayer != null)
			{
				DragRacingMultiplayerTree currentRacingTree = _freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.CurrentRacingTree;
				if (currentRacingTree != null)
				{
					HandlePropsActivate(state: true, _freeDriveProvider.DragRacingControlMultiplayer.SelectedDistance, currentRacingTree.gameObject.name);
				}
			}
			else
			{
				HandlePropsActivate(state: false, -1, "");
			}
		}

		private void HandlePropsActivate(bool state, int distanceIndex, string racingTreeName)
		{
			SetList(listGO, active: false);
			SetList(listGO2, active: false);
			SetList(listGO3, active: false);
			if ((!(racingTreeName != base.gameObject.name) || !(racingTreeName != otherDistanceTreeName)) && state)
			{
				switch (distanceIndex)
				{
				case 0:
					SetList(listGO, active: true);
					break;
				case 1:
					SetList(listGO2, active: true);
					break;
				case 2:
					SetList(listGO3, active: true);
					break;
				}
			}
		}

		private void SetList(List<GameObject> list, bool active)
		{
			foreach (GameObject item in list)
			{
				if ((bool)item)
				{
					item.SetActive(active);
				}
			}
		}

		private void OnDestroy()
		{
			GlobalEventSystem.OnDragRacingPropsActivate -= HandlePropsActivate;
		}
	}
}
