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
		private Button ClassButton;

		[SerializeField]
		private TextMeshProUGUI[] ToChangeColorTexts;

		[SerializeField]
		private TextMeshProUGUI FirstPacesCount;

		[SerializeField]
		private TextMeshProUGUI SecondPacesCount;

		[SerializeField]
		private TextMeshProUGUI ThirdPacesCount;

		[SerializeField]
		private TextMeshProUGUI BestTime;

		[SerializeField]
		private GameObject BestTimeObject;

		[SerializeField]
		private GameObject LockObject;

		[SerializeField]
		private GameObject WinStatisticObject;

		[SerializeField]
		private Color DefaultColorOfTitle;

		private int _classTime;

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
