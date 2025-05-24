using System.Collections.Generic;
using System.Reflection;
using Photon;
using UnityEngine;

[AddComponentMenu("Photon Networking/Photon View &v")]
public class PhotonView : Photon.MonoBehaviour
{
	public int ownerId;

	public byte group;

	protected internal bool mixedModeIsReliable;

	public bool OwnerShipWasTransfered;

	public int prefixBackup = -1;

	internal object[] instantiationDataField;

	protected internal object[] lastOnSerializeDataSent;

	protected internal object[] lastOnSerializeDataReceived;

	public ViewSynchronization synchronization;

	public OnSerializeTransform onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;

	public OnSerializeRigidBody onSerializeRigidBodyOption = OnSerializeRigidBody.All;

	public OwnershipOption ownershipTransfer;

	public List<Component> ObservedComponents;

	private Dictionary<Component, MethodInfo> m_OnSerializeMethodInfos = new Dictionary<Component, MethodInfo>(3);

	[SerializeField]
	private int viewIdField;

	public int instantiationId;

	public int currentMasterID = -1;

	protected internal bool didAwake;

	[SerializeField]
	protected internal bool isRuntimeInstantiated;

	protected internal bool removedFromLocalViewList;

	internal UnityEngine.MonoBehaviour[] RpcMonoBehaviours;

	private MethodInfo OnSerializeMethodInfo;

	private bool failedToFindOnSerialize;

	public int prefix
	{
		get
		{
			if (prefixBackup == -1 && PhotonNetwork.networkingPeer != null)
			{
				prefixBackup = PhotonNetwork.networkingPeer.currentLevelPrefix;
			}
			return prefixBackup;
		}
		set
		{
			prefixBackup = value;
		}
	}

	public object[] instantiationData
	{
		get
		{
			if (!didAwake)
			{
				instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(instantiationId);
			}
			return instantiationDataField;
		}
		set
		{
			instantiationDataField = value;
		}
	}

	public int viewID
	{
		get
		{
			return viewIdField;
		}
		set
		{
			bool num = didAwake && viewIdField == 0;
			ownerId = value / PhotonNetwork.MAX_VIEW_IDS;
			viewIdField = value;
			if (num)
			{
				PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			}
		}
	}

	public bool isSceneView => CreatorActorNr == 0;

	public PhotonPlayer owner => PhotonPlayer.Find(ownerId);

	public int OwnerActorNr => ownerId;

	public bool isOwnerActive
	{
		get
		{
			if (ownerId != 0)
			{
				return PhotonNetwork.networkingPeer.mActors.ContainsKey(ownerId);
			}
			return false;
		}
	}

	public int CreatorActorNr => viewIdField / PhotonNetwork.MAX_VIEW_IDS;

	public bool isMine
	{
		get
		{
			if (ownerId != PhotonNetwork.player.ID)
			{
				if (!isOwnerActive)
				{
					return PhotonNetwork.isMasterClient;
				}
				return false;
			}
			return true;
		}
	}

	protected internal void Awake()
	{
		if (viewID != 0)
		{
			PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(instantiationId);
		}
		didAwake = true;
	}

	public void RequestOwnership()
	{
		PhotonNetwork.networkingPeer.RequestOwnership(viewID, ownerId);
	}

	public void TransferOwnership(PhotonPlayer newOwner)
	{
		TransferOwnership(newOwner.ID);
	}

	public void TransferOwnership(int newOwnerId)
	{
		PhotonNetwork.networkingPeer.TransferOwnership(viewID, newOwnerId);
		ownerId = newOwnerId;
	}

