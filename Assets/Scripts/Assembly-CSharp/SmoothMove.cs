using System;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMove : MonoBehaviour
{
	[Serializable]
	public struct BufferRecord
	{
		public Vector3 pos;

		public Quaternion rot;

		public double time;

		public BufferRecord(Vector3 pos, Quaternion rot, double time)
		{
			this.pos = pos;
			this.rot = rot;
			this.time = time;
		}
	}

	private List<BufferRecord> BufferedPositions = new List<BufferRecord>();

	private PhotonView photonView;

	public BufferRecord first;

	public int count;

	private void Start()
	{
		photonView = GetComponent<PhotonView>();
	}

	private void Update()
	{
		count = BufferedPositions.Count;
		if (!photonView.isMine)
		{
			UpdateClonePosition();
		}
	}

	public void AddStreamPack(Vector3 recivedPlayerPos, Quaternion recivedPlayerRot)
	{
		BufferRecord item = new BufferRecord(recivedPlayerPos, recivedPlayerRot, PhotonNetwork.time);
		BufferedPositions.Add(item);
		if (BufferedPositions.Count > 4)
		{
			BufferedPositions.RemoveAt(0);
		}
		if (first.time == 0.0)
		{
			first = item;
		}
	}

	private void UpdateClonePosition()
	{
		if (BufferedPositions.Count >= 2)
		{
			Predicate<BufferRecord> match = lastBuf;
			int num = BufferedPositions.FindLastIndex(match);
			if (num != -1)
			{
				if (num == BufferedPositions.Count - 1)
				{
					base.transform.position = BufferedPositions[num].pos;
					base.transform.rotation = BufferedPositions[num].rot;
				}
				else
				{
					float t = (float)((myPhotonTime() - BufferedPositions[num].time) / (BufferedPositions[num + 1].time - BufferedPositions[num].time));
					base.transform.position = Vector3.Lerp(BufferedPositions[num].pos, BufferedPositions[num + 1].pos, t);
					base.transform.rotation = Quaternion.Lerp(BufferedPositions[num].rot, BufferedPositions[num + 1].rot, t);
				}
			}
			else
			{
				base.transform.position = BufferedPositions[0].pos;
				base.transform.rotation = BufferedPositions[0].rot;
			}
		}
		else if (BufferedPositions.Count == 1)
		{
			base.transform.position = BufferedPositions[0].pos;
			base.transform.rotation = BufferedPositions[0].rot;
		}
	}

	private double myPhotonTime()
	{
		return PhotonNetwork.time - 0.20000000298023224;
	}

	private bool lastBuf(BufferRecord arg)
	{
		return arg.time < myPhotonTime();
	}
}
