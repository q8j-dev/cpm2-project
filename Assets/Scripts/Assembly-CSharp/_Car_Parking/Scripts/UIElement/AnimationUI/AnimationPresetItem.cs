using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Animations.Emotes;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.UIElement.AnimationUI
{
	public class AnimationPresetItem : MonoBehaviour
	{
		private const float LockedIconAlpha = 0.4f;

		private const float DefaultIconAlpha = 1f;

		[SerializeField]
		private Button SelectButton;

		[SerializeField]
		private Image Icon = default;

		private IEmoteEntry _entry;

		private EmotionController _emotionController;

		private readonly float _scaleDuration = 0.1f;

		public void Init(IEmoteEntry entry, EmotionController emotionController, bool genderLocked = false)
		{
			Icon.sprite = entry.Sprite;
			_entry = entry;
			_emotionController = emotionController;
			SelectButton.onClick.RemoveListener(OnSelected);
			SelectButton.onClick.AddListener(OnSelected);
			SelectButton.interactable = !genderLocked;
			Color color = Icon.color;
			color.a = (genderLocked ? 0.4f : 1f);
			Icon.color = color;
		}

		public static bool IsGenderLocked(IEmoteEntry entry, int gender)
		{
			if (entry != null && entry.GenderMismatchPolicy == GenderMismatchPolicy.ShowDisabled)
			{
				return !PoseAudienceRules.Matches(entry.Audience, gender);
			}
			return false;
		}

		private void OnSelected()
		{
			if (!RagdollCPM.IsDriverRagdolled())
			{
				Icon.transform.DOScale(1.5f, _scaleDuration).OnComplete(() =>
				{
					_emotionController.MakeCurrentMovement(_entry);
					Icon.transform.localScale = Vector3.one;
				});
			}
		}

		private void OnValidate()
		{
			SelectButton = GetComponent<Button>();
		}
	}
}
