using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	[CreateAssetMenu(menuName = "CPM/Input Hint Sprite Map")]
	public class InputHintSpriteMap : ScriptableObject
	{
		[Serializable]
		public struct BindingSprite
		{
			public string BindingPath;

			public Sprite KeyboardSprite;

			public Sprite XboxSprite;

			public Sprite PlayStationSprite;

			public Sprite GenericSprite;

			public Sprite WheelSprite;
		}

		[SerializeField]
		private BindingSprite[] Bindings;

		private readonly Dictionary<string, BindingSprite> bindingsDB = new Dictionary<string, BindingSprite>(StringComparer.OrdinalIgnoreCase);

		private void Reset()
		{
			Bindings = new BindingSprite[103]
			{
				new BindingSprite
				{
					BindingPath = "<Keyboard>/w"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/s"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/a"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/d"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/v"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/g"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/q"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/e"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/r"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/t"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/y"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/u"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/i"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/o"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/p"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/h"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/j"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/k"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/l"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/z"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/x"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/c"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/b"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/n"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/m"
				},
				new BindingSprite
				{
					BindingPath = "<Mouse>/scroll/y"
				},
				new BindingSprite
				{
					BindingPath = "<Mouse>/scroll/down"
				},
				new BindingSprite
				{
					BindingPath = "<Mouse>/middleButton"
				},
				new BindingSprite
				{
					BindingPath = "<Mouse>/leftButton"
				},
				new BindingSprite
				{
					BindingPath = "<Mouse>/rightButton"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/escape"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/enter"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/tab"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/backspace"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/capsLock"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/alt"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/space"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/ctrl"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/leftCtrl"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/shift"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/leftShift"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/upArrow"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/downArrow"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/leftArrow"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/rightArrow"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/semicolon"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/quote"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/comma"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/period"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/slash"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/backquote"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/leftBracket"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/rightBracket"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/backslash"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/minus"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/equals"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/numpadPlus"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f1"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f2"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f3"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f4"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f5"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f6"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f7"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f8"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f9"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f10"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f11"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/f12"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/1"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/2"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/3"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/4"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/5"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/6"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/7"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/8"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/9"
				},
				new BindingSprite
				{
					BindingPath = "<Keyboard>/0"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/buttonSouth"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/buttonEast"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/buttonWest"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/buttonNorth"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/leftTrigger"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/rightTrigger"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/leftShoulder"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/rightShoulder"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/leftStickPress"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/rightStickPress"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/start"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/select"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/dpad/up"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/dpad/down"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/dpad/left"
				},
				new BindingSprite
				{
					BindingPath = "<Gamepad>/dpad/right"
				},
				new BindingSprite
				{
					BindingPath = "<HID::Gudsen MOZA R5 Base>/trigger"
				},
				new BindingSprite
				{
					BindingPath = "<HID::Gudsen MOZA R5 Base>/button2"
				},
				new BindingSprite
				{
					BindingPath = "<HID::Gudsen MOZA R5 Base>/button5"
				},
				new BindingSprite
				{
					BindingPath = "<HID::Gudsen MOZA R5 Base>/button6"
				},
				new BindingSprite
				{
					BindingPath = "<HID::Gudsen MOZA R5 Base>/button7"
				},
				new BindingSprite
				{
					BindingPath = "<HID::Gudsen MOZA R5 Base>/button8"
				},
				new BindingSprite
				{
					BindingPath = "<HID::Gudsen MOZA R5 Base>/button13"
				}
			};
		}

		private void OnEnable()
		{
			bindingsDB.Clear();
			if (Bindings == null)
			{
				return;
			}
			BindingSprite[] bindings = Bindings;
			for (int i = 0; i < bindings.Length; i++)
			{
				BindingSprite value = bindings[i];
				if (!string.IsNullOrEmpty(value.BindingPath))
				{
					bindingsDB[value.BindingPath] = value;
				}
			}
		}

		public Sprite GetSprite(string bindingPath, InputDeviceType deviceType)
		{
			if (string.IsNullOrEmpty(bindingPath))
			{
				return null;
			}
			if (!TryFind(bindingPath, out var binding))
			{
				Debug.LogWarning("[InputHintSpriteMap] Path not found: " + bindingPath);
				return null;
			}
			return deviceType switch
			{
				InputDeviceType.KeyboardMouse => binding.KeyboardSprite, 
				InputDeviceType.Xbox => binding.XboxSprite, 
				InputDeviceType.PlayStation => binding.PlayStationSprite, 
				InputDeviceType.Wheel => binding.WheelSprite, 
				_ => binding.GenericSprite, 
			};
		}

		public bool HasSprite(string bindingPath, InputDeviceType deviceType)
		{
			if (string.IsNullOrEmpty(bindingPath) || !TryFind(bindingPath, out var binding))
			{
				return false;
			}
			return deviceType switch
			{
				InputDeviceType.KeyboardMouse => binding.KeyboardSprite, 
				InputDeviceType.Xbox => binding.XboxSprite, 
				InputDeviceType.PlayStation => binding.PlayStationSprite, 
				InputDeviceType.Wheel => binding.WheelSprite, 
				_ => binding.GenericSprite, 
			} != null;
		}

		private bool TryFind(string bindingPath, out BindingSprite binding)
		{
			if (bindingsDB.TryGetValue(bindingPath, out binding))
			{
				return true;
			}
			string text = ((EndsWith(bindingPath, "/up") || EndsWith(bindingPath, "/down")) ? "/y" : ((EndsWith(bindingPath, "/left") || EndsWith(bindingPath, "/right")) ? "/x" : null));
			if (text == null)
			{
				return false;
			}
			string text2 = bindingPath.Substring(0, bindingPath.LastIndexOf('/'));
			if (!bindingsDB.TryGetValue(text2 + text, out binding))
			{
				return bindingsDB.TryGetValue(text2, out binding);
			}
			return true;
		}

		private static bool EndsWith(string path, string suffix)
		{
			return path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
		}
	}
}
