using System;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class SmoothLoadCarInMenu
	{
		public static void SkinPersonRendererEnabler(bool enable)
		{
			if (!V.DriverPerson)
			{
				return;
			}
			SkinnedMeshRenderer[] componentsInChildren = V.DriverPerson.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = enable;
			}
			if (!enable)
			{
				Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					SkinPersonRendererEnabler(enable: true);
				}).AddTo(V.DriverPerson);
			}
		}

		public static void MeshRendererInChildrenEnabler(GameObject obj, bool enable)
		{
			MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = enable;
			}
			if (!enable)
			{
				Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					MeshRendererInChildrenEnabler(obj, enable: true);
				}).AddTo(obj);
			}
		}
	}
}
