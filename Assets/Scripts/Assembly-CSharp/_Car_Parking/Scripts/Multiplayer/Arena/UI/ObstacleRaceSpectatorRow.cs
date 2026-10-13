using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ObstacleRaceSpectatorRow : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		[SerializeField]
		private TextMeshProUGUI[] PlaceTexts;

		[SerializeField]
		private Text[] NameTexts;

		[SerializeField]
		private TextMeshProUGUI[] ValueTexts;

		[SerializeField]
		private GameObject WatchableGraphic;

		[Header("Selection")]
		[SerializeField]
		private GameObject[] SelectedGraphics;

		[SerializeField]
		private GameObject[] NormalGraphics;

		[SerializeField]
		private Color SelectedColor;

		[SerializeField]
		private Color NormalColor;

		private int _actor;

		public event Action<int> Clicked;

		private void Awake()
		{
			if ((bool)Button)
			{
				Button.onClick.AddListener(OnClicked);
			}
		}

		private void OnDestroy()
		{
			if ((bool)Button)
			{
				Button.onClick.RemoveListener(OnClicked);
			}
		}

		public void Bind(in SpectatorListRow row)
		{
			_actor = row.Actor;
			TextMeshProUGUI[] placeTexts = PlaceTexts;
			int place = row.Place;
			SetText(placeTexts, place.ToString());
			SetText(NameTexts, row.UserName);
			SetText(ValueTexts, row.ValueText);
			if ((bool)Button)
			{
				Button.interactable = row.Watchable;
			}
			if ((bool)WatchableGraphic)
			{
				WatchableGraphic.SetActive(row.Watchable);
			}
			SelectedGraphics.SetActiveStatusAll(row.Selected);
			NormalGraphics.SetActiveStatusAll(!row.Selected);
			SetTextColor(PlaceTexts, row.Selected ? SelectedColor : NormalColor);
			SetTextColor(NameTexts, row.Selected ? SelectedColor : NormalColor);
			SetTextColor(ValueTexts, row.Selected ? SelectedColor : NormalColor);
		}

		public void Show(bool visible)
		{
			base.gameObject.SetActive(visible);
		}

		private static void SetText(TextMeshProUGUI[] targets, string value)
		{
			if (targets == null)
			{
				return;
			}
			for (int i = 0; i < targets.Length; i++)
			{
				if ((bool)targets[i])
				{
					targets[i].text = value;
				}
			}
		}

		private static void SetText(Text[] targets, string value)
		{
			if (targets == null)
			{
				return;
			}
			for (int i = 0; i < targets.Length; i++)
			{
				if ((bool)targets[i])
				{
					targets[i].text = value;
				}
			}
		}

		private static void SetTextColor(Text[] targets, Color value)
		{
			if (targets == null)
			{
				return;
			}
			for (int i = 0; i < targets.Length; i++)
			{
				if ((bool)targets[i])
				{
					targets[i].color = value;
				}
			}
		}

		private static void SetTextColor(TextMeshProUGUI[] targets, Color value)
		{
			if (targets == null)
			{
				return;
			}
			for (int i = 0; i < targets.Length; i++)
			{
				if ((bool)targets[i])
				{
					targets[i].color = value;
				}
			}
		}

		private void OnClicked()
		{
			Clicked?.Invoke(_actor);
		}
	}
}
