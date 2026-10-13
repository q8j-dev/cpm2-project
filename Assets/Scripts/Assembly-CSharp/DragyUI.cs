using TMPro;
using UnityEngine;

public class DragyUI : MonoBehaviour
{
	public TextMeshProUGUI label0_100;

	public TextMeshProUGUI label0_100Slope;

	public GameObject valid0_100;

	public GameObject inValid0_100;

	public TextMeshProUGUI label100_200;

	public TextMeshProUGUI label100_200Slope;

	public GameObject valid100_200;

	public GameObject inValid100_200;

	public TextMeshProUGUI label0_60;

	public TextMeshProUGUI label0_60Slope;

	public GameObject valid0_60;

	public GameObject inValid0_60;

	public TextMeshProUGUI label402;

	public TextMeshProUGUI label402Slope;

	public GameObject valid402;

	public GameObject inValid402;

	public void ResetValues()
	{
		clearResult();
		GameObject gameObject = V.Car;
		if (gameObject == null)
		{
			gameObject = V.LastUsedCar;
		}
		if ((bool)gameObject)
		{
			Dragy component = gameObject.GetComponent<Dragy>();
			component.kmh100Result = 0f;
			component.kmh100slope = 0f;
			component.kmh100200Result = 0f;
			component.kmh100200Slope = 0f;
			component.mph60Result = 0f;
			component.mph60Slope = 0f;
			component.m402Result = 0f;
			component.m402Slope = 0f;
		}
	}

	public void clearResult()
	{
		if (label0_60.gameObject.activeSelf)
		{
			label0_60.text = "-";
			label0_100.text = "-";
			label0_60Slope.text = "";
			label0_100Slope.text = "";
			label100_200.text = "-";
			label100_200Slope.text = "";
			label402.text = "-";
			label402Slope.text = "";
			valid0_60.SetActive(value: false);
			valid0_100.SetActive(value: false);
			inValid0_60.SetActive(value: false);
			inValid0_100.SetActive(value: false);
			inValid402.SetActive(value: false);
			valid402.SetActive(value: false);
			valid100_200.SetActive(value: false);
			inValid100_200.SetActive(value: false);
		}
	}

	public void OnDraggyOpen()
	{
		clearResult();
		GameObject gameObject = V.Car;
		if (gameObject == null)
		{
			gameObject = V.LastUsedCar;
		}
		if ((bool)gameObject)
		{
			bool flag = false;
			Dragy component = gameObject.GetComponent<Dragy>();
			label0_100.text = ((component.kmh100Result == 0f) ? "-" : (component.kmh100Result.ToString("F2") + "s"));
			label0_100Slope.text = ((component.kmh100slope == 0f) ? "-" : (component.kmh100slope.ToString("F2") + "%"));
			if (component.kmh100slope != 0f)
			{
				flag = component.kmh100slope > -1f;
				inValid0_100.SetActive(!flag);
				valid0_100.SetActive(flag);
			}
			label100_200.text = ((component.kmh100200Result == 0f) ? "-" : (component.kmh100200Result.ToString("F2") + "s"));
			label100_200Slope.text = ((component.kmh100200Slope == 0f) ? "-" : (component.kmh100200Slope.ToString("F2") + "%"));
			if (component.kmh100200Slope != 0f)
			{
				flag = component.kmh100200Slope > -1f;
				inValid100_200.SetActive(!flag);
				valid100_200.SetActive(flag);
			}
			label0_60.text = ((component.mph60Result == 0f) ? "-" : (component.mph60Result.ToString("F2") + "s"));
			label0_60Slope.text = ((component.mph60Slope == 0f) ? "-" : (component.mph60Slope.ToString("F2") + "%"));
			if (component.mph60Slope != 0f)
			{
				flag = component.mph60Slope > -1f;
				inValid0_60.SetActive(!flag);
				valid0_60.SetActive(flag);
			}
			label402.text = ((component.m402Result == 0f) ? "-" : (component.m402Result.ToString("F2") + "s"));
			label402Slope.text = ((component.m402Slope == 0f) ? "-" : (component.m402Slope.ToString("F2") + "%"));
			if (component.m402Slope != 0f)
			{
				flag = component.m402Slope > -1f;
				inValid402.SetActive(!flag);
				valid402.SetActive(flag);
			}
		}
	}
}
