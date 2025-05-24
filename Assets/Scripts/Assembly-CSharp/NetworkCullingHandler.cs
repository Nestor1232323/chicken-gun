using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkCullingHandler : MonoBehaviour, IPunObservable
{
	private int orderIndex;

	private CullArea cullArea;

	private List<byte> previousActiveCells;

	private List<byte> activeCells;

	private PhotonView pView;

	private Vector3 lastPosition;

	private Vector3 currentPosition;

	private void OnEnable()
	{
		if (pView == null)
		{
			pView = GetComponent<PhotonView>();
			if (!pView.isMine)
			{
				return;
			}
		}
		if (cullArea == null)
		{
			cullArea = Object.FindObjectOfType<CullArea>();
		}
		previousActiveCells = new List<byte>(0);
		activeCells = new List<byte>(0);
		currentPosition = (lastPosition = base.transform.position);
	}

	private void Start()
	{
		if (pView.isMine && PhotonNetwork.inRoom)
		{
			if (cullArea.NumberOfSubdivisions == 0)
			{
				pView.group = cullArea.FIRST_GROUP_ID;
				PhotonNetwork.SetInterestGroups(cullArea.FIRST_GROUP_ID, enabled: true);
			}
			else
			{
				pView.ObservedComponents.Add(this);
			}
		}
	}

	private void Update()
	{
		if (pView.isMine)
		{
			lastPosition = currentPosition;
			currentPosition = base.transform.position;
			if (currentPosition != lastPosition && HaveActiveCellsChanged())
			{
				UpdateInterestGroups();
			}
		}
	}

	private void OnGUI()
	{
		if (!pView.isMine)
		{
			return;
		}
		string text = "Inside cells:\n";
		string text2 = "Subscribed cells:\n";
		for (int i = 0; i < activeCells.Count; i++)
		{
			if (i <= cullArea.NumberOfSubdivisions)
			{
				text = text + activeCells[i] + " | ";
			}
			text2 = text2 + activeCells[i] + " | ";
		}
		GUI.Label(new Rect(20f, (float)Screen.height - 120f, 200f, 40f), "<color=white>PhotonView Group: " + pView.group + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
		GUI.Label(new Rect(20f, (float)Screen.height - 100f, 200f, 40f), "<color=white>" + text + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
		GUI.Label(new Rect(20f, (float)Screen.height - 60f, 200f, 40f), "<color=white>" + text2 + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
	}

	private bool HaveActiveCellsChanged()
	{
		if (cullArea.NumberOfSubdivisions == 0)
		{
			return false;
		}
		previousActiveCells = new List<byte>(activeCells);
		activeCells = cullArea.GetActiveCells(base.transform.position);
		while (activeCells.Count <= cullArea.NumberOfSubdivisions)
		{
			activeCells.Add(cullArea.FIRST_GROUP_ID);
		}
		if (activeCells.Count != previousActiveCells.Count)
		{
			return true;
		}
		if (activeCells[cullArea.NumberOfSubdivisions] != previousActiveCells[cullArea.NumberOfSubdivisions])
		{
			return true;
		}
		return false;
	}

	private void UpdateInterestGroups()
	{
		List<byte> list = new List<byte>(0);
		foreach (byte previousActiveCell in previousActiveCells)
		{
			if (!activeCells.Contains(previousActiveCell))
			{
				list.Add(previousActiveCell);
			}
		}
		PhotonNetwork.SetInterestGroups(list.ToArray(), activeCells.ToArray());
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		while (activeCells.Count <= cullArea.NumberOfSubdivisions)
		{
			activeCells.Add(cullArea.FIRST_GROUP_ID);
		}
		if (cullArea.NumberOfSubdivisions == 1)
		{
			orderIndex = ++orderIndex % cullArea.SUBDIVISION_FIRST_LEVEL_ORDER.Length;
			pView.group = activeCells[cullArea.SUBDIVISION_FIRST_LEVEL_ORDER[orderIndex]];
		}
		else if (cullArea.NumberOfSubdivisions == 2)
		{
			orderIndex = ++orderIndex % cullArea.SUBDIVISION_SECOND_LEVEL_ORDER.Length;
			pView.group = activeCells[cullArea.SUBDIVISION_SECOND_LEVEL_ORDER[orderIndex]];
		}
		else if (cullArea.NumberOfSubdivisions == 3)
		{
			orderIndex = ++orderIndex % cullArea.SUBDIVISION_THIRD_LEVEL_ORDER.Length;
			pView.group = activeCells[cullArea.SUBDIVISION_THIRD_LEVEL_ORDER[orderIndex]];
		}
	}
}
