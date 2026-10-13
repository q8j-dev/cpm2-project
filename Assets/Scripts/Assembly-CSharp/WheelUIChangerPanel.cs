using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.VisualTuning;

public class WheelUIChangerPanel : MonoBehaviour
{
	private const int MaxWheelAxleType = 3;

	[SerializeField]
	private TextMeshProUGUI tyreTypeTitle = default;

	[SerializeField]
	private SimpleScrollSnap wheelDiameterScrollRect = default;

	[SerializeField]
	private SimpleScrollSnap rimWidthScrollRect = default;

	[SerializeField]
	private SimpleScrollSnap tyreWidthScrollRect = default;

	[SerializeField]
	private SimpleScrollSnap tyreProfileScrollRect = default;

	[SerializeField]
	private SimpleScrollSnap tyreTypeScrollRect = default;

	[SerializeField]
	private TyreWidthUIElement wheelConfigurationElement = default;

	private VisualTuningWindow _visualTuningWindow;

	private int _wheelDiameterId;

	private int _rimWidthId;

	private int _tyreWidthId;

	private int _tyreProfileId;

	private int _tyreTypeId;

	private WheelDiameter[] _tyreDiameterUIElements = new WheelDiameter[0];

	private RimWidth[] _rimWidthUIElements = new RimWidth[0];

	private TyreWidth[] _tyreWidthUIElements = new TyreWidth[0];

	private TyreProfile[] _tyreProfileUIElements = new TyreProfile[0];

	private TyreType[] _rimModelUIElements = new TyreType[0];

	private IPrefabFactory _prefabFactory;

	private IVisualTuningViewModelComplexOne _visualTuningViewModelComplexOne;

	[HideInInspector]
	public WheelTuningAxleType currentWheelAxleType;

	private IWheelsDataService _wheelsDataService;

	private float _frontMaxRadius = float.PositiveInfinity;

	private float _rearMaxRadius = float.PositiveInfinity;

	private readonly List<TyreWidthUIElement> _diameterItems = new List<TyreWidthUIElement>();

	private readonly List<TyreWidthUIElement> _rimWidthItems = new List<TyreWidthUIElement>();

	private readonly List<TyreWidthUIElement> _widthItems = new List<TyreWidthUIElement>();

	private readonly List<TyreWidthUIElement> _profileItems = new List<TyreWidthUIElement>();

	private readonly List<GameObject> _rimModelChildren = new List<GameObject>();

	private const int ListsReadyAll = 7;

	private int _listsReadyMask;

	private CancellationTokenSource _initCts;

	private const string LocBrakeDiscTooLarge = "Visualtuningwindow.Brakediscistoolarge";

	private const string LocWheelSizeNotForType = "Visualtuningwindow.Wheelsizenotavailablefortyretype";

	private const string LocRimWidthNotForType = "Visualtuningwindow.Rimwidthnotavailablefortyretype";

	private const string LocTyreWidthNotForType = "Visualtuningwindow.Tyrewidthnotavailablefortyretype";

	private const string LocProfileNotForType = "Visualtuningwindow.Tyreprofilenotavailablefortyretype";

	private const string LocTyreDoesNotFitArch = "Visualtuningwindow.Tyredoesnotfitwheelarch";

	private const string LocProfileTooLowForSize = "Visualtuningwindow.Tyreprofiletoolowforwheelsize";

	private const string LocRimTooWideForTyre = "Visualtuningwindow.Rimistoowideforthistyre";

	private const string LocRimTooNarrowForTyre = "Visualtuningwindow.Rimistoonarrowforthistyre";

	private const string LocTyreTooWideForRim = "Visualtuningwindow.Tyreistoowideforthisrim";

	private const string LocTyreTooNarrowForRim = "Visualtuningwindow.Tyreistoonarrowforthisrim";

	private bool _geometryLocked;

	private const float MmPerInch = 25.4f;

	public bool IgnoreExecute { get; set; }

	public void SetWheelRadiusLimits(float frontMax, float rearMax)
	{
		_frontMaxRadius = ((frontMax > 0f) ? frontMax : float.PositiveInfinity);
		_rearMaxRadius = ((rearMax > 0f) ? rearMax : float.PositiveInfinity);
	}

	private bool IsDiameterReachable(WheelDiameter d, float maxRadius)
	{
		if (float.IsPositiveInfinity(maxRadius))
		{
			return true;
		}
		TyreWidth tyreWidth = Enum.GetValues(typeof(TyreWidth)).Cast<TyreWidth>().Min();
		TyreProfile tyreProfile = Enum.GetValues(typeof(TyreProfile)).Cast<TyreProfile>().Min();
		return WheelData.CalculateWheelRadius(d, tyreWidth, tyreProfile) <= maxRadius;
	}

	private float GetActiveMaxRadius()
	{
		return currentWheelAxleType switch
		{
			WheelTuningAxleType.Front => _frontMaxRadius, 
			WheelTuningAxleType.Rear => _rearMaxRadius, 
			_ => Mathf.Min(_frontMaxRadius, _rearMaxRadius), 
		};
	}

	private WheelData GetActiveWheelData()
	{
		CarInfo carInfo = _visualTuningWindow.GetCarInfo();
		if (carInfo == null)
		{
			return null;
		}
		if (currentWheelAxleType != WheelTuningAxleType.Rear)
		{
			return carInfo.TuningData.FrontWheelData;
		}
		return carInfo.TuningData.RearWheelData;
	}

