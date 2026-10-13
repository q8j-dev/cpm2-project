using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Car_Parking.Scripts.EditableHUD
{
	[RequireComponent(typeof(IsActiveGetterAllGameObjectsActive))]
	public class EditableHUDElement : MonoBehaviour
	{
		public interface IHUDUpdated
		{
			void OnEditableHUDUpdated(EditableHUDElement element);
		}

		public const string PlayerPrefsKeyAllKeys = "EditableHUDAllKeys";

		public const string PlayerPrefsSplitSign = "###";

		[SerializeField]
		private RectTransform RectTransform = default;

		[SerializeField]
		private EditableHUDElementIsActiveGetter[] IsActiveGetters = default;

		[SerializeField]
		private EditableHUDElementEditorGraphics EditorGraphics = default;

		private ElementDefaultData _defaultData;

		private ElementChangesData _changesData;

		private FakeElement _fakeElement;

		private EditableHUDConfig _config;

		public string PlayerPrefsKey => SceneManager.GetActiveScene().name + base.gameObject.name + "0002";

		public bool IsStatic
		{
			get
			{
				if (!(_config != null))
				{
					string text = base.gameObject.name;
					return text == "SettingsButton" || text == "CloseButton";
				}
				return _config.IsStaticElement(base.gameObject.name);
			}
		}

		public bool IsActive
		{
			get
			{
				EditableHUDElementIsActiveGetter[] isActiveGetters = IsActiveGetters;
				if (isActiveGetters == null || isActiveGetters.Length <= 0)
				{
					return base.gameObject.activeSelf;
				}
				isActiveGetters = IsActiveGetters;
				foreach (EditableHUDElementIsActiveGetter editableHUDElementIsActiveGetter in isActiveGetters)
				{
					if ((bool)editableHUDElementIsActiveGetter && !editableHUDElementIsActiveGetter.IsActive)
					{
						return false;
					}
				}
				return true;
			}
		}

		public Vector2 CentreScreenPosition => RectTransform.TransformPoint(RectTransform.rect.center);

		public Vector2 RectSize => RectTransform.rect.size;

		public bool HaveChanges
		{
			get
			{
				if (_changesData != null)
				{
					return !IsStatic;
				}
				return false;
			}
		}

		public Vector3 ChangesPosition
		{
			get
			{
				return HaveChanges ? _changesData.Position : ((_defaultData != null) ? _defaultData.ScreenPositionCentre : CentreScreenPosition);
			}
			set
			{
				CreateChangesDataIfNeeded();
				_changesData.Position = value;
				SaveChanges();
			}
		}

		public float Scale
		{
			get
			{
				if (!HaveChanges)
				{
					return 1f;
				}
				return _changesData.Scale;
			}
			set
			{
				CreateChangesDataIfNeeded();
				_changesData.Scale = value;
				SaveChanges();
			}
		}

		public bool IsVisible
		{
			get
			{
				if (HaveChanges)
				{
					return _changesData.Visible;
				}
				return true;
			}
			set
			{
				CreateChangesDataIfNeeded();
				_changesData.Visible = value;
				SaveChanges();
			}
		}

		public event Action<EditableHUDElement> OnIsActiveChanged;

		public event Action<EditableHUDElement> OnChangesChanged;

		public event Action<EditableHUDElement, ElementParentInCanvasBase> OnPrepareToChangeParent;

		public void Initialization(EditableHUDConfig config)
		{
			_config = config;
			for (int i = 0; i < IsActiveGetters.Length; i++)
			{
				if ((bool)IsActiveGetters[i])
				{
					IsActiveGetters[i].OnIsActiveChanged -= OnGettterIsActiveChanged;
					IsActiveGetters[i].OnIsActiveChanged += OnGettterIsActiveChanged;
				}
			}
			for (int j = 0; j < IsActiveGetters.Length; j++)
			{
				if ((bool)IsActiveGetters[j])
				{
					IsActiveGetters[j].Initialization();
				}
			}
			SaveDefaultDataIfNeed();
		}

		public void LoadChangesData()
		{
			if (PlayerPrefs.HasKey(PlayerPrefsKey))
			{
				CreateChangesDataIfNeeded();
			}
		}

		public void GoToDefault()
		{
			DestroyFakeElement();
			OnPrepareToChangeParent?.Invoke(this, null);
			SaveDefaultDataIfNeed();
			base.transform.SetParent(_defaultData.Parent);
			RectTransform.anchoredPosition = _defaultData.Position;
			base.transform.localScale = _defaultData.Scale;
			base.transform.SetSiblingIndex(_defaultData.Index);
			ParentUpdated();
		}

		public void PrepareToChangeParent(FakeElement fakeElementPrefab, ElementParentInCanvasBase parentInCanvas)
		{
			DestroyFakeElement();
			_fakeElement = UnityEngine.Object.Instantiate(fakeElementPrefab, _defaultData.Parent);
			_fakeElement.RectTransform.sizeDelta = RectTransform.sizeDelta;
			_fakeElement.RectTransform.anchoredPosition = _defaultData.Position;
			_fakeElement.transform.localScale = _defaultData.Scale;
			_fakeElement.transform.SetSiblingIndex(_defaultData.Index);
			OnPrepareToChangeParent?.Invoke(this, parentInCanvas);
		}

		public void ParentUpdated()
		{
			GetComponent<IHUDUpdated>()?.OnEditableHUDUpdated(this);
		}

		public void ClearChanges()
		{
			if (_changesData != null)
			{
				_changesData.ClearPlayerPrefs(PlayerPrefsKey);
				PlayerPrefs.DeleteKey(PlayerPrefsKey);
				_changesData = null;
				string text = PlayerPrefs.GetString("EditableHUDAllKeys", "");
				int num = text.IndexOf(PlayerPrefsKey);
				if (num >= 0)
				{
					string text2 = text.Substring(0, num);
					string text3 = text.Substring(num + PlayerPrefsKey.Length);
					num = text3.IndexOf("###");
					if (num >= 0)
					{
						text3 = text3.Substring(num + "###".Length);
					}
					else
					{
						num = text2.LastIndexOf("###");
						if (num >= 0)
						{
							text2 = text2.Substring(0, num);
						}
					}
					PlayerPrefs.SetString("EditableHUDAllKeys", text2 + text3);
				}
			}
			OnChangesChanged?.Invoke(this);
		}

		public void ShowEditorGraphics(Transform editorGraphicsParent)
		{
			if ((bool)EditorGraphics)
			{
				EditorGraphics.Show(editorGraphicsParent);
				base.transform.parent.gameObject.SetActive(value: false);
			}
		}

		public void HideEditorGraphics()
		{
			if ((bool)EditorGraphics)
			{
				EditorGraphics.Hide();
				base.transform.parent.gameObject.SetActive(value: true);
			}
		}

		private void OnDestroy()
		{
			if (IsActiveGetters != null)
			{
				for (int i = 0; i < IsActiveGetters.Length; i++)
				{
					if ((object)IsActiveGetters[i] != null)
					{
						IsActiveGetters[i].OnIsActiveChanged -= OnGettterIsActiveChanged;
					}
				}
			}
			OnIsActiveChanged = null;
			OnChangesChanged = null;
			OnPrepareToChangeParent = null;
		}

		private void OnGettterIsActiveChanged(EditableHUDElementIsActiveGetter isActiveGetter)
		{
			OnIsActiveChanged?.Invoke(this);
		}

		private void CreateChangesDataIfNeeded()
		{
			if (_changesData != null)
			{
				return;
			}
			SaveDefaultDataIfNeed();
			_changesData = new ElementChangesData(_defaultData.ScreenPositionCentre);
			if (PlayerPrefs.HasKey(PlayerPrefsKey))
			{
				_changesData.Load(PlayerPrefsKey);
			}
			PlayerPrefs.SetInt(PlayerPrefsKey, 1);
			string text = PlayerPrefs.GetString("EditableHUDAllKeys", "");
			if (!text.Contains(PlayerPrefsKey))
			{
				if (text.Length > 0)
				{
					text += "###";
				}
				text += PlayerPrefsKey;
				PlayerPrefs.SetString("EditableHUDAllKeys", text);
			}
		}

		private void DestroyFakeElement()
		{
			if ((bool)_fakeElement)
			{
				UnityEngine.Object.Destroy(_fakeElement.gameObject);
			}
			_fakeElement = null;
		}

		private void SaveDefaultDataIfNeed()
		{
			if (_defaultData == null)
			{
				_defaultData = new ElementDefaultData(base.transform.parent, RectTransform.anchoredPosition, base.transform.localScale, base.transform.GetSiblingIndex(), CentreScreenPosition);
			}
		}

		private void SaveChanges()
		{
			if (_changesData != null)
			{
				_changesData.Save(PlayerPrefsKey);
				OnChangesChanged?.Invoke(this);
			}
		}
	}
}
