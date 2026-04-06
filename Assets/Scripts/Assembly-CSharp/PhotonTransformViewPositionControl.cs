using System.Collections.Generic;
using UnityEngine;

public class PhotonTransformViewPositionControl
{
	private PhotonTransformViewPositionModel m_Model;

	private float m_CurrentSpeed;

	private double m_LastSerializeTime;

	private Vector3 m_SynchronizedSpeed = Vector3.zero;

	private float m_SynchronizedTurnSpeed;

	private Vector3 m_NetworkPosition;

	private Queue<Vector3> m_OldNetworkPositions = new Queue<Vector3>();

	private bool m_UpdatedPositionAfterOnSerialize = true;

	public PhotonTransformViewPositionControl(PhotonTransformViewPositionModel model)
	{
		m_Model = model;
	}

	private Vector3 GetOldestStoredNetworkPosition()
	{
		Vector3 result = m_NetworkPosition;
		if (m_OldNetworkPositions.Count > 0)
		{
			result = m_OldNetworkPositions.Peek();
		}
		return result;
	}

	public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
	{
		m_SynchronizedSpeed = speed;
		m_SynchronizedTurnSpeed = turnSpeed;
	}

	public Vector3 UpdatePosition(Vector3 currentPosition)
	{
		Vector3 vector = GetNetworkPosition() + GetExtrapolatedPositionOffset();
		switch (m_Model.InterpolateOption)
		{
		case PhotonTransformViewPositionModel.InterpolateOptions.Disabled:
			if (!m_UpdatedPositionAfterOnSerialize)
			{
				currentPosition = vector;
				m_UpdatedPositionAfterOnSerialize = true;
			}
			break;
		case PhotonTransformViewPositionModel.InterpolateOptions.FixedSpeed:
			currentPosition = Vector3.MoveTowards(currentPosition, vector, Time.deltaTime * m_Model.InterpolateMoveTowardsSpeed);
			break;
		case PhotonTransformViewPositionModel.InterpolateOptions.EstimatedSpeed:
			if (m_OldNetworkPositions.Count != 0)
			{
				float num = Vector3.Distance(m_NetworkPosition, GetOldestStoredNetworkPosition()) / (float)m_OldNetworkPositions.Count * (float)PhotonNetwork.sendRateOnSerialize;
				currentPosition = Vector3.MoveTowards(currentPosition, vector, Time.deltaTime * num);
			}
			break;
		case PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues:
			currentPosition = ((m_SynchronizedSpeed.magnitude != 0f) ? Vector3.MoveTowards(currentPosition, vector, Time.deltaTime * m_SynchronizedSpeed.magnitude) : vector);
			break;
		case PhotonTransformViewPositionModel.InterpolateOptions.Lerp:
			currentPosition = Vector3.Lerp(currentPosition, vector, Time.deltaTime * m_Model.InterpolateLerpSpeed);
			break;
		}
		if (m_Model.TeleportEnabled && Vector3.Distance(currentPosition, GetNetworkPosition()) > m_Model.TeleportIfDistanceGreaterThan)
		{
			currentPosition = GetNetworkPosition();
		}
		return currentPosition;
	}

	public Vector3 GetNetworkPosition()
	{
		return m_NetworkPosition;
	}

	public Vector3 GetExtrapolatedPositionOffset()
	{
		float num = (float)(PhotonNetwork.time - m_LastSerializeTime);
		if (m_Model.ExtrapolateIncludingRoundTripTime)
		{
			num += (float)PhotonNetwork.GetPing() / 1000f;
		}
		Vector3 result = Vector3.zero;
		switch (m_Model.ExtrapolateOption)
		{
		case PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues:
		{
			Quaternion quaternion = Quaternion.Euler(0f, m_SynchronizedTurnSpeed * num, 0f);
			result = quaternion * (m_SynchronizedSpeed * num);
			break;
		}
		case PhotonTransformViewPositionModel.ExtrapolateOptions.FixedSpeed:
		{
			Vector3 normalized = (m_NetworkPosition - GetOldestStoredNetworkPosition()).normalized;
			result = normalized * m_Model.ExtrapolateSpeed * num;
			break;
		}
		case PhotonTransformViewPositionModel.ExtrapolateOptions.EstimateSpeedAndTurn:
		{
			Vector3 vector = (m_NetworkPosition - GetOldestStoredNetworkPosition()) * PhotonNetwork.sendRateOnSerialize;
			result = vector * num;
			break;
		}
		}
		return result;
	}

	public void OnPhotonSerializeView(Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info)
	{
		if (m_Model.SynchronizeEnabled)
		{
			if (stream.isWriting)
			{
				SerializeData(currentPosition, stream, info);
			}
			else
			{
				DeserializeData(stream, info);
			}
			m_LastSerializeTime = PhotonNetwork.time;
			m_UpdatedPositionAfterOnSerialize = false;
		}
	}

	private void SerializeData(Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(currentPosition);
		m_NetworkPosition = currentPosition;
		if (m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues || m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues)
		{
			stream.SendNext(m_SynchronizedSpeed);
			stream.SendNext(m_SynchronizedTurnSpeed);
		}
	}

	private void DeserializeData(PhotonStream stream, PhotonMessageInfo info)
	{
		Vector3 networkPosition = (Vector3)stream.ReceiveNext();
		if (m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues || m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues)
		{
			m_SynchronizedSpeed = (Vector3)stream.ReceiveNext();
			m_SynchronizedTurnSpeed = (float)stream.ReceiveNext();
		}
		if (m_OldNetworkPositions.Count == 0)
		{
			m_NetworkPosition = networkPosition;
		}
		m_OldNetworkPositions.Enqueue(m_NetworkPosition);
		m_NetworkPosition = networkPosition;
		while (m_OldNetworkPositions.Count > m_Model.ExtrapolateNumberOfStoredPositions)
		{
			m_OldNetworkPositions.Dequeue();
		}
	}
}
