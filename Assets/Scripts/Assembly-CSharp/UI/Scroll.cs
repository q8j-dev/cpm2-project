using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;

namespace UI
{
	public class Scroll : MonoBehaviour
	{
		public delegate void OnButtonClickDelegate(ref Node node, Transform canvas, MODE mode, CREATE createMode);

		public delegate void OnChangeCar();

		public static readonly uint UiEvent = AudioWrapper.GetId("Ui_Event");

		public static readonly uint UiSwitch = AudioWrapper.GetId("UiSwitch");

		public static readonly uint ScrollSound = AudioWrapper.GetId("Scroll");

		[Range(25f, 100f)]
		public float radius = 1f;

		[Range(1f, 2f)]
		public float offset = 1f;

		[Range(0f, 1.5f)]
		public float size = 1f;

		[Range(0.5f, 1f)]
		public float alpha = 0.7f;

		[Header("Addressables")]
		public bool usePath;

		public string? path;

		public Node? node;

		public CoreGameManager? mainController;

		public Texture[]? textureReferences;

		private Node? _parent;

		private int oldButtonPressedCount;

		private int newButtonPressedCount;

		private int num;

		private int touchCount;

		private float _height;

		private float _width;

		private float _oldOffset;

		private float _alpha = 1f;

		private float _maxTouchDistance;

		private bool touching;

		private bool inertia;

		private bool buttonPressed;

		private bool scroll;

		private bool _open;

		private bool _UIPressed;

		private bool _stopPressed;

		private bool canBuy;

		private Vector3? _prev;

		private double? _direction;

		private float _max;

		private float _min;

		private float _center;

		private Coroutine? _backInertiaCoroutine;

		private Coroutine? _fadeCoroutine;

		private Coroutine? _blinkCoroutine;

		private Coroutine? changeCoroutine;

		private Dictionary<int, int>? _carOrder;

		[SerializeField]
		private GameObject carImage;

		[SerializeField]
		private GameObject carImageIndexed;

		private List<Transform>? _transform;

		private Vector3 prevTouchPos;

		public static event OnButtonClickDelegate? ButtonClickDelegate;

		public event OnChangeCar? ChangeCar;

		private void Awake()
		{
			mainController.OnOpen += (int i) =>
			{
				num = i;
			};
		}

		public void Create(int[] cars)
		{
			_parent = node;
			_alpha = alpha;
			_oldOffset = offset;
			_width = (float)Screen.width / 4f / (2.5f - size);
			_height = _width / 2f;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < mainController.CarOrder.Length; i++)
			{
				dictionary[mainController.CarOrder[i]] = i;
			}
			if (_backInertiaCoroutine != null)
			{
				StopCoroutine(_backInertiaCoroutine);
			}
			if (_fadeCoroutine != null)
			{
				StopCoroutine(_fadeCoroutine);
			}
			if (_blinkCoroutine != null)
			{
				StopCoroutine(_blinkCoroutine);
			}
			if (changeCoroutine != null)
			{
				StopCoroutine(changeCoroutine);
			}
			_carOrder = new Dictionary<int, int>();
			node?.DestroyPanels();
			node = null;
			for (int j = 0; j < cars.Length; j++)
			{
				_carOrder.Add(j, cars[j]);
			}
			for (int k = 0; k < cars.Length; k++)
			{
				if (k == 0)
				{
					node = new Node(null, _carOrder[0], dictionary[_carOrder[0]], k);
					_parent = node;
				}
				else
				{
					_parent.right = new Node(_parent, _carOrder[k], dictionary[_carOrder[k]], k);
					_parent = _parent.right;
				}
			}
			if (node != null)
			{
				node?.CreatePanels(carImage, base.transform.GetChild(0), (usePath: usePath, path: path, referenceTexture: textureReferences.ToList()));
			}
			if (_carOrder.Count > 0)
			{
				OnOpen(0);
			}
			CreateImage();
		}

