using UnityEngine;

public static class TextureExtensions
{
	public static Sprite ToSprite(this Texture texture)
	{
		if (texture == null)
		{
			Debug.LogError("Input texture is null. Cannot create sprite.");
			return null;
		}
		RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 32);
		Graphics.Blit(texture, renderTexture);
		Texture2D texture2D = new Texture2D(texture.width, texture.height);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, texture.width, texture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		Object.Destroy(renderTexture);
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.one * 0.5f);
	}
}
