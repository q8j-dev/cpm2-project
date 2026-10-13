using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

public class VisualItemElement : MonoBehaviour, ICell
{
	public bool isNotBodyKitItem;

	public bool IsBought;

	public int index;

	public RawImage elementImage;

	public AspectRatioFitter AspectRatioFitterElementImage;

	public RectTransform RectTransformElementImage;

	public TextMeshProUGUI elementPrice;

	public Toggle Button;

	public Image moneyIcon;

	public BodyKitType bodyKitType;

	public TextMeshProUGUI titleText;

	public CanvasGroup canvasGroup;

	public NewUIPlaySound NewUIPlaySound;

	public Image SpecialStickerImage;

	private void Start()
	{
		Button.onValueChanged.AddListener(ChangeColor);
	}

	public virtual void ChangeColor(bool isOn)
	{
	}

	public virtual void Init(VisualTuningCellData visualTuningCellData)
	{
		Button.onValueChanged.RemoveAllListeners();
		visualTuningCellData.buttonSetAction?.Invoke(base.gameObject, visualTuningCellData.data);
	}
}
