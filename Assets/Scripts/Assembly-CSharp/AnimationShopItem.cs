using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Animations.Emotes;

public class AnimationShopItem : MonoBehaviour
{
	private const float LockedIconAlpha = 0.4f;

	private const float DefaultIconAlpha = 1f;

	public TextMeshProUGUI PriceLabel;

	public Image Icon;

	public Toggle Toggle;

	[HideInInspector]
	public int price;

	public int id;

	private AnimationShop _animationShop;

	[SerializeField]
	private Image Background;

	[SerializeField]
	private Sprite SelectedSprite;

	[SerializeField]
	private Sprite DeselectedSprite;

	[SerializeField]
	private GameObject LockBadge;

	private bool _showsCoinPrice = true;

	private bool _locked;

	private bool _disabled;

	private void Start()
	{
		if (!_showsCoinPrice)
		{
			ApplyLockedVisual();
		}
		else
		{
			PriceLabel.text = price.ToString();
		}
	}

	public void Init(IEmoteEntry entry, AnimationShop animationShop, bool disabled = false)
	{
		_animationShop = animationShop;
		Icon.sprite = entry.Sprite;
		id = entry.Id;
		_showsCoinPrice = entry.BuyFlow == EmoteBuyFlow.Coins;
		_disabled = disabled;
		Toggle.interactable = !disabled;
		Toggle.SetIsOnWithoutNotify(value: false);
		if ((bool)Background && (bool)DeselectedSprite)
		{
			Background.sprite = DeselectedSprite;
		}
		if (!_showsCoinPrice)
		{
			_locked = !entry.IsOwned;
			price = 0;
			ApplyLockedVisual();
			return;
		}
		price = ((!entry.IsOwned) ? entry.Price : 0);
		PriceLabel.text = price.ToString();
		if ((bool)LockBadge)
		{
			LockBadge.SetActive(value: false);
		}
		SetIconAlpha(_disabled ? 0.4f : 1f);
	}

	private void ApplyLockedVisual()
	{
		if ((bool)LockBadge)
		{
			LockBadge.SetActive(_locked);
		}
		if ((bool)PriceLabel)
		{
			PriceLabel.text = "";
		}
		SetIconAlpha((_locked || _disabled) ? 0.4f : 1f);
	}

	public void OnPressed(bool state)
	{
		if (state)
		{
			_animationShop.ItemPressed(this);
			Background.sprite = SelectedSprite;
		}
		else
		{
			Background.sprite = DeselectedSprite;
		}
	}

	private void SetIconAlpha(float alpha)
	{
		Color color = Icon.color;
		color.a = alpha;
		Icon.color = color;
	}
}
