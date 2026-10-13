using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI
{
	public class LockCarButtonReference : MonoBehaviour
	{
		[SerializeField]
		private GameObject unlockSprite;

		[SerializeField]
		private GameObject lockSprite;

		[SerializeField]
		private GameObject Root;

		private Sprite unlockSpriteImage;

		private Sprite lockSpriteImage;

		private LockCarButtonController _lockCarButtonController;

		[Inject]
		public void Construct(LockCarButtonController lockCarButtonController)
		{
			_lockCarButtonController = lockCarButtonController;
			_lockCarButtonController.SetButtonReference(this);
		}

		private void Awake()
		{
			unlockSpriteImage = unlockSprite.GetComponent<Image>().sprite;
			lockSpriteImage = lockSprite.GetComponent<Image>().sprite;
		}

		public void ChangeSprite(bool isLocked)
		{
			lockSprite.SetActive(isLocked);
			unlockSprite.SetActive(!isLocked);
		}

		public Sprite GetLockButtonSprite()
		{
			if (unlockSprite.activeSelf)
			{
				return lockSpriteImage;
			}
			return unlockSpriteImage;
		}

		public void SetActiveRoot(bool isActive)
		{
			if ((bool)Root)
			{
				Root.SetActive(isActive);
			}
		}
	}
}