		private void Start()
		{
			mainController.BuyButtonPressed += (int i) =>
			{
				if (ButtonClickDelegate != null)
				{
					ButtonClickDelegate(ref node, base.transform.GetChild(0), MODE.NONE, CREATE.NO);
				}
				if (i == 1)
				{
					OnDisable();
				}
			};
			mainController.ButtonPressed += (int i) =>
			{
				buttonPressed = i == 1;
				newButtonPressedCount++;
			};
			mainController.OnChangeCarButtonClick += (int i) =>
			{
				touching = false;
				try
				{
					if (i > 0)
					{
						Left();
					}
					else
					{
						Right();
					}
				}
				catch (Exception)
				{
				}
				StopAllFadeCoroutines();
				_blinkCoroutine = StartCoroutine(BlinkFade(1f, alpha, 1.1f));
				if (_backInertiaCoroutine != null)
				{
					StopCoroutine(_backInertiaCoroutine);
				}
				_backInertiaCoroutine = StartCoroutine(BackInertia(mode: false, ChangeCarMode: true));
			};
		}

		private void Update()
		{
			if (node == null || node?.transform == null)
			{
				return;
			}
			if (!_open)
			{
				OnOpen(num);
				_open = true;
			}
			if (Input.GetMouseButtonDown(0))
			{
				touching = false;
				if (Math.Abs(_direction.GetValueOrDefault()) > 0.0 && _parent.index != node.index)
				{
					_stopPressed = true;
				}
				else
				{
					_stopPressed = false;
				}
				buttonPressed = oldButtonPressedCount != newButtonPressedCount;
				_maxTouchDistance = 0f;
				prevTouchPos = Input.mousePosition;
				if (_backInertiaCoroutine != null)
				{
					StopCoroutine(_backInertiaCoroutine);
				}
				_direction = 0.0;
				inertia = true;
				if (Input.mousePosition.y < _max && Input.mousePosition.y > _min && _alpha + 0.05f > alpha)
				{
					FlagsStorage.IgnoreTouchesOnMenu = true;
					scroll = _alpha + 0.05f > alpha;
					StopAllFadeCoroutines();
					_fadeCoroutine = StartCoroutine(Fade(1, 1.05f, 1f));
				}
			}
			if (Input.GetMouseButton(0))
			{
				if (_backInertiaCoroutine != null)
				{
					StopCoroutine(_backInertiaCoroutine);
				}
				_maxTouchDistance = ((_maxTouchDistance < Mathf.Abs(Vector3.Distance(Input.mousePosition, prevTouchPos))) ? Mathf.Abs(Vector3.Distance(Input.mousePosition, prevTouchPos)) : _maxTouchDistance);
				if (scroll && _maxTouchDistance > 10f)
				{
					if (Input.mousePosition.y > _max || Input.mousePosition.y < _min)
					{
						if (_prev.HasValue)
						{
							_direction = _prev?.x - Input.mousePosition.x;
						}
						_prev = null;
						inertia = true;
						return;
					}
					inertia = false;
					_direction = _prev?.x - Input.mousePosition.x;
					touching = true;
					_prev = Input.mousePosition;
				}
			}
			if (Input.GetMouseButtonUp(0))
			{
				inertia = true;
				_maxTouchDistance = ((_maxTouchDistance < Mathf.Abs(Vector3.Distance(Input.mousePosition, prevTouchPos))) ? Mathf.Abs(Vector3.Distance(Input.mousePosition, prevTouchPos)) : _maxTouchDistance);
				if (!FlagsStorage.IgnoreTouchesOnMenu)
				{
					touching = false;
					_stopPressed = false;
					if (_backInertiaCoroutine != null)
					{
						StopCoroutine(_backInertiaCoroutine);
					}
					if (_maxTouchDistance < 10f && !buttonPressed)
					{
						_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
					}
				}
				FlagsStorage.IgnoreTouchesOnMenu = false;
				if (_maxTouchDistance < 10f)
				{
					if (Input.mousePosition.y < _max && Input.mousePosition.y > _min && !_stopPressed)
					{
						touching = false;
						int? orderByClick = node.GetOrderByClick(Input.mousePosition);
						if (changeCoroutine != null)
						{
							StopCoroutine(changeCoroutine);
						}
						changeCoroutine = StartCoroutine(Change(orderByClick ?? node.order));
					}
					else if (_stopPressed)
					{
						_stopPressed = false;
						if (_backInertiaCoroutine != null)
						{
							StopCoroutine(_backInertiaCoroutine);
						}
						_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
					}
					else if (!_UIPressed && !buttonPressed)
					{
						StopAllFadeCoroutines();
					}
					if (!buttonPressed && !_UIPressed)
					{
						if (_backInertiaCoroutine != null)
						{
							StopCoroutine(_backInertiaCoroutine);
						}
						_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: false));
					}
				}
				else if (Input.mousePosition.y < _max && Input.mousePosition.y > _min)
				{
					_direction = _prev?.x - Input.mousePosition.x;
					_prev = null;
					inertia = true;
				}
				else if (_stopPressed)
				{
					_stopPressed = false;
					if (_backInertiaCoroutine != null)
					{
						StopCoroutine(_backInertiaCoroutine);
					}
					_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
				}
				oldButtonPressedCount = newButtonPressedCount;
			}
			touchCount = Input.touchCount;
			if (touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began)
				{
					touching = false;
					_stopPressed = ((Math.Abs(_direction.GetValueOrDefault()) > 0.0 && _parent.index != node.index) ? true : false);
					buttonPressed = oldButtonPressedCount != newButtonPressedCount;
					_maxTouchDistance = 0f;
					prevTouchPos = touch.position;
					if (_backInertiaCoroutine != null)
					{
						StopCoroutine(_backInertiaCoroutine);
					}
					_direction = 0.0;
					inertia = true;
					if (touch.position.y < _max && touch.position.y > _min && _alpha + 0.05f > alpha)
					{
						FlagsStorage.IgnoreTouchesOnMenu = true;
						scroll = _alpha + 0.05f > alpha;
						StopAllFadeCoroutines();
						_fadeCoroutine = StartCoroutine(Fade(1, 1.05f, 1f));
					}
					else if (touch.position.y > _max || touch.position.y < _min)
					{
						touching = false;
						scroll = false;
					}
					Moved(touch);
				}
				if (touch.phase == TouchPhase.Moved)
				{
					Moved(touch);
				}
				if (touch.phase == TouchPhase.Ended)
				{
					inertia = true;
					_maxTouchDistance = ((_maxTouchDistance < Mathf.Abs(Vector3.Distance(touch.position, prevTouchPos))) ? Mathf.Abs(Vector3.Distance(touch.position, prevTouchPos)) : _maxTouchDistance);
					if (!FlagsStorage.IgnoreTouchesOnMenu)
					{
						touching = false;
						_stopPressed = false;
						if (_backInertiaCoroutine != null)
						{
							StopCoroutine(_backInertiaCoroutine);
						}
						if (_maxTouchDistance < 10f && !buttonPressed)
						{
							_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
						}
					}
					FlagsStorage.IgnoreTouchesOnMenu = false;
					if (_maxTouchDistance < 10f)
					{
						if (touch.position.y < _max && touch.position.y > _min && !_stopPressed)
						{
							touching = false;
							int? orderByClick2 = node.GetOrderByClick(Input.mousePosition);
							if (changeCoroutine != null)
							{
								StopCoroutine(changeCoroutine);
							}
							changeCoroutine = StartCoroutine(Change(orderByClick2 ?? node.order));
						}
						else if (_stopPressed)
						{
							_stopPressed = false;
							if (_backInertiaCoroutine != null)
							{
								StopCoroutine(_backInertiaCoroutine);
							}
							_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
						}
						else if (!_UIPressed && !buttonPressed)
						{
							StopAllFadeCoroutines();
						}
						if (!buttonPressed && !_UIPressed)
						{
							if (_backInertiaCoroutine != null)
							{
								StopCoroutine(_backInertiaCoroutine);
							}
							_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: false));
						}
					}
					else if (touch.position.y < _max && touch.position.y > _min)
					{
						_direction = _prev?.x - Input.mousePosition.x;
						_prev = null;
						inertia = true;
					}
					else if (_stopPressed)
					{
						_stopPressed = false;
						if (_backInertiaCoroutine != null)
						{
							StopCoroutine(_backInertiaCoroutine);
						}
						_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
					}
					oldButtonPressedCount = newButtonPressedCount;
				}
			}
			else if (Input.touchCount > 1)
			{
				if (_parent.index == node.index)
				{
					_stopPressed = false;
				}
				if (_backInertiaCoroutine != null)
				{
					StopCoroutine(_backInertiaCoroutine);
				}
				_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
				inertia = true;
				touching = false;
				_direction = 0.0;
				return;
			}
			if (changeCoroutine == null)
			{
				ContinuedUpdate();
			}
		}

		private void ContinuedUpdate()
		{
			if (canBuy || !node.transform)
			{
				return;
			}
			if (_oldOffset != offset)
			{
				_oldOffset = offset;
				node.FixPosition(new Vector3((float)Screen.width / 2f, 0f, 0f), new Vector2(_width, _height), offset, radius);
			}
			Camera main = Camera.main;
			if (!main)
			{
				return;
			}
			_width = (float)Screen.width / 4f / (2.5f - size);
			_height = _width / 2f;
			node.FixScale(2.5f - size, _alpha);
			node.Move(_direction, radius);
			RectTransform component = base.transform.GetChild(0).GetComponent<RectTransform>();
			_max = main.WorldToScreenPoint(component.position).y + component.rect.height / 2f;
			_min = main.WorldToScreenPoint(component.position).y - component.rect.height / 2f;
			if (inertia)
			{
				_direction /= 1.100000023841858;
			}
			if (main.WorldToScreenPoint(node.rect.position).x > (float)Screen.width / 2f + (float)Screen.width / 8f && touching)
			{
				try
				{
					Right();
				}
				catch (Exception)
				{
					node.FixPosition(new Vector3((float)Screen.width / 2f, 0f, 0f), new Vector2(_width, _height), offset, radius);
					_direction = 0.0;
				}
			}
			if (main.WorldToScreenPoint(node.rect.position).x < (float)Screen.width / 2f - (float)Screen.width / 8f && touching)
			{
				try
				{
					Left();
				}
				catch (Exception)
				{
					node.FixPosition(new Vector3((float)Screen.width / 2f, 0f, 0f), new Vector2(_width, _height), offset, radius);
					_direction = 0.0;
				}
			}
			if (inertia && _direction > -1.0 && _direction < 1.0)
			{
				inertia = false;
				_direction = 0.0;
				if (touching && touchCount <= 1)
				{
					StopAllFadeCoroutines();
					if (_backInertiaCoroutine != null)
					{
						StopCoroutine(_backInertiaCoroutine);
					}
					_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
					if (ChangeCar != null)
					{
						ChangeCar();
					}
				}
				else if (!touching && _parent != node && ChangeCar != null)
				{
					ChangeCar();
				}
			}
			changeCoroutine = null;
		}

		private void Moved(Touch touch)
		{
			if (_backInertiaCoroutine != null)
			{
				StopCoroutine(_backInertiaCoroutine);
			}
			_maxTouchDistance = ((_maxTouchDistance < Mathf.Abs(Vector3.Distance(touch.position, prevTouchPos))) ? Mathf.Abs(Vector3.Distance(touch.position, prevTouchPos)) : _maxTouchDistance);
			if (!scroll || !(_maxTouchDistance > 10f))
			{
				return;
			}
			if (touch.position.y > _max || touch.position.y < _min)
			{
				if (_prev.HasValue)
				{
					_direction = _prev?.x - touch.position.x;
				}
				_prev = null;
				inertia = true;
			}
			else
			{
				inertia = false;
				_direction = _prev?.x - touch.position.x;
				touching = true;
				_prev = touch.position;
			}
		}

		private IEnumerator Change(int order)
		{
			if (node == null)
			{
				yield break;
			}
			if (scroll)
			{
				while (node.order != order)
				{
					try
					{
						if (order > node.order)
						{
							Left();
						}
						else
						{
							Right();
						}
					}
					catch (Exception)
					{
					}
					yield return new WaitForFixedUpdate();
				}
				if (_backInertiaCoroutine != null)
				{
					StopCoroutine(_backInertiaCoroutine);
				}
				_backInertiaCoroutine = StartCoroutine(BackInertia(mode: true, ChangeCarMode: true));
			}
			ContinuedUpdate();
		}

		private void StopAllFadeCoroutines()
		{
			if (_fadeCoroutine != null)
			{
				StopCoroutine(_fadeCoroutine);
				_fadeCoroutine = null;
			}
			if (_blinkCoroutine != null)
			{
				StopCoroutine(_blinkCoroutine);
				_blinkCoroutine = null;
			}
		}

		private void OnEnable()
		{
			if (mainController.carImage != null)
			{
				mainController.carImage.SetActive(value: false);
			}
			ButtonClickDelegate += mainController.ChangeSellModel;
		}

		private void OnDisable()
		{
			if (node != null)
			{
				num = node.order;
			}
			_prev = null;
			touching = false;
			_parent = node;
			inertia = false;
			_direction = 0.0;
			buttonPressed = false;
			scroll = false;
			_UIPressed = false;
			_stopPressed = false;
			canBuy = false;
			ButtonClickDelegate -= mainController.ChangeSellModel;
			node?.DestroyPanels();
			if (_backInertiaCoroutine != null)
			{
				StopCoroutine(_backInertiaCoroutine);
			}
			if (_fadeCoroutine != null)
			{
				StopCoroutine(_fadeCoroutine);
			}
			if (_blinkCoroutine != null)
			{
				StopCoroutine(_blinkCoroutine);
			}
			_open = false;
		}

		private IEnumerator BlinkFade(float max, float min, float speed)
		{
			if (node != null)
			{
				while (_alpha < max)
				{
					_alpha *= speed;
					yield return new WaitForFixedUpdate();
				}
				yield return new WaitForSeconds(0.2f);
				while (_alpha > min)
				{
					_alpha /= speed;
					yield return new WaitForFixedUpdate();
				}
				if (min <= 0.055f)
				{
					base.transform.GetChild(0).gameObject.SetActive(value: false);
				}
				_blinkCoroutine = null;
			}
		}

		private IEnumerator BackInertia(bool mode, bool ChangeCarMode)
		{
			if (node == null)
			{
				yield break;
			}
			if (ChangeCarMode && _parent != node)
			{
				ButtonClickDelegate(ref node, base.transform.GetChild(0), MODE.CHECK, CREATE.YES);
				_parent = node;
			}
			base.transform.GetChild(0).gameObject.SetActive(value: true);
			Camera mainCamera = Camera.main;
			if ((bool)mainCamera)
			{
				while (Mathf.Abs(mainCamera.WorldToScreenPoint(node.rect.position).x - (float)Screen.width / 2f) > 1f)
				{
					node.SmoothFixPosition(new Vector3(((float)Screen.width / 2f - mainCamera.WorldToScreenPoint(node.rect.position).x) / 100f, 0f, 0f));
					yield return new WaitForEndOfFrame();
				}
				while (_blinkCoroutine != null)
				{
					yield return new WaitForEndOfFrame();
				}
				if (mode)
				{
					StopAllFadeCoroutines();
					_fadeCoroutine = StartCoroutine((_alpha > alpha) ? Fade(-1, 1.05f, alpha) : Fade(1, 1.5f, alpha));
				}
			}
		}

		private IEnumerator Fade(int state, float speed, float newAlpha)
		{
			if (node == null)
			{
				yield break;
			}
			if (state == 1)
			{
				while (_alpha < newAlpha)
				{
					_alpha *= speed;
					yield return new WaitForFixedUpdate();
				}
				yield break;
			}
			if (state == -1)
			{
				while (inertia)
				{
					yield return new WaitForFixedUpdate();
				}
				while (_alpha > newAlpha)
				{
					_alpha /= speed;
					yield return new WaitForFixedUpdate();
				}
				if (newAlpha <= 0.055f)
				{
					base.transform.GetChild(0).gameObject.SetActive(value: false);
				}
				yield break;
			}
			switch (state)
			{
			case 0:
				while (_alpha < 1f)
				{
					_alpha *= speed;
					yield return new WaitForFixedUpdate();
				}
				_fadeCoroutine = StartCoroutine(Fade(-1, 1.05f, alpha));
				break;
			case 2:
				_fadeCoroutine = StartCoroutine((_alpha > alpha) ? Fade(-1, 1.05f, alpha) : Fade(1, 1.5f, alpha));
				break;
			}
		}

		private void OnOpen(int num)
		{
			while (node.order != num)
			{
				try
				{
					if (num > node.order)
					{
						Left(CREATE.NO);
					}
					else
					{
						Right(CREATE.NO);
					}
				}
				catch (Exception)
				{
					return;
				}
			}
			if (ButtonClickDelegate != null)
			{
				ButtonClickDelegate(ref node, base.transform.GetChild(0), MODE.NONE, CREATE.YES);
			}
			_parent = node;
			node.FixPosition(new Vector3((float)Screen.width / 2f, 0f, 0f), new Vector2(_width, _height), offset, radius);
			if (node.order >= 12)
			{
				mainController.SellCarRewardShowButton.gameObject.SetActive(value: false);
			}
		}

		private void Left(CREATE create = CREATE.YES)
		{
			if (node?.right == null)
			{
				throw new ArgumentException("It's left most element");
			}
			node.parent = node.right;
			Node right = node.right;
			node.right = null;
			node = right;
			node.left = node.parent;
			node.parent = null;
			node.FixScale(2.5f - size, _alpha);
			if (create == CREATE.YES)
			{
				CreateImage();
			}
		}

		private void Right(CREATE create = CREATE.YES)
		{
			if (node?.left == null)
			{
				throw new ArgumentException("It's right most element");
			}
			node.parent = node.left;
			Node left = node.left;
			node.left = null;
			node = left;
			node.right = node.parent;
			node.parent = null;
			node.FixScale(2.5f - size, _alpha);
			if (create == CREATE.YES)
			{
				CreateImage();
			}
		}

		private void CreateImage()
		{
			AudioWrapper.SetSwitch(UiSwitch, ScrollSound, base.gameObject);
			AudioWrapper.PostEvent(UiEvent, base.gameObject);
			if (mainController.carImage != null)
			{
				mainController.carImage.SetActive(value: false);
			}
			if (mainController.SellCarModel != null)
			{
				mainController.SellCarModel.SetActive(_parent == node && mainController.IsSellCarModelReady);
			}
			if (node != null && _parent != node)
			{
				Node? obj = node;
				if (obj != null && obj.index < 12)
				{
					mainController.SellCarRewardShowButton.gameObject.SetActive(value: true);
				}
				else
				{
					mainController.SellCarRewardShowButton.gameObject.SetActive(value: false);
				}
				mainController.carImage.transform.GetChild(1).GetComponent<RawImage>().texture = node?.GetTexture();
				RectTransform component = mainController.carImage.GetComponent<RectTransform>();
				component.sizeDelta = new Vector2((float)Screen.width / 3f, (float)Screen.height / 3f);
				component.anchoredPosition3D = new Vector3((float)Screen.width / 2f, (float)Screen.height / 3f, 0f);
				mainController.carImage.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2((float)Screen.width / 3f, (float)Screen.height / 3f);
				RectTransform component2 = mainController.carImage.transform.GetChild(2).GetComponent<RectTransform>();
				RectTransform component3 = component2.GetChild(0).GetComponent<RectTransform>();
				component3.sizeDelta = new Vector2((float)Screen.width / 25f, (float)Screen.width / 25f);
				component3.anchoredPosition3D = new Vector3(0f, (float)(-Screen.height) / 5.5f, 0f);
				RectTransform component4 = component2.GetChild(1).GetComponent<RectTransform>();
				component4.sizeDelta = new Vector2((float)Screen.width / 27f, (float)Screen.width / 27f);
				component4.anchoredPosition3D = new Vector3(0f, (float)(-Screen.height) / 5.5f, 0f);
				RectTransform component5 = component4.GetChild(0).GetComponent<RectTransform>();
				component5.sizeDelta = component4.sizeDelta * 2f;
				component5.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				Text component6 = component5.GetComponent<Text>();
				Node? obj2 = node;
				component6.text = $"{((obj2 != null) ? new int?(obj2.order + 1) : ((int?)null))}\n/{_carOrder.Count}";
			}
		}
	}
}
