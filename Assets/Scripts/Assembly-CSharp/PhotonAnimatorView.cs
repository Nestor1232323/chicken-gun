using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhotonView))]
[AddComponentMenu("Photon Networking/Photon Animator View")]
public class PhotonAnimatorView : MonoBehaviour, IPunObservable
{
	public enum ParameterType
	{
		Float = 1,
		Int = 3,
		Bool = 4,
		Trigger = 9
	}

	public enum SynchronizeType
	{
		Disabled = 0,
		Discrete = 1,
		Continuous = 2
	}

	[Serializable]
	public class SynchronizedParameter
	{
		public ParameterType Type;

		public SynchronizeType SynchronizeType;

		public string Name;
	}

	[Serializable]
	public class SynchronizedLayer
	{
		public SynchronizeType SynchronizeType;

		public int LayerIndex;
	}

	private Animator m_Animator;

	private PhotonStreamQueue m_StreamQueue;

	[HideInInspector]
	[SerializeField]
	private bool ShowLayerWeightsInspector = true;

	[HideInInspector]
	[SerializeField]
	private bool ShowParameterInspector = true;

	[HideInInspector]
	[SerializeField]
	private List<SynchronizedParameter> m_SynchronizeParameters = new List<SynchronizedParameter>();

	[HideInInspector]
	[SerializeField]
	private List<SynchronizedLayer> m_SynchronizeLayers = new List<SynchronizedLayer>();

	private Vector3 m_ReceiverPosition;

	private float m_LastDeserializeTime;

	private bool m_WasSynchronizeTypeChanged = true;

	private PhotonView m_PhotonView;

	private List<string> m_raisedDiscreteTriggersCache = new List<string>();

	private void Awake()
	{
		m_PhotonView = GetComponent<PhotonView>();
		m_StreamQueue = new PhotonStreamQueue(120);
		m_Animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (m_Animator.applyRootMotion && !m_PhotonView.isMine && PhotonNetwork.connected)
		{
			m_Animator.applyRootMotion = false;
		}
		if (!PhotonNetwork.inRoom || PhotonNetwork.room.PlayerCount <= 1)
		{
			m_StreamQueue.Reset();
		}
		else if (m_PhotonView.isMine)
		{
			SerializeDataContinuously();
			CacheDiscreteTriggers();
		}
		else
		{
			DeserializeDataContinuously();
		}
	}

	public void CacheDiscreteTriggers()
	{
		for (int i = 0; i < m_SynchronizeParameters.Count; i++)
		{
			SynchronizedParameter synchronizedParameter = m_SynchronizeParameters[i];
			if (synchronizedParameter.SynchronizeType == SynchronizeType.Discrete && synchronizedParameter.Type == ParameterType.Trigger && m_Animator.GetBool(synchronizedParameter.Name) && synchronizedParameter.Type == ParameterType.Trigger)
			{
				m_raisedDiscreteTriggersCache.Add(synchronizedParameter.Name);
				break;
			}
		}
	}

	public bool DoesLayerSynchronizeTypeExist(int layerIndex)
	{
		return m_SynchronizeLayers.FindIndex((SynchronizedLayer item) => item.LayerIndex == layerIndex) != -1;
	}

	public bool DoesParameterSynchronizeTypeExist(string name)
	{
		return m_SynchronizeParameters.FindIndex((SynchronizedParameter item) => item.Name == name) != -1;
	}

	public List<SynchronizedLayer> GetSynchronizedLayers()
	{
		return m_SynchronizeLayers;
	}

	public List<SynchronizedParameter> GetSynchronizedParameters()
	{
		return m_SynchronizeParameters;
	}

	public SynchronizeType GetLayerSynchronizeType(int layerIndex)
	{
		int num = m_SynchronizeLayers.FindIndex((SynchronizedLayer item) => item.LayerIndex == layerIndex);
		if (num == -1)
		{
			return SynchronizeType.Disabled;
		}
		return m_SynchronizeLayers[num].SynchronizeType;
	}

	public SynchronizeType GetParameterSynchronizeType(string name)
	{
		int num = m_SynchronizeParameters.FindIndex((SynchronizedParameter item) => item.Name == name);
		if (num == -1)
		{
			return SynchronizeType.Disabled;
		}
		return m_SynchronizeParameters[num].SynchronizeType;
	}

