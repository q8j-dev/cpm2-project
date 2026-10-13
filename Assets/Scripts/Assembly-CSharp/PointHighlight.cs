using System.Collections;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;
using UnityEngine.UI;

public class PointHighlight : MonoBehaviour
{
	private class HighLight
	{
		public Text mText;

		public ChartItemEffect mPoint;

		public CharItemEffectController mControl;

		public HighLight(Text t, ChartItemEffect p)
		{
			mText = t;
			mPoint = p;
		}
	}

	public Text TextPrefab;

	public ChartItemEffect PointHoverPrefab;

	public int FontSize = 5;

	public Vector3 TextOffset;

	private int fractionDigits;

	private GraphChart mChart;

	private List<HighLight> mItems = new List<HighLight>();

	private List<HighLight> mRemoved = new List<HighLight>();

	private void Start()
	{
		ItemLabels component = GetComponent<ItemLabels>();
		if (component != null)
		{
			fractionDigits = component.FractionDigits;
		}
		else
		{
			fractionDigits = 2;
		}
		GraphChart component2 = GetComponent<GraphChart>();
		if (component2 != null)
		{
			mChart = component2;
		}
	}

	private void Update()
	{
		mRemoved.RemoveAll((HighLight x) =>
		{
			if (!x.mControl.enabled)
			{
				ChartCommon.SafeDestroy(x.mText.gameObject);
				ChartCommon.SafeDestroy(x.mPoint.gameObject);
				return true;
			}
			return false;
		});
	}

	private IEnumerator SelectText(Text text, GameObject point)
	{
		yield return new WaitForEndOfFrame();
		if (text != null)
		{
			ChartItemEvents component = text.GetComponent<ChartItemEvents>();
			if (component != null)
			{
				component.OnMouseHover.Invoke(component.gameObject);
			}
			component = point.GetComponent<ChartItemEvents>();
			if (component != null)
			{
				component.OnMouseHover.Invoke(component.gameObject);
			}
		}
	}

	private void ClearHighLight()
	{
		for (int i = 0; i < mItems.Count; i++)
		{
			RemoveText(mItems[i]);
		}
		mItems.Clear();
	}

	private void RemoveText(HighLight h)
	{
		if (h.mText != null)
		{
			ChartItemEvents component = h.mText.GetComponent<ChartItemEvents>();
			h.mControl = h.mText.GetComponent<CharItemEffectController>();
			if (component != null && h.mControl != null)
			{
				component.OnMouseLeave.Invoke(component.gameObject);
				mRemoved.Add(h);
			}
			else
			{
				ChartCommon.SafeDestroy(h.mText);
			}
			component = h.mPoint.GetComponent<ChartItemEvents>();
			if (component != null)
			{
				component.OnMouseLeave.Invoke(component.gameObject);
			}
			else
			{
				ChartCommon.SafeDestroy(h.mPoint);
			}
		}
	}

	private void PopText(string data, Vector3 position, bool worldPositionStays)
	{
		if (!(GetComponentInParent<Canvas>() == null) && !(TextPrefab == null))
		{
			ClearHighLight();
			GameObject gameObject = Object.Instantiate(PointHoverPrefab.gameObject, position, Quaternion.identity);
			GameObject gameObject2 = Object.Instantiate(TextPrefab.gameObject, position + TextOffset, Quaternion.identity);
			Text component = gameObject2.GetComponent<Text>();
			component.maskable = false;
			component.text = data;
			component.fontSize = FontSize;
			gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			if (worldPositionStays)
			{
				gameObject2.transform.position = position + TextOffset;
				gameObject.transform.position = position;
			}
			else
			{
				gameObject2.transform.localPosition = position + TextOffset;
				gameObject.transform.localPosition = position;
			}
			Vector3 localPosition = gameObject2.transform.localPosition;
			localPosition.z = 0f;
			gameObject2.transform.localPosition = localPosition;
			localPosition = gameObject.transform.localPosition;
			localPosition.z = 0f;
			gameObject.transform.localPosition = localPosition;
			mItems.Add(new HighLight(component, gameObject.GetComponent<ChartItemEffect>()));
			StartCoroutine(SelectText(component, gameObject));
		}
	}

	public void HighlightPoint(string category, int index)
	{
		DoubleVector3 point = mChart.DataSource.GetPoint(category, index);
		string data = mChart.FormatItem(point.x, point.y);
		if (mChart.PointToWorldSpace(out var result, point.x, point.y, category))
		{
			PopText(data, result, worldPositionStays: true);
		}
	}
}
