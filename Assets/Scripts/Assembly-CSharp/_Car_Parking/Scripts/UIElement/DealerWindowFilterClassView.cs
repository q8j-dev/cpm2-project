using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class DealerWindowFilterClassView : MonoBehaviour
	{
		public const float AnimationDuration = 0.5f;

		[SerializeField]
		private Button ArrowButton;

		[SerializeField]
		private Button ExtraButton;

		[SerializeField]
		private DealerWindowFilterSelectedClassView DealerWindowFilterSelectedClassView;

		[SerializeField]
		private DealerWindowFilterClassArrow DealerWindowFilterClassArrow;

		[SerializeField]
		private DealerWindowFilterClassListView DealerWindowFilterClassListView;

		private bool _classVisible;

		private void OnEnable()
		{
			ArrowButton.onClick.AddListener(ArrowButtonOnClickCallback);
			ExtraButton.onClick.AddListener(ArrowButtonOnClickCallback);
		}

		private void OnDisable()
		{
			ArrowButton.onClick.RemoveListener(ArrowButtonOnClickCallback);
			ExtraButton.onClick.AddListener(ArrowButtonOnClickCallback);
		}

		private void ArrowButtonOnClickCallback()
		{
			_classVisible = !_classVisible;
			DealerWindowFilterSelectedClassView.UpdateVisible(!_classVisible);
			DealerWindowFilterClassArrow.UpdateState(_classVisible);
			DealerWindowFilterClassListView.UpdateVisible(_classVisible);
			ExtraButton.gameObject.SetActive(!_classVisible);
		}
	}
}
