using TouchControlsKit;
using UnityEngine;

public class API_Demo : MonoBehaviour
{
	public bool windowsEnabled;

	private int screenWidth;

	private int screenHeight;

	private Rect hideBtnSize;

	private Rect disBtnSize;

	private Rect leftWindow;

	private Rect rightWindow;

	private bool showingTouchzones = true;

	private void Update()
	{
		if (screenWidth != Screen.width || screenHeight != Screen.height)
		{
			screenWidth = Screen.width;
			screenHeight = Screen.height;
			disBtnSize.x = (float)screenWidth - (float)screenWidth / 100f * 57.5f;
			disBtnSize.y = 5f;
			disBtnSize.width = (float)screenWidth / 100f * 15.25f;
			disBtnSize.height = (float)screenHeight / 14f;
			hideBtnSize.x = (float)screenWidth - (float)screenWidth / 100f * 57.5f;
			hideBtnSize.y = disBtnSize.height + 12f;
			hideBtnSize.width = (float)screenWidth / 100f * 15.25f;
			hideBtnSize.height = (float)screenHeight / 14f;
			leftWindow.x = 5f;
			rightWindow.x = (float)screenWidth - (float)screenWidth / 2.45f;
			ref Rect reference = ref rightWindow;
			float y = (leftWindow.y = 5f);
			reference.y = y;
			ref Rect reference2 = ref rightWindow;
			y = (leftWindow.width = (float)screenWidth / 2.5f);
			reference2.width = y;
			ref Rect reference3 = ref rightWindow;
			y = (leftWindow.height = (float)screenHeight / 2f);
			reference3.height = y;
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(disBtnSize, "Enable / Disable \nControllers"))
		{
			TCKInput.SetActive(!TCKInput.Instance.enabled);
		}
		if (TCKInput.Instance.enabled)
		{
			if (GUI.Button(hideBtnSize, "Show / Hide \nTouch Zones"))
			{
				showingTouchzones = !showingTouchzones;
				TCKInput.ShowingTouchZone(showingTouchzones);
			}
			if (windowsEnabled)
			{
				GUILayout.BeginArea(leftWindow);
				GUILayout.BeginVertical("Box");
				SetGuiStyle("<b>Joystick</b>");
				Axes("Joystick");
				Sens("Joystick");
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
			if (windowsEnabled)
			{
				GUILayout.BeginArea(rightWindow);
				GUILayout.BeginVertical("Box");
				SetGuiStyle("<b>Touchpad</b>");
				Axes("Touchpad");
				Sens("Touchpad");
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
		}
	}

	private void Sens(string ctrlName)
	{
		float sensitivity = TCKInput.GetSensitivity(ctrlName);
		sensitivity = customSlider("Sensitivity", sensitivity, 1f, 10f);
		TCKInput.SetSensitivity(ctrlName, sensitivity);
	}

	private void Axes(string ctrlName)
	{
		GUILayout.BeginHorizontal();
		bool axisEnable = TCKInput.GetAxisEnable(ctrlName, "Horizontal");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Enable X Axis", GUILayout.Width(115f));
		axisEnable = GUILayout.Toggle(axisEnable, string.Empty);
		GUILayout.EndHorizontal();
		TCKInput.SetAxisEnable(ctrlName, "Horizontal", axisEnable);
		if (axisEnable)
		{
			bool axisInverse = TCKInput.GetAxisInverse(ctrlName, "Horizontal");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Inverse X", GUILayout.Width(60f));
			axisInverse = GUILayout.Toggle(axisInverse, string.Empty);
			GUILayout.EndHorizontal();
			TCKInput.SetAxisInverse(ctrlName, "Horizontal", axisInverse);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		bool axisEnable2 = TCKInput.GetAxisEnable(ctrlName, "Vertical");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Enable Y Axis", GUILayout.Width(115f));
		axisEnable2 = GUILayout.Toggle(axisEnable2, string.Empty);
		GUILayout.EndHorizontal();
		TCKInput.SetAxisEnable(ctrlName, "Vertical", axisEnable2);
		if (axisEnable2)
		{
			bool axisInverse2 = TCKInput.GetAxisInverse(ctrlName, "Vertical");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Inverse Y", GUILayout.Width(60f));
			axisInverse2 = GUILayout.Toggle(axisInverse2, string.Empty);
			GUILayout.EndHorizontal();
			TCKInput.SetAxisInverse(ctrlName, "Vertical", axisInverse2);
		}
		GUILayout.EndHorizontal();
	}

	private void SetGuiStyle(string labelName)
	{
		GUIStyle style = GUI.skin.GetStyle("Label");
		style.richText = true;
		style.alignment = TextAnchor.UpperCenter;
		style.normal.textColor = Color.red;
		GUILayout.Label(labelName, style);
		style.richText = false;
		style.alignment = TextAnchor.UpperLeft;
		style.normal.textColor = Color.white;
	}

	private float customSlider(string label, float currentValue, float minValue, float maxValue)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(115f));
		currentValue = GUILayout.HorizontalSlider(currentValue, minValue, maxValue);
		GUILayout.Space(10f);
		GUILayout.Label($"{currentValue:F2}", GUILayout.MaxWidth(50f));
		GUILayout.EndHorizontal();
		return currentValue;
	}
}
