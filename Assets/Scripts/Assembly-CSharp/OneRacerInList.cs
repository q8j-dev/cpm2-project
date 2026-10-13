using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.UIElement;

public class OneRacerInList : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI nickName;

	[SerializeField]
	private RawImage carIcon;

	[SerializeField]
	private FlagDrawer flagDrawer;

	public bool isLooserListElmnt;

	[SerializeField]
	private TextMeshProUGUI placeText;

	[SerializeField]
	private Image placeIcon;

	[SerializeField]
	private Sprite[] _sprites;

	public void Init(MainCarValues carValues, int index)
	{
		if ((bool)carIcon)
		{
			carIcon.texture = Resources.Load<Texture>($"UI/CarImages/b{carValues.CarID}");
		}
		if ((bool)flagDrawer)
		{
			flagDrawer.Set(carValues);
		}
		if (isLooserListElmnt)
		{
			nickName.text = carValues.NickName;
			InitLoserMenuPlace(index);
		}
		else
		{
			nickName.text = index + "." + carValues.NickName;
		}
	}

	private void InitLoserMenuPlace(int index)
	{
		switch (index)
		{
		case 1:
			placeIcon.sprite = _sprites[0];
			placeText.text = "1st winner";
			break;
		case 2:
			placeIcon.sprite = _sprites[1];
			placeText.text = "2nd winner";
			break;
		case 3:
			placeIcon.sprite = _sprites[2];
			placeText.text = "3rd winner";
			break;
		}
	}
}
