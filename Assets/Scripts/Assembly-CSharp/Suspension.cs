using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DriveForce))]
[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Suspension/Suspension", 0)]
public class Suspension : MonoBehaviour
{
	[NonSerialized]
	public Transform tr;

	private Rigidbody rb;

	private VehicleParent vp;

	[NonSerialized]
	public bool flippedSide;

	[NonSerialized]
	public float flippedSideFactor;

	[NonSerialized]
	public Quaternion initialRotation;

	public Wheel wheel;

	private CapsuleCollider compressCol;

	[Tooltip("Generate a capsule collider for hard compressions")]
	public bool generateHardCollider = true;

	[Tooltip("Multiplier for the radius of the hard collider")]
	public float hardColliderRadiusFactor = 1f;

	private float hardColliderRadiusFactorPrev;

	private float setHardColliderRadiusFactor;

	private Transform compressTr;

	[Header("Brakes and Steering")]
	public float brakeForce;

	public float ebrakeForce;

	[Range(-180f, 180f)]
	public float steerRangeMin;

	[Range(-180f, 180f)]
	public float steerRangeMax;

	[Tooltip("How much the wheel is steered")]
	public float steerFactor = 1f;

	[Range(-1f, 1f)]
	public float steerAngle;

	[NonSerialized]
	public float steerDegrees;

	[Tooltip("Effect of Ackermann steering geometry")]
	public float ackermannFactor;

	[Tooltip("The camber of the wheel as it travels, x-axis = compression, y-axis = angle")]
	public AnimationCurve camberCurve = AnimationCurve.Linear(0f, -10f, 1f, 10f);

	[NonSerialized]
	public float camberAngle;

	[Tooltip("Adjust the camber as if it was connected to a solid axle, opposite wheel must be set")]
	public bool solidAxleCamber;

	public Suspension oppositeWheel;

	[Tooltip("Angle at which the suspension points out to the side")]
	[Range(-89.999f, 89.999f)]
	public float sideAngle;

	[Range(-89.999f, 89.999f)]
	public float casterAngle;

	[Range(-89.999f, 89.999f)]
	public float toeAngle;

	[Tooltip("Wheel offset from its pivot point")]
	public float pivotOffset;

	[NonSerialized]
	public List<SuspensionPart> movingParts = new List<SuspensionPart>();

	[Header("Spring")]
	public float suspensionDistance;

	[NonSerialized]
	public float compression;

	[Tooltip("Should be left at 1 unless testing suspension travel")]
	[Range(0f, 1f)]
	public float targetCompression;

	[NonSerialized]
	public float penetration;

	public float springForce;

	[Tooltip("Force of the curve depending on it's compression, x-axis = compression, y-axis = force")]
	public AnimationCurve springForceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Tooltip("Exponent for spring force based on compression")]
	public float springExponent = 1f;

	public float springDampening;

	[Tooltip("How quickly the suspension extends if it's not grounded")]
	public float extendSpeed = 20f;

	[Tooltip("Apply forces to prevent the wheel from intersecting with the ground, not necessary if generating a hard collider")]
	public bool applyHardContactForce = true;

	public float hardContactForce = 50f;

	public float hardContactSensitivity = 2f;

	[Tooltip("Apply suspension forces at ground point")]
	public bool applyForceAtGroundContact = true;

	[Tooltip("Apply suspension forces along local up direction instead of ground normal")]
	public bool leaningForce;

	[NonSerialized]
	public Vector3 maxCompressPoint;

	[NonSerialized]
	public Vector3 springDirection;

	[NonSerialized]
	public Vector3 upDir;

	[NonSerialized]
	public Vector3 forwardDir;

	[NonSerialized]
	public DriveForce targetDrive;

	[NonSerialized]
	public SuspensionPropertyToggle properties;

	[NonSerialized]
	public bool steerEnabled = true;

	[NonSerialized]
	public bool steerInverted;

	[NonSerialized]
	public bool driveEnabled = true;

	[NonSerialized]
	public bool driveInverted;

	[NonSerialized]
	public bool ebrakeEnabled = true;

	[NonSerialized]
	public bool skidSteerBrake;

	[Header("Damage")]
	[Tooltip("Point around which the suspension pivots when damaged")]
	public Vector3 damagePivot;

	[Tooltip("Compression amount to remain at when wheel is detached")]
	[Range(0f, 1f)]
	public float detachedCompression = 0.5f;

	public float jamForce = float.PositiveInfinity;

	[NonSerialized]
	public bool jammed;

	private void Start()
	{
		tr = base.transform;
		rb = (Rigidbody)F.GetTopmostParentComponent<Rigidbody>(tr);
		vp = (VehicleParent)F.GetTopmostParentComponent<VehicleParent>(tr);
		targetDrive = GetComponent<DriveForce>();
		flippedSide = Vector3.Dot(tr.forward, vp.transform.right) < 0f;
		flippedSideFactor = ((!flippedSide) ? 1 : (-1));
		initialRotation = tr.localRotation;
		if (Application.isPlaying)
		{
			GetCamber();
			if (generateHardCollider)
			{
				GameObject gameObject = new GameObject("Compress Collider");
				gameObject.layer = GlobalControl.ignoreWheelCastLayer;
				compressTr = gameObject.transform;
				compressTr.parent = tr;
				compressTr.localPosition = Vector3.zero;
				compressTr.localEulerAngles = new Vector3(camberAngle, 0f, (0f - casterAngle) * flippedSideFactor);
				compressCol = gameObject.AddComponent<CapsuleCollider>();
				compressCol.direction = 1;
				setHardColliderRadiusFactor = hardColliderRadiusFactor;
				hardColliderRadiusFactorPrev = setHardColliderRadiusFactor;
				compressCol.radius = wheel.rimWidth * hardColliderRadiusFactor;
				compressCol.height = ((!wheel.popped) ? Mathf.Lerp(wheel.rimRadius, wheel.tireRadius, wheel.tirePressure) : wheel.rimRadius) * 2f;
				compressCol.material = GlobalControl.frictionlessMatStatic;
			}
			steerRangeMax = Mathf.Max(steerRangeMin, steerRangeMax);
			properties = GetComponent<SuspensionPropertyToggle>();
			if ((bool)properties)
			{
				UpdateProperties();
			}
		}
	}

	private void FixedUpdate()
	{
		upDir = tr.up;
		forwardDir = tr.forward;
		targetCompression = 1f;
		GetCamber();
		GetSpringVectors();
		if (wheel.connected)
		{
			compression = Mathf.Min(targetCompression, (!(suspensionDistance > 0f)) ? 0f : Mathf.Clamp01(wheel.contactPoint.distance / suspensionDistance));
			penetration = Mathf.Min(0f, wheel.contactPoint.distance);
		}
		else
		{
			compression = detachedCompression;
			penetration = 0f;
		}
		if (targetCompression > 0f)
		{
			ApplySuspensionForce();
		}
		if (generateHardCollider)
		{
			setHardColliderRadiusFactor = hardColliderRadiusFactor;
			if (hardColliderRadiusFactorPrev != setHardColliderRadiusFactor || wheel.updatedSize || wheel.updatedPopped)
			{
				if (wheel.rimWidth > wheel.actualRadius)
				{
					compressCol.direction = 2;
					compressCol.radius = wheel.actualRadius * hardColliderRadiusFactor;
					compressCol.height = wheel.rimWidth * 2f;
				}
				else
				{
					compressCol.direction = 1;
					compressCol.radius = wheel.rimWidth * hardColliderRadiusFactor;
					compressCol.height = wheel.actualRadius * 2f;
				}
			}
			hardColliderRadiusFactorPrev = setHardColliderRadiusFactor;
		}
		if (wheel.connected)
		{
			if ((bool)wheel.targetDrive)
			{
				targetDrive.active = driveEnabled;
				targetDrive.feedbackRPM = wheel.targetDrive.feedbackRPM;
				wheel.targetDrive.SetDrive(targetDrive);
			}
		}
		else
		{
			targetDrive.feedbackRPM = targetDrive.rpm;
		}
	}

	private void Update()
	{
		GetCamber();
		if (!Application.isPlaying)
		{
			GetSpringVectors();
		}
		steerDegrees = Mathf.Lerp(steerRangeMin, steerRangeMax, (steerAngle + 1f) * 0.5f);
	}

	private void ApplySuspensionForce()
	{
		if (!wheel.grounded || !wheel.connected)
		{
			return;
		}
		float z = vp.norm.InverseTransformDirection(rb.GetPointVelocity(tr.position)).z;
		if (suspensionDistance > 0f && targetCompression > 0f)
		{
			Vector3 vector = ((!leaningForce) ? vp.norm.forward : Vector3.Lerp(upDir, vp.norm.forward, Mathf.Abs(Mathf.Pow(Vector3.Dot(vp.norm.forward, vp.upDir), 5f)))) * springForce * (Mathf.Pow(springForceCurve.Evaluate(1f - compression), Mathf.Max(1f, springExponent)) - (1f - targetCompression) - springDampening * Mathf.Clamp(z, -1f, 1f));
			rb.AddForceAtPosition(vector, (!applyForceAtGroundContact) ? wheel.tr.position : wheel.contactPoint.point, ForceMode.Acceleration);
			if ((bool)wheel.contactPoint.col.attachedRigidbody)
			{
				wheel.contactPoint.col.attachedRigidbody.AddForceAtPosition(-vector, wheel.contactPoint.point, ForceMode.Acceleration);
			}
		}
		if (compression == 0f && !generateHardCollider && applyHardContactForce)
		{
			rb.AddForceAtPosition(-vp.norm.TransformDirection(0f, 0f, Mathf.Clamp(z, (0f - hardContactSensitivity) * TimeMaster.fixedTimeFactor, 0f) + penetration) * hardContactForce * Mathf.Clamp01(TimeMaster.fixedTimeFactor), (!applyForceAtGroundContact) ? wheel.tr.position : wheel.contactPoint.point, ForceMode.Acceleration);
		}
	}

	private void GetSpringVectors()
	{
		if (!Application.isPlaying)
		{
			tr = base.transform;
			flippedSide = Vector3.Dot(tr.forward, vp.transform.right) < 0f;
			flippedSideFactor = ((!flippedSide) ? 1 : (-1));
		}
		maxCompressPoint = tr.position;
		float num = (0f - Mathf.Sin(casterAngle * ((float)Math.PI / 180f))) * flippedSideFactor;
		float num2 = 0f - Mathf.Sin(sideAngle * ((float)Math.PI / 180f));
		springDirection = tr.TransformDirection(num, Mathf.Max(Mathf.Abs(num), Mathf.Abs(num2)) - 1f, num2).normalized;
	}

	private void GetCamber()
	{
		if (solidAxleCamber && (bool)oppositeWheel && wheel.connected)
		{
			if ((bool)oppositeWheel.wheel.rim && (bool)wheel.rim)
			{
				Vector3 vector = tr.InverseTransformDirection((oppositeWheel.wheel.rim.position - wheel.rim.position).normalized);
				camberAngle = Mathf.Atan2(vector.z, vector.y) * 57.29578f + 90f;
			}
		}
		else
		{
			camberAngle = camberCurve.Evaluate((!Application.isPlaying || !wheel.connected) ? targetCompression : wheel.travelDist);
		}
	}

	public void UpdateProperties()
	{
		if (!properties)
		{
			return;
		}
		SuspensionToggledProperty[] array = properties.properties;
		foreach (SuspensionToggledProperty suspensionToggledProperty in array)
		{
			switch ((int)suspensionToggledProperty.property)
			{
			case 0:
				steerEnabled = suspensionToggledProperty.toggled;
				break;
			case 1:
				steerInverted = suspensionToggledProperty.toggled;
				break;
			case 2:
				driveEnabled = suspensionToggledProperty.toggled;
				break;
			case 3:
				driveInverted = suspensionToggledProperty.toggled;
				break;
			case 4:
				ebrakeEnabled = suspensionToggledProperty.toggled;
				break;
			case 5:
				skidSteerBrake = suspensionToggledProperty.toggled;
				break;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!tr)
		{
			tr = base.transform;
		}
		if ((bool)wheel && (bool)wheel.rim)
		{
			Vector3 position = wheel.rim.position;
			float num = 0f - Mathf.Sin(camberAngle * ((float)Math.PI / 180f));
			float num2 = Mathf.Sin(Mathf.Lerp(steerRangeMin, steerRangeMax, (steerAngle + 1f) * 0.5f) * ((float)Math.PI / 180f));
			float num3 = Mathf.Sin(steerRangeMin * ((float)Math.PI / 180f));
			float num4 = Mathf.Sin(steerRangeMax * ((float)Math.PI / 180f));
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(position, 0.05f);
			Gizmos.DrawLine(position, position + tr.TransformDirection(num3, num * (1f - Mathf.Abs(num3)), Mathf.Cos(steerRangeMin * ((float)Math.PI / 180f)) * (1f - Mathf.Abs(num))).normalized);
			Gizmos.DrawLine(position, position + tr.TransformDirection(num4, num * (1f - Mathf.Abs(num4)), Mathf.Cos(steerRangeMax * ((float)Math.PI / 180f)) * (1f - Mathf.Abs(num))).normalized);
			Gizmos.DrawLine(position + tr.TransformDirection(num3, num * (1f - Mathf.Abs(num3)), Mathf.Cos(steerRangeMin * ((float)Math.PI / 180f)) * (1f - Mathf.Abs(num))).normalized * 0.9f, position + tr.TransformDirection(num4, num * (1f - Mathf.Abs(num4)), Mathf.Cos(steerRangeMax * ((float)Math.PI / 180f)) * (1f - Mathf.Abs(num))).normalized * 0.9f);
			Gizmos.DrawLine(position, position + tr.TransformDirection(num2, num * (1f - Mathf.Abs(num2)), Mathf.Cos(steerRangeMin * ((float)Math.PI / 180f)) * (1f - Mathf.Abs(num))).normalized);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(tr.TransformPoint(damagePivot), 0.05f);
	}
}
