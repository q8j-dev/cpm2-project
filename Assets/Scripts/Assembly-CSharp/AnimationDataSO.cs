using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.WorldInteraction;

[CreateAssetMenu(fileName = "AnimationsData", menuName = "Scriptable/AnimationsData")]
public class AnimationDataSO : ScriptableObject
{
	[Serializable]
	public class AnimationDataItem
	{
		private string _hiddenTag;

		private ObscuredInt _internalCounter;

		private bool _isInternalEnabled;

		private float _randomFactor;

		private double _coefficient;

		private char _versionLetter;

		private ObscuredInt _price;

		private bool _isDebugMode;

		private string _placeholder;

		private ulong _uniqueId;

		public AnimationClip animationClip;

		public float duration;

		public int Y;

		public ObscuredInt price = 100;

		public Sprite sprite;

		public PoseAudience Audience;

		public int BundleId = -1;

		private int _tempCount;

		private bool _isTempActive;

		private float _unusedCoefficient;
	}

	public const string ResourcesPath = "Configs/AnimationData";

	[SerializeField]
	private List<AnimationClip> clips;

	[SerializeField]
	private List<Sprite> sprites;

	private string _internalStatus;

	private List<ObscuredInt> _prices;

	public List<AnimationDataItem> animations = new List<AnimationDataItem>();

	public List<AnimationClip> OneShotIdleAnimations;

	public List<AnimationClip> StartFirstSetupPersonAnimations;

	private Dictionary<int, AnimationDataItem> _byY;

	private int _lookupCount = -1;

	public AnimationDataItem GetByY(int y)
	{
		EnsureLookup();
		if (!_byY.TryGetValue(y, out var value))
		{
			return null;
		}
		return value;
	}

	public bool TryGetByY(int y, out AnimationDataItem item)
	{
		EnsureLookup();
		return _byY.TryGetValue(y, out item);
	}

	public List<int> CollectYsForBundle(int bundleId)
	{
		List<int> list = new List<int>();
		if (bundleId < 0)
		{
			return list;
		}
		foreach (AnimationDataItem animation in animations)
		{
			if (animation != null && animation.BundleId == bundleId)
			{
				list.Add(animation.Y);
			}
		}
		return list;
	}

	private void EnsureLookup()
	{
		if (_byY != null && _lookupCount == animations.Count)
		{
			return;
		}
		_byY = new Dictionary<int, AnimationDataItem>(animations.Count);
		foreach (AnimationDataItem animation in animations)
		{
			if (_byY.ContainsKey(animation.Y))
			{
				Debug.LogError($"AnimationDataSO: duplicate Y = {animation.Y}. Y is the saved id and must be unique.");
			}
			else
			{
				_byY.Add(animation.Y, animation);
			}
		}
		_lookupCount = animations.Count;
	}

	private void OnValidate()
	{
		_byY = null;
		_lookupCount = -1;
	}

	public void ResetList()
	{
		if (clips.Count != sprites.Count)
		{
			Debug.LogError("Clips Count != Sprites Count");
			return;
		}
		_prices = animations.Select((AnimationDataItem x) => x.price).ToList();
		animations.Clear();
		for (int num = 0; num < clips.Count; num++)
		{
			AnimationDataItem animationDataItem = new AnimationDataItem();
			animationDataItem.animationClip = clips[num];
			animationDataItem.sprite = sprites[num];
			animationDataItem.Y = num;
			AnimationDataItem animationDataItem2 = animationDataItem;
			float duration;
			if (num <= 4)
			{
				switch (num)
				{
				case 1:
					duration = 0.8f;
					break;
				case 2:
				case 3:
					duration = 3f;
					break;
				case 4:
					duration = 17f;
					break;
				default:
					duration = animationDataItem.duration;
					break;
				}
			}
			else
			{
				duration = animationDataItem.animationClip.length * 0.95f;
			}
			animationDataItem2.duration = duration;
			if (num < _prices.Count)
			{
				animationDataItem.price = _prices[num];
			}
			animations.Add(animationDataItem);
		}
		_byY = null;
		_lookupCount = -1;
	}
}
