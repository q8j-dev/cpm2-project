using UnityEngine;
using UnityEngine.UI;

namespace Crystal
{
	public class SafeAreaLayoutElement : MonoBehaviour
	{
		private readonly SafeArea.SimDevice Sim;

		private Rect[] NSA_iPhoneX = new Rect[2]
		{
			new Rect(0f, 0.04187192f, 1f, 0.9039409f),
			new Rect(0.054187194f, 0.056f, 0.89162564f, 0.944f)
		};

		private Rect[] NSA_iPhoneXsMax = new Rect[2]
		{
			new Rect(0f, 0.03794643f, 1f, 0.9129464f),
			new Rect(0.04910714f, 7f / 138f, 101f / 112f, 131f / 138f)
		};

		private Rect[] NSA_Pixel3XL_LSL = new Rect[2]
		{
			new Rect(0f, 0f, 1f, 0.94222975f),
			new Rect(0f, 0f, 0.94222975f, 1f)
		};

		private Rect[] NSA_Pixel3XL_LSR = new Rect[2]
		{
			new Rect(0f, 0f, 1f, 0.94222975f),
			new Rect(0.05777027f, 0f, 0.94222975f, 1f)
		};

		private LayoutElement LayoutElement;

		private Rect LastSafeArea = new Rect(0f, 0f, 0f, 0f);

		private Vector2Int LastScreenSize = new Vector2Int(0, 0);

		private ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;

		private float InitWidth;

		private float InitHeight;

		[SerializeField]
		private bool ConformX = true;

		[SerializeField]
		private bool IsLeft = true;

		[SerializeField]
		private bool ConformY = true;

		[SerializeField]
		private bool IsBottom = true;

		[SerializeField]
		private bool Logging;

		private void Awake()
		{
			LayoutElement = GetComponent<LayoutElement>();
			if (LayoutElement == null)
			{
				Debug.LogError("Cannot apply safe area - no LayoutElement found on " + base.name);
				Object.Destroy(this);
			}
			else
			{
				InitWidth = Mathf.Max(LayoutElement.minWidth, LayoutElement.preferredWidth);
				InitHeight = Mathf.Max(LayoutElement.minHeight, LayoutElement.preferredHeight);
				Refresh();
			}
		}

		private void LateUpdate()
		{
			Refresh();
		}

		private void Refresh()
		{
			Rect safeArea = GetSafeArea();
			if (safeArea != LastSafeArea || Screen.width != LastScreenSize.x || Screen.height != LastScreenSize.y || Screen.orientation != LastOrientation)
			{
				LastScreenSize.x = Screen.width;
				LastScreenSize.y = Screen.height;
				LastOrientation = Screen.orientation;
				ApplySafeArea(safeArea);
			}
		}

		private Rect GetSafeArea()
		{
			Rect result = Screen.safeArea;
			if (Application.isEditor && Sim != SafeArea.SimDevice.None)
			{
				Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
				switch (Sim)
				{
				case SafeArea.SimDevice.iPhoneX:
					rect = ((Screen.height <= Screen.width) ? NSA_iPhoneX[1] : NSA_iPhoneX[0]);
					break;
				case SafeArea.SimDevice.iPhoneXsMax:
					rect = ((Screen.height <= Screen.width) ? NSA_iPhoneXsMax[1] : NSA_iPhoneXsMax[0]);
					break;
				case SafeArea.SimDevice.Pixel3XL_LSL:
					rect = ((Screen.height <= Screen.width) ? NSA_Pixel3XL_LSL[1] : NSA_Pixel3XL_LSL[0]);
					break;
				case SafeArea.SimDevice.Pixel3XL_LSR:
					rect = ((Screen.height <= Screen.width) ? NSA_Pixel3XL_LSR[1] : NSA_Pixel3XL_LSR[0]);
					break;
				}
				result = new Rect((float)Screen.width * rect.x, (float)Screen.height * rect.y, (float)Screen.width * rect.width, (float)Screen.height * rect.height);
			}
			return result;
		}

		private void ApplySafeArea(Rect r)
		{
			LastSafeArea = r;
			if (!ConformX)
			{
				r.x = 0f;
				r.width = Screen.width;
			}
			if (!ConformY)
			{
				r.y = 0f;
				r.height = Screen.height;
			}
			if (Screen.width > 0 && Screen.height > 0)
			{
				Vector2 position = r.position;
				Vector2 vector = r.position + r.size;
				if (position.x >= 0f && position.y >= 0f && vector.x >= 0f && vector.y >= 0f)
				{
					if (ConformX)
					{
						float num = (IsLeft ? position.x : ((float)Screen.width - vector.x));
						LayoutElement layoutElement = LayoutElement;
						float minWidth = (LayoutElement.preferredWidth = num + InitWidth);
						layoutElement.minWidth = minWidth;
					}
					if (ConformY)
					{
						float num3 = (IsBottom ? position.y : ((float)Screen.height - vector.y));
						LayoutElement layoutElement2 = LayoutElement;
						float minWidth = (LayoutElement.preferredHeight = num3 + InitHeight);
						layoutElement2.minHeight = minWidth;
					}
				}
			}
			if (Logging)
			{
				Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}", base.name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
			}
		}
	}
}
