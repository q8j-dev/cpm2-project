using System;
using UniRx;

public class SpeedCheckActionButton : UIActionButton
{
	private IDisposable _carSpeedCheck;

	private VehicleData _vehicleData;

	private const int MaxCarSpeedToShowWindow = 3;

	public override void Init()
	{
		if ((bool)V.Car)
		{
			_vehicleData = V.Car.GetComponent<VehicleData>();
		}
		_carSpeedCheck = Observable.Interval(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
		{
			if (!V.Car || !_vehicleData)
			{
				Close();
			}
			else
			{
				bool flag = (bool)V.CoreGameManager && (bool)V.CoreGameManager.MenuLevelParent && V.CoreGameManager.MenuLevelParent.isDroneEnabled;
				bool interactable = _vehicleData.GetPowertrain.SpeedKmh < 3f && !flag;
				SetInteractable(interactable, flag);
			}
		}).AddTo(this);
		base.Init();
	}

	private void OnDestroy()
	{
		_carSpeedCheck?.Dispose();
	}
}
