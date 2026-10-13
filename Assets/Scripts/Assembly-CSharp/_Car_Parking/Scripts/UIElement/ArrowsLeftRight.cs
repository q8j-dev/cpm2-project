using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ArrowsLeftRight : MonoBehaviour
	{
		[SerializeField]
		private Button ButtonLeft;

		[SerializeField]
		private Button ButtonRight;

		[SerializeField]
		private TMP_Text Title;

		public Action<ArrowsLeftRight, int> OnClick;

		public string TitleText
		{
			get
			{
				if (!(Title != null))
				{
					return "";
				}
				return Title.text;
			}
			set
			{
				if ((bool)Title)
				{
					Title.text = value;
				}
			}
		}

		private void OnEnable()
		{
			ButtonLeft.onClick.AddListener(OnButtonLeftClick);
			ButtonRight.onClick.AddListener(OnButtonRightClick);
		}

		private void OnDisable()
		{
			ButtonLeft.onClick.RemoveListener(OnButtonLeftClick);
			ButtonRight.onClick.RemoveListener(OnButtonRightClick);
		}

		private void OnButtonLeftClick()
		{
			OnClick?.Invoke(this, -1);
		}

		private void OnButtonRightClick()
		{
			OnClick?.Invoke(this, 1);
		}
	}
}
