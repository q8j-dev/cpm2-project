using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class HDHeadLightFlare : MonoBehaviour
{
	public HDHeadLightFlareType FlareType;

	private MeshRenderer Rend;

	private Material FlareMat;

	private int matid;

	private Tweener tw;

	private bool IsOn;

	private float flareValue;

	private void Awake()
	{
		Rend = GetComponent<MeshRenderer>();
		FlareMat = Rend.material;
		matid = Shader.PropertyToID("_FlarePower");
		flareValue = 0f;
		IsOn = false;
		Rend.enabled = false;
		FlareMat.SetFloat(matid, flareValue);
		base.transform.localScale = Vector3.one;
	}

	public void Switch(bool isOn, float switchTime = 0.1f)
	{
		if (IsOn == isOn)
		{
			return;
		}
		IsOn = isOn;
		if (IsOn && (bool)Rend)
		{
			Rend.enabled = true;
		}
		float num = (IsOn ? 1f : 0f);
		if (tw != null && tw.IsActive())
		{
			tw.Kill();
		}
		tw = DOTween.To(() => flareValue, (float x) =>
		{
			flareValue = x;
		}, num, Mathf.Abs(num - flareValue) * switchTime).SetEase(Ease.InOutQuad).OnUpdate(() =>
		{
			if ((bool)FlareMat)
			{
				FlareMat.SetFloat(matid, flareValue);
			}
		})
			.OnComplete(() =>
			{
				if ((bool)FlareMat)
				{
					FlareMat.SetFloat(matid, flareValue);
				}
				if (!IsOn && (bool)Rend)
				{
					Rend.enabled = false;
				}
			});
	}

	private void OnDestroy()
	{
		if (tw != null && tw.IsActive())
		{
			tw.Kill();
		}
	}
}
