using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

namespace _Car_Parking.Scripts.UIElement
{
	public class DragClassUIElement : MonoBehaviour
	{
		[SerializeField]
		private Button ClassButton = default;

		[SerializeField]
		private TextMeshProUGUI[] ToChangeColorTexts = default;

		[SerializeField]
		private TextMeshProUGUI FirstPacesCount = default;

		[SerializeField]
		private TextMeshProUGUI SecondPacesCount = default;

		[SerializeField]
		private TextMeshProUGUI ThirdPacesCount = default;

		[SerializeField]
		private TextMeshProUGUI BestTime = default;

		[SerializeField]
		private GameObject BestTimeObject = default;

		[SerializeField]
		private GameObject LockObject = default;

		[SerializeField]
		private GameObject WinStatisticObject = default;

		[SerializeField]
		private Color DefaultColorOfTitle = default;

		public Action OnStartButtonClicked { get; set; }

		private void Start()
		{
			ClassButton.onClick.AddListener(StartClass);
		}

		private void OnDestroy()
		{
			ClassButton.onClick.RemoveAllListeners();
		}

		private void StartClass()
		{
			OnStartButtonClicked?.Invoke();
		}

		public void Init(RacingClassRecord racingClassRecord, bool mustLock = false)
		{
			Lock(mustLock);
			if (racingClassRecord != null)
			{
				SetPlacesCount(racingClassRecord);
				if (racingClassRecord.BestTime > 0f)
				{
					SetBestTime($"{racingClassRecord.BestTime:F3} s");
				}
			}
			else
			{
				WinStatisticObject.SetActive(value: false);
				BestTimeObject.SetActive(value: false);
			}
		}

		private void SetPlacesCount(RacingClassRecord racingClassRecord)
		{
			if (racingClassRecord.FirstPlace > 0)
			{
				FirstPacesCount.transform.parent.gameObject.SetActive(value: true);
				FirstPacesCount.text = racingClassRecord.FirstPlace.ToString();
			}
			if (racingClassRecord.SecondPlace > 0)
			{
				SecondPacesCount.transform.parent.gameObject.SetActive(value: true);
				SecondPacesCount.text = racingClassRecord.SecondPlace.ToString();
			}
			if (racingClassRecord.ThirdPlace > 0)
			{
				ThirdPacesCount.transform.parent.gameObject.SetActive(value: true);
				ThirdPacesCount.text = racingClassRecord.ThirdPlace.ToString();
			}
		}

		private void SetBestTime(string time)
		{
			BestTimeObject.SetActive(value: true);
			BestTime.text = time;
		}

		private void Lock(bool mustLock)
		{
			LockObject.SetActive(mustLock);
			ClassButton.image.color = (mustLock ? Color.black : Color.white);
			ClassButton.interactable = !mustLock;
			TextMeshProUGUI[] toChangeColorTexts = ToChangeColorTexts;
			for (int i = 0; i < toChangeColorTexts.Length; i++)
			{
				toChangeColorTexts[i].color = (mustLock ? Color.grey : DefaultColorOfTitle);
			}
		}
	}
}
