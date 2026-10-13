using System;
using UnityEngine;

[Serializable]
public struct CircularMenuPanelConfig
{
	public int amountOfElements;

	public Vector2 cellPivot;

	public Vector2 cellSize;

	public Sprite normalSprite;

	public Sprite hoveredSprite;

	public Sprite pressedSprite;

	public float itemXOffset;

	public float itemYOffset;
}
