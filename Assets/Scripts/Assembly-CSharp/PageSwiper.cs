using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IDragHandler, IEventSystemHandler, IEndDragHandler
{
	private Vector3 panelLocation;

	public float percentThreshold = 0.2f;

	private const float easing = 0.5f;

	public int totalPages = 1;

	public int currentPage = 1;

	private void Start()
	{
		panelLocation = base.transform.localPosition;
	}

	public void OnDrag(PointerEventData data)
	{
		float x = data.pressPosition.x - data.position.x;
		base.transform.localPosition = panelLocation - new Vector3(x, 0f, 0f);
	}

	public void OnEndDrag(PointerEventData data)
	{
		float num = (data.pressPosition.x - data.position.x) / (float)Screen.width;
		if (Mathf.Abs(num) >= percentThreshold)
		{
			Vector3 endpos = panelLocation;
			if (num > 0f && currentPage < totalPages)
			{
				currentPage++;
				endpos += new Vector3(-Screen.width, 0f, 0f);
			}
			else if (num < 0f && currentPage > 1)
			{
				currentPage--;
				endpos += new Vector3(Screen.width, 0f, 0f);
			}
			StartCoroutine(SmoothMove(base.transform.localPosition, endpos, 0.5f));
			panelLocation = endpos;
		}
		else
		{
			StartCoroutine(SmoothMove(base.transform.localPosition, panelLocation, 0.5f));
		}
	}

	private IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
	{
		float t = 0f;
		while ((double)t <= 1.0)
		{
			t += Time.deltaTime / seconds;
			base.transform.localPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}
	}
}
