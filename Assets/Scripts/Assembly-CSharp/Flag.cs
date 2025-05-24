using System;
using System.Collections;
using Photon;
using UnityEngine;

public class Flag : Photon.MonoBehaviour
{
	public TeamID myTeam;

	private CharacterMotor myOwner;

	[SerializeField]
	private Transform initPos;

	public bool isOnBase = true;

	private Rigidbody rb;

	[SerializeField]
	private Collider hardCollider;

	[SerializeField]
	private MeshRenderer meshRenderer;

	private CaptureFlagModeManager _captureFlagModeManager;

	private CaptureFlagModeManager captureFlagModeManager
	{
		get
		{
			if (_captureFlagModeManager == null)
			{
				_captureFlagModeManager = UnityEngine.Object.FindObjectOfType<CaptureFlagModeManager>();
			}
			return _captureFlagModeManager;
		}
	}

	private IEnumerator Start()
	{
		if (GameController.gameConfigData.gameMode != GameMode.CaptureFlag)
		{
			base.gameObject.SetActive(value: false);
			yield break;
		}
		rb = GetComponent<Rigidbody>();
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerJoined));
		GameController.MatchFinidhed += OnMatchFinished;
		while (GameController.instance == null || GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
		yield return null;
		Material material = meshRenderer.material;
		if (GameController.instance.OurPlayer.myTeam != TeamID.None)
		{
			if (myTeam == GameController.instance.OurPlayer.myTeam)
			{
				material.color = DataModel.instance.teamA_Color;
			}
			else
			{
				material.color = DataModel.instance.teamB_Color;
			}
		}
		else
		{
			material.color = Color.black;
		}
	}

	private void OnMatchFinished()
	{
		SetFlagBusy(-1);
		base.transform.position = initPos.position;
	}

	private void OnDestroy()
	{
		GameController.MatchFinidhed -= OnMatchFinished;
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerJoined));
	}

	private void OnNewPlayerJoined(CharacterMotor newPlayer)
	{
		if (base.photonView.isMine && myOwner != null)
		{
			PhotonNetwork.RPC(base.photonView, "SetFlagBusy", PhotonTargets.All, false, myOwner.photonView.viewID);
		}
	}

	private void LateUpdate()
	{
		if (myOwner != null)
		{
			if (!myOwner.IsAlive())
			{
				OnDrop();
			}
			else
			{
				base.transform.position = myOwner.transform.position + Vector3.up * 1.8f;
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		Debug.Log("OnTriggerEnter = " + collider.name);
		if (!base.photonView.isMine)
		{
			return;
		}
		if (collider.CompareTag("FlagPoint") && collider.GetComponent<FlagPoint>() != null)
		{
			if (myOwner == null)
			{
				return;
			}
			Debug.Log("Flagpoint = " + collider.name);
			if (collider.GetComponent<FlagPoint>().team != myTeam)
			{
				TeamID teamID = TeamID.TeamA;
				if (teamID == myTeam)
				{
					teamID = TeamID.TeamB;
				}
				captureFlagModeManager.OnFlagCaptured(teamID, myOwner);
				base.transform.position = initPos.position;
				PhotonNetwork.RPC(base.photonView, "SetFlagBusy", PhotonTargets.All, false, -1);
			}
			else if (myTeam == myOwner.myTeam)
			{
				base.transform.position = initPos.position;
				PhotonNetwork.RPC(base.photonView, "SetFlagBusy", PhotonTargets.All, false, -1);
			}
		}
		else if (myOwner == null && collider.CompareTag("Player") && (!isOnBase || myTeam != collider.GetComponent<CharacterMotor>().myTeam))
		{
			PhotonNetwork.RPC(base.photonView, "SetFlagBusy", PhotonTargets.All, false, collider.GetComponent<PhotonView>().viewID);
		}
	}

	[PunRPC]
	private void SetFlagBusy(int ownerId)
	{
		if (ownerId == -1)
		{
			myOwner = null;
			base.transform.localScale = Vector3.one;
			isOnBase = true;
		}
		else
		{
			PhotonView photonView = PhotonView.Find(ownerId);
			if (photonView != null)
			{
				myOwner = photonView.GetComponent<CharacterMotor>();
				base.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				isOnBase = false;
			}
		}
		rb.useGravity = false;
		rb.isKinematic = true;
		hardCollider.enabled = false;
	}

	private void OnDrop()
	{
		myOwner = null;
		PhotonNetwork.RPC(base.photonView, "OnDropR", PhotonTargets.All, false);
	}

	[PunRPC]
	private void OnDropR()
	{
		myOwner = null;
		isOnBase = false;
		base.transform.localScale = Vector3.one;
		if (base.photonView.isMine)
		{
			rb.isKinematic = false;
		}
	}
}
