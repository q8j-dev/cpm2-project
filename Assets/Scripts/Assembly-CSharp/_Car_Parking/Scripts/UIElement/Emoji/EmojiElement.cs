using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.UIElement.Emoji
{
	public class EmojiElement : MonoBehaviour
	{
		[SerializeField]
		private Button CurrentButton = default;

		[SerializeField]
		private int Id = default;

		private MenuLevelParent _menuLevelParent;

		[Inject]
		public void Construct(MenuLevelParent menuLevelParent)
		{
			_menuLevelParent = menuLevelParent;
		}

		private void Start()
		{
			CurrentButton.onClick.AddListener(ChooseEmoji);
		}

		private void ChooseEmoji()
		{
			_menuLevelParent.PlayEmoji(Id);
		}
	}
}