	public void SetLayerSynchronized(int layerIndex, SynchronizeType synchronizeType)
	{
		if (Application.isPlaying)
		{
			m_WasSynchronizeTypeChanged = true;
		}
		int num = m_SynchronizeLayers.FindIndex((SynchronizedLayer item) => item.LayerIndex == layerIndex);
		if (num == -1)
		{
			m_SynchronizeLayers.Add(new SynchronizedLayer
			{
				LayerIndex = layerIndex,
				SynchronizeType = synchronizeType
			});
		}
		else
		{
			m_SynchronizeLayers[num].SynchronizeType = synchronizeType;
		}
	}

	public void SetParameterSynchronized(string name, ParameterType type, SynchronizeType synchronizeType)
	{
		if (Application.isPlaying)
		{
			m_WasSynchronizeTypeChanged = true;
		}
		int num = m_SynchronizeParameters.FindIndex((SynchronizedParameter item) => item.Name == name);
		if (num == -1)
		{
			m_SynchronizeParameters.Add(new SynchronizedParameter
			{
				Name = name,
				Type = type,
				SynchronizeType = synchronizeType
			});
		}
		else
		{
			m_SynchronizeParameters[num].SynchronizeType = synchronizeType;
		}
	}

	private void SerializeDataContinuously()
	{
		if (m_Animator == null)
		{
			return;
		}
		for (int i = 0; i < m_SynchronizeLayers.Count; i++)
		{
			if (m_SynchronizeLayers[i].SynchronizeType == SynchronizeType.Continuous)
			{
				m_StreamQueue.SendNext(m_Animator.GetLayerWeight(m_SynchronizeLayers[i].LayerIndex));
			}
		}
		for (int j = 0; j < m_SynchronizeParameters.Count; j++)
		{
			SynchronizedParameter synchronizedParameter = m_SynchronizeParameters[j];
			if (synchronizedParameter.SynchronizeType == SynchronizeType.Continuous)
			{
				switch (synchronizedParameter.Type)
				{
				case ParameterType.Bool:
					m_StreamQueue.SendNext(m_Animator.GetBool(synchronizedParameter.Name));
					break;
				case ParameterType.Float:
					m_StreamQueue.SendNext(m_Animator.GetFloat(synchronizedParameter.Name));
					break;
				case ParameterType.Int:
					m_StreamQueue.SendNext(m_Animator.GetInteger(synchronizedParameter.Name));
					break;
				case ParameterType.Trigger:
					m_StreamQueue.SendNext(m_Animator.GetBool(synchronizedParameter.Name));
					break;
				}
			}
		}
	}

	private void DeserializeDataContinuously()
	{
		if (!m_StreamQueue.HasQueuedObjects())
		{
			return;
		}
		for (int i = 0; i < m_SynchronizeLayers.Count; i++)
		{
			if (m_SynchronizeLayers[i].SynchronizeType == SynchronizeType.Continuous)
			{
				m_Animator.SetLayerWeight(m_SynchronizeLayers[i].LayerIndex, (float)m_StreamQueue.ReceiveNext());
			}
		}
		for (int j = 0; j < m_SynchronizeParameters.Count; j++)
		{
			SynchronizedParameter synchronizedParameter = m_SynchronizeParameters[j];
			if (synchronizedParameter.SynchronizeType == SynchronizeType.Continuous)
			{
				switch (synchronizedParameter.Type)
				{
				case ParameterType.Bool:
					m_Animator.SetBool(synchronizedParameter.Name, (bool)m_StreamQueue.ReceiveNext());
					break;
				case ParameterType.Float:
					m_Animator.SetFloat(synchronizedParameter.Name, (float)m_StreamQueue.ReceiveNext());
					break;
				case ParameterType.Int:
					m_Animator.SetInteger(synchronizedParameter.Name, (int)m_StreamQueue.ReceiveNext());
					break;
				case ParameterType.Trigger:
					m_Animator.SetBool(synchronizedParameter.Name, (bool)m_StreamQueue.ReceiveNext());
					break;
				}
			}
		}
	}

