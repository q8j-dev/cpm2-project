using UnityEngine;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	public class ItemPatchNotes : SummaryOfEventItem
	{
		private const string PlayerPrefsConfigHash = "PatchNotesConfigHashV6";

		[SerializeField]
		private GameObject NewMark;

		[SerializeField]
		private PatchNotesConfig PatchNotesConfig;

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)PatchNotesConfig && (bool)NewMark)
			{
				NewMark.gameObject.SetActive(PlayerPrefs.GetInt("PatchNotesConfigHashV6") != PatchNotesConfig.HashData);
			}
		}

		protected override void ClickCallback()
		{
			if ((bool)PatchNotesConfig)
			{
				PlayerPrefs.SetInt("PatchNotesConfigHashV6", PatchNotesConfig.HashData);
			}
			base.ClickCallback();
		}
	}
}
