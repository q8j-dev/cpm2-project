using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public abstract class PropertyWindowBase<TC, TD> : MonoBehaviour
	{
		[SerializeField]
		protected Button SelectButton;

		[SerializeField]
		protected CanvasGroup SelectCanvasGroup;

		[SerializeField]
		protected Button BuyButton;

		private int _selectedItem = -1;

		private PlaceUIItem<TC, TD> _currentItem;

		private Tween _selectTween;

		private Tween _buyTween;

		protected abstract List<PlaceUIItem<TC, TD>> PlaceUIItems { get; }

		protected int SelectedItem => _selectedItem;

		private void OnEnable()
		{
			Init();
			SelectButton.onClick.AddListener(OnSelectItem);
			BuyButton.onClick.AddListener(OnBuyItem);
		}

		private void OnDisable()
		{
			if ((bool)_currentItem)
			{
				_currentItem.IsChosen = false;
			}
			_currentItem = null;
			_selectedItem = -1;
			foreach (PlaceUIItem<TC, TD> placeUIItem in PlaceUIItems)
			{
				placeUIItem.Toggled -= OnItemToggled;
			}
			SelectButton.onClick.RemoveListener(OnSelectItem);
			BuyButton.onClick.RemoveListener(OnBuyItem);
		}

		protected virtual void Init()
		{
			foreach (PlaceUIItem<TC, TD> placeUIItem in PlaceUIItems)
			{
				placeUIItem.Init();
				placeUIItem.Toggled += OnItemToggled;
			}
		}

		protected void SetButtonsVisibility(bool state, bool animate = true)
		{
			if (animate)
			{
				_selectTween?.Kill();
				_buyTween?.Kill();
				_selectTween = UIAnimator.SetActiveWithPopFade(SelectButton.gameObject, state, 0.25f, Ease.InBounce);
				_buyTween = UIAnimator.SetActiveWithPopFade(BuyButton.gameObject, !state, 0.25f, Ease.InBounce);
			}
			else
			{
				SelectButton.gameObject.SetActive(state);
				BuyButton.gameObject.SetActive(!state);
			}
		}

		protected void ChooseItem(int index, bool withAnimation = true)
		{
			if (!(_currentItem == PlaceUIItems[index]))
			{
				if ((bool)_currentItem)
				{
					_currentItem.IsChosen = false;
				}
				_selectedItem = index;
				_currentItem = PlaceUIItems[index];
				UpdateButton(withAnimation);
			}
		}

		protected virtual void UpdateButton(bool animate = true)
		{
		}

		protected virtual void OnSelectItem()
		{
		}

		protected virtual void OnBuyItem()
		{
		}

		private void OnItemToggled(int index)
		{
			ChooseItem(index);
		}

		private void OnDestroy()
		{
			_selectTween?.Kill();
			_buyTween?.Kill();
		}
	}
}
