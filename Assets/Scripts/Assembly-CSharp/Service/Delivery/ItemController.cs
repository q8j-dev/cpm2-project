using Photon.Pun;
using UnityEngine;

namespace Service.Delivery
{
	public class ItemController : MonoBehaviour
	{
		public Item item;

		[SerializeField]
		private ContainerController containerController;

		public Rigidbody Rigidbody;

		public Collider Collider;

		public GameObject groundLight;

		private Material _lightMaterial;

		public void Init(int id, Transform child, bool isMine)
		{
			containerController = child.parent.GetComponent<ContainerController>();
			Rigidbody = GetComponent<Rigidbody>();
			Collider = GetComponent<Collider>();
			if (!isMine)
			{
				Rigidbody.useGravity = true;
				Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			}
			int type = (PhotonNetwork.IsConnected ? ((int)containerController.trailer.photonView.InstantiationData[0]) : ((Order)Global.TaskManager.Task.Order).id);
			item = new Item(id, child, (ITEM_TYPE)type);
			containerController.items.Add(this);
		}

		private void OnCollisionEnter(Collision other)
		{
			if (other.collider.CompareTag("DriverPerson"))
			{
				if (!(Vector3.Distance(containerController.transform.position, base.transform.position) > 100f))
				{
					item.onGroud = false;
					item.status = true;
					if ((bool)groundLight)
					{
						groundLight.SetActive(value: false);
					}
					containerController.Relocate(this);
				}
			}
			else
			{
				item.OnCollision();
			}
		}

		public void CreateGroundLight(Vector3 position)
		{
			if (!groundLight)
			{
				groundLight = Object.Instantiate(Global.TaskManager.DeliveryManager.groundLightPrefab, base.transform);
			}
			else
			{
				groundLight.SetActive(value: true);
			}
			groundLight.transform.position = position;
			SetScale(groundLight.transform);
			if (!_lightMaterial)
			{
				_lightMaterial = groundLight.transform.GetChild(0).GetComponent<Renderer>().material;
			}
			Color color = Global.TaskManager.DeliveryManager.gradient.Evaluate(1f - (float)item.health / 100f);
			color.a = 0.3f;
			_lightMaterial.SetColor("_TintColor", Global.TaskManager.DeliveryManager.gradient.Evaluate(1f - (float)item.health / 100f));
		}

		private void SetScale(Transform trans)
		{
			_ = base.transform.localScale;
			trans.rotation = Quaternion.Euler(0f, base.transform.rotation.y, base.transform.rotation.z);
			float num = Vector3.Distance(Collider.bounds.min, Collider.bounds.max);
			trans.localScale = new Vector3(num, num, num);
		}

		public void SetConstraints(bool freeze)
		{
			if (!Rigidbody)
			{
				Rigidbody = GetComponent<Rigidbody>();
			}
			Rigidbody.isKinematic = freeze;
		}

		public void Relocate()
		{
			base.transform.parent = containerController.transform;
			base.transform.localRotation = item.rotation;
			base.transform.localPosition = item.position;
			if (containerController.trailer.photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber && PhotonNetwork.IsConnected)
			{
				SetConstraints(freeze: true);
			}
		}

		public void SetLocation(Vector3 pos)
		{
			base.transform.SetParent(V.GlobalPoint);
			base.transform.localPosition = pos;
			item.onGroud = true;
			item.status = false;
			Rigidbody.useGravity = true;
			if (containerController.trailer.photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber && PhotonNetwork.IsConnected)
			{
				SetConstraints(freeze: false);
			}
		}
	}
}