	private WheelDiameter GetCurrentVisibleDiameter()
	{
		if (_tyreDiameterUIElements.Length == 0)
		{
			return GetActiveWheelData()?.wheelSize ?? ((WheelDiameter)0);
		}
		int num = Mathf.Clamp(wheelDiameterScrollRect.CenteredPanel, 0, _tyreDiameterUIElements.Length - 1);
		return _tyreDiameterUIElements[num];
	}

	private TyreWidth GetCurrentVisibleTyreWidth()
	{
		if (_tyreWidthUIElements.Length == 0)
		{
			return GetActiveWheelData()?.tyreWidth ?? ((TyreWidth)0);
		}
		int num = Mathf.Clamp(tyreWidthScrollRect.CenteredPanel, 0, _tyreWidthUIElements.Length - 1);
		return _tyreWidthUIElements[num];
	}

	private TyreProfile GetCurrentVisibleTyreProfile()
	{
		if (_tyreProfileUIElements.Length == 0)
		{
			return GetActiveWheelData()?.tyreProfile ?? ((TyreProfile)0);
		}
		int num = Mathf.Clamp(tyreProfileScrollRect.CenteredPanel, 0, _tyreProfileUIElements.Length - 1);
		return _tyreProfileUIElements[num];
	}

	private TyreType GetCurrentVisibleTyreType()
	{
		if (_rimModelUIElements.Length == 0)
		{
			return GetActiveWheelData()?.tyreType ?? TyreType.Touring;
		}
		int num = Mathf.Clamp(tyreTypeScrollRect.CenteredPanel, 0, _rimModelUIElements.Length - 1);
		return _rimModelUIElements[num];
	}

	private RimWidth GetCurrentVisibleRimWidth()
	{
		if (_rimWidthUIElements.Length == 0)
		{
			return GetActiveWheelData()?.rimWidth ?? ((RimWidth)0);
		}
		int num = Mathf.Clamp(rimWidthScrollRect.CenteredPanel, 0, _rimWidthUIElements.Length - 1);
		return _rimWidthUIElements[num];
	}

	private int GetBrakeDiscMinWheelInch()
	{
		CarInfo carInfo = _visualTuningWindow.GetCarInfo();
		if (carInfo?.TuningData == null)
		{
			return 0;
		}
		int valueOrDefault = (carInfo.TuningData.FrontWheelData?.BrakesData?.GetMinWheelInch()).GetValueOrDefault();
		int valueOrDefault2 = (carInfo.TuningData.RearWheelData?.BrakesData?.GetMinWheelInch()).GetValueOrDefault();
		return currentWheelAxleType switch
		{
			WheelTuningAxleType.Front => valueOrDefault, 
			WheelTuningAxleType.Rear => valueOrDefault2, 
			_ => Mathf.Max(valueOrDefault, valueOrDefault2), 
		};
	}

	private bool IsDiameterBrakeBlocked(WheelDiameter d)
	{
		return (int)d < GetBrakeDiscMinWheelInch();
	}

	private static int TyreRimFitSign(TyreWidth tyre, RimWidth rim)
	{
		if (TyreTypeRanges.IsTyreWidthCompatibleWithRim(tyre, rim))
		{
			return 0;
		}
		if (!((float)tyre / 25.4f - (float)rim / 10f < -3.5f))
		{
			return 1;
		}
		return -1;
	}

	private string GetDiameterRejectReason(WheelDiameter d, TyreWidth w, TyreProfile p, TyreType type)
	{
		if (IsDiameterBrakeBlocked(d))
		{
			return "Visualtuningwindow.Brakediscistoolarge";
		}
		if (!TyreTypeRanges.IsDiameterInRange(type, d))
		{
			return "Visualtuningwindow.Wheelsizenotavailablefortyretype";
		}
		if (WouldExceedRadiusLimit(d, w, p))
		{
			return "Visualtuningwindow.Tyredoesnotfitwheelarch";
		}
		return null;
	}

	private string GetRimWidthRejectReason(RimWidth rw, TyreWidth w, TyreType type)
	{
		int num = TyreRimFitSign(w, rw);
		if (num < 0)
		{
			return "Visualtuningwindow.Tyreistoonarrowforthisrim";
		}
		if (num > 0)
		{
			return "Visualtuningwindow.Tyreistoowideforthisrim";
		}
		if (!TyreTypeRanges.IsRimWidthInRange(type, rw))
		{
			return "Visualtuningwindow.Rimwidthnotavailablefortyretype";
		}
		return null;
	}

	private string GetTyreWidthRejectReason(TyreWidth w, WheelDiameter d, TyreProfile p, RimWidth rw, TyreType type)
	{
		int num = TyreRimFitSign(w, rw);
		if (num < 0)
		{
			return "Visualtuningwindow.Rimistoowideforthistyre";
		}
		if (num > 0)
		{
			return "Visualtuningwindow.Rimistoonarrowforthistyre";
		}
		if (!TyreTypeRanges.IsWidthInRange(type, w))
		{
			return "Visualtuningwindow.Tyrewidthnotavailablefortyretype";
		}
		if (WouldExceedRadiusLimit(d, w, p))
		{
			return "Visualtuningwindow.Tyredoesnotfitwheelarch";
		}
		return null;
	}

	private string GetProfileRejectReason(TyreProfile p, WheelDiameter d, TyreWidth w, TyreType type)
	{
		if (p < TyreTypeRanges.MinProfileForDiameter(d))
		{
			return "Visualtuningwindow.Tyreprofiletoolowforwheelsize";
		}
		if (!TyreTypeRanges.IsProfileInRange(type, p))
		{
			return "Visualtuningwindow.Tyreprofilenotavailablefortyretype";
		}
		if (WouldExceedRadiusLimit(d, w, p))
		{
			return "Visualtuningwindow.Tyredoesnotfitwheelarch";
		}
		return null;
	}

