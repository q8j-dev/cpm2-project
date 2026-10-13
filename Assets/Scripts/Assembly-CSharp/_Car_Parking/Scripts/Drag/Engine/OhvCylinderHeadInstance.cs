using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class OhvCylinderHeadInstance : CylinderHeadInstance
	{
		[Header("Components")]
		public CamshaftInstance<OHV_Camshaft> Camshaft;

		private List<PartInstanceBase> _subParts;

		public override List<PartInstanceBase> SubParts
		{
			get
			{
				if (_subParts == null || _subParts.Count == 0)
				{
					_subParts = new List<PartInstanceBase> { Camshaft };
				}
				return _subParts;
			}
		}

		public override void UpdateParams()
		{
			base.UpdateParams();
			if (Camshaft != null)
			{
				Camshaft.UpdateVariables();
				InMin = base.IntakeTunnelArea * Camshaft.Part.lift_in_close * 0.1f * Part.intake_rocker_ratio;
				InMax = base.IntakeTunnelArea * Camshaft.Part.lift_in_open * 0.1f * Part.intake_rocker_ratio;
				OutMin = base.ExhaustTunnelArea * Camshaft.Part.lift_out_close * 0.1f * Part.exhaust_rocker_ratio;
				OutMax = base.ExhaustTunnelArea * Camshaft.Part.lift_out_open * 0.1f * Part.exhaust_rocker_ratio;
				TimeInOpen = Camshaft.time_in_open;
				TimeInClose = Camshaft.time_in_close;
				TimeOutOpen = Camshaft.time_out_open;
				TimeOutClose = Camshaft.time_out_close;
			}
		}
	}
}
