using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NewUI.Scripts1.Elements;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Racing_Scripts.RaceInfo
{
	public class RaceInfoDropdown : MonoBehaviour
	{
		[SerializeField]
		private Button OpenCloseButton = default;

		[SerializeField]
		private RotatorByTween ArrowRotator = default;

		[SerializeField]
		private float ArrowDefaultAngle = 180f;

		[SerializeField]
		private RaceWonPlacesItem AllClassesInfo = default;

		[SerializeField]
		private RaceWonPlacesItem[] RaceWonPlaces = default;

		[SerializeField]
		private CanvasGroupStateTweener CanvasGroupStateTweener = default;

		[SerializeField]
		private Transform ClosePosition = default;

		[SerializeField]
		private MoverByTween[] Items = default;

		private bool _isOpen;

		private void Start()
		{
			OpenCloseButton.onClick.AddListener(OpenCloseDropdown);
			UpdateVisible(isVisible: false, immediate: true);
		}

		public void SetAllInfo(List<CircuitRacingRecord> classRecords)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (CircuitRacingRecord classRecord in classRecords)
			{
				RaceWonPlaces.First((RaceWonPlacesItem i) => i.TimeClass == classRecord.ClassIndex).SetPlacesCount(classRecord.FirstPlace, classRecord.SecondPlace, classRecord.ThirdPlace);
				num += classRecord.FirstPlace;
				num2 += classRecord.SecondPlace;
				num3 += classRecord.ThirdPlace;
			}
			AllClassesInfo.SetPlacesCount(num, num2, num3);
		}

		private void OpenCloseDropdown()
		{
			_isOpen = !_isOpen;
			Vector3 rotation = new Vector3(0f, 0f, _isOpen ? 0f : ArrowDefaultAngle);
			ArrowRotator.Rotate(rotation, 0.5f, Ease.OutSine, isLocal: true);
			UpdateVisible(_isOpen);
		}

		private void UpdateVisible(bool isVisible, bool immediate = false)
		{
			CanvasGroupStateTweener.ChangeState(isVisible);
			for (int i = 0; i < Items.Length; i++)
			{
				float duration = (immediate ? 0f : Mathf.Lerp(0f, 0.5f, Mathf.InverseLerp(0f, Items.Length - 1, i)));
				if (isVisible)
				{
					Items[i].MoveToDefault(duration, Ease.OutSine, isLocal: false);
				}
				else
				{
					Items[i].Move(ClosePosition.position, duration, Ease.OutSine, isLocal: false);
				}
			}
		}
	}
}
