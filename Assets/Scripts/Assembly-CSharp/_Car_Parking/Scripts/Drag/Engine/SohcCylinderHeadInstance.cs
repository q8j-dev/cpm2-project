using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class SohcCylinderHeadInstance : CylinderHeadInstance
	{
		[Header("Components")]
		public CamshaftInstance<SOHC_Camshaft> Camshaft;

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
				InMin = base.IntakeTunnelArea * Camshaft.Part.lift_in_close * 0.1f;
				InMax = base.IntakeTunnelArea * Camshaft.Part.lift_in_open * 0.1f;
				OutMin = base.ExhaustTunnelArea * Camshaft.Part.lift_out_close * 0.1f;
				OutMax = base.ExhaustTunnelArea * Camshaft.Part.lift_out_open * 0.1f;
				TimeInOpen = Camshaft.time_in_open;
				TimeInClose = Camshaft.time_in_close;
				TimeOutOpen = Camshaft.time_out_open;
				TimeOutClose = Camshaft.time_out_close;
			}
		}

		public override float GetInertia()
		{
			float num = base.GetInertia();
			if (Camshaft != null)
			{
				num += Camshaft.Part.GetInertia();
			}
			return num;
		}
	}
}
