using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Steam.UI
{
	public class FpvCameraSetupUIHider : MonoBehaviour
	{
		private readonly struct HiddenTarget
		{
			private readonly CanvasGroup _canvasGroup;

			private readonly GameObject _gameObject;

			private readonly float _alpha;

			private readonly bool _interactable;

			private readonly bool _blocksRaycasts;

			private HiddenTarget(CanvasGroup canvasGroup, GameObject gameObject, float alpha, bool interactable, bool blocksRaycasts)
			{
				_canvasGroup = canvasGroup;
				_gameObject = gameObject;
				_alpha = alpha;
				_interactable = interactable;
				_blocksRaycasts = blocksRaycasts;
			}

			public static HiddenTarget FromCanvasGroup(CanvasGroup canvasGroup)
			{
				return new HiddenTarget(canvasGroup, null, canvasGroup.alpha, canvasGroup.interactable, canvasGroup.blocksRaycasts);
			}

			public static HiddenTarget FromGameObject(GameObject gameObject)
			{
				return new HiddenTarget(null, gameObject, 0f, interactable: false, blocksRaycasts: false);
			}

			public void Restore()
			{
				if ((bool)_canvasGroup)
				{
					_canvasGroup.alpha = _alpha;
					_canvasGroup.interactable = _interactable;
					_canvasGroup.blocksRaycasts = _blocksRaycasts;
				}
				else if ((bool)_gameObject)
				{
					_gameObject.SetActive(value: true);
				}
			}
		}

		[SerializeField]
		private GameObject[] _toHide = default;

		[Tooltip("Подсказки ввода держат свой CanvasGroup с ignoreParentGroups, поэтому их гасим общим сигналом CleanUI. Подсказка, которая должна остаться, помечается IgnoreCleanUi.")]
		[SerializeField]
		private bool _hideInputHints = true;

		[Tooltip("Запасная ссылка на случай, если на сцене нет автоматической инъекции.")]
		[SerializeField]
		private InputMapSetEvent _inputMapSetEventAsset = default;

		private readonly List<HiddenTarget> _hidden = new List<HiddenTarget>();

		private InputMapSetEvent _inputMapSetEvent;

		private bool _uiCleanedByMe;

		private bool _isHidden;

		[Inject]
		public void Construct(InputMapSetEvent inputMapSetEvent)
		{
			_inputMapSetEvent = inputMapSetEvent;
		}

		private void Start()
		{
			if (!_inputMapSetEvent)
			{
				_inputMapSetEvent = _inputMapSetEventAsset;
			}
			if ((bool)_inputMapSetEvent)
			{
				_inputMapSetEvent.Subscribe(OnInputGroupChanged);
				OnInputGroupChanged(_inputMapSetEvent.CurrentInputGroup);
			}
		}

		private void OnDestroy()
		{
			if ((bool)_inputMapSetEvent)
			{
				_inputMapSetEvent.Unsubscribe(OnInputGroupChanged);
			}
			Show();
		}

		private void OnInputGroupChanged(InputGroupsEnum group)
		{
			if (group == InputGroupsEnum.CameraSetup)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}

		private void Hide()
		{
			if (_isHidden)
			{
				return;
			}
			_isHidden = true;
			if (_hideInputHints)
			{
				_uiCleanedByMe = true;
				GlobalEventSystem.OnCleanUI?.Invoke(obj: true);
			}
			if (_toHide == null)
			{
				return;
			}
			GameObject[] toHide = _toHide;
			foreach (GameObject gameObject in toHide)
			{
				if (!gameObject)
				{
					continue;
				}
				if (gameObject.transform is RectTransform)
				{
					if (!gameObject.TryGetComponent<CanvasGroup>(out var component))
					{
						component = gameObject.AddComponent<CanvasGroup>();
					}
					_hidden.Add(HiddenTarget.FromCanvasGroup(component));
					component.alpha = 0f;
					component.interactable = false;
					component.blocksRaycasts = false;
				}
				else
				{
					_hidden.Add(HiddenTarget.FromGameObject(gameObject));
					gameObject.SetActive(value: false);
				}
			}
		}

		private void Show()
		{
			if (!_isHidden)
			{
				return;
			}
			_isHidden = false;
			foreach (HiddenTarget item in _hidden)
			{
				item.Restore();
			}
			_hidden.Clear();
			if (_uiCleanedByMe)
			{
				_uiCleanedByMe = false;
				GlobalEventSystem.OnCleanUI?.Invoke(obj: false);
			}
		}
	}
}
