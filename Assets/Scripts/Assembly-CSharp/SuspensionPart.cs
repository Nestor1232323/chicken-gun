using System;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Suspension/Suspension Part", 1)]
public class SuspensionPart : MonoBehaviour
{
	private Transform tr;

	private Wheel wheel;

	public Suspension suspension;

	public bool isHub;

	[Header("Connections")]
	[Tooltip("Object to point at")]
	public Transform connectObj;

	[Tooltip("Local space point to point at in connectObj")]
	public Vector3 connectPoint;

	[NonSerialized]
	public Vector3 initialConnectPoint;

	private Vector3 localConnectPoint;

	[Tooltip("Rotate to point at target?")]
	public bool rotate = true;

	[Tooltip("Scale along local z-axis to reach target?")]
	public bool stretch;

	private float initialDist;

	private Vector3 initialScale;

	[Header("Solid Axle")]
	public bool solidAxle;

	public bool invertRotation;

	[Tooltip("Does this part connect to a solid axle?")]
	public bool solidAxleConnector;

	public Wheel wheel1;

	public Wheel wheel2;

	private Vector3 wheelConnect1;

	private Vector3 wheelConnect2;

	private Vector3 parentUpDir;

	private void Start()
	{
		tr = base.transform;
		initialConnectPoint = connectPoint;
		if ((bool)suspension)
		{
			suspension.movingParts.Add(this);
			if ((bool)suspension.wheel)
			{
				wheel = suspension.wheel;
			}
		}
		if ((bool)connectObj && !isHub && Application.isPlaying)
		{
			initialDist = Mathf.Max(Vector3.Distance(tr.position, connectObj.TransformPoint(connectPoint)), 0.01f);
			initialScale = tr.localScale;
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			tr = base.transform;
			if ((bool)suspension && (bool)suspension.wheel)
			{
				wheel = suspension.wheel;
			}
		}
		if (!tr)
		{
			return;
		}
		if (!solidAxle && (((bool)suspension && !solidAxleConnector) || solidAxleConnector))
		{
			if (isHub && (bool)wheel && !solidAxleConnector)
			{
				if ((bool)wheel.rim)
				{
					tr.position = wheel.rim.position;
					tr.rotation = Quaternion.LookRotation(wheel.rim.forward, suspension.upDir);
					tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, tr.localEulerAngles.y, (0f - suspension.casterAngle) * suspension.flippedSideFactor);
				}
			}
			else
			{
				if (isHub || !connectObj)
				{
					return;
				}
				localConnectPoint = connectObj.TransformPoint(connectPoint);
				if (rotate)
				{
					tr.rotation = Quaternion.LookRotation((localConnectPoint - tr.position).normalized, (!solidAxleConnector) ? suspension.upDir : tr.parent.forward);
					if (!solidAxleConnector)
					{
						tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, tr.localEulerAngles.y, (0f - suspension.casterAngle) * suspension.flippedSideFactor);
					}
				}
				if (stretch && Application.isPlaying)
				{
					tr.localScale = new Vector3(tr.localScale.x, tr.localScale.y, initialScale.z * (Vector3.Distance(tr.position, localConnectPoint) / initialDist));
				}
			}
		}
		else if (solidAxle && (bool)wheel1 && (bool)wheel2 && (bool)wheel1.rim && (bool)wheel2.rim && (bool)wheel1.suspensionParent && (bool)wheel2.suspensionParent)
		{
			parentUpDir = tr.parent.up;
			wheelConnect1 = wheel1.rim.TransformPoint(0f, 0f, 0f - wheel1.suspensionParent.pivotOffset);
			wheelConnect2 = wheel2.rim.TransformPoint(0f, 0f, 0f - wheel2.suspensionParent.pivotOffset);
			tr.rotation = Quaternion.LookRotation(((wheelConnect1 + wheelConnect2) * 0.5f - tr.position).normalized, parentUpDir);
			tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, tr.localEulerAngles.y, Vector3.Angle((wheelConnect1 - wheelConnect2).normalized, tr.parent.right) * Mathf.Sign(Vector3.Dot((wheelConnect1 - wheelConnect2).normalized, parentUpDir)) * Mathf.Sign(tr.localPosition.z) * (float)((!invertRotation) ? 1 : (-1)));
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!tr)
		{
			tr = base.transform;
		}
		Gizmos.color = Color.green;
		if (!isHub && (bool)connectObj && !solidAxle)
		{
			localConnectPoint = connectObj.TransformPoint(connectPoint);
			Gizmos.DrawLine(tr.position, localConnectPoint);
			Gizmos.DrawWireSphere(localConnectPoint, 0.01f);
		}
		else if (solidAxle && (bool)wheel1 && (bool)wheel2 && (bool)wheel1.rim && (bool)wheel2.rim && (bool)wheel1.suspensionParent && (bool)wheel2.suspensionParent)
		{
			wheelConnect1 = wheel1.rim.TransformPoint(0f, 0f, 0f - wheel1.suspensionParent.pivotOffset);
			wheelConnect2 = wheel2.rim.TransformPoint(0f, 0f, 0f - wheel2.suspensionParent.pivotOffset);
			Gizmos.DrawLine(wheelConnect1, wheelConnect2);
			Gizmos.DrawWireSphere(wheelConnect1, 0.01f);
			Gizmos.DrawWireSphere(wheelConnect2, 0.01f);
		}
	}
}
