using System.Collections.Generic;
using UnityEngine;

public class PhotonStreamQueue
{
	private int m_SampleRate;

	private int m_SampleCount;

	private int m_ObjectsPerSample = -1;

	private float m_LastSampleTime = float.NegativeInfinity;

	private int m_LastFrameCount = -1;

	private int m_NextObjectIndex = -1;

	private List<object> m_Objects = new List<object>();

	private bool m_IsWriting;

	public PhotonStreamQueue(int sampleRate)
	{
		m_SampleRate = sampleRate;
	}

	private void BeginWritePackage()
	{
		if (Time.realtimeSinceStartup < m_LastSampleTime + 1f / (float)m_SampleRate)
		{
			m_IsWriting = false;
			return;
		}
		if (m_SampleCount == 1)
		{
			m_ObjectsPerSample = m_Objects.Count;
		}
		else if (m_SampleCount > 1 && m_Objects.Count / m_SampleCount != m_ObjectsPerSample)
		{
			Debug.LogWarning("The number of objects sent via a PhotonStreamQueue has to be the same each frame");
			Debug.LogWarning("Objects in List: " + m_Objects.Count + " / Sample Count: " + m_SampleCount + " = " + m_Objects.Count / m_SampleCount + " != " + m_ObjectsPerSample);
		}
		m_IsWriting = true;
		m_SampleCount++;
		m_LastSampleTime = Time.realtimeSinceStartup;
	}

	public void Reset()
	{
		m_SampleCount = 0;
		m_ObjectsPerSample = -1;
		m_LastSampleTime = float.NegativeInfinity;
		m_LastFrameCount = -1;
		m_Objects.Clear();
	}

	public void SendNext(object obj)
	{
		if (Time.frameCount != m_LastFrameCount)
		{
			BeginWritePackage();
		}
		m_LastFrameCount = Time.frameCount;
		if (m_IsWriting)
		{
			m_Objects.Add(obj);
		}
	}

	public bool HasQueuedObjects()
	{
		return m_NextObjectIndex != -1;
	}

	public object ReceiveNext()
	{
		if (m_NextObjectIndex == -1)
		{
			return null;
		}
		if (m_NextObjectIndex >= m_Objects.Count)
		{
			m_NextObjectIndex -= m_ObjectsPerSample;
		}
		return m_Objects[m_NextObjectIndex++];
	}

	public void Serialize(PhotonStream stream)
	{
		if (m_Objects.Count > 0 && m_ObjectsPerSample < 0)
		{
			m_ObjectsPerSample = m_Objects.Count;
		}
		stream.SendNext(m_SampleCount);
		stream.SendNext(m_ObjectsPerSample);
		for (int i = 0; i < m_Objects.Count; i++)
		{
			stream.SendNext(m_Objects[i]);
		}
		m_Objects.Clear();
		m_SampleCount = 0;
	}

	public void Deserialize(PhotonStream stream)
	{
		m_Objects.Clear();
		m_SampleCount = (int)stream.ReceiveNext();
		m_ObjectsPerSample = (int)stream.ReceiveNext();
		for (int i = 0; i < m_SampleCount * m_ObjectsPerSample; i++)
		{
			m_Objects.Add(stream.ReceiveNext());
		}
		if (m_Objects.Count > 0)
		{
			m_NextObjectIndex = 0;
		}
		else
		{
			m_NextObjectIndex = -1;
		}
	}
}
