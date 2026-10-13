using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class DohcCylinderHeadInstance : CylinderHeadInstance
	{
		[Header("Components")]
		public CamshaftInstance<DOHC_Camshaft_Intake> IntakeCamshaft;

		public CamshaftInstance<DOHC_Camshaft_Exhaust> ExhaustCamshaft;

		private List<PartInstanceBase> _subParts;

		public override List<PartInstanceBase> SubParts
		{
			get
			{
				if (_subParts == null || _subParts.Count == 0)
				{
					_subParts = new List<PartInstanceBase> { IntakeCamshaft, ExhaustCamshaft };
				}
				return _subParts;
			}
		}

		[ContextMenu("ApplySpec")]
		public override void UpdateParams()
		{
			base.UpdateParams();
			if (IntakeCamshaft != null)
			{
				IntakeCamshaft.UpdateVariables();
				InMin = base.IntakeTunnelArea * IntakeCamshaft.Part.lift_in_close;
				InMax = base.IntakeTunnelArea * IntakeCamshaft.Part.lift_in_open;
				TimeInOpen = IntakeCamshaft.time_in_open;
				TimeInClose = IntakeCamshaft.time_in_close;
			}
			if (ExhaustCamshaft != null)
			{
				ExhaustCamshaft.UpdateVariables();
				OutMin = base.ExhaustTunnelArea * ExhaustCamshaft.Part.lift_out_close;
				OutMax = base.ExhaustTunnelArea * ExhaustCamshaft.Part.lift_out_open;
				TimeOutOpen = ExhaustCamshaft.time_out_open;
				TimeOutClose = ExhaustCamshaft.time_out_close;
			}
		}

		public override float GetInertia()
		{
			float num;
			if (inertia > 0f)
			{
				num = inertia;
			}
			else
			{
				inertia = Utils.KgToInertia(Part.Mass);
				num = inertia;
			}
			if (IntakeCamshaft != null)
			{
				num += 2f * IntakeCamshaft.Part.GetInertia();
			}
			if (ExhaustCamshaft != null)
			{
				num += 2f * ExhaustCamshaft.Part.GetInertia();
			}
			return num;
		}
	}
}
