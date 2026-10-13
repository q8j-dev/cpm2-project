using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public abstract class IsActiveGetterGameObjectsBase : EditableHUDElementIsActiveGetter
	{
		[Serializable]
		protected class GameObjectActiveSelfData
		{
			public GameObject GameObject;

			[NonSerialized]
			public bool ActiveSelf;

			public GameObjectActiveSelfData(GameObject gameObject)
			{
				GameObject = gameObject;
			}
		}

		[SerializeField]
		protected GameObjectActiveSelfData[] GameObjectActiveSelfDatas;

		public override void Initialization()
		{
			GameObjectActiveSelfData[] gameObjectActiveSelfDatas = GameObjectActiveSelfDatas;
			if (gameObjectActiveSelfDatas == null || gameObjectActiveSelfDatas.Length <= 0)
			{
				GameObjectActiveSelfDatas = BuildChainFromHierarchy();
			}
			for (int i = 0; i < GameObjectActiveSelfDatas.Length; i++)
			{
				InitGameObjectActiveSelfData(GameObjectActiveSelfDatas[i]);
			}
		}

		protected override void SetRefs()
		{
			base.SetRefs();
			GameObjectActiveSelfDatas = BuildChainFromHierarchy();
		}

		private GameObjectActiveSelfData[] BuildChainFromHierarchy()
		{
			List<GameObject> list = new List<GameObject>();
			Transform parent = base.transform;
			while ((bool)parent)
			{
				list.Add(parent.gameObject);
				parent = parent.parent;
			}
			GameObjectActiveSelfData[] array = new GameObjectActiveSelfData[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new GameObjectActiveSelfData(list[i]);
			}
			return array;
		}

		private void InitGameObjectActiveSelfData(GameObjectActiveSelfData gameObjectActiveSelfData)
		{
			if ((bool)gameObjectActiveSelfData.GameObject)
			{
				gameObjectActiveSelfData.ActiveSelf = gameObjectActiveSelfData.GameObject.activeSelf;
				(from _ in Observable.EveryUpdate()
					where (bool)gameObjectActiveSelfData.GameObject && gameObjectActiveSelfData.GameObject.activeSelf != gameObjectActiveSelfData.ActiveSelf
					select _).Subscribe((long _) =>
				{
					gameObjectActiveSelfData.ActiveSelf = gameObjectActiveSelfData.GameObject.activeSelf;
					OnOnIsActiveChangedEvent();
				}).AddTo(this);
			}
		}
	}
}
