using GME_voice_chat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.UIElement.HUD;

namespace _Car_Parking.Scripts.UIElement.VoiceChat
{
	public class VoiceChatButtonHandler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		[SerializeField]
		private Image Icon;

		[SerializeField]
		private Color PressedColor;

		[SerializeField]
		private bool CheckDefaultState;

		private Vector3 _defaultSize;

		private MenuLevelParent _menuLevelParent;

		private HudColorChanger _hudColorChanger;

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _onVoiceHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnVoice);

		public static bool IsActiveMicState
		{
			get
			{
				if (FlagsStorage.IsMultiFree)
				{
					return PlayerPrefs.GetInt("VoiceChat", GMEVoiceChatManager.VoiceChatDefault) == 1;
				}
				return false;
			}
		}

		[Inject]
		public void Construct(MenuLevelParent menuLevelParent, HudColorChanger hudColorChanger, EventRegistry eventRegistry)
		{
			_hudColorChanger = hudColorChanger;
			_menuLevelParent = menuLevelParent;
			_eventRegistry = eventRegistry;
		}

		private void Start()
		{
			_defaultSize = Icon.transform.localScale;
			_menuLevelParent.VoiceRecordReleased += SetDefaultState;
			_onVoiceHandle.Get(_eventRegistry).Subscribe(OnVoiceInput);
			if (CheckDefaultState)
			{
				base.gameObject.SetActive(IsActiveMicState);
			}
		}

		private void OnVoiceInput(bool started)
		{
			if (started)
			{
				SetPressedState();
				_menuLevelParent.OnVoiceRecordPressed();
			}
			else
			{
				SetDefaultState();
				_menuLevelParent.OnVoiceRecordReleased();
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			SetPressedState();
			_menuLevelParent.OnVoiceRecordPressed();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			SetDefaultState();
			_menuLevelParent.OnVoiceRecordReleased();
		}

		private void OnDestroy()
		{
			_onVoiceHandle.Get(_eventRegistry).Unsubscribe(OnVoiceInput);
			_menuLevelParent.VoiceRecordReleased -= SetDefaultState;
		}

		private void SetPressedState()
		{
			SetState(PressedColor, _defaultSize * 2f);
		}

		private void SetDefaultState()
		{
			SetState(_hudColorChanger.CurrentColor, _defaultSize);
		}

		private void SetState(Color color, Vector3 size)
		{
			color.a = Icon.color.a;
			Icon.color = color;
		}
	}
}
