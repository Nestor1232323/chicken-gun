using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[AddComponentMenu("Photon Networking/Photon Transform View")]
public class PhotonTransformView : MonoBehaviour, IPunObservable
{
	[SerializeField]
	public PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

	[SerializeField]
	public PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

	[SerializeField]
	public PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

	private PhotonTransformViewPositionControl m_PositionControl;

	private PhotonTransformViewRotationControl m_RotationControl;

	private PhotonTransformViewScaleControl m_ScaleControl;

	private PhotonView m_PhotonView;

	private bool m_ReceivedNetworkUpdate;

	private bool m_firstTake;

	private void Awake()
	{
		m_PhotonView = GetComponent<PhotonView>();
		m_PositionControl = new PhotonTransformViewPositionControl(m_PositionModel);
		m_RotationControl = new PhotonTransformViewRotationControl(m_RotationModel);
		m_ScaleControl = new PhotonTransformViewScaleControl(m_ScaleModel);
	}

	private void OnEnable()
	{
		m_firstTake = true;
	}

	private void Update()
	{
		if (!(m_PhotonView == null) && !m_PhotonView.isMine && PhotonNetwork.connected)
		{
			UpdatePosition();
			UpdateRotation();
			UpdateScale();
		}
	}

	private void UpdatePosition()
	{
		if (m_PositionModel.SynchronizeEnabled && m_ReceivedNetworkUpdate)
		{
			base.transform.localPosition = m_PositionControl.UpdatePosition(base.transform.localPosition);
		}
	}

	private void UpdateRotation()
	{
		if (m_RotationModel.SynchronizeEnabled && m_ReceivedNetworkUpdate)
		{
			base.transform.localRotation = m_RotationControl.GetRotation(base.transform.localRotation);
		}
	}

	private void UpdateScale()
	{
		if (m_ScaleModel.SynchronizeEnabled && m_ReceivedNetworkUpdate)
		{
			base.transform.localScale = m_ScaleControl.GetScale(base.transform.localScale);
		}
	}

	public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
	{
		m_PositionControl.SetSynchronizedValues(speed, turnSpeed);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		m_PositionControl.OnPhotonSerializeView(base.transform.localPosition, stream, info);
		m_RotationControl.OnPhotonSerializeView(base.transform.localRotation, stream, info);
		m_ScaleControl.OnPhotonSerializeView(base.transform.localScale, stream, info);
		if (!m_PhotonView.isMine && m_PositionModel.DrawErrorGizmo)
		{
			DoDrawEstimatedPositionError();
		}
		if (!stream.isReading)
		{
			return;
		}
		m_ReceivedNetworkUpdate = true;
		if (m_firstTake)
		{
			m_firstTake = false;
			if (m_PositionModel.SynchronizeEnabled)
			{
				base.transform.localPosition = m_PositionControl.GetNetworkPosition();
			}
			if (m_RotationModel.SynchronizeEnabled)
			{
				base.transform.localRotation = m_RotationControl.GetNetworkRotation();
			}
			if (m_ScaleModel.SynchronizeEnabled)
			{
				base.transform.localScale = m_ScaleControl.GetNetworkScale();
			}
		}
	}

	private void DoDrawEstimatedPositionError()
	{
		Vector3 vector = m_PositionControl.GetNetworkPosition();
		if (base.transform.parent != null)
		{
			vector = base.transform.parent.position + vector;
		}
		Debug.DrawLine(vector, base.transform.position, Color.red, 2f);
		Debug.DrawLine(base.transform.position, base.transform.position + Vector3.up, Color.green, 2f);
		Debug.DrawLine(vector, vector + Vector3.up, Color.red, 2f);
	}
}
