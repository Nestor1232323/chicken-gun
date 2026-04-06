using System.Collections;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
	public bool DestroyByRpc;

	public void OnClick()
	{
		if (!DestroyByRpc)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			base.photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered);
		}
	}

	[PunRPC]
	public IEnumerator DestroyRpc()
	{
		Object.Destroy(base.gameObject);
		yield return 0;
		PhotonNetwork.UnAllocateViewID(base.photonView.viewID);
	}
}
