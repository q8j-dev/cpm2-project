using System;
using UnityEngine;

namespace Service.Delivery
{
	[Serializable]
	public class Item
	{
		public int id;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;

		public bool status;

		public bool onGroud;

		public int collisionCount;

		public int health;

		private System.Random rand;

		[SerializeField]
		private int _price;

		public int price => (int)((float)_price * (float)health / 100f);

		public Item(int id, Transform transform, ITEM_TYPE type)
		{
			rand = new System.Random();
			this.id = id;
			position = transform.localPosition;
			rotation = transform.localRotation;
			scale = transform.localScale;
			status = true;
			switch (type)
			{
			case ITEM_TYPE.BOX:
				_price = rand.Next(200, 300);
				break;
			case ITEM_TYPE.CEMENT:
				_price = rand.Next(250, 400);
				break;
			case ITEM_TYPE.WOOD:
				_price = rand.Next(200, 250);
				break;
			default:
				_price = rand.Next(10, 20);
				break;
			}
			collisionCount = 0;
			health = 100;
			onGroud = false;
		}

		public void OnCollision()
		{
			rand = new System.Random();
			if (!status)
			{
				collisionCount++;
				if (health > 0)
				{
					health -= rand.Next(10);
				}
				if (health < 0)
				{
					health = 0;
				}
			}
		}
	}
}
