using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

public class UIMapLeaderboardWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private Button CloseButton = default;

	[SerializeField]
	private UITimeBannerController UITimeBannerController = default;

	[SerializeField]
	private Button BackgroundBackButton = default;

	[SerializeField]
	private RectTransform ContentPanel = default;

	private TimeBannersManager _timeBannersManager;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(TimeBannersManager timeBannersManager)
	{
		_timeBannersManager = timeBannersManager;
	}

	protected override void Start()
	{
		base.Start();
		CloseButton.onClick.AddListener(Close);
		BackgroundBackButton.onClick.AddListener(Close);
		InitBackButton();
		base.GUID = base.gameObject.name;
	}

	public void Init(int trackIndex)
	{
		_timeBannersManager.GetTimeBannersManager(trackIndex).UpdateSpecificBanner(UITimeBannerController);
		UIHelper.DelayedRebuildLayout(ContentPanel).Forget();
	}

	public void Back()
	{
		Close();
	}

	public void InitBackButton()
	{
		_managerUI.SetCurrentWindow(this);
	}
}
