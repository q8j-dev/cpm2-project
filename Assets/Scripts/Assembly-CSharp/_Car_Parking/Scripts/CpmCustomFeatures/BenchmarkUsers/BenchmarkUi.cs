using System.Collections.Generic;
using System.Linq;
using ChartAndGraph;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.CpmCustomFeatures.Benchmarking;

namespace _Car_Parking.Scripts.CpmCustomFeatures.BenchmarkUsers
{
	public class BenchmarkUi : MonoBehaviour
	{
		public GraphChart GraphChart;

		public Text Text;

		public BenchmarkReport LastReport;

		public Toggle[] Toggles;

		public Canvas Canvas;

		public Button SendButton;

		public Button CloseButton;

		public Text SettingsText;

		private readonly Dictionary<string, string> _chatIds = new Dictionary<string, string>
		{
			["Maksim"] = "547729441",
			["Daniyar"] = "548521192",
			["Ilyas"] = "748869657"
		};

		private void OnEnable()
		{
			SettingsText.text = GenerateSettingsText();
		}

		private string GenerateSettingsText()
		{
			CPMGraphicsSettingsManager instance = CPMGraphicsSettingsManager.Instance;
			int quality = instance.Quality;
			string text = "";
			string text2 = string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(text + "Quality: " + quality switch
			{
				0 => "Low".Colored(Color.red), 
				1 => "Medium".Colored(Color.yellow), 
				2 => "High".Colored(Color.green), 
			}, "\n"), $"Render Scale: {instance.RenderScale}\n"), $"Clip Distance: {instance.ClipDistance}\n"), $"Detail Distance: {V.DetailDistance}\n"), $"Max Framerate: {instance.MaxFramerate}\n"), $"Camera Render Distance: {instance.CameraRenderDistance}\n"), $"Shadows Quality: {instance.ShadowsQuality}\n"), "Effects:\n"), $"Tone Mapping: {instance.Tonemapping}\n"), $"Anti Aliasing: {instance.AAQuality}\n"), $"Bloom: {instance.UseBloom}\n"), $"Blur: {instance.UseBlur}\n");
			float num = ((SystemInfo.batteryLevel < 0f) ? 1f : SystemInfo.batteryLevel);
			return text2 + "Current Battery: " + ColorExtensions.Colored(color: Color.Lerp(Color.red, Color.green, num), s: $"{Mathf.RoundToInt(num * 100f)}%");
		}

		private void OnDisable()
		{
		}

		public void Send()
		{
			Canvas.renderMode = RenderMode.ScreenSpaceCamera;
			SendButton.SetActive(active: false);
			CloseButton.SetActive(active: false);
			Toggle[] toggles = Toggles;
			for (int i = 0; i < toggles.Length; i++)
			{
				toggles[i].SetActive(active: false);
			}
			foreach (Toggle item in Toggles.Where((Toggle t) => t.isOn))
			{
				_ = item;
			}
			toggles = Toggles;
			for (int i = 0; i < toggles.Length; i++)
			{
				toggles[i].SetActive(active: true);
			}
			SendButton.SetActive(active: true);
			CloseButton.SetActive(active: true);
			Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}
	}
}
