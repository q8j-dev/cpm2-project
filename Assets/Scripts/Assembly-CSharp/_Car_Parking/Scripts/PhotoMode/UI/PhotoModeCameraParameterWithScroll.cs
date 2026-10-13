using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class PhotoModeCameraParameterWithScroll : PhotoModeCameraParameter
	{
		[SerializeField]
		protected PhotoModeScroll Scroll;

		protected override void Start()
		{
			base.Start();
			Scroll.OnValueChanged += OnScrollValueChanged;
			Scroll.OnValueCentering += CheckForReset;
		}

		protected override void OnDestroy()
		{
			SaveValue(Scroll.LastCenteredIndex);
			base.OnDestroy();
			Scroll.OnValueChanged -= OnScrollValueChanged;
			Scroll.OnValueCentering -= CheckForReset;
		}

		public override void SetSelectionActive(bool active)
		{
			Scroll.SetActive(active);
		}

		public override void ResetValueText()
		{
			Scroll.ResetText();
		}

		protected virtual void OnScrollValueChanged(float value)
		{
		}

		protected override void OnToggleValueChanged(bool isOn)
		{
			base.OnToggleValueChanged(isOn);
			if (isOn)
			{
				Scroll.ResetText();
				int selectedIndex = ((Scroll.LastCenteredIndex == -1) ? base.Info.ScrollDefaultValues.DefaultItemIndex : Scroll.LastCenteredIndex);
				CheckForReset(selectedIndex);
			}
		}

		public override void ResetSelection()
		{
			Scroll.Init(base.Info.ScrollDefaultValues, base.Info.ScrollDefaultValues.DefaultItemIndex);
		}

		public void AdjustValue(int direction)
		{
			int panel = Scroll.LastCenteredIndex + direction;
			Scroll.GoToPanel(panel);
		}

		protected override void SetSavedValue()
		{
			int savedValue = GetSavedValue();
			Scroll.Init(base.Info.ScrollDefaultValues, savedValue);
			OnScrollValueChanged(Scroll.GetScrollValue(savedValue));
		}

		private void CheckForReset(int selectedIndex)
		{
			SetResetInteractable(selectedIndex != base.Info.ScrollDefaultValues.DefaultItemIndex);
		}
	}
}
