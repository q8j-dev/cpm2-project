using DG.Tweening;
using TMPro;
using UnityEngine;

public class CircuitTopUserMeshLabel : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro NameText;

	[SerializeField]
	private TextMeshPro PlaceText;

	[SerializeField]
	private Renderer[] Renderers;

	private Material[] _cloneMat;

	public void SetCamera(Transform cameraTransform)
	{
		if (Renderers.Length != 0)
		{
			_cloneMat = new Material[Renderers.Length];
			for (int i = 0; i < Renderers.Length; i++)
			{
				_cloneMat[i] = new Material(Renderers[i].material);
				Renderers[i].sharedMaterial = _cloneMat[i];
			}
		}
	}

	public void InitTexts(string racerName, string place)
	{
		NameText.text = racerName;
		PlaceText.text = place;
	}

	public void SetPlace(string place)
	{
		PlaceText.text = place;
	}

	public void SetAlpha(float alpha)
	{
		NameText.DOKill();
		NameText.DOFade(alpha, 0.5f);
		PlaceText.DOKill();
		PlaceText.DOFade(alpha, 0.5f);
		Renderer[] renderers = Renderers;
		foreach (Renderer obj in renderers)
		{
			obj.sharedMaterial.DOKill();
			obj.sharedMaterial.DOFade(alpha, 0.5f);
		}
	}

	private void OnDestroy()
	{
		if (_cloneMat == null || _cloneMat.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _cloneMat.Length; i++)
		{
			if ((bool)_cloneMat[i])
			{
				CommonExtensions.DestroyMat(ref _cloneMat[i]);
			}
		}
	}
}
