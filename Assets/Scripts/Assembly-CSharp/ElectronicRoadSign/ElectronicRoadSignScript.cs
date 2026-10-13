using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ElectronicRoadSign
{
	public class ElectronicRoadSignScript : MonoBehaviour
	{
		public const string DISPLAYABLE_CHARACTERS = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!$^()-+=<>?,./:'*_\"#;[]{}%&";

		private const int _TEXTURE_COLUMNS = 16;

		private const float _TEXTURE_PANEL_HEIGHT = 90f / 1024f;

		private const float _TEXTURE_PANEL_WIDTH = 0.0625f;

		private const float _TEXTURE_PANEL_MARGIN = 0.001f;

		private readonly int CharCountInLine = 8;

		private readonly int MaxLinesCount = 3;

		public string MessageText = "WARNING ZOMBIES AHEAD!!!";

		public float ScreenDisplayTime = 5f;

		private string[] _lines;

		private string _text;

		private bool _lastLineClosed = true;

		private int[,] _panelUVs;

		private Mesh _mesh;

		private bool _initialized;

		private List<Vector2> roundedUVs;

		private List<Vector2> newUVs;

		private CancellationTokenSource _switchScreenCts;

		private void Start()
		{
			InitializeDisplay().Forget();
		}

		private void Update()
		{
			if (_initialized && _text != MessageText)
			{
				_switchScreenCts?.Cancel();
				_switchScreenCts?.Dispose();
				_switchScreenCts = null;
				ParseText();
			}
		}

		private void OnDestroy()
		{
			_switchScreenCts?.Cancel();
			_switchScreenCts?.Dispose();
			_switchScreenCts = null;
		}

		private async UniTaskVoid InitializeDisplay()
		{
			_initialized = false;
			_panelUVs = new int[24, 4];
			_mesh = GetComponent<MeshFilter>().mesh;
			if (roundedUVs == null)
			{
				roundedUVs = new List<Vector2>();
				_mesh.GetUVs(0, roundedUVs);
				for (int i = 0; i < roundedUVs.Count; i++)
				{
					Vector2 value = roundedUVs[i];
					value.x = Mathf.RoundToInt(roundedUVs[i].x * 1000f);
					value.y = Mathf.RoundToInt(roundedUVs[i].y * 1000f);
					roundedUVs[i] = value;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					int num = j * 8 + k;
					_panelUVs[num, 0] = FindUV(_mesh, new Vector2((float)num * 0.02f, 1f));
					_panelUVs[num, 1] = FindUV(_mesh, new Vector2((float)num * 0.02f + 0.01f, 1f));
					_panelUVs[num, 2] = FindUV(_mesh, new Vector2((float)num * 0.02f, 0.995f));
					_panelUVs[num, 3] = FindUV(_mesh, new Vector2((float)num * 0.02f + 0.01f, 0.995f));
					await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
				}
			}
			_initialized = true;
			roundedUVs.Clear();
			roundedUVs = null;
			newUVs = new List<Vector2>();
			_mesh.GetUVs(0, newUVs);
			ParseText();
		}

		private async UniTaskVoid SwitchScreenAsync(CancellationToken ct = default(CancellationToken))
		{
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(ScreenDisplayTime), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				ClearDisplay();
				DisplayText();
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void ParseText()
		{
			_lastLineClosed = true;
			MessageText = MessageText.ToUpper();
			_text = MessageText;
			string[] array = new string[3] { "            ", "            ", "            " };
			if (!string.IsNullOrEmpty(MessageText) && MessageText != "0")
			{
				int length = MessageText.Length;
				for (int i = 0; i < MaxLinesCount; i++)
				{
					int num = i * CharCountInLine;
					if (num >= length)
					{
						break;
					}
					int length2 = Mathf.Min(CharCountInLine, length - num);
					string text = MessageText.Substring(num, length2);
					array[i] = text;
				}
			}
			_lines = array;
			DisplayText();
		}

		private void DisplayText()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					DisplayCharacter(i, j, (j < _lines[i].Length) ? _lines[i][j] : ' ');
				}
			}
			_mesh.SetUVs(0, newUVs);
		}

		private void ClearDisplay()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					DisplayCharacter(i, j, ' ');
				}
			}
			_mesh.SetUVs(0, newUVs);
		}

		private void DisplayCharacter(int row, int col, char p)
		{
			int num = row * 8 + col;
			int num2 = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!$^()-+=<>?,./:'*_\"#;[]{}%&".IndexOf(p);
			if (num2 < 0)
			{
				num2 = 0;
			}
			float num3 = (float)(num2 % 16) * 0.0625f + 0.001f;
			float num4 = 1f - Mathf.Floor(num2 / 16) * (90f / 1024f) + 0.001f;
			if (newUVs.Count > 0)
			{
				newUVs[_panelUVs[num, 0]] = new Vector2(num3, num4);
				newUVs[_panelUVs[num, 1]] = new Vector2(num3 + 0.0625f, num4);
				newUVs[_panelUVs[num, 2]] = new Vector2(num3, num4 - 90f / 1024f);
				newUVs[_panelUVs[num, 3]] = new Vector2(num3 + 0.0625f, num4 - 90f / 1024f);
			}
		}

		private int FindUV(Mesh mesh, Vector2 uv)
		{
			int num = roundedUVs.IndexOf(new Vector2(Mathf.RoundToInt(uv.x * 1000f), Mathf.RoundToInt(uv.y * 1000f)));
			if (num >= 0)
			{
				return num;
			}
			return -1;
		}
	}
}
