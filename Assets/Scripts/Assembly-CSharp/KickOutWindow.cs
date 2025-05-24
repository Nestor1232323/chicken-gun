using System.Collections.Generic;
using Photon;
using UnityEngine;
using UnityEngine.UI;

public class KickOutWindow : Photon.MonoBehaviour
{
	public Transform rowsContainer;

	[SerializeField]
	private GameObject window;

	[SerializeField]
	private GameObject rowPrefab;

	[SerializeField]
	private GameObject kickedMeAlert;

	private void CreatePlayersList()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in rowsContainer)
		{
			list.Add(item.gameObject);
		}
		list.ForEach(delegate(GameObject child)
		{
			child.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			Object.Destroy(child);
		});
		int num = 1;
		foreach (CharacterMotor player in GameController.instance.Players)
		{
			if (player.photonView.isMine)
			{
				continue;
			}
			KickOutListRow component = Object.Instantiate(rowPrefab).GetComponent<KickOutListRow>();
			component.transform.SetParent(rowsContainer);
			component.transform.localScale = Vector3.one;
			component.SetIndex(num);
			component.SetPlayerName(player.playerInfo.name);
			int p = player.photonView.viewID;
			component.GetComponentInChildren<Button>().onClick.AddListener(delegate
			{
				if (PhotonNetwork.isMasterClient)
				{
					UnityEngine.MonoBehaviour.print("kick " + p);
					PhotonNetwork.RPC(base.photonView, "KickPlayer", PhotonTargets.All, false, p);
				}
			});
			num++;
		}
	}

	public void Show(bool show)
	{
	}

	[PunRPC]
	private void KickPlayer(int viewId)
	{
		UnityEngine.MonoBehaviour.print("kick RPC " + viewId);
		PhotonView photonView = PhotonView.Find(viewId);
		if (photonView != null && photonView.isMine)
		{
			kickedMeAlert.SetActive(value: true);
			Invoke("KickMe", 1.14f);
		}
	}

	private void KickMe()
	{
		PhotonNetwork.LeaveRoom();
	}
}
