using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.WindowSystem.PropertyWindow
{
	[RequireComponent(typeof(Button))]
	public class PropertiesUISection : MonoBehaviour
	{
		[SerializeField]
		private Image FrameImage;

		private Button _selectButton;

		[field: SerializeField]
		public PropertyType PropertyType { get; private set; }

		public event Action<PropertiesUISection> Selected;

		private void Awake()
		{
			_selectButton = base.gameObject.GetComponent<Button>();
			_selectButton.onClick.AddListener(OnClick);
		}

		public void Select(bool value, bool withAnimation = true)
		{
			FrameImage.DOKill();
			FrameImage.DOFade(value ? 1 : 0, withAnimation ? 0.4f : 0f);
		}

		private void OnClick()
		{
			Selected?.Invoke(this);
		}
	}
}
