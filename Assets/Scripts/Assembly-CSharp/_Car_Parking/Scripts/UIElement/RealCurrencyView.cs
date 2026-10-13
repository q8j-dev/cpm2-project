using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.UIElement
{
	public class RealCurrencyView : MonoBehaviour
	{
		[SerializeField]
		private Text Text;

		[SerializeField]
		private GameObject TextRoot;

		[SerializeField]
		private GameObject LoadingRoot;

		private RectTransform _parentRectTransform;

		private IPurchaseSystem _purchaseSystem;

		[Inject]
		public void Construct(IPurchaseSystem purchaseSystem)
		{
			_purchaseSystem = purchaseSystem;
		}

		private void Start()
		{
			if ((bool)base.gameObject.transform.parent)
			{
				_parentRectTransform = base.gameObject.transform.parent.GetComponent<RectTransform>();
			}
		}

		public void UpdateText(string text)
		{
			Text.text = text;
			TextRoot.SetActive(_purchaseSystem.IsInitialized);
			LoadingRoot.SetActive(!TextRoot.activeSelf);
			if ((bool)_parentRectTransform)
			{
				UIHelper.DelayedRebuildLayout(_parentRectTransform);
			}
		}

		public void HideLoading()
		{
			LoadingRoot.SetActive(value: false);
		}
	}
}
