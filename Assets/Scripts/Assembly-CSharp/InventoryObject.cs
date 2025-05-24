using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class InventoryObject : InventoryItem, ISelectable
{
	public bool _isSelected;

	[HideInInspector]
	public bool shouldDeselectAfterCreation;

	public bool isPhysicsObject;

	protected Rigidbody rb;

	[SerializeField]
	private Sprite itemIcon;

	public bool isSelectable = true;

	public bool raycastGroundForSpawnPoint;

	public Transform tempHitPoint;

	protected IEnumerator fadeVelCrt;

	public virtual bool isSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
		}
	}

	public Vector3 Size
	{
		get
		{
			if (GetComponent<Collider>() != null)
			{
				return GetComponent<Collider>().bounds.size;
			}
			return Vector3.one;
		}
	}

	public virtual Sprite icon
	{
		get
		{
			if (GetComponent<ShopItem>() != null)
			{
				return GetComponent<ShopItem>().icon;
			}
			return itemIcon;
		}
	}

	protected virtual void Awake()
	{
		if (isPhysicsObject)
		{
			if (base.gameObject.GetComponent<Rigidbody>() == null)
			{
				rb = base.gameObject.AddComponent<Rigidbody>();
			}
			else
			{
				rb = base.gameObject.GetComponent<Rigidbody>();
			}
		}
		else if (base.gameObject.GetComponent<Rigidbody>() != null)
		{
			Object.Destroy(base.gameObject.GetComponent<Rigidbody>());
		}
		if (!base.photonView.isMine && rb != null)
		{
			rb.isKinematic = true;
			rb.useGravity = false;
		}
		else if (!base.photonView.isMine && rb != null)
		{
			rb.isKinematic = false;
			rb.useGravity = true;
		}
	}

	protected virtual void Start()
	{
	}

	public void OnOwnershipRequest(object[] viewAndPlayer)
	{
		PhotonView photonView = viewAndPlayer[0] as PhotonView;
		PhotonPlayer photonPlayer = viewAndPlayer[1] as PhotonPlayer;
		Debug.Log(string.Concat("OnO==wnershipRequest(): Player ", photonPlayer, " requests ownership of: ", photonView, "."));
		photonView.TransferOwnership(photonPlayer.ID);
	}

	public virtual void Select()
	{
		isSelected = true;
		if (rb != null)
		{
			rb.isKinematic = true;
		}
	}

	public virtual void Select(int senderViewId)
	{
		Select();
	}

	public virtual void Deselect()
	{
		isSelected = false;
		if (rb != null)
		{
			if (base.photonView.isMine)
			{
				rb.isKinematic = false;
				rb.useGravity = true;
			}
			else
			{
				rb.isKinematic = false;
				rb.useGravity = true;
			}
		}
	}

	public virtual void Deselect(int senderViewId)
	{
		isSelected = false;
		if (rb != null && base.photonView.isMine)
		{
			rb.isKinematic = false;
		}
	}

	public void Thow(Vector3 vel)
	{
		rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			rb = base.gameObject.AddComponent<Rigidbody>();
		}
		rb.isKinematic = false;
		rb.useGravity = true;
		rb.velocity = vel;
	}

	protected virtual void OnCollisionExit(Collision collisionInfo)
	{
		if (!base.photonView.isMine)
		{
			if (fadeVelCrt != null)
			{
				StopCoroutine(fadeVelCrt);
			}
			fadeVelCrt = FadeVelocity();
			StartCoroutine(fadeVelCrt);
		}
	}

	protected virtual IEnumerator FadeVelocity()
	{
		if (!(rb == null))
		{
			yield return new WaitForSeconds(2f);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}
}
