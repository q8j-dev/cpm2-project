using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Audio;

public class UICupElement : MonoBehaviour
{
	private readonly uint PassGoldSoundEvent = AudioWrapper.GetId("Race_1st");

	private readonly uint PassSilverSoundEvent = AudioWrapper.GetId("Race_2d");

	private readonly uint PassCopperSoundEvent = AudioWrapper.GetId("Race_3d");

	public Color[] Colors;

	public GameObject MarkElement;

	public Image CupImage;

	private Tweener _tween;

	public void SetProgress(int progress, bool muted = false)
	{
		if (Colors == null || Colors.Length == 0)
		{
			return;
		}
		if (progress < 0 || progress >= Colors.Length)
		{
			progress = Mathf.Clamp(progress, 0, Colors.Length - 1);
		}
		CupImage.color = Colors[progress];
		if (progress == 0)
		{
			return;
		}
		_tween?.Kill();
		if (!muted)
		{
			SoundEventsDataHelper.PlayUISound(GetSoundKey(progress), base.gameObject);
			_tween = CupImage.transform.DOShakeScale(0.5f).SetLink(base.gameObject, LinkBehaviour.KillOnDestroy).OnComplete(() =>
			{
				MarkElement.SetActive(value: true);
			});
		}
		else
		{
			MarkElement.SetActive(value: true);
		}
	}

	private void OnDestroy()
	{
		_tween?.Kill();
	}

	private uint GetSoundKey(int index)
	{
		return index switch
		{
			1 => PassGoldSoundEvent, 
			2 => PassSilverSoundEvent, 
			3 => PassCopperSoundEvent, 
			_ => PassCopperSoundEvent, 
		};
	}
}
