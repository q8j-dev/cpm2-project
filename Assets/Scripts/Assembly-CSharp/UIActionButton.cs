using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.SOAP;

public class UIActionButton : MonoBehaviour
{
	[SerializeField]
	private Button ActoinButton;

	[SerializeField]
	private GameObject ActiveGameObject;

	[SerializeField]
	private GameObject NotActiveGameObject;

	private Action _activeCallback;

	private Action _notActiveCallback;

	private bool _isActive;

	private EventRegistry _eventRegistry;

	private RegistryHandle<ScriptableEvent> _interactHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Interact);

	[Inject]
	public void Construct(EventRegistry eventRegistry)
	{
		_eventRegistry = eventRegistry;
	}

	private void Start()
	{
		Init();
	}

	public virtual void Init()
	{
		ActoinButton.onClick.AddListener(OnButtonClick);
		_interactHandle.Get(_eventRegistry).Subscribe(OnButtonClick);
	}

	private void OnButtonClick()
	{
		if (_isActive)
		{
			_activeCallback?.Invoke();
		}
		else
		{
			_notActiveCallback?.Invoke();
		}
	}

	public void Init(Action activeCallback, Action notActiveCallback)
	{
		_activeCallback = activeCallback;
		_notActiveCallback = notActiveCallback;
	}

	public void SetInteractable(bool interactable, bool deactivateButtons)
	{
		if (deactivateButtons)
		{
			_isActive = false;
			if ((bool)ActoinButton)
			{
				ActoinButton.interactable = false;
			}
			if (ActiveGameObject.activeSelf)
			{
				ActiveGameObject.SetActive(value: false);
			}
			if (NotActiveGameObject.activeSelf)
			{
				NotActiveGameObject.SetActive(value: false);
			}
		}
		else
		{
			if ((bool)ActoinButton)
			{
				ActoinButton.interactable = true;
			}
			_isActive = interactable;
			ActiveGameObject.SetActive(interactable);
			NotActiveGameObject.SetActive(!interactable);
		}
	}

	public void Close()
	{
		_interactHandle.Get(_eventRegistry)?.Unsubscribe(OnButtonClick);
		if ((bool)base.gameObject)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
