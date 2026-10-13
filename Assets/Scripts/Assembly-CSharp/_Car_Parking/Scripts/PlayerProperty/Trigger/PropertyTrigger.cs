using System;
using Car_Parking.Scripts.Racing;
using DG.Tweening;
using MVPFree.MapIcons.Enum;
using Service;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VContainer;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.PlayerProperty.Processor;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.PlayerProperty.Trigger
{
	public abstract class PropertyTrigger : MonoBehaviour, IIntervalUpdatable
	{
		private const float ReferenceFieldOfView = 60f;

		private static readonly int BaseMap = Shader.PropertyToID("_Icon");

		private static readonly int IconSize = Shader.PropertyToID("_IconSize");

		private static readonly int IconSizeFade = Shader.PropertyToID("_IconSizeFade");

		private static readonly float ReferenceFovTangent = Mathf.Tan((float)Math.PI / 6f);

		[SerializeField]
		private DecalProjector DecalProjector;

		[SerializeField]
		private MeshRenderer MainMeshRenderer;

		[SerializeField]
		private ZoneTriggerHandler ZoneTriggerHandler;

		[SerializeField]
		private RieltorModel RieltorModel;

		private IPropertyTriggerProcessor _propertyTriggerProcessor;

		private FreeDriveProvider _freeDriveProvider;

		private FreeDriveControl _freeDriveControl;

		private CircleProgressBarItem _progressBarItem;

		private MenuLevelParent _menuLevelParent;

		private PropertyTriggerService _propertyTriggerService;

		private VehicleData _vehicleData;

		private Vector3 _startSize;

		private Material _mainMeshMaterialInstance;

		private float _baseIconSize;

		private OptionalDependency<TriggerCore> _triggerCoreDep;

		private TriggerCore _triggerCore;

		protected PropertyConfig PropertyConfig { get; private set; }

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, MenuLevelParent menuLevelParent, PropertyTriggerService propertyTriggerService, PropertyConfig propertyConfig, OptionalDependency<TriggerCore> triggerCoreDep)
		{
			_freeDriveProvider = freeDriveProvider;
			_menuLevelParent = menuLevelParent;
			_propertyTriggerService = propertyTriggerService;
			PropertyConfig = propertyConfig;
			_triggerCoreDep = triggerCoreDep;
		}

		protected void InitializeProcessor(IPropertyTriggerProcessor propertyTriggerProcessor)
		{
			_propertyTriggerProcessor = propertyTriggerProcessor;
		}

		public void HideTriggerVisual()
		{
			SwitchOffNpc();
			Vector3 localScale = base.transform.localScale;
			DOTween.Sequence().Append(base.transform.DOScale(localScale * 1.2f, 0.25f)).Append(base.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
			DOTween.Sequence().Append(DOTween.To(() => DecalProjector.size, (Vector3 v) =>
			{
				DecalProjector.size = new Vector3(v.x, v.y, _startSize.z);
			}, new Vector3(_startSize.x * 1.2f, _startSize.y * 1.2f, _startSize.z), 0.25f).SetEase(Ease.OutBack)).Append(DOTween.To(() => DecalProjector.size, (Vector3 v) =>
			{
				DecalProjector.size = new Vector3(v.x, v.y, _startSize.z);
			}, Vector3.zero, 0.2f).SetEase(Ease.InBack));
		}

		private void SwitchOffNpc()
		{
			if (RieltorModel != null && RieltorModel.gameObject.activeSelf)
			{
				RieltorModel.gameObject.SetActive(value: false);
			}
		}

		protected virtual void Awake()
		{
			if ((bool)DecalProjector)
			{
				_startSize = DecalProjector.size;
			}
			ZoneTriggerHandler.CanWithoutCar = true;
			_triggerCore = _triggerCoreDep?.Value;
		}

		protected virtual void Start()
		{
			ZoneTriggerHandler zoneTriggerHandler = ZoneTriggerHandler;
			zoneTriggerHandler.OnZoneEntered = (Action)Delegate.Combine(zoneTriggerHandler.OnZoneEntered, new Action(OnZoneEnter));
			ZoneTriggerHandler zoneTriggerHandler2 = ZoneTriggerHandler;
			zoneTriggerHandler2.OnZoneExit = (Action)Delegate.Combine(zoneTriggerHandler2.OnZoneExit, new Action(OnZoneExit));
		}

		public void SetEnabled(bool value)
		{
			ZoneTriggerHandler.Enabled = value;
		}

		public void IntervalUpdate()
		{
			ZoneTriggerHandler.CheckZone();
		}

		private void OnOutOfCarChanged(bool value)
		{
			if (ZoneTriggerHandler.IsInZone && V.CoreGameManager.ManagerUI.IsParkingMenuActive)
			{
				ZoneTriggerHandler.ForceExit();
			}
		}

		private void OnZoneEnter()
		{
			if (V.CoreGameManager.ManagerUI.IsParkingMenuActive)
			{
				_propertyTriggerService.SetAction(() =>
				{
					OnButtonPressed(_freeDriveProvider.FirstCarMultiPlayer);
				});
				_propertyTriggerService.CreateButton().Forget();
			}
		}

		private void OnZoneExit()
		{
			_propertyTriggerService.SetAction(null);
			_propertyTriggerService.CloseActionButton();
		}

		private void OnButtonPressed(GameObject car)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.AcceptSwitch, base.gameObject);
			_propertyTriggerService.CloseActionButton();
			if (Global.TaskManager.Task != null || FlagsStorage.IsPhotoMode || _menuLevelParent.isDroneEnabled)
			{
				V.CoreGameManager.SetMessage("Free.FinishCurrentMission", 3f);
			}
			else
			{
				_propertyTriggerProcessor?.Process();
			}
		}

		protected void SetMainImage(IconType iconType)
		{
			if ((bool)DecalProjector)
			{
				DecalProjector.material = PropertyConfig.GetDecalMarker(iconType);
			}
			if ((bool)MainMeshRenderer && !_mainMeshMaterialInstance)
			{
				_mainMeshMaterialInstance = MainMeshRenderer.material;
				_mainMeshMaterialInstance.SetTexture(BaseMap, PropertyConfig.GetMarkerSprite(iconType));
				_baseIconSize = _mainMeshMaterialInstance.GetFloat(IconSize);
				_mainMeshMaterialInstance.SetFloat(IconSizeFade, 0f);
			}
			if ((bool)RieltorModel)
			{
				RieltorModel.SetIcon(PropertyConfig.GetRieltorIcon(iconType));
			}
		}

		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera renderingCamera)
		{
			if ((bool)_mainMeshMaterialInstance && !renderingCamera.orthographic)
			{
				float num = Mathf.Tan(renderingCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f));
				_mainMeshMaterialInstance.SetFloat(IconSize, _baseIconSize * num / ReferenceFovTangent);
			}
		}

		protected virtual void OnEnable()
		{
			_triggerCore?.Register(this);
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OnOutOfCarChanged));
			RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
		}

		protected virtual void OnDisable()
		{
			RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OnOutOfCarChanged));
			_propertyTriggerService?.CloseActionButton();
			_triggerCore?.Unregister(this);
		}

		protected virtual void OnDestroy()
		{
			if (ZoneTriggerHandler != null)
			{
				ZoneTriggerHandler zoneTriggerHandler = ZoneTriggerHandler;
				zoneTriggerHandler.OnZoneEntered = (Action)Delegate.Remove(zoneTriggerHandler.OnZoneEntered, new Action(OnZoneEnter));
				ZoneTriggerHandler zoneTriggerHandler2 = ZoneTriggerHandler;
				zoneTriggerHandler2.OnZoneExit = (Action)Delegate.Remove(zoneTriggerHandler2.OnZoneExit, new Action(OnZoneExit));
			}
			CommonExtensions.DestroyMat(ref _mainMeshMaterialInstance);
		}
	}
}
