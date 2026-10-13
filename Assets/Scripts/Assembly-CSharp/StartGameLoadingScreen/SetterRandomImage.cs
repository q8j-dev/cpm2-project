using UnityEngine;
using UnityEngine.UI;

namespace StartGameLoadingScreen
{
	public class SetterRandomImage : MonoBehaviour
	{
		private readonly string _keyImage = "ImageLoadingScreen";

		private readonly string _keyCurrentImage = "CurrentImage";

		[SerializeField]
		private Image Image = default;

		[SerializeField]
		private Sprite[] Sprites = default;

		private void Awake()
		{
			int num = Random.Range(0, Sprites.Length);
			if (PlayerPrefs.HasKey(_keyImage))
			{
				int num2 = PlayerPrefs.GetInt(_keyImage);
				if (num == num2)
				{
					num++;
					if (num == Sprites.Length)
					{
						num = 0;
					}
				}
			}
			PlayerPrefs.SetInt(_keyImage, num);
			Image.sprite = Sprites[num];
			PlayerPrefs.SetInt(_keyCurrentImage, num);
		}
	}
}
