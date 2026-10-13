using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditableHUDController : MonoBehaviour, IInitializable
	{
		[SerializeField]
		private EditableHUDCanvasGameplay CanvasGameplay;

		[SerializeField]
		private EditableHUDCanvasEditorWindow CanvasEditor;

		[SerializeField]
		private int PriorityIndex;

		[SerializeField]
		private EditableHUDConfig Config;

		private EditableHUDElement[] _elements;

		public bool WasInitialization => _elements != null;

		public int Priority => PriorityIndex;

		public bool HaveActiveElements
		{
			get
			{
				if (_elements != null)
				{
					for (int i = 0; i < _elements.Length; i++)
					{
						if (_elements[i].IsActive)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public void Initialize()
		{
			EditableHUDControllerReference.AddHudController(this);
		}

		public void ShowEditor(bool clearEditor)
		{
			if (!CanvasEditor || _elements == null)
			{
				return;
			}
			InitializationIfNeed().Forget();
			if (clearEditor)
			{
				CanvasEditor.Clear();
			}
			CanvasEditor.gameObject.SetActive(value: true);
			for (int i = 0; i < _elements.Length; i++)
			{
				if ((bool)_elements[i] && _elements[i].IsActive)
				{
					_elements[i].GoToDefault();
					CanvasEditor.AddElement(_elements[i], Config.FakeElementPrefab, Config);
				}
			}
		}

		private void OnEnable()
		{
			if (!CanvasEditor)
			{
				CanvasEditor = Object.FindObjectOfType<EditableHUDCanvasEditorWindow>(includeInactive: true);
			}
			InitializationIfNeed().Forget();
		}

		private void Awake()
		{
			Initialize();
		}

		private void OnDestroy()
		{
			EditableHUDControllerReference.RemoveHudController(this);
			if ((bool)CanvasEditor)
			{
				CanvasEditor.OnClose -= OnCanvasEditorClose;
			}
		}

		private void OnCanvasEditorClose(EditableHUDCanvasEditorWindow editorWindow, List<EditableHUDElement> elements)
		{
			for (int i = 0; i < elements.Count; i++)
			{
				if ((bool)elements[i] && Enumerable.Contains(_elements, elements[i]))
				{
					elements[i].GoToDefault();
					if (elements[i].HaveChanges)
					{
						CanvasGameplay.AddElement(elements[i], Config.FakeElementPrefab, Config);
					}
				}
			}
		}

		private async UniTaskVoid InitializationIfNeed()
		{
			if (_elements != null)
			{
				return;
			}
			await UniTask.NextFrame();
			if (!this)
			{
				return;
			}
			_elements = GetComponentsInChildren<EditableHUDElement>(includeInactive: true);
			for (int i = 0; i < _elements.Length; i++)
			{
				_elements[i].Initialization(Config);
			}
			for (int j = 0; j < _elements.Length; j++)
			{
				_elements[j].LoadChangesData();
				if (_elements[j].HaveChanges && (bool)CanvasGameplay)
				{
					CanvasGameplay.AddElement(_elements[j], Config.FakeElementPrefab, Config);
				}
			}
			if ((bool)CanvasEditor)
			{
				CanvasEditor.OnClose += OnCanvasEditorClose;
			}
		}
	}
}
