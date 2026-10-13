using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

public class AllInventoryPopup : MonoBehaviour
{
	[SerializeField]
	private InventroyPopupElement InventroyPopupElementPrefab;

	private List<InventroyPopupElement> _elements = new List<InventroyPopupElement>();

	[SerializeField]
	private Transform ContentPlace;

	[SerializeField]
	private TextMeshProUGUI TitleText;

	[SerializeField]
	private Button ConfirmButton;

	[SerializeField]
	private Button CancelButton;

	[SerializeField]
	private AllPartsInventoryUIWindow AllPartsInventoryUIWindow;

	[SerializeField]
	private ContentSizeFitter BuyButtonSizeFitter;

	[SerializeField]
	private TextMeshProUGUI ConfirmText;

	[SerializeField]
	private Image MoneyIcon;

	[SerializeField]
	private LayoutElement ContentLayoutElement;

	[SerializeField]
	private RectTransform ViewportRect;

	private IPrefabFactory _prefabFactory;

	private Action _onConfirm;

	private Action _onCancel;

	[Inject]
	public void Construct(IPrefabFactory prefabFactory)
	{
		_prefabFactory = prefabFactory;
	}

	private void Start()
	{
		ContentLayoutElement.minWidth = ViewportRect.rect.width;
		ConfirmButton.onClick.AddListener(Confirm);
		CancelButton.onClick.AddListener(Cancel);
	}

	public void Init(List<PartData> partDatas, bool isSell, Action callback)
	{
		_onConfirm = callback;
		ConfirmText.text = (isSell ? new LocalizedString("Allpartsinventorywindow.Sell") : new LocalizedString("Allpartsinventorywindow.Set"));
		UIHelper.SizeFitterMaxWidth(BuyButtonSizeFitter, 300f).Forget();
		TitleText.text = (isSell ? new LocalizedString("AllInventoryPopup.Confirmsell") : new LocalizedString("AllInventoryPopup.Confirmset"));
		MoneyIcon.gameObject.SetActive(isSell);
		for (int i = 0; i < partDatas.Count; i++)
		{
			if (partDatas[i] != null)
			{
				InventroyPopupElement inventroyPopupElement = _prefabFactory.Instantiate(InventroyPopupElementPrefab, ContentPlace);
				_elements.Add(inventroyPopupElement);
				inventroyPopupElement.Init(partDatas[i]);
			}
		}
	}

	private void Confirm()
	{
		_onConfirm?.Invoke();
	}

	private void Cancel()
	{
		_onCancel?.Invoke();
		base.gameObject.SetActive(value: false);
		AllPartsInventoryUIWindow.OnCancelPopup();
	}

	private void OnDisable()
	{
		for (int i = 0; i < _elements.Count; i++)
		{
			UnityEngine.Object.Destroy(_elements[i].gameObject);
		}
		_elements.Clear();
	}
}
