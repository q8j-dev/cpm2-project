using System;
using CPM.Database.Constructors;
using CPMEngine;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class DynamicCanvasNormalTire : MonoBehaviour
{
	public Camera canvasCamera;

	public Transform parentVinyls;

	public Material basTextureMat;

	public GameObject DecalPrefab;

	public TextMesh text;

	public TextMesh textBack;

	public TextMesh text2;

	public TextMesh text2Back;

	private Vector3 posText1;

	private Vector3 posText2;

	public Texture offroadTexture;

	public Texture touringTexture;

	public Texture sportTexture;

	public SpriteRenderer degradationLayer;

	private GlobalCollectionsProvider _collectionsProvider;

	private IDisposable _renderDisposable;

	[Inject]
	public void Construct(GlobalCollectionsProvider collectionsProvider)
	{
		_collectionsProvider = collectionsProvider;
	}

	private void Awake()
	{
		canvasCamera.enabled = false;
		canvasCamera.targetTexture = null;
		posText1 = text.transform.localPosition;
		posText2 = text2.transform.localPosition;
		WheelConstructor.DynamicCanvasNormal = this;
	}

	public void ChangeTireType(TyreType type)
	{
		Material material = basTextureMat;
		material.mainTexture = type switch
		{
			TyreType.Offroad => offroadTexture, 
			TyreType.Buggy => offroadTexture, 
			TyreType.Touring => touringTexture, 
			_ => sportTexture, 
		};
		RenderAfter(0.01f);
	}

	public RenderTexture CreateNewTextureTire(RenderTexture renderTexture, string textInput)
	{
		int num = 512;
		text.text = textInput;
		if (!renderTexture)
		{
			renderTexture = new RenderTexture(num, num, 0, RenderTextureFormat.ARGB32);
			renderTexture.useMipMap = false;
			renderTexture.name = "tireRTN";
			_collectionsProvider.RegisterRenderTexture(renderTexture);
		}
		canvasCamera.targetTexture = renderTexture;
		Render();
		return renderTexture;
	}

	public void RenderAfter(float delay)
	{
		_renderDisposable?.Dispose();
		_renderDisposable = Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe((long _) =>
		{
			Render();
		}).AddTo(this);
	}

	public void CopyText(TextMesh textMesh, float health)
	{
		text.font = textMesh.font;
		text2Back.font = textMesh.font;
		text2.font = textMesh.font;
		textBack.font = textMesh.font;
		text.text = textMesh.text;
		text.transform.localScale = textMesh.transform.localScale;
		textBack.text = textMesh.text;
		textBack.transform.localScale = Vector3.one * 1.01f;
		text2.text = textMesh.text;
		text2.transform.localScale = textMesh.transform.localScale;
		text2Back.text = textMesh.text;
		text2Back.transform.localScale = Vector3.one * 1.01f;
		text.GetComponent<Renderer>().sharedMaterial = textMesh.GetComponent<MeshRenderer>().material;
		text2.GetComponent<Renderer>().sharedMaterial = textMesh.GetComponent<MeshRenderer>().material;
		textBack.GetComponent<Renderer>().sharedMaterial = textMesh.GetComponent<MeshRenderer>().material;
		text2Back.GetComponent<Renderer>().sharedMaterial = textMesh.GetComponent<MeshRenderer>().material;
		int indexFontStyle = 0;
		if (textMesh.fontStyle == FontStyle.Italic)
		{
			indexFontStyle = 1;
		}
		else if (textMesh.fontStyle == FontStyle.Bold)
		{
			indexFontStyle = 2;
		}
		else if (textMesh.fontStyle == FontStyle.BoldAndItalic)
		{
			indexFontStyle = 3;
		}
		PlayerDatas.ItalicOrBold(indexFontStyle, text);
		PlayerDatas.ItalicOrBold(indexFontStyle, textBack);
		PlayerDatas.ItalicOrBold(indexFontStyle, text2);
		PlayerDatas.ItalicOrBold(indexFontStyle, text2Back);
		Color color = degradationLayer.color;
		color.a = 1f - health;
		degradationLayer.color = color;
		RenderAfter(0.01f);
	}

	public void RenderText(string text, int fontIndex, int fontStyle, Color color, GameObject car)
	{
		VehicleData component = car.GetComponent<VehicleData>();
		if ((bool)component.WheelComposition.TireRenderTexture)
		{
			this.text.text = text;
			text2.text = text;
			this.text.color = color;
			text2.color = color;
			canvasCamera.targetTexture = component.WheelComposition.TireRenderTexture;
			PlayerDatas.ItalicOrBold(fontStyle, this.text);
			PlayerDatas.ItalicOrBold(fontStyle, text2);
			ChangeFont(fontIndex);
			RenderAfter(0.01f);
		}
	}

	public void ChangeFont(int indexFont)
	{
		text.font = V.Fonts[indexFont];
		text2.font = V.Fonts[indexFont];
		text.GetComponent<Renderer>().sharedMaterial = V.Fonts[indexFont].material;
		text2.GetComponent<Renderer>().sharedMaterial = V.Fonts[indexFont].material;
	}

	public void ChangeTextScale(int size, int font)
	{
		Vector3 localScale = kofSize(size) * getFontKof(font);
		setFontPos(font);
		text.transform.localScale = localScale;
		text2.transform.localScale = localScale;
		Render();
	}

	public float getFontKof(int font)
	{
		return font switch
		{
			0 => 1f, 
			4 => 1.3f, 
			5 => 1.3f, 
			6 => 1.3f, 
			9 => 1.1f, 
			10 => 1.3f, 
			11 => 1.3f, 
			7 => 1.1f, 
			_ => 1.2f, 
		};
	}

	private void setFontPos(int font)
	{
		switch (font)
		{
		case 8:
		{
			Vector3 localPosition2 = posText1;
			localPosition2.y -= 0.01f;
			text.transform.localPosition = localPosition2;
			localPosition2 = posText2;
			localPosition2.y -= 0.01f;
			text2.transform.localPosition = localPosition2;
			break;
		}
		case 7:
		{
			Vector3 localPosition = posText1;
			localPosition.y -= 0.005f;
			text.transform.localPosition = localPosition;
			localPosition = posText2;
			localPosition.y -= 0.005f;
			text2.transform.localPosition = localPosition;
			break;
		}
		default:
			text.transform.localPosition = posText1;
			text2.transform.localPosition = posText2;
			break;
		}
	}

	private void Render()
	{
		canvasCamera.Render();
	}

	public void SetTextColor(Color color)
	{
		text.color = color;
		text2.color = color;
		canvasCamera.Render();
	}

	public bool isCleared()
	{
		int num = 0;
		foreach (Transform parentVinyl in parentVinyls)
		{
			_ = parentVinyl;
			num++;
		}
		return num == 0;
	}

	private Vector3 kofSize(int size)
	{
		return size switch
		{
			0 => new Vector3(8f, 7f, 1f), 
			1 => new Vector3(10f, 7f, 1f), 
			2 => new Vector3(11f, 10f, 1f), 
			3 => new Vector3(12f, 10f, 1f), 
			4 => new Vector3(12f, 10f, 1f), 
			5 => new Vector3(10f, 8f, 1f), 
			6 => new Vector3(10f, 7f, 1f), 
			7 => new Vector3(15f, 9f, 1f), 
			_ => new Vector3(1f, 1f, 1f), 
		};
	}
}