	private void SerializeDataDiscretly(PhotonStream stream)
	{
		for (int i = 0; i < m_SynchronizeLayers.Count; i++)
		{
			if (m_SynchronizeLayers[i].SynchronizeType == SynchronizeType.Discrete)
			{
				stream.SendNext(m_Animator.GetLayerWeight(m_SynchronizeLayers[i].LayerIndex));
			}
		}
		for (int j = 0; j < m_SynchronizeParameters.Count; j++)
		{
			SynchronizedParameter synchronizedParameter = m_SynchronizeParameters[j];
			if (synchronizedParameter.SynchronizeType == SynchronizeType.Discrete)
			{
				switch (synchronizedParameter.Type)
				{
				case ParameterType.Bool:
					stream.SendNext(m_Animator.GetBool(synchronizedParameter.Name));
					break;
				case ParameterType.Float:
					stream.SendNext(m_Animator.GetFloat(synchronizedParameter.Name));
					break;
				case ParameterType.Int:
					stream.SendNext(m_Animator.GetInteger(synchronizedParameter.Name));
					break;
				case ParameterType.Trigger:
					stream.SendNext(m_raisedDiscreteTriggersCache.Contains(synchronizedParameter.Name));
					break;
				}
			}
		}
		m_raisedDiscreteTriggersCache.Clear();
	}

	private void DeserializeDataDiscretly(PhotonStream stream)
	{
		for (int i = 0; i < m_SynchronizeLayers.Count; i++)
		{
			if (m_SynchronizeLayers[i].SynchronizeType == SynchronizeType.Discrete)
			{
				m_Animator.SetLayerWeight(m_SynchronizeLayers[i].LayerIndex, (float)stream.ReceiveNext());
			}
		}
		for (int j = 0; j < m_SynchronizeParameters.Count; j++)
		{
			SynchronizedParameter synchronizedParameter = m_SynchronizeParameters[j];
			if (synchronizedParameter.SynchronizeType != SynchronizeType.Discrete)
			{
				continue;
			}
			switch (synchronizedParameter.Type)
			{
			case ParameterType.Bool:
				if (!(stream.PeekNext() is bool))
				{
					return;
				}
				m_Animator.SetBool(synchronizedParameter.Name, (bool)stream.ReceiveNext());
				break;
			case ParameterType.Float:
				if (!(stream.PeekNext() is float))
				{
					return;
				}
				m_Animator.SetFloat(synchronizedParameter.Name, (float)stream.ReceiveNext());
				break;
			case ParameterType.Int:
				if (!(stream.PeekNext() is int))
				{
					return;
				}
				m_Animator.SetInteger(synchronizedParameter.Name, (int)stream.ReceiveNext());
				break;
			case ParameterType.Trigger:
				if (!(stream.PeekNext() is bool))
				{
					return;
				}
				if ((bool)stream.ReceiveNext())
				{
					m_Animator.SetTrigger(synchronizedParameter.Name);
				}
				break;
			}
		}
	}

	private void SerializeSynchronizationTypeState(PhotonStream stream)
	{
		byte[] array = new byte[m_SynchronizeLayers.Count + m_SynchronizeParameters.Count];
		for (int i = 0; i < m_SynchronizeLayers.Count; i++)
		{
			array[i] = (byte)m_SynchronizeLayers[i].SynchronizeType;
		}
		for (int j = 0; j < m_SynchronizeParameters.Count; j++)
		{
			array[m_SynchronizeLayers.Count + j] = (byte)m_SynchronizeParameters[j].SynchronizeType;
		}
		stream.SendNext(array);
	}

	private void DeserializeSynchronizationTypeState(PhotonStream stream)
	{
		byte[] array = (byte[])stream.ReceiveNext();
		for (int i = 0; i < m_SynchronizeLayers.Count; i++)
		{
			m_SynchronizeLayers[i].SynchronizeType = (SynchronizeType)array[i];
		}
		for (int j = 0; j < m_SynchronizeParameters.Count; j++)
		{
			m_SynchronizeParameters[j].SynchronizeType = (SynchronizeType)array[m_SynchronizeLayers.Count + j];
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (m_Animator == null)
		{
			return;
		}
		if (stream.isWriting)
		{
			if (m_WasSynchronizeTypeChanged)
			{
				m_StreamQueue.Reset();
				SerializeSynchronizationTypeState(stream);
				m_WasSynchronizeTypeChanged = false;
			}
			m_StreamQueue.Serialize(stream);
			SerializeDataDiscretly(stream);
		}
		else
		{
			if (stream.PeekNext() is byte[])
			{
				DeserializeSynchronizationTypeState(stream);
			}
			m_StreamQueue.Deserialize(stream);
			DeserializeDataDiscretly(stream);
		}
	}
}