	public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		if (CreatorActorNr == 0 && !OwnerShipWasTransfered && (currentMasterID == -1 || ownerId == currentMasterID))
		{
			ownerId = newMasterClient.ID;
		}
		currentMasterID = newMasterClient.ID;
	}

	protected internal void OnDestroy()
	{
		if (!removedFromLocalViewList)
		{
			bool num = PhotonNetwork.networkingPeer.LocalCleanPhotonView(this);
			bool flag = false;
			if (num && !flag && instantiationId > 0 && !PhotonHandler.AppQuits && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log("PUN-instantiated '" + base.gameObject.name + "' got destroyed by engine. This is OK when loading levels. Otherwise use: PhotonNetwork.Destroy().");
			}
		}
	}

	public void SerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (ObservedComponents != null && ObservedComponents.Count > 0)
		{
			for (int i = 0; i < ObservedComponents.Count; i++)
			{
				SerializeComponent(ObservedComponents[i], stream, info);
			}
		}
	}

	public void DeserializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (ObservedComponents != null && ObservedComponents.Count > 0)
		{
			for (int i = 0; i < ObservedComponents.Count; i++)
			{
				DeserializeComponent(ObservedComponents[i], stream, info);
			}
		}
	}

	protected internal void DeserializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		if (component == null)
		{
			return;
		}
		if (component is UnityEngine.MonoBehaviour)
		{
			ExecuteComponentOnSerialize(component, stream, info);
		}
		else if (component is Transform)
		{
			Transform transform = (Transform)component;
			switch (onSerializeTransformOption)
			{
			case OnSerializeTransform.All:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				transform.localScale = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeTransform.OnlyPosition:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeTransform.OnlyRotation:
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				break;
			case OnSerializeTransform.OnlyScale:
				transform.localScale = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeTransform.PositionAndRotation:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				break;
			}
		}
		else if (component is Rigidbody)
		{
			Rigidbody rigidbody = (Rigidbody)component;
			switch (onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.All:
				rigidbody.velocity = (Vector3)stream.ReceiveNext();
				rigidbody.angularVelocity = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				rigidbody.angularVelocity = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeRigidBody.OnlyVelocity:
				rigidbody.velocity = (Vector3)stream.ReceiveNext();
				break;
			}
		}
		else if (component is Rigidbody2D)
		{
			Rigidbody2D rigidbody2D = (Rigidbody2D)component;
			switch (onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.All:
				rigidbody2D.velocity = (Vector2)stream.ReceiveNext();
				rigidbody2D.angularVelocity = (float)stream.ReceiveNext();
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				rigidbody2D.angularVelocity = (float)stream.ReceiveNext();
				break;
			case OnSerializeRigidBody.OnlyVelocity:
				rigidbody2D.velocity = (Vector2)stream.ReceiveNext();
				break;
			}
		}
		else
		{
			Debug.LogError("Type of observed is unknown when receiving.");
		}
	}

	protected internal void SerializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		if (component == null)
		{
			return;
		}
		if (component is UnityEngine.MonoBehaviour)
		{
			ExecuteComponentOnSerialize(component, stream, info);
		}
		else if (component is Transform)
		{
			Transform transform = (Transform)component;
			switch (onSerializeTransformOption)
			{
			case OnSerializeTransform.All:
				stream.SendNext(transform.localPosition);
				stream.SendNext(transform.localRotation);
				stream.SendNext(transform.localScale);
				break;
			case OnSerializeTransform.OnlyPosition:
				stream.SendNext(transform.localPosition);
				break;
			case OnSerializeTransform.OnlyRotation:
				stream.SendNext(transform.localRotation);
				break;
			case OnSerializeTransform.OnlyScale:
				stream.SendNext(transform.localScale);
				break;
			case OnSerializeTransform.PositionAndRotation:
				stream.SendNext(transform.localPosition);
				stream.SendNext(transform.localRotation);
				break;
			}
		}
		else if (component is Rigidbody)
		{
			Rigidbody rigidbody = (Rigidbody)component;
			switch (onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.All:
				stream.SendNext(rigidbody.velocity);
				stream.SendNext(rigidbody.angularVelocity);
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				stream.SendNext(rigidbody.angularVelocity);
				break;
			case OnSerializeRigidBody.OnlyVelocity:
				stream.SendNext(rigidbody.velocity);
				break;
			}
		}
		else if (component is Rigidbody2D)
		{
			Rigidbody2D rigidbody2D = (Rigidbody2D)component;
			switch (onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.All:
				stream.SendNext(rigidbody2D.velocity);
				stream.SendNext(rigidbody2D.angularVelocity);
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				stream.SendNext(rigidbody2D.angularVelocity);
				break;
			case OnSerializeRigidBody.OnlyVelocity:
				stream.SendNext(rigidbody2D.velocity);
				break;
			}
		}
		else
		{
			Debug.LogError("Observed type is not serializable: " + component.GetType());
		}
	}

	protected internal void ExecuteComponentOnSerialize(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		if (component is IPunObservable punObservable)
		{
			punObservable.OnPhotonSerializeView(stream, info);
		}
		else
		{
			if (!(component != null))
			{
				return;
			}
			MethodInfo value = null;
			if (!m_OnSerializeMethodInfos.TryGetValue(component, out value))
			{
				if (!NetworkingPeer.GetMethod(component as UnityEngine.MonoBehaviour, PhotonNetworkingMessage.OnPhotonSerializeView.ToString(), out value))
				{
					Debug.LogError("The observed monobehaviour (" + component.name + ") of this PhotonView does not implement OnPhotonSerializeView()!");
					value = null;
				}
				m_OnSerializeMethodInfos.Add(component, value);
			}
			if (value != null)
			{
				value.Invoke(component, new object[2] { stream, info });
			}
		}
	}

	public void RefreshRpcMonoBehaviourCache()
	{
		RpcMonoBehaviours = GetComponents<UnityEngine.MonoBehaviour>();
	}

	public void RPC(string methodName, PhotonTargets target, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt: false, parameters);
	}

	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, parameters);
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, encrpyt: false, parameters);
	}

	public void RpcSecure(string methodName, PhotonPlayer targetPlayer, bool encrypt, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, parameters);
	}

	public static PhotonView Get(Component component)
	{
		return component.GetComponent<PhotonView>();
	}

	public static PhotonView Get(GameObject gameObj)
	{
		return gameObj.GetComponent<PhotonView>();
	}

	public static PhotonView Find(int viewID)
	{
		return PhotonNetwork.networkingPeer.GetPhotonView(viewID);
	}

	public override string ToString()
	{
		return string.Format("View ({3}){0} on {1} {2}", viewID, (!(base.gameObject != null)) ? "GO==null" : base.gameObject.name, (!isSceneView) ? string.Empty : "(scene)", prefix);
	}
}
