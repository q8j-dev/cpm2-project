using CPMEngine;
using UnityEngine;

[ExecuteInEditMode]
public class DashBoard : MonoBehaviour
{
	public enum Docking
	{
		Left = 0,
		Right = 1
	}

	public enum Unit
	{
		Kmh = 0,
		Mph = 1
	}

	public enum SpeedoMeterType
	{
		RigidBody = 0,
		Wheel = 1
	}

	public int depth = 2;

	public Texture2D tachoMeter;

	[HideInInspector]
	public bool tachoMeterDisabled;

	public Vector2 tachoMeterPosition;

	public Docking tachoMeterDocking;

	public float tachoMeterDimension = 1f;

	public Texture2D tachoMeterNeedle;

	public Vector2 tachoMeterNeedleSize;

	public float tachoMeterNeedleAngle;

	public float RPMFactor = 3.5714f;

	public Texture2D speedoMeter;

	public Texture2D torqueMeter;

	[HideInInspector]
	public bool speedoMeterDisabled;

	public Vector2 speedoMeterPosition;

	public Docking speedoMeterDocking = Docking.Right;

	public float speedoMeterDimension = 1f;

	public Texture2D speedoMeterNeedle;

	public Vector2 speedoMeterNeedleSize;

	public float speedoMeterNeedleAngle;

	public float speedoMeterFactor;

	public Vector2 torqueMeterPosition;

	public Docking torqueMeterDocking = Docking.Right;

	public float torqueMeterDimension = 1f;

	public Texture2D torqueMeterNeedle;

	public Vector2 torqueMeterNeedleSize;

	public float torqueMeterNeedleAngle;

	public float torqueMeterFactor;

	public SpeedoMeterType speedoMeterType;

	public SpeedoMeterType torqueMeterType;

	public Unit digitalSpeedoUnit;

	public GUIStyle digitalSpeedoStyle;

	public Vector2 digitalSpeedoPosition;

	public Docking digitalSpeedoDocking;

	public GUIStyle gearMonitorStyle;

	public Vector2 gearMonitorPosition;

	public Docking gearMonitorDocking;

	public Texture2D clutchMonitor;

	public Texture2D throttleMonitor;

	public Texture2D brakeMonitor;

	public Vector2 pedalsMonitorPosition;

	public Texture2D ABS;

	public Texture2D TCS;

	public Texture2D ESP;

	public Vector2 dashboardLightsPosition;

	public Docking dashboardLightsDocking;

	public float dashboardLightsDimension = 1f;

	public GameObject digitalSpeedoOnBoard;

	public GameObject digitalGearOnBoard;

	public GameObject tachoMeterNeedleOnBoard;

	public GameObject speedoMeterNeedleOnBoard;

	public TextMesh textMeshSpeed;

	public TextMesh textMeshGear;

	public bool CarChanged;

	[HideInInspector]
	public bool showGUIDashboard = true;

	private float factor;

	private int sign = 1;

	private int shift;

	public float differ;

	public float differ2;

	public float kf;

	public float kf2;

	public float tachoDiff;

	public float tachoKoff;

	public float speedoDiff;

	public float speedoKoff;

	private void Start()
	{
		if (base.transform.parent != null)
		{
			Object.Destroy(base.gameObject);
		}
		if (digitalSpeedoOnBoard != null)
		{
			textMeshSpeed = digitalSpeedoOnBoard.GetComponent<TextMesh>();
		}
		if (digitalSpeedoUnit == Unit.Kmh)
		{
			factor = 3.6f;
		}
		else
		{
			factor = 2.237f;
		}
		if ((bool)speedoMeter && (bool)tachoMeter)
		{
			updateValues(Screen.width, Screen.height);
		}
	}

	public void updateValues(int w, int h)
	{
	}

	public float scale_widht(float input)
	{
		return (float)Screen.width * input / 1366f;
	}

	private float scale_height(float input)
	{
		return (float)Screen.height * input / 730f;
	}

	public float scale_widht2(float input, int resolution)
	{
		return (float)resolution * input / 1366f;
	}

	private float scale_height2(float input, int resolution)
	{
		return (float)resolution * input / 730f;
	}
}
