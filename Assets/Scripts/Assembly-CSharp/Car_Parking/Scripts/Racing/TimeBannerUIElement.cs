using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace Car_Parking.Scripts.Racing
{
	public class TimeBannerUIElement : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI PlaceText;

		[SerializeField]
		private Image RankIcon;

		[SerializeField]
		private Text UserNameText;

		[SerializeField]
		private Image CarImage;

		[SerializeField]
		private TextMeshProUGUI ClassText;

		[SerializeField]
		private TextMeshProUGUI TimeText;

		[SerializeField]
		private GameObject IsUserPanel;

		[Header("Running Line")]
		public RectTransform viewport;

		public float speed = 50f;

		[SerializeField]
		private RectTransform textRect;

		private float _startPosX;

		private float _endPosX;

		[SerializeField]
		private bool _needScroll;

		public void Init(BannerData data)
		{
			PlaceText.text = $"#{data.Place + 1}";
			UserNameText.text = data.UserName;
			ClassText.text = data.CarId.ToString();
			ClassText.text = data.ClassId.ToString();
			InitImages(data.CarId, data.Rank).Forget();
			SetValueFormat(data.PointValue, data.TimeAttackMode);
			base.gameObject.SetActive(value: true);
			if ((bool)IsUserPanel)
			{
				IsUserPanel.SetActive(data.IsUser);
			}
		}

		private void SetValueFormat(float value, TimeAttackMode mode)
		{
			switch (mode)
			{
			case TimeAttackMode.Drag:
				InitClassName(value);
				break;
			case TimeAttackMode.Drift:
				InitDriftScore(value);
				break;
			default:
				InitTime(value);
				break;
			}
		}

		private void InitTime(float time)
		{
			TimeText.text = Utils.FormatTime(time);
		}

		private void InitClassName(float classId)
		{
			string text = $"{classId} s";
			if ((int)classId == 8)
			{
				text = new LocalizedString("DragRacingUI.Unlim");
			}
			TimeText.text = text;
		}

		private void InitDriftScore(float driftScore)
		{
			TimeText.text = Utils.GetFormattedResultWithSpaces(driftScore);
		}

		public void RemoveUser()
		{
			PlaceText.text = "";
			UserNameText.text = "";
			ClassText.text = "";
			TimeText.text = "";
			ClassText.text = "";
			base.gameObject.SetActive(value: false);
		}

		private async UniTaskVoid InitImages(int carId, ObscuredInt? rankId)
		{
			Image carImage = CarImage;
			carImage.sprite = (await Resources.LoadAsync<Sprite>($"UI/CarImages/b{carId}")) as Sprite;
			carImage = RankIcon;
			ObscuredInt? obscuredInt = rankId;
			carImage.sprite = (await Resources.LoadAsync<Sprite>("icons/Rating/Rank_0" + obscuredInt.ToString())) as Sprite;
		}
	}
}
