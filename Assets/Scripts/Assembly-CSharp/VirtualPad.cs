using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

[InputControlLayout(displayName = "VirtualPad")]
public class VirtualPad : InputDevice
{
	[InputControl(layout = "Axis")]
	public AxisControl Throttle { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl Brake { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl Clutch { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl Handbrake { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl Starter { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl LeftSignal { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl RightSignal { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl EmergencySignal { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl Horn { get; private set; }

	[InputControl(layout = "Axis")]
	public AxisControl Headlights { get; private set; }

	protected override void FinishSetup()
	{
		base.FinishSetup();
		Throttle = GetChildControl<AxisControl>("throttle");
		Brake = GetChildControl<AxisControl>("brake");
		Clutch = GetChildControl<AxisControl>("clutch");
		Handbrake = GetChildControl<AxisControl>("handbrake");
		Starter = GetChildControl<AxisControl>("starter");
		LeftSignal = GetChildControl<AxisControl>("leftSignal");
		RightSignal = GetChildControl<AxisControl>("rightSignal");
		EmergencySignal = GetChildControl<AxisControl>("emergencySignal");
		Horn = GetChildControl<AxisControl>("horn");
		Headlights = GetChildControl<AxisControl>("headlights");
	}
}