	private void SetGeometryRowsVisible(bool visible)
	{
		SetRowVisible(wheelDiameterScrollRect, visible);
		SetRowVisible(rimWidthScrollRect, visible);
		SetRowVisible(tyreWidthScrollRect, visible);
		SetRowVisible(tyreProfileScrollRect, visible);
	}

	private static void SetRowVisible(SimpleScrollSnap scroll, bool visible)
	{
		if ((bool)scroll && (bool)scroll.transform.parent)
		{
			scroll.transform.parent.gameObject.SetActive(visible);
		}
	}

	private void ShowRejectReason(string localizationKey)
	{
		if (!string.IsNullOrEmpty(localizationKey))
		{
			_visualTuningWindow.ShowNotification(new LocalizedString(localizationKey));
		}
	}

	private bool WouldExceedRadiusLimit(WheelDiameter size, TyreWidth width, TyreProfile profile)
	{
		float activeMaxRadius = GetActiveMaxRadius();
		if (float.IsPositiveInfinity(activeMaxRadius))
		{
			return false;
		}
		return WheelData.CalculateWheelRadius(size, width, profile) > activeMaxRadius;
	}

	private void RecomputeFeasibility()
	{
		float activeMaxRadius = GetActiveMaxRadius();
		bool flag = float.IsPositiveInfinity(activeMaxRadius);
		WheelDiameter currentVisibleDiameter = GetCurrentVisibleDiameter();
		TyreWidth currentVisibleTyreWidth = GetCurrentVisibleTyreWidth();
		TyreProfile currentVisibleTyreProfile = GetCurrentVisibleTyreProfile();
		TyreType currentVisibleTyreType = GetCurrentVisibleTyreType();
		RimWidth currentVisibleRimWidth = GetCurrentVisibleRimWidth();
		TyreTypeRanges.Range range = TyreTypeRanges.Get(currentVisibleTyreType);
		int brakeDiscMinWheelInch = GetBrakeDiscMinWheelInch();
		for (int i = 0; i < _tyreDiameterUIElements.Length && i < _diameterItems.Count; i++)
		{
			WheelDiameter wheelDiameter = _tyreDiameterUIElements[i];
			bool flag2 = wheelDiameter >= range.MinDiameter && wheelDiameter <= range.MaxDiameter;
			bool flag3 = flag || WheelData.CalculateWheelRadius(wheelDiameter, currentVisibleTyreWidth, currentVisibleTyreProfile) <= activeMaxRadius;
			bool flag4 = (int)wheelDiameter >= brakeDiscMinWheelInch;
			if ((bool)_diameterItems[i])
			{
				if (!flag4 & flag2 & flag3)
				{
					_diameterItems[i].SetWarning();
				}
				else
				{
					_diameterItems[i].SetValid(flag2 & flag3 & flag4);
				}
			}
		}
		for (int j = 0; j < _rimWidthUIElements.Length && j < _rimWidthItems.Count; j++)
		{
			RimWidth rimWidth = _rimWidthUIElements[j];
			bool flag5 = rimWidth >= range.MinRimWidth && rimWidth <= range.MaxRimWidth;
			bool flag6 = TyreTypeRanges.IsTyreWidthCompatibleWithRim(currentVisibleTyreWidth, rimWidth);
			if ((bool)_rimWidthItems[j])
			{
				_rimWidthItems[j].SetValid(flag5 & flag6);
			}
		}
		for (int k = 0; k < _tyreWidthUIElements.Length && k < _widthItems.Count; k++)
		{
			TyreWidth tyreWidth = _tyreWidthUIElements[k];
			bool flag7 = tyreWidth >= range.MinWidth && tyreWidth <= range.MaxWidth;
			bool flag8 = flag || WheelData.CalculateWheelRadius(currentVisibleDiameter, tyreWidth, currentVisibleTyreProfile) <= activeMaxRadius;
			bool flag9 = TyreTypeRanges.IsTyreWidthCompatibleWithRim(tyreWidth, currentVisibleRimWidth);
			if ((bool)_widthItems[k])
			{
				_widthItems[k].SetValid(flag7 & flag8 & flag9);
			}
		}
		TyreProfile tyreProfile = TyreTypeRanges.MinProfileForDiameter(currentVisibleDiameter);
		for (int l = 0; l < _tyreProfileUIElements.Length && l < _profileItems.Count; l++)
		{
			TyreProfile tyreProfile2 = _tyreProfileUIElements[l];
			bool flag10 = tyreProfile2 >= range.MinProfile && tyreProfile2 <= range.MaxProfile;
			bool flag11 = tyreProfile2 >= tyreProfile;
			bool flag12 = flag || WheelData.CalculateWheelRadius(currentVisibleDiameter, currentVisibleTyreWidth, tyreProfile2) <= activeMaxRadius;
			if ((bool)_profileItems[l])
			{
				_profileItems[l].SetValid(flag10 & flag11 & flag12);
			}
		}
	}

	private int FindRollbackTarget(List<TyreWidthUIElement> items, int prevIdx)
	{
		if (prevIdx >= 0 && prevIdx < items.Count && (bool)items[prevIdx] && items[prevIdx].IsValid)
		{
			return prevIdx;
		}
		int num = -1;
		int num2 = int.MaxValue;
		for (int i = 0; i < items.Count; i++)
		{
			if ((bool)items[i] && items[i].IsValid)
			{
				int num3 = Mathf.Abs(i - prevIdx);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
		}
		if (num < 0)
		{
			return prevIdx;
		}
		return num;
	}

	private void SnapTo(SimpleScrollSnap scroll, int idx)
	{
		try
		{
			IgnoreExecute = true;
			scroll.GoToPanel(idx);
		}
		finally
		{
			IgnoreExecute = false;
		}
	}

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, IWheelsDataService wheelsDataService)
	{
		_wheelsDataService = wheelsDataService;
		_prefabFactory = prefabFactory;
	}

