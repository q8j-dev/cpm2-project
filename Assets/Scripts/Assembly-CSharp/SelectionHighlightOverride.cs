using UnityEngine;
using UnityEngine.UI;

public class SelectionHighlightOverride
{
	private const bool _debug = false;

	private Selectable _overridden;

	private Selectable.Transition _overriddenTransition;

	private SpriteState _originalSpriteState;

	private ColorBlock _originalColors;

	public void Apply(GameObject selected)
	{
		Selectable component = null;
		if ((bool)selected)
		{
			selected.TryGetComponent<Selectable>(out component);
		}
		if (component == _overridden)
		{
			return;
		}
		Restore();
		if (!component)
		{
			return;
		}
		switch (component.transition)
		{
		case Selectable.Transition.SpriteSwap:
		{
			SpriteState spriteState = component.spriteState;
			if (spriteState.selectedSprite == null && spriteState.highlightedSprite != null)
			{
				_originalSpriteState = spriteState;
				spriteState.selectedSprite = spriteState.highlightedSprite;
				component.spriteState = spriteState;
				_overridden = component;
				_overriddenTransition = Selectable.Transition.SpriteSwap;
			}
			break;
		}
		case Selectable.Transition.ColorTint:
		{
			ColorBlock colors = component.colors;
			if (colors.selectedColor == colors.normalColor && colors.highlightedColor != colors.normalColor)
			{
				_originalColors = colors;
				colors.selectedColor = colors.highlightedColor;
				component.colors = colors;
				_overridden = component;
				_overriddenTransition = Selectable.Transition.ColorTint;
			}
			break;
		}
		}
	}

	public void Restore()
	{
		if ((bool)_overridden && _overridden.transition == _overriddenTransition)
		{
			switch (_overriddenTransition)
			{
			case Selectable.Transition.SpriteSwap:
				_overridden.spriteState = _originalSpriteState;
				break;
			case Selectable.Transition.ColorTint:
				_overridden.colors = _originalColors;
				break;
			}
		}
		_overridden = null;
	}
}
