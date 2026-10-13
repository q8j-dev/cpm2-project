using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class DealerWindowFilterClassView : MonoBehaviour
	{
		public const float AnimationDuration = 0.5f;

		[SerializeField]
		private Button ArrowButton = default;

		[SerializeField]
		private Button ExtraButton = default;

		[SerializeField]
		private DealerWindowFilterSelectedClassView DealerWindowFilterSelectedClassView = default;

		[SerializeField]
		private DealerWindowFilterClassArrow DealerWindowFilterClassArrow = default;

		[SerializeField]
		private DealerWindowFilterClassListView DealerWindowFilterClassListView = default;

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