	public int GetCurrentWheelId()
	{
		if (currentWheelAxleType != WheelTuningAxleType.Front && currentWheelAxleType != WheelTuningAxleType.Both)
		{
			return _visualTuningWindow.GetCarInfo().TuningData.RearWheelData.RimModelId;
		}
		return _visualTuningWindow.GetCarInfo().TuningData.FrontWheelData.RimModelId;
	}

	public void InitWithoutNotify(WheelData wheelData)
	{
		tyreProfileScrollRect.OnPanelSelected.RemoveAllListeners();
		SelectTyreProfile(wheelData);
		tyreProfileScrollRect.OnPanelSelected.AddListener(OnTyreProfileChanged);
		rimWidthScrollRect.OnPanelSelected.RemoveAllListeners();
		SelectRimWidth(wheelData);
		rimWidthScrollRect.OnPanelSelected.AddListener(OnRimWidthChanged);
		tyreWidthScrollRect.OnPanelSelected.RemoveAllListeners();
		SelectTyreWidth(wheelData);
		tyreWidthScrollRect.OnPanelSelected.AddListener(OnTyreWidthChanged);
		tyreTypeScrollRect.OnPanelSelected.RemoveAllListeners();
		SelectRimModel(wheelData);
		tyreTypeScrollRect.OnPanelSelected.AddListener(OnTyreTypeChanged);
		wheelDiameterScrollRect.OnPanelSelected.RemoveAllListeners();
		SelectTyreDiameter(wheelData);
		wheelDiameterScrollRect.OnPanelSelected.AddListener(OnWheelDiameterChanged);
		if (_diameterItems.Count > 0 && _widthItems.Count > 0 && _profileItems.Count > 0)
		{
			RecomputeFeasibility();
		}
	}

	public void Init(WheelData wheelData, IVisualTuningViewModelComplexOne visualTuningViewModelComplexOne, VisualTuningWindow visualTuningWindow)
	{
		_visualTuningWindow = visualTuningWindow;
		_visualTuningViewModelComplexOne = visualTuningViewModelComplexOne;
		_geometryLocked = F1TyreRules.IsF1(visualTuningWindow.GetCarInfo()?.CarID ?? 0);
		SetGeometryRowsVisible(!_geometryLocked);
		_initCts?.Cancel();
		_initCts?.Dispose();
		_initCts = new CancellationTokenSource();
		CancellationToken token = _initCts.Token;
		if (!_geometryLocked)
		{
			if (_tyreProfileUIElements.Length != 0)
			{
				SelectTyreProfile(wheelData);
			}
			else
			{
				_tyreProfileUIElements = Enum.GetValues(typeof(TyreProfile)).Cast<TyreProfile>().Reverse()
					.ToArray();
				_tyreProfileId = Array.IndexOf(_tyreProfileUIElements, wheelData.tyreProfile);
				CreateWheelItems(_tyreProfileUIElements, token).Forget();
			}
			if (_rimWidthUIElements.Length != 0)
			{
				SelectRimWidth(wheelData);
			}
			else
			{
				_rimWidthUIElements = Enum.GetValues(typeof(RimWidth)).Cast<RimWidth>().Reverse()
					.ToArray();
				_rimWidthId = Array.IndexOf(_rimWidthUIElements, wheelData.rimWidth);
				CreateWheelItems(_rimWidthUIElements, token).Forget();
			}
			if (_tyreWidthUIElements.Length != 0)
			{
				SelectTyreWidth(wheelData);
			}
			else
			{
				int num = visualTuningWindow.GetCarInfo()?.CarID ?? 0;
				bool isF1 = CarIdStorage.F1Cars.Contains(num);
				_tyreWidthUIElements = (from TyreWidth w in Enum.GetValues(typeof(TyreWidth))
					where isF1 || w != TyreWidth.W455
					select w).Reverse().ToArray();
				_tyreWidthId = Array.IndexOf(_tyreWidthUIElements, wheelData.tyreWidth);
				if (_tyreWidthId < 0)
				{
					_tyreWidthId = 0;
				}
				CreateWheelItems(_tyreWidthUIElements, token).Forget();
			}
		}
		if (_rimModelUIElements.Length != 0)
		{
			SelectRimModel(wheelData);
		}
		else
		{
			int typeCarId = visualTuningWindow.GetCarInfo()?.CarID ?? 0;
			_rimModelUIElements = (from TyreType t in Enum.GetValues(typeof(TyreType))
				where TyreClassRules.IsTyreAllowed(t, typeCarId)
				select t).Reverse().ToArray();
			_tyreTypeId = Array.IndexOf(_rimModelUIElements, wheelData.tyreType);
			if (_tyreTypeId < 0)
			{
				_tyreTypeId = 0;
			}
			CreateWheelItems(_rimModelUIElements, token).Forget();
		}
		if (!_geometryLocked)
		{
			if (_tyreDiameterUIElements.Length != 0)
			{
				SelectTyreDiameter(wheelData);
			}
			else
			{
				float maxRadius = Mathf.Min(_frontMaxRadius, _rearMaxRadius);
				_tyreDiameterUIElements = (from WheelDiameter d in Enum.GetValues(typeof(WheelDiameter))
					where IsDiameterReachable(d, maxRadius)
					select d).Reverse().ToArray();
				_wheelDiameterId = Array.IndexOf(_tyreDiameterUIElements, wheelData.wheelSize);
				if (_wheelDiameterId < 0)
				{
					_wheelDiameterId = 0;
				}
				CreateWheelItems(_tyreDiameterUIElements, token).Forget();
			}
		}
		IgnoreExecute = false;
		if (_diameterItems.Count > 0 && _widthItems.Count > 0 && _profileItems.Count > 0)
		{
			RecomputeFeasibility();
		}
	}

