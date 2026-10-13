using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ShowHideVinylForOtherCarButton : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		[SerializeField]
		private Image Icon;

		[SerializeField]
		private Sprite SpriteShow;

		[SerializeField]
		private Sprite SpriteHide;

		[SerializeField]
		private TMP_Text TextShowVinyl;

		[SerializeField]
		private TMP_Text TextHideVinyl;

		[SerializeField]
		private RectTransform MainRect;

		private MultiPersonControl _mpc;

		public void Init(MultiPersonControl mpc)
		{
			_mpc = mpc;
			UpdateView();
		}

		private void OnEnable()
		{
			UpdateView();
			Button.onClick.AddListener(OnClickButton);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnClickButton);
		}

		private void OnClickButton()
		{
			MultiPlayerPosControl multiPlayerPosControl = GetMultiPlayerPosControl();
			if ((bool)multiPlayerPosControl)
			{
				multiPlayerPosControl.IsVinylsBanned = !multiPlayerPosControl.IsVinylsBanned;
				UpdateView();
			}
		}

		private void UpdateView()
		{
			Button.gameObject.SetActive(value: false);
			MultiPlayerPosControl multiPlayerPosControl = GetMultiPlayerPosControl();
			if ((bool)multiPlayerPosControl)
			{
				Button.gameObject.SetActive(value: true);
				Icon.sprite = (multiPlayerPosControl.IsVinylsBanned ? SpriteHide : SpriteShow);
				TextShowVinyl.gameObject.SetActive(multiPlayerPosControl.IsVinylsBanned);
				TextHideVinyl.gameObject.SetActive(!TextShowVinyl.gameObject.activeSelf);
				(multiPlayerPosControl.IsVinylsBanned ? TextShowVinyl : TextHideVinyl).ForceMeshUpdate();
			}
			UIHelper.DelayedRebuildLayout(MainRect).Forget();
		}

		private MultiPlayerPosControl GetMultiPlayerPosControl()
		{
			if (!_mpc || _mpc.ownPlayer)
			{
				return null;
			}
			GameObject firstAndOwnCar = _mpc.FirstAndOwnCar;
			if (!firstAndOwnCar)
			{
				PhotonView photonView = PhotonNetwork.GetPhotonView(_mpc.OwnCarViewID);
				if ((bool)photonView && photonView.TryGetComponent<VehicleData>(out var _))
				{
					firstAndOwnCar = photonView.gameObject;
				}
			}
			if (!firstAndOwnCar)
			{
				return null;
			}
			return firstAndOwnCar.GetComponent<MultiPlayerPosControl>();
		}
	}
}
