using System;
using CPM.Transmission;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class GearBoxInstance : PartInstance<GearBoxPart>, IReturnablePart<TransmissionValues>
	{
		private readonly int _gearboxType;

		private readonly GearboxKind _kind;

		private readonly float _torqueSplit;

		private readonly float[] _ratio;

		private readonly float _finalDrive;

		private readonly bool _hasLaunchControl;

		private readonly float _launchControlRpm;

		private readonly float _launchControlTime;

		private readonly float _inertia;

		private readonly AutoShifterModes _autoModes;

		private readonly int _lowMaxGear;

		public int Gears
		{
			get
			{
				if (_ratio.Length > 3)
				{
					return _ratio.Length - 2;
				}
				return 0;
			}
		}

		public GearBoxInstance(GearBoxPart part)
		{
			Part = part;
			TransmissionValues getParams = Part.GetParams;
			_torqueSplit = getParams.TorqueSplit;
			_ratio = getParams.GearRatio;
			_finalDrive = getParams.FinalDrive;
			_gearboxType = getParams.Type;
			_hasLaunchControl = getParams.LaunchControl;
			_launchControlRpm = getParams.LaunchRpm;
			_launchControlTime = getParams.LaunchTime;
			_inertia = getParams.Inertia;
			_kind = getParams.Kind;
			_autoModes = getParams.AutoModes;
			_lowMaxGear = getParams.LowMaxGear;
		}

		public TransmissionValues GetValue()
		{
			return new TransmissionValues
			{
				TorqueSplit = _torqueSplit,
				GearRatio = (_ratio.Clone() as float[]),
				FinalDrive = _finalDrive,
				Type = _gearboxType,
				LaunchControl = _hasLaunchControl,
				LaunchRpm = _launchControlRpm,
				LaunchTime = _launchControlTime,
				Inertia = _inertia,
				Kind = _kind,
				AutoModes = _autoModes,
				LowMaxGear = _lowMaxGear
			};
		}
	}
}
