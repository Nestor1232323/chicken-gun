using System;
using Photon;
using UnityEngine;

public class PlayerInventaryManager : Photon.MonoBehaviour
{
	public Transform bodyContainer;

	[SerializeField]
	private InventoryObject currentItemObject;

	public InventoryInputController inventoryInputController;

	public Transform itemPivotContainer;

	public InventoryObject sceneItem;

	public LayerMask layerMaskForTarget;

	[SerializeField]
	private InventoryConnectionLine connectionLine;

	private float distanceToSelectedItem;

	public Transform bulletPivot;

	public Vector3 dirToItem;

	private void Start()
	{
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerJoined));
		CharacterMotor component = GetComponent<CharacterMotor>();
		component.PlayerCrashed = (Action<UnityEngine.Object>)Delegate.Combine(component.PlayerCrashed, new Action<UnityEngine.Object>(OnPlayerCrash));
		if (base.photonView.isMine && (bool)GameWindow.instance)
		{
			inventoryInputController = GameWindow.instance.inventoryInputController;
			inventoryInputController.inventoryManager = this;
			inventoryInputController.ShowSelectBtn(show: true);
			inventoryInputController.ShowDeselectBtn(show: false);
			inventoryInputController.ShowRemoveItemButton(show: false);
			inventoryInputController.ShowDistanceButtons(show: false);
			inventoryInputController.ShowRotationPanel(show: false);
		}
	}

	private void OnNewPlayerJoined(CharacterMotor player)
	{
		if (currentItemObject != null)
		{
			Vector3 position = currentItemObject.transform.position;
			PhotonNetwork.RPC(base.photonView, "UpdStateOnNew", PhotonTargets.Others, false, currentItemObject.photonView.viewID, position);
		}
	}

	[PunRPC]
	private void UpdStateOnNew(int myCurrentItemId, Vector3 hitPoint)
	{
		ItemSelectedRPC(myCurrentItemId, base.photonView.viewID, hitPoint);
	}

	private void OnDestroy()
	{
		if (GameController.instance != null)
		{
			GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerJoined));
			CharacterMotor component = GetComponent<CharacterMotor>();
			component.PlayerCrashed = (Action<UnityEngine.Object>)Delegate.Remove(component.PlayerCrashed, new Action<UnityEngine.Object>(OnPlayerCrash));
		}
	}

	private void OnPlayerCrash(object sender)
	{
		if (base.photonView.isMine)
		{
			DeselectCurrentItem();
		}
	}

	public void CreateItem(InventoryObject item)
	{
		if (currentItemObject != null && currentItemObject.isSelected)
		{
			return;
		}
		if (item.GetComponent<InvZombie>() != null && UnityEngine.Object.FindObjectsOfType<InvZombie>().Length >= 5)
		{
			Debug.Log("too many zombies");
			GameMessageLogger.instance.LogMessage("too many zombies");
			return;
		}
		if (item.category == InventoryCategoryType.Vehicles)
		{
			Vehicle[] array = UnityEngine.Object.FindObjectsOfType<Vehicle>();
			if (array.Length >= ArenaScript.instance.maxVehiclesCount)
			{
				Vehicle[] array2 = Array.FindAll(array, (Vehicle v2) => v2.AreFreeForSitting);
				UnityEngine.MonoBehaviour.print(array2.Length);
				if (array2.Length == 0)
				{
					Debug.Log("Too many vehicles in the scene!");
					GameMessageLogger.instance.LogMessage(LocalizatioManager.GetStringByKey("too_many_vehicles"));
					return;
				}
				PhotonView photonView = array2[UnityEngine.Random.Range(0, array2.Length)].photonView;
				PhotonNetwork.RPC(base.photonView, "RemoveObject", PhotonTargets.All, false, photonView.viewID);
			}
		}
		else
		{
			InventoryObject[] array3 = UnityEngine.Object.FindObjectsOfType<InventoryObject>();
			if (array3.Length >= ArenaScript.instance.maxInvObjectsCount)
			{
				InventoryObject[] array4 = Array.FindAll(array3, (InventoryObject iObj) => !iObj.isSelected);
				array4 = Array.FindAll(array4, (InventoryObject iObj) => iObj.GetComponent<Vehicle>() == null);
				if (array4 != null && array3.Length != 0)
				{
					PhotonView photonView2 = array4[UnityEngine.Random.Range(0, array4.Length)].photonView;
					PhotonNetwork.RPC(base.photonView, "RemoveObject", PhotonTargets.All, false, photonView2.viewID);
				}
			}
		}
		Vector3 position = itemPivotContainer.position;
		Quaternion rotation = itemPivotContainer.rotation;
		if ((item.GetComponent<InvZombie>() != null || item.raycastGroundForSpawnPoint) && Physics.Raycast(itemPivotContainer.position, Vector3.down, out var hitInfo, 250f))
		{
			position = hitInfo.point - Vector3.up * 0f;
			if (position.y < GameController.instance.OurPlayer.transform.position.y)
			{
				position.y = GameController.instance.OurPlayer.transform.position.y;
			}
		}
		currentItemObject = PhotonNetwork.Instantiate(item.prefabName, position, rotation, 0).GetComponent<InventoryObject>();
		if (currentItemObject.Size.z > 3f)
		{
			currentItemObject.transform.position = currentItemObject.transform.position + currentItemObject.Size.z * 0.5f * itemPivotContainer.forward;
		}
		distanceToSelectedItem = 3f;
		if (base.photonView.isMine && item.category == InventoryCategoryType.Vehicles)
		{
			currentItemObject.transform.SetEulerY(currentItemObject.transform.eulerAngles.y + 90f);
		}
		if (currentItemObject.shouldDeselectAfterCreation)
		{
			if (currentItemObject.isPhysicsObject)
			{
				currentItemObject.transform.position = currentItemObject.transform.position + Vector3.up * 0f;
			}
			DecelectItemRPC(base.photonView.viewID);
		}
		else
		{
			currentItemObject.isSelected = true;
			inventoryInputController.ShowSelectBtn(show: false);
			inventoryInputController.ShowDeselectBtn(show: true);
			inventoryInputController.ShowRemoveItemButton(show: true);
			inventoryInputController.ShowDistanceButtons(show: true);
			inventoryInputController.ShowRotationPanel(show: true);
			GameWindow.instance.ShowGetInCarBtn(show: false);
			SelectItemSync(currentItemObject.GetComponent<PhotonView>().viewID, currentItemObject.transform.position);
		}
		GameWindow.instance.ShowInventary(show: false);
	}

	[PunRPC]
	private void RemoveObject(int id)
	{
		PhotonView photonView = PhotonView.Find(id);
		if (photonView != null && photonView.isMine)
		{
			PhotonNetwork.Destroy(photonView);
		}
	}

	private void Update()
	{
		if (!currentItemObject)
		{
			return;
		}
		bulletPivot = GameController.instance.OurPlayer.playerWeaponManager.CurrentWeapon.bulletPivots[0];
		if ((bool)bulletPivot)
		{
			if (base.photonView.isMine)
			{
				currentItemObject.tempHitPoint.rotation = itemPivotContainer.transform.rotation;
				Vector3 vector = bulletPivot.TransformDirection(dirToItem);
				Vector3 position = Vector3.Lerp(currentItemObject.tempHitPoint.transform.position, bulletPivot.position + vector * distanceToSelectedItem, Time.deltaTime * 7.9f);
				currentItemObject.tempHitPoint.transform.position = position;
				connectionLine.SetEndPos(currentItemObject.tempHitPoint.position);
				Debug.DrawLine(bulletPivot.position, bulletPivot.position + vector * 999f, Color.green);
			}
			else
			{
				connectionLine.SetEndPos(currentItemObject.transform.position);
			}
		}
	}

	public bool SelectItemInFront()
	{
		if (Physics.Raycast(Camera.main.ScreenPointToRay(GameWindow.instance.AimSprite.rectTransform.position), out var hitInfo, 20f) && hitInfo.collider.GetComponent<InventoryObject>() != null && !hitInfo.collider.GetComponent<InventoryObject>().isSelected && hitInfo.collider.GetComponent<InventoryObject>().isSelectable)
		{
			SelectItemSync(hitInfo.collider.GetComponent<InventoryObject>().photonView.viewID, hitInfo.point);
			if (hitInfo.collider.GetComponent<InventoryObject>().photonView.ownerId != PhotonNetwork.player.ID)
			{
				Debug.Log("RRRRrequesting ownership. .");
				hitInfo.collider.GetComponent<InventoryObject>().photonView.RequestOwnership();
			}
			else
			{
				Debug.Log("Not requesting ownership. Already mine.");
			}
			inventoryInputController.ShowSelectBtn(show: false);
			inventoryInputController.ShowDeselectBtn(show: true);
			inventoryInputController.ShowRemoveItemButton(show: true);
			inventoryInputController.ShowDistanceButtons(show: true);
			inventoryInputController.ShowRotationPanel(show: true);
			GameWindow.instance.ShowGetInCarBtn(show: false);
		}
		return false;
	}

	private void SelectItemSync(int itemPhotonViewId, Vector3 hitPos)
	{
		PhotonNetwork.RPC(base.photonView, "ItemSelectedRPC", PhotonTargets.All, false, itemPhotonViewId, base.photonView.viewID, hitPos);
	}

	[PunRPC]
	public void ItemSelectedRPC(int itemPhotonViewId, int selectorViewId, Vector3 hitPos)
	{
		if (GameController.instance.OurPlayer == null || GameController.instance.OurPlayer.playerWeaponManager.CurrentWeapon == null)
		{
			return;
		}
		bulletPivot = GameController.instance.OurPlayer.playerWeaponManager.CurrentWeapon.bulletPivots[0];
		GameObject gameObject = new GameObject();
		gameObject.name = "SelectionPoint";
		gameObject.transform.position = hitPos;
		gameObject.transform.rotation = itemPivotContainer.transform.rotation;
		dirToItem = (hitPos - bulletPivot.transform.position).normalized;
		dirToItem = bulletPivot.InverseTransformDirection(dirToItem);
		distanceToSelectedItem = (hitPos - bulletPivot.transform.position).magnitude;
		PhotonView photonView = PhotonView.Find(itemPhotonViewId);
		if (photonView != null)
		{
			currentItemObject = photonView.gameObject.GetComponent<InventoryObject>();
			photonView.gameObject.GetComponent<ISelectable>().Select(selectorViewId);
			currentItemObject.tempHitPoint = gameObject.transform;
			if (base.photonView.isMine)
			{
				currentItemObject.transform.SetParent(gameObject.transform);
			}
			connectionLine.Show(show: true);
			if (base.photonView.isMine)
			{
				inventoryInputController.ShowSelectBtn(show: false);
				inventoryInputController.ShowDeselectBtn(show: true);
			}
		}
		if (!currentItemObject.isPhysicsObject && currentItemObject.GetComponent<Rigidbody>() != null)
		{
			UnityEngine.Object.Destroy(currentItemObject.GetComponent<Rigidbody>());
		}
	}

	public void DeselectCurrentItem()
	{
		PhotonNetwork.RPC(base.photonView, "DecelectItemRPC", PhotonTargets.All, false, base.photonView.viewID);
	}

	[PunRPC]
	public void DecelectItemRPC(int selectorViewId)
	{
		if (currentItemObject != null)
		{
			currentItemObject.transform.SetParent(null);
			currentItemObject.gameObject.GetComponent<ISelectable>().Deselect(selectorViewId);
		}
		connectionLine.Show(show: false);
		if (base.photonView.isMine)
		{
			inventoryInputController.ShowSelectBtn(show: true);
			inventoryInputController.ShowDeselectBtn(show: false);
			inventoryInputController.ShowRemoveItemButton(show: false);
			inventoryInputController.ShowDistanceButtons(show: false);
			inventoryInputController.ShowRotationPanel(show: false);
		}
		currentItemObject = null;
	}

	public void OnOwnershipRequest(object[] viewAndPlayer)
	{
		PhotonView photonView = viewAndPlayer[0] as PhotonView;
		PhotonPlayer photonPlayer = viewAndPlayer[1] as PhotonPlayer;
		Debug.Log(string.Concat("???????????OnOwnershipRequest(): Player ", photonPlayer, " requests ownership of: ", photonView, "."));
		photonView.TransferOwnership(photonPlayer.ID);
	}

	public void UpdateDistance(int sign)
	{
		currentItemObject.tempHitPoint.position += (currentItemObject.tempHitPoint.position - bulletPivot.position).normalized * 1.5f * Time.deltaTime * sign;
	}

	public void RemoveSelectedItem()
	{
		connectionLine.Show(show: false);
		currentItemObject.transform.SetParent(null);
		PhotonNetwork.Destroy(currentItemObject.gameObject);
		inventoryInputController.ShowSelectBtn(show: true);
		inventoryInputController.ShowDeselectBtn(show: false);
		inventoryInputController.ShowRemoveItemButton(show: false);
		inventoryInputController.ShowDistanceButtons(show: false);
		inventoryInputController.ShowRotationPanel(show: false);
	}

	public void SetRotationDelta(Vector2 delta)
	{
		if (currentItemObject != null)
		{
			currentItemObject.transform.Rotate(itemPivotContainer.up, (0f - delta.x) * 0.505f);
		}
	}

	public void ThrowItem(float strengthK)
	{
		Vector3 vector = connectionLine.endPos - connectionLine.startPos.position;
		InventoryObject inventoryObject = currentItemObject;
		DeselectCurrentItem();
		if (strengthK < 0.12f)
		{
			strengthK = 0.12f;
		}
		if (!(inventoryObject.GetComponent<MeshCollider>() != null) || inventoryObject.GetComponent<MeshCollider>().convex)
		{
			inventoryObject.Thow(vector.normalized * strengthK * 80f);
		}
	}
}
