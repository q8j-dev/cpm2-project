using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class VisualTuningViewElement : MonoBehaviour
	{
		public string Key;

		private IVisualTuningViewModel _visualTuningViewModel;

		private IVisualTuningViewModelComplexOne _visualTuningViewModelComplexOne;

		private IVisualTuningViewModelComplexParams _visualTuningViewModelComplexParams;

		[Inject]
		public void Construct(IVisualTuningViewModel visualTuningViewModel, IVisualTuningViewModelComplexOne visualTuningViewModelComplexOne, IVisualTuningViewModelComplexParams visualTuningViewModelComplexParams)
		{
			_visualTuningViewModelComplexParams = visualTuningViewModelComplexParams;
			_visualTuningViewModelComplexOne = visualTuningViewModelComplexOne;
			_visualTuningViewModel = visualTuningViewModel;
		}

		private void Awake()
		{
			if (TryGetComponent<Button>(out var component))
			{
				component.onClick.AddListener(Click3);
			}
		}

		private void Click()
		{
			_visualTuningViewModel.Execute(Key);
		}

		private void Click2()
		{
			_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "Select", Random.Range(1, 100));
		}

		private void Click3()
		{
			_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Colors, "Select", ColorsScope.ColorsType.Car, Random.ColorHSV());
		}
	}
}
