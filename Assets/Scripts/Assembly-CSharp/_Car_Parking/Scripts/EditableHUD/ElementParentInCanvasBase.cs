using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public abstract class ElementParentInCanvasBase : MonoBehaviour
	{
		[SerializeField]
		protected RectTransform RectTransform;

		protected EditableHUDElement element;

		private EditableHUDConfig _config;

		public EditableHUDElement Element => element;

		public EditableHUDConfig Config => _config;

		public Vector3 Position
		{
			get
			{
				if (!Element)
				{
					return base.transform.position;
				}
				return Element.ChangesPosition;
			}
		}

		public float Scale
		{
			get
			{
				if (!Element)
				{
					return 1f;
				}
				return Element.Scale;
			}
		}

		public abstract bool IsVisible { get; }

		protected virtual Transform ElementsParent => base.transform;

		protected virtual float SizeDeltaScale => 1f;

		public event Action<ElementParentInCanvasBase, EditableHUDElement, Vector3> OnPositionChanged;

		public event Action<ElementParentInCanvasBase, EditableHUDElement, float> OnScaleChanged;

		public event Action<ElementParentInCanvasBase, EditableHUDElement, bool> OnVisibleChanged;

		protected void Init(EditableHUDElement elementValue, FakeElement fakeElementPrefab, EditableHUDConfig config)
		{
			RemoveElementListeners();
			_config = config;
			element = elementValue;
			base.transform.position = element.CentreScreenPosition;
			RectTransform.sizeDelta = element.RectSize * SizeDeltaScale;
			element.PrepareToChangeParent(fakeElementPrefab, this);
			element.transform.SetParent(ElementsParent);
			OnHUDChangesUpdated(element);
			AddElementListeners();
			element.ParentUpdated();
		}

		protected virtual void OnHUDChangesUpdated(EditableHUDElement element)
		{
			OnPositionChanged?.Invoke(this, element, element.ChangesPosition);
			OnScaleChanged?.Invoke(this, element, element.Scale);
		}

		protected virtual void OnIsActiveChanged(EditableHUDElement element)
		{
		}

		protected virtual void OnDestroy()
		{
			RemoveElementListeners();
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected void VisibleChanged(bool isVisible)
		{
			OnVisibleChanged?.Invoke(this, element, isVisible);
		}

		private void AddElementListeners()
		{
			if ((bool)element)
			{
				element.OnChangesChanged += OnHUDChangesUpdated;
				element.OnPrepareToChangeParent += OnPrepareToChangeParent;
				element.OnIsActiveChanged += OnIsActiveChanged;
			}
		}

		private void RemoveElementListeners()
		{
			if ((bool)element)
			{
				element.OnChangesChanged -= OnHUDChangesUpdated;
				element.OnPrepareToChangeParent -= OnPrepareToChangeParent;
				element.OnIsActiveChanged -= OnIsActiveChanged;
			}
		}

		private void OnPrepareToChangeParent(EditableHUDElement elementValue, ElementParentInCanvasBase parent)
		{
			if (parent != this)
			{
				RemoveElementListeners();
				element = null;
				DestroyDelay().Forget();
			}
		}

		private async UniTaskVoid DestroyDelay()
		{
			await UniTask.DelayFrame(1);
			if ((bool)this && (bool)base.gameObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