	private void SelectTyreProfile(WheelData wheelData)
	{
		_tyreProfileId = Array.IndexOf(_tyreProfileUIElements, wheelData.tyreProfile);
		GoToElementTyreProfileElement();
	}

	private void SelectRimWidth(WheelData wheelData)
	{
		_rimWidthId = Array.IndexOf(_rimWidthUIElements, wheelData.rimWidth);
		GoToRimWidthElement();
	}

	private void SelectTyreWidth(WheelData wheelData)
	{
		_tyreWidthId = Array.IndexOf(_tyreWidthUIElements, wheelData.tyreWidth);
		GoToTyreWidthElement();
	}

	private void SelectRimModel(WheelData wheelData)
	{
		_tyreTypeId = Array.IndexOf(_rimModelUIElements, wheelData.tyreType);
		GoToTyreTypeElement();
	}

	private void SelectTyreDiameter(WheelData wheelData)
	{
		_wheelDiameterId = Array.IndexOf(_tyreDiameterUIElements, wheelData.wheelSize);
		GoToTyreDiameterElement();
	}

	public bool GetWheelPaid(int index)
	{
		return _wheelsDataService.BoughtWheels.Contains(index);
	}

	private void GoToElementTyreProfileElement()
	{
		for (int i = 0; i < _tyreProfileUIElements.Length; i++)
		{
			if (i == _tyreProfileId)
			{
				tyreProfileScrollRect.GoToPanel(i);
			}
		}
	}

	private void GoToTyreWidthElement()
	{
		for (int i = 0; i < _tyreWidthUIElements.Length; i++)
		{
			if (i == _tyreWidthId)
			{
				tyreWidthScrollRect.GoToPanel(i);
			}
		}
	}

	private void GoToRimWidthElement()
	{
		for (int i = 0; i < _rimWidthUIElements.Length; i++)
		{
			if (i == _rimWidthId)
			{
				rimWidthScrollRect.GoToPanel(i);
			}
		}
	}

	private void GoToTyreDiameterElement()
	{
		for (int i = 0; i < _tyreDiameterUIElements.Length; i++)
		{
			if (i == _wheelDiameterId)
			{
				wheelDiameterScrollRect.GoToPanel(i);
			}
		}
	}

	private void GoToTyreTypeElement()
	{
		for (int i = 0; i < _rimModelUIElements.Length; i++)
		{
			if (i == _tyreTypeId)
			{
				tyreTypeScrollRect.GoToPanel(i);
			}
		}
	}

