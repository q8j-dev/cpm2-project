using System;
using CPM.Database.Constructors;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class DynamicCanvasTire : MonoBehaviour
{
	public Camera canvasCamera;

	public Transform parentVinyls;

	public Material basTextureMat;

	public GameObject DecalPrefab;

	public TextMesh text;

	public TextMesh text2;

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
		WheelConstructor.DynamicCanvasTire = this;
	}

	public void ClearAllVinyls()
	{
		text.text = "";
		text2.text = "";
	}

	private void ChangeTireType(TyreType type)
	{
		Material material = basTextureMat;
		material.mainTexture = type switch
		{
			TyreType.Offroad => offroadTexture, 
			TyreType.Buggy => offroadTexture, 
			TyreType.Touring => touringTexture, 
			_ => sportTexture, 
		};
		WheelConstructor.DynamicCanvasNormal.ChangeTireType(type);
	}

	public RenderTexture CreateNewTextureTire(RenderTexture renderTexture, string textInput)
	{
		int num = 512;
		if (SystemInfo.systemMemorySize > 2100)
		{
			num = 1024;
		}
		if ((bool)text)
		{
			text.text = textInput;
		}
		if (!renderTexture)
		{
			renderTexture = new RenderTexture(num, num, 0, RenderTextureFormat.ARGB32);
			renderTexture.useMipMap = false;
			_collectionsProvider.RegisterRenderTexture(renderTexture);
		}
		if (canvasCamera == null)
		{
			MonoBehaviour.print("canvas cam is null");
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

	public void RenderText(string text, int fontIndex, int fontStyle, Color color, GameObject car, float health)
	{
		VehicleData component = car.GetComponent<VehicleData>();
		Powertrain component2 = car.GetComponent<Powertrain>();
		if ((bool)component.WheelComposition.TireRenderTexture)
		{
			this.text.text = text;
			text2.text = text;
			this.text.color = color;
			text2.color = color;
			canvasCamera.targetTexture = component.WheelComposition.TireRenderTexture;
			PlayerDatas.ItalicOrBold(fontStyle, this.text);
			PlayerDatas.ItalicOrBold(fontStyle, text2);
			Color color2 = degradationLayer.color;
			color2.a = 1f - health;
			degradationLayer.color = color2;
			ChangeFont(fontIndex);
			ChangeTireType(component2.GetTyreType(AxleType.Front));
			RenderAfter(0.01f);
		}
	}

	public void updateHealth(GameObject car, float health)
	{
		VehicleData component = car.GetComponent<VehicleData>();
		canvasCamera.targetTexture = component.WheelComposition.TireRenderTexture;
		if ((bool)component.WheelComposition.TireRenderNormalTexture)
		{
			WheelConstructor.DynamicCanvasNormal.canvasCamera.targetTexture = component.WheelComposition.TireRenderNormalTexture;
		}
		TyreType tyreType = component.GetPowertrain.GetTyreType(AxleType.Front);
		ChangeTireType(tyreType);
		MultiPlayerPosControl component2 = car.GetComponent<MultiPlayerPosControl>();
		int carId = component.CarId;
		string text;
		int indexFont;
		Color color;
		int indexFontStyle;
		if (component2.isOnlineCar)
		{
			text = component2.tireText;
			indexFont = component2.tireFont;
			color = component2.tireTextColor;
			indexFontStyle = component2.tireFontStyle;
		}
		else
		{
			text = ObscuredPrefs.GetString("tireText" + carId);
			indexFont = ObscuredPrefs.GetInt("tireFont" + carId);
			color = PlayerDatas.GetColor("tireTextColor" + carId, Color.white);
			indexFontStyle = ObscuredPrefs.GetInt("tireFontStyle" + carId);
		}
		PlayerDatas.ItalicOrBold(indexFontStyle, this.text);
		PlayerDatas.ItalicOrBold(indexFontStyle, text2);
		this.text.text = text;
		text2.text = text;
		this.text.color = color;
		text2.color = color;
		Color color2 = degradationLayer.color;
		color2.a = 1f - health;
		degradationLayer.color = color2;
		ChangeFont(indexFont);
		RenderAfter(0.01f);
	}

	public void ChangeFont(int indexFont)
	{
		text.font = V.Fonts[indexFont];
		text2.font = V.Fonts[indexFont];
		text.GetComponent<Renderer>().sharedMaterial = V.Fonts[indexFont].material;
		text2.GetComponent<Renderer>().sharedMaterial = V.Fonts[indexFont].material;
		WheelConstructor.DynamicCanvasNormal.CopyText(text, 1f - degradationLayer.color.a);
	}

	public void ChangeTextScale(int size, int font)
	{
		Vector3 localScale = KofSize(size) * GetFontKof(font) * 1.2f;
		SetFontPos(font);
		text.transform.localScale = localScale;
		text2.transform.localScale = localScale;
		WheelConstructor.DynamicCanvasNormal.CopyText(text, 1f - degradationLayer.color.a);
		Render();
	}

	private float GetFontKof(int font)
	{
		switch (font)
		{
		case 0:
			return 1f;
		case 4:
		case 5:
		case 6:
			return 1.3f;
		case 9:
			return 1.1f;
		case 10:
		case 11:
			return 1.3f;
		case 7:
			return 1.1f;
		default:
			return 1.2f;
		}
	}

	private void SetFontPos(int font)
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

	private Vector3 KofSize(int size)
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
