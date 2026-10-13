using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.UIElement;

public class FlexibleTextPanel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TextElmnt;

	[SerializeField]
	private RectTransform LayoutGroup;

	public void SetText(string text)
	{
		TextElmnt.text = text;
		UIHelper.DelayedRebuildLayout(LayoutGroup).Forget();
	}
}