	private async UniTaskVoid CreateWheelItems(TyreProfile[] array, CancellationToken ct)
	{
		try
		{
			_profileItems.Clear();
			foreach (TyreProfile tyreProfile in array)
			{
				TyreWidthUIElement tyreWidthUIElement = _prefabFactory.Instantiate(wheelConfigurationElement, tyreProfileScrollRect.ScrollRect.content);
				int num = (int)tyreProfile;
				tyreWidthUIElement.Init(num.ToString());
				_profileItems.Add(tyreWidthUIElement);
			}
			tyreProfileScrollRect.gameObject.SetActive(value: true);
			tyreProfileScrollRect.OnPanelSelected.AddListener(OnTyreProfileChanged);
			tyreProfileScrollRect.Setup();
			tyreProfileScrollRect.enabled = true;
			await UniTask.Yield(ct);
			SnapTo(tyreProfileScrollRect, _tyreProfileId);
			MarkListReady(4);
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTaskVoid CreateWheelItems(RimWidth[] array, CancellationToken ct)
	{
		try
		{
			_rimWidthItems.Clear();
			foreach (RimWidth rimWidth in array)
			{
				TyreWidthUIElement tyreWidthUIElement = _prefabFactory.Instantiate(wheelConfigurationElement, rimWidthScrollRect.ScrollRect.content);
				tyreWidthUIElement.Init(((float)rimWidth / 10f).ToString("0.0", CultureInfo.InvariantCulture));
				_rimWidthItems.Add(tyreWidthUIElement);
			}
			rimWidthScrollRect.gameObject.SetActive(value: true);
			rimWidthScrollRect.OnPanelSelected.AddListener(OnRimWidthChanged);
			rimWidthScrollRect.Setup();
			rimWidthScrollRect.enabled = true;
			await UniTask.Yield(ct);
			SnapTo(rimWidthScrollRect, _rimWidthId);
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTaskVoid CreateWheelItems(TyreWidth[] array, CancellationToken ct)
	{
		try
		{
			_widthItems.Clear();
			foreach (TyreWidth tyreWidth in array)
			{
				TyreWidthUIElement tyreWidthUIElement = _prefabFactory.Instantiate(wheelConfigurationElement, tyreWidthScrollRect.ScrollRect.content);
				int num = (int)tyreWidth;
				tyreWidthUIElement.Init(num.ToString());
				_widthItems.Add(tyreWidthUIElement);
			}
			tyreWidthScrollRect.gameObject.SetActive(value: true);
			tyreWidthScrollRect.OnPanelSelected.AddListener(OnTyreWidthChanged);
			tyreWidthScrollRect.Setup();
			tyreWidthScrollRect.enabled = true;
			await UniTask.Yield(ct);
			SnapTo(tyreWidthScrollRect, _tyreWidthId);
			MarkListReady(2);
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTaskVoid CreateWheelItems(TyreType[] array, CancellationToken ct)
	{
		try
		{
			_rimModelChildren.Clear();
			foreach (TyreType type in array)
			{
				TyreWidthUIElement tyreWidthUIElement = _prefabFactory.Instantiate(wheelConfigurationElement, tyreTypeScrollRect.ScrollRect.content);
				tyreWidthUIElement.Init(GetLocalizedType(type));
				_rimModelChildren.Add(tyreWidthUIElement.gameObject);
			}
			tyreTypeTitle.text = new LocalizedString("DragRacingCarSettingsUI.TyreType");
			tyreTypeScrollRect.gameObject.SetActive(value: true);
			tyreTypeScrollRect.OnPanelSelected.AddListener(OnTyreTypeChanged);
			tyreTypeScrollRect.Setup();
			tyreTypeScrollRect.enabled = true;
			await UniTask.Yield(ct);
			SnapTo(tyreTypeScrollRect, _tyreTypeId);
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void MarkListReady(int bit)
	{
		_listsReadyMask |= bit;
		if (_listsReadyMask == 7)
		{
			RecomputeFeasibility();
		}
	}

	private string GetLocalizedType(TyreType type)
	{
		return new LocalizedString($"DragRacingCarSettingsUI.{type}");
	}

	private async UniTaskVoid CreateWheelItems(WheelDiameter[] array, CancellationToken ct)
	{
		try
		{
			_diameterItems.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				WheelDiameter wheelDiameter = array[i];
				TyreWidthUIElement tyreWidthUIElement = _prefabFactory.Instantiate(wheelConfigurationElement, wheelDiameterScrollRect.ScrollRect.content);
				tyreWidthUIElement.Init(wheelDiameter.ToString());
				_diameterItems.Add(tyreWidthUIElement);
			}
			wheelDiameterScrollRect.gameObject.SetActive(value: true);
			wheelDiameterScrollRect.OnPanelSelected.AddListener(OnWheelDiameterChanged);
			wheelDiameterScrollRect.Setup();
			wheelDiameterScrollRect.enabled = true;
			await UniTask.Yield(ct);
			SnapTo(wheelDiameterScrollRect, _wheelDiameterId);
			MarkListReady(1);
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void OnWheelDiameterChanged(int i)
	{
		if (IgnoreExecute)
		{
			return;
		}
		int num = wheelDiameterScrollRect.CenteredPanel;
		TyreWidth currentVisibleTyreWidth = GetCurrentVisibleTyreWidth();
		TyreProfile currentVisibleTyreProfile = GetCurrentVisibleTyreProfile();
		TyreType currentVisibleTyreType = GetCurrentVisibleTyreType();
		string diameterRejectReason = GetDiameterRejectReason(_tyreDiameterUIElements[num], currentVisibleTyreWidth, currentVisibleTyreProfile, currentVisibleTyreType);
		if (diameterRejectReason != null)
		{
			int num2 = FindRollbackTarget(_diameterItems, _wheelDiameterId);
			SnapTo(wheelDiameterScrollRect, num2);
			ShowRejectReason(diameterRejectReason);
			if (num2 == _wheelDiameterId)
			{
				return;
			}
			num = num2;
		}
		WheelDiameter wheelDiameter = _tyreDiameterUIElements[num];
		_visualTuningWindow.PlaySound(SoundEventsDataHelper.TuningSelectRimSizeSwitch);
		LogWheelRadius("OnWheelDiameterChanged", wheelDiameter, currentVisibleTyreWidth, currentVisibleTyreProfile);
		_wheelDiameterId = num;
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectWheelDiameter", wheelDiameter);
		TyreProfile tyreProfile = TyreTypeRanges.MinProfileForDiameter(wheelDiameter);
		if (currentVisibleTyreProfile < tyreProfile)
		{
			int num3 = Array.IndexOf(_tyreProfileUIElements, tyreProfile);
			if (num3 >= 0)
			{
				SnapTo(tyreProfileScrollRect, num3);
				_tyreProfileId = num3;
				_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreProfile", tyreProfile);
			}
		}
		RecomputeFeasibility();
		_visualTuningWindow.SetActiveControlButtons(isActive: true);
	}

	public void OnRimWidthChanged(int i)
	{
		if (IgnoreExecute)
		{
			return;
		}
		int num = rimWidthScrollRect.CenteredPanel;
		TyreType currentVisibleTyreType = GetCurrentVisibleTyreType();
		TyreWidth currentVisibleTyreWidth = GetCurrentVisibleTyreWidth();
		string rimWidthRejectReason = GetRimWidthRejectReason(_rimWidthUIElements[num], currentVisibleTyreWidth, currentVisibleTyreType);
		if (rimWidthRejectReason != null)
		{
			int num2 = FindRollbackTarget(_rimWidthItems, _rimWidthId);
			SnapTo(rimWidthScrollRect, num2);
			ShowRejectReason(rimWidthRejectReason);
			if (num2 == _rimWidthId)
			{
				return;
			}
			num = num2;
		}
		RimWidth rimWidth = _rimWidthUIElements[num];
		LogWheelRadius("OnRimWidthChanged", GetCurrentVisibleDiameter(), GetCurrentVisibleTyreWidth(), GetCurrentVisibleTyreProfile());
		_rimWidthId = num;
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectRimWidth", rimWidth);
		RecomputeFeasibility();
		_visualTuningWindow.SetActiveControlButtons(isActive: true);
	}

	public void OnTyreProfileChanged(int i)
	{
		if (IgnoreExecute)
		{
			return;
		}
		int num = tyreProfileScrollRect.CenteredPanel;
		WheelDiameter currentVisibleDiameter = GetCurrentVisibleDiameter();
		TyreWidth currentVisibleTyreWidth = GetCurrentVisibleTyreWidth();
		TyreType currentVisibleTyreType = GetCurrentVisibleTyreType();
		string profileRejectReason = GetProfileRejectReason(_tyreProfileUIElements[num], currentVisibleDiameter, currentVisibleTyreWidth, currentVisibleTyreType);
		if (profileRejectReason != null)
		{
			int num2 = FindRollbackTarget(_profileItems, _tyreProfileId);
			SnapTo(tyreProfileScrollRect, num2);
			ShowRejectReason(profileRejectReason);
			if (num2 == _tyreProfileId)
			{
				return;
			}
			num = num2;
		}
		TyreProfile tyreProfile = _tyreProfileUIElements[num];
		LogWheelRadius("OnTyreProfileChanged", currentVisibleDiameter, currentVisibleTyreWidth, tyreProfile);
		_tyreProfileId = num;
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreProfile", tyreProfile);
		RecomputeFeasibility();
		_visualTuningWindow.SetActiveControlButtons(isActive: true);
	}

	public void OnTyreTypeChanged(int i)
	{
		if (IgnoreExecute)
		{
			return;
		}
		int centeredPanel = tyreTypeScrollRect.CenteredPanel;
		TyreType tyreType = _rimModelUIElements[centeredPanel];
		TyreWidth tyreWidth = GetCurrentVisibleTyreWidth();
		TyreWidth tyreWidth2 = TyreTypeRanges.ClampToRange(tyreType, tyreWidth);
		if (tyreWidth2 != tyreWidth)
		{
			int num = Array.IndexOf(_tyreWidthUIElements, tyreWidth2);
			if (num >= 0)
			{
				SnapTo(tyreWidthScrollRect, num);
				_tyreWidthId = num;
				_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreWidth", tyreWidth2);
				tyreWidth = tyreWidth2;
			}
		}
		TyreProfile tyreProfile = GetCurrentVisibleTyreProfile();
		WheelDiameter currentVisibleDiameter = GetCurrentVisibleDiameter();
		TyreProfile tyreProfile2 = TyreTypeRanges.ClampToRange(tyreType, tyreProfile, currentVisibleDiameter);
		if (tyreProfile2 != tyreProfile)
		{
			int num2 = Array.IndexOf(_tyreProfileUIElements, tyreProfile2);
			if (num2 >= 0)
			{
				SnapTo(tyreProfileScrollRect, num2);
				_tyreProfileId = num2;
				_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreProfile", tyreProfile2);
				tyreProfile = tyreProfile2;
			}
		}
		RimWidth rimWidth = ((_rimWidthUIElements.Length != 0) ? _rimWidthUIElements[Mathf.Clamp(rimWidthScrollRect.CenteredPanel, 0, _rimWidthUIElements.Length - 1)] : ((RimWidth)0));
		RimWidth rimWidth2 = TyreTypeRanges.ClampToRange(tyreType, rimWidth);
		if (rimWidth2 != rimWidth)
		{
			int num3 = Array.IndexOf(_rimWidthUIElements, rimWidth2);
			if (num3 >= 0)
			{
				SnapTo(rimWidthScrollRect, num3);
				_rimWidthId = num3;
				_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectRimWidth", rimWidth2);
				rimWidth = rimWidth2;
			}
		}
		TyreWidth tyreWidth3 = TyreTypeRanges.ClampTyreWidthToRim(tyreWidth, rimWidth);
		if (tyreWidth3 != tyreWidth)
		{
			int num4 = Array.IndexOf(_tyreWidthUIElements, tyreWidth3);
			if (num4 >= 0)
			{
				SnapTo(tyreWidthScrollRect, num4);
				_tyreWidthId = num4;
				_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreWidth", tyreWidth3);
				tyreWidth = tyreWidth3;
			}
		}
		WheelDiameter currentVisibleDiameter2 = GetCurrentVisibleDiameter();
		WheelDiameter wheelDiameter = TyreTypeRanges.ClampToRange(tyreType, currentVisibleDiameter2);
		if (wheelDiameter != currentVisibleDiameter2 && !IsDiameterBrakeBlocked(wheelDiameter))
		{
			int num5 = Array.IndexOf(_tyreDiameterUIElements, wheelDiameter);
			if (num5 >= 0)
			{
				SnapTo(wheelDiameterScrollRect, num5);
				_wheelDiameterId = num5;
				_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectWheelDiameter", wheelDiameter);
			}
		}
		if (WouldExceedRadiusLimit(GetCurrentVisibleDiameter(), tyreWidth, tyreProfile))
		{
			int num6 = _wheelDiameterId;
			for (int j = _wheelDiameterId + 1; j < _tyreDiameterUIElements.Length; j++)
			{
				if (!IsDiameterBrakeBlocked(_tyreDiameterUIElements[j]) && !WouldExceedRadiusLimit(_tyreDiameterUIElements[j], tyreWidth, tyreProfile))
				{
					num6 = j;
					break;
				}
			}
			if (num6 != _wheelDiameterId)
			{
				SnapTo(wheelDiameterScrollRect, num6);
				_wheelDiameterId = num6;
				_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectWheelDiameter", _tyreDiameterUIElements[num6]);
			}
			if (WouldExceedRadiusLimit(GetCurrentVisibleDiameter(), tyreWidth, tyreProfile))
			{
				for (int k = _tyreProfileId + 1; k < _tyreProfileUIElements.Length; k++)
				{
					TyreProfile tyreProfile3 = _tyreProfileUIElements[k];
					if (TyreTypeRanges.IsProfileInRange(tyreType, tyreProfile3, GetCurrentVisibleDiameter()))
					{
						SnapTo(tyreProfileScrollRect, k);
						_tyreProfileId = k;
						tyreProfile = tyreProfile3;
						_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreProfile", tyreProfile3);
						if (!WouldExceedRadiusLimit(GetCurrentVisibleDiameter(), tyreWidth, tyreProfile))
						{
							break;
						}
					}
				}
			}
			if (WouldExceedRadiusLimit(GetCurrentVisibleDiameter(), tyreWidth, tyreProfile))
			{
				for (int l = _tyreWidthId + 1; l < _tyreWidthUIElements.Length; l++)
				{
					TyreWidth tyreWidth4 = _tyreWidthUIElements[l];
					if (TyreTypeRanges.IsWidthInRange(tyreType, tyreWidth4) && TyreTypeRanges.IsTyreWidthCompatibleWithRim(tyreWidth4, rimWidth))
					{
						SnapTo(tyreWidthScrollRect, l);
						_tyreWidthId = l;
						tyreWidth = tyreWidth4;
						_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreWidth", tyreWidth4);
						if (!WouldExceedRadiusLimit(GetCurrentVisibleDiameter(), tyreWidth, tyreProfile))
						{
							break;
						}
					}
				}
			}
		}
		_tyreTypeId = centeredPanel;
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreType", tyreType);
		LogWheelRadius("OnTyreTypeChanged", GetCurrentVisibleDiameter(), tyreWidth, tyreProfile);
		RecomputeFeasibility();
		_visualTuningWindow.SetActiveControlButtons(isActive: true);
	}

	public void OnTyreWidthChanged(int i)
	{
		if (IgnoreExecute)
		{
			return;
		}
		int num = tyreWidthScrollRect.CenteredPanel;
		WheelDiameter currentVisibleDiameter = GetCurrentVisibleDiameter();
		TyreProfile currentVisibleTyreProfile = GetCurrentVisibleTyreProfile();
		TyreType currentVisibleTyreType = GetCurrentVisibleTyreType();
		RimWidth currentVisibleRimWidth = GetCurrentVisibleRimWidth();
		string tyreWidthRejectReason = GetTyreWidthRejectReason(_tyreWidthUIElements[num], currentVisibleDiameter, currentVisibleTyreProfile, currentVisibleRimWidth, currentVisibleTyreType);
		if (tyreWidthRejectReason != null)
		{
			int num2 = FindRollbackTarget(_widthItems, _tyreWidthId);
			SnapTo(tyreWidthScrollRect, num2);
			ShowRejectReason(tyreWidthRejectReason);
			if (num2 == _tyreWidthId)
			{
				return;
			}
			num = num2;
		}
		TyreWidth tyreWidth = _tyreWidthUIElements[num];
		LogWheelRadius("OnTyreWidthChanged", currentVisibleDiameter, tyreWidth, currentVisibleTyreProfile);
		_tyreWidthId = num;
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Wheels, "SelectTyreWidth", tyreWidth);
		RecomputeFeasibility();
		_visualTuningWindow.SetActiveControlButtons(isActive: true);
	}

	private void LogWheelRadius(string source, WheelDiameter size, TyreWidth width, TyreProfile profile)
	{
		WheelData.CalculateWheelRadius(size, width, profile);
		float activeMaxRadius = GetActiveMaxRadius();
		if (!float.IsPositiveInfinity(activeMaxRadius))
		{
			activeMaxRadius.ToString("F4");
		}
	}

	private void OnDisable()
	{
		_initCts?.Cancel();
		_initCts?.Dispose();
		_initCts = null;
		DisarmScroll(wheelDiameterScrollRect, OnWheelDiameterChanged);
		DisarmScroll(rimWidthScrollRect, OnRimWidthChanged);
		DisarmScroll(tyreWidthScrollRect, OnTyreWidthChanged);
		DisarmScroll(tyreProfileScrollRect, OnTyreProfileChanged);
		DisarmScroll(tyreTypeScrollRect, OnTyreTypeChanged);
		ClearScrollChildren(_diameterItems);
		ClearScrollChildren(_rimWidthItems);
		ClearScrollChildren(_widthItems);
		ClearScrollChildren(_profileItems);
		ClearScrollChildrenGo(_rimModelChildren);
		_tyreDiameterUIElements = Array.Empty<WheelDiameter>();
		_rimWidthUIElements = Array.Empty<RimWidth>();
		_tyreWidthUIElements = Array.Empty<TyreWidth>();
		_tyreProfileUIElements = Array.Empty<TyreProfile>();
		_rimModelUIElements = Array.Empty<TyreType>();
		_listsReadyMask = 0;
	}

	private static void DisarmScroll(SimpleScrollSnap scroll, UnityAction<int> handler)
	{
		if ((bool)scroll)
		{
			scroll.enabled = false;
			scroll.OnPanelSelected.RemoveListener(handler);
		}
	}

	private void ClearScrollChildren(List<TyreWidthUIElement> items)
	{
		foreach (TyreWidthUIElement item in items)
		{
			if (item != null)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		items.Clear();
	}

	private void ClearScrollChildrenGo(List<GameObject> children)
	{
		foreach (GameObject child in children)
		{
			if (child != null)
			{
				UnityEngine.Object.Destroy(child);
			}
		}
		children.Clear();
	}

	public int OnArrowPressed(bool isLeft)
	{
		return (int)Mathf.Repeat((float)(currentWheelAxleType + ((!isLeft) ? 1 : (-1))), 3f);
	}
}
