using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class DropDownListView : MonoBehaviour
	{
		public const float AnimationDuration = 0.5f;

		[SerializeField]
		private Button ArrowButton = default;

		[SerializeField]
		private Button ExtraButton = default;

		[SerializeField]
		private DropDownListSelectedView DropDownListSelectedView = default;

		[SerializeField]
		private DropDownListArrow DropDownListArrow = default;

		[SerializeField]
		private DropDownListListView DropDownListListView = default;

		private bool _listVisible;

		private void OnEnable()
		{
			ArrowButton.onClick.AddListener(ArrowButtonOnClickCallback);
			if ((bool)ExtraButton)
			{
				ExtraButton.onClick.AddListener(ArrowButtonOnClickCallback);
			}
		}

		private void OnDisable()
		{
			ArrowButton.onClick.RemoveListener(ArrowButtonOnClickCallback);
			if ((bool)ExtraButton)
			{
				ExtraButton.onClick.AddListener(ArrowButtonOnClickCallback);
			}
		}

		private void ArrowButtonOnClickCallback()
		{
			_listVisible = !_listVisible;
			DropDownListSelectedView.UpdateVisible(!_listVisible);
			DropDownListArrow.UpdateState(_listVisible);
			DropDownListListView.UpdateVisible(_listVisible);
			if ((bool)ExtraButton)
			{
				ExtraButton.gameObject.SetActive(!_listVisible);
			}
		}
	}
}
