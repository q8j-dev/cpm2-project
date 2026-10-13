#nullable enable annotations
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class Node
	{
		public int index;

		public int order;

		public int carNumber;

		public Transform transform;

		public RectTransform rect;

		public Node parent;

		public Node? left;

		public Node? right;

		private Texture texture;

		public Node(Transform transform, [CanBeNull] Node parent, int index)
		{
			rect = transform.GetComponent<RectTransform>();
			this.index = index;
			this.transform = transform;
			right = null;
			left = null;
			this.parent = parent;
		}

		public Node([CanBeNull] Node parent, int carNumber, int index, int order)
		{
			this.carNumber = carNumber;
			this.index = index;
			this.order = order;
			right = null;
			left = null;
			this.parent = parent;
		}

		public override string ToString()
		{
			if (left != null && right != null)
			{
				return transform.name + "\nright: " + right.transform.name + "\nleft: " + left.transform.name;
			}
			if (left != null)
			{
				return transform.name + "\nleft: " + left.transform.name;
			}
			if (right != null)
			{
				return transform.name + "\nright: " + right.transform.name;
			}
			return transform.name;
		}

		public Texture GetTexture()
		{
			return texture;
		}

		public void FixPosition(Vector3 position, Vector2 scale, float offset, float radius)
		{
			transform.SetAsFirstSibling();
			RectTransform component = transform.GetComponent<RectTransform>();
			component.anchoredPosition3D = position;
			float num = Mathf.Abs((float)(Screen.width / 2) - component.anchoredPosition3D.x) / 100f;
			float num2 = radius - Mathf.Sqrt(Mathf.Abs(radius * radius - num * num));
			num2 *= 100f;
			left?.FixPosition(new Vector3(position.x - scale.x / 2f * offset, num2, position.z), scale, offset, radius);
			right?.FixPosition(new Vector3(position.x + scale.x / 2f * offset, num2, position.z), scale, offset, radius);
		}

		public void SmoothFixPosition(Vector3 direction)
		{
			transform.Translate(direction * Time.deltaTime);
			left?.SmoothFixPosition(direction);
			right?.SmoothFixPosition(direction);
		}

		public void FixScale(float size, float alpha)
		{
			transform.SetAsFirstSibling();
			float num = ((rect.anchoredPosition3D.x > (float)Screen.width / 2f) ? ((float)Screen.width - rect.anchoredPosition3D.x) : rect.anchoredPosition3D.x);
			rect.sizeDelta = new Vector2(((float)Screen.width / 2f + num) / 4f / size, ((float)Screen.width / 2f + num) / 8f / size);
			rect.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(((float)Screen.width / 2f + num) / 4f / size, ((float)Screen.width / 2f + num) / 8f / size);
			rect.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(((float)Screen.width / 2f + num) / 4f / size, ((float)Screen.width / 2f + num) / 24f / size);
			Color color = transform.GetChild(1).GetComponent<RawImage>().color;
			float num2 = ((float)Screen.width / 2f + num) / (float)Screen.width * alpha;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			else if (num2 > 1f)
			{
				num2 = 1f;
			}
			transform.GetChild(1).GetComponent<RawImage>().color = new Color(color.r, color.g, color.b, num2);
			color = transform.GetChild(0).GetComponent<RawImage>().color;
			transform.GetChild(0).GetComponent<RawImage>().color = new Color(color.r, color.g, color.b, num2);
			left?.FixScale(size, alpha);
			right?.FixScale(size, alpha);
		}

		public void Move(double? direction, float radius)
		{
			RectTransform component = transform.GetComponent<RectTransform>();
			if (index == 0)
			{
				_ = (0.0 - direction).GetValueOrDefault() * (double)component.rect.width / (double)Screen.width / 4.0;
			}
			transform.Translate(new Vector3((float)(0.0 - direction).GetValueOrDefault() / 10f, 0f, 0f) * Time.deltaTime);
			float num = Mathf.Abs((float)(Screen.width / 2) - component.anchoredPosition3D.x) / 100f;
			float num2 = radius - Mathf.Sqrt(Mathf.Abs(radius * radius - num * num));
			num2 *= 100f;
			component.anchoredPosition3D = new Vector3(component.anchoredPosition3D.x, num2, component.anchoredPosition3D.z);
			component.GetChild(0).GetComponent<RectTransform>().anchoredPosition3D = new Vector2(0f, (0f - component.rect.width) / 6f);
			left?.Move(direction, radius);
			right?.Move(direction, radius);
		}

		public void CreatePanels(GameObject prefab, Transform transformParent, (bool usePath, string path, List<Texture> referenceTexture) options)
		{
			GameObject gameObject = Object.Instantiate(prefab, transformParent);
			transform = gameObject.transform;
			rect = gameObject.transform.GetComponent<RectTransform>();
			gameObject.transform.GetChild(0).gameObject.SetActive(ObscuredPrefs.GetInt("car" + carNumber) == 1);
			if (options.usePath)
			{
				ResourceRequest rr = Resources.LoadAsync(options.path + "/b" + carNumber, typeof(Texture2D));
				rr.completed += (AsyncOperation operation) =>
				{
					texture = rr.asset as Texture;
					transform.GetChild(1).GetComponent<RawImage>().texture = texture;
				};
			}
			left?.CreatePanels(prefab, transformParent, options);
			right?.CreatePanels(prefab, transformParent, options);
		}

		public void DestroyPanels()
		{
			if (transform != null)
			{
				Object.Destroy(transform.gameObject);
			}
			transform = null;
			rect = null;
			left?.DestroyPanels();
			left = null;
			right?.DestroyPanels();
			right = null;
		}

		public int? GetOrderByClick(Vector3 touchPosition)
		{
			if (Click(touchPosition))
			{
				return order;
			}
			int? obj = left?.GetOrderByClick(touchPosition);
			int? num = right?.GetOrderByClick(touchPosition);
			return obj ?? num;
		}

		private bool Click(Vector3 touchPosition)
		{
			float height = rect.rect.height;
			float width = rect.rect.width;
			Vector3 vector = Camera.main.WorldToScreenPoint(rect.position);
			if (!(touchPosition.x > vector.x - width / 2f) || !(touchPosition.x < vector.x + width / 2f) || !(touchPosition.y > vector.y - height / 2f) || !(touchPosition.y < vector.y + height / 2f))
			{
				return false;
			}
			return true;
		}
	}
}
