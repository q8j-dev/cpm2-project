using CPM.Scripts.VinylDrawer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StickerItem : MonoBehaviour
{
	public Image sprite;

	public int Order;

	public int stckerIndex;

	public Vector3 position;

	public Vector3 positionIcon;

	public Vector3 euler;

	public Vector3 scale;

	public Color color;

	public Transform ThisVinyl;

	public RawImage backround;

	public float yAngle;

	public float xAngle;

	public int flipX;

	public Color InitialColorBackround;

	public bool isText;

	public string text;

	public int fontIndex;

	public bool italic;

	public bool bold;

	public Vector3 scaleQuadEuler;

	public TMP_Text orderLabel;

	public GameObject checkMark;

	public int oldIndex = -1;

	public Color oldColor;

	private VinylsEditor _vinylsEditor;

	public void Init(VinylsEditor vinylsEditor)
	{
		_vinylsEditor = vinylsEditor;
	}

	public void Select()
	{
		_vinylsEditor.SelectSticker(this);
	}

	public void UpdateOrder()
	{
		orderLabel.text = (Order + 1).ToString();
	}

	private void Awake()
	{
		if (orderLabel == null)
		{
			orderLabel = GetComponent<TMP_Text>();
		}
		if ((bool)checkMark)
		{
			checkMark.SetActive(value: false);
		}
		if (TryGetComponent<Button>(out var component))
		{
			component.onClick.AddListener(Select);
		}
	}
}
