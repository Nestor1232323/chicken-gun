using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Hover/Hover Wheel", 1)]
public class HoverWheel : MonoBehaviour
{
	private Transform tr;

	private Rigidbody rb;

	private VehicleParent vp;

	[NonSerialized]
	public HoverContact contactPoint = new HoverContact();

	[NonSerialized]
	public bool getContact = true;

	[NonSerialized]
	public bool grounded;

	public float hoverDistance;

	[Tooltip("If the distance to the ground is less than this, extra hovering force will be applied based on the buffer float force")]
	public float bufferDistance;

	private Vector3 upDir;

	[NonSerialized]
	public bool doFloat;

	public float floatForce = 1f;

	public float bufferFloatForce = 2f;

	[Tooltip("Strength of the suspension depending on how compressed it is, x-axis = compression, y-axis = force")]
	public AnimationCurve floatForceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float floatExponent = 1f;

	public float floatDampening;

	private float compression;

	[NonSerialized]
	public float targetSpeed;

	[NonSerialized]
	public float targetForce;

	private float flippedSideFactor;

	public float brakeForce = 1f;

	public float ebrakeForce = 2f;

	[NonSerialized]
	public float steerRate;

	[Tooltip("How much the wheel steers")]
	public float steerFactor;

	public float sideFriction;

	[Header("Visual Wheel")]
	public Transform visualWheel;

	public float visualTiltRate = 10f;

	public float visualTiltAmount = 0.5f;

	private GameObject detachedWheel;

	private MeshCollider detachedCol;

	private Rigidbody detachedBody;

	private MeshFilter detachFilter;

	[Header("Damage")]
	public float detachForce = float.PositiveInfinity;

	public float mass = 0.05f;

	[NonSerialized]
	public bool connected = true;

	[NonSerialized]
	public bool canDetach;

	public Mesh wheelMeshLoose;

	public PhysicMaterial detachedWheelMaterial;

	private void Start()
	{
		tr = base.transform;
		rb = (Rigidbody)F.GetTopmostParentComponent<Rigidbody>(tr);
		vp = (VehicleParent)F.GetTopmostParentComponent<VehicleParent>(tr);
		flippedSideFactor = ((Vector3.Dot(tr.forward, vp.transform.right) < 0f) ? 1 : (-1));
		canDetach = detachForce < float.PositiveInfinity && Application.isPlaying;
		bufferDistance = Mathf.Min(hoverDistance, bufferDistance);
		if (canDetach)
		{
			detachedWheel = new GameObject(vp.transform.name + "'s Detached Wheel");
			detachedWheel.layer = LayerMask.NameToLayer("Detachable Part");
			detachFilter = detachedWheel.AddComponent<MeshFilter>();
			detachFilter.sharedMesh = visualWheel.GetComponent<MeshFilter>().sharedMesh;
			detachedWheel.AddComponent<MeshRenderer>().sharedMaterial = visualWheel.GetComponent<MeshRenderer>().sharedMaterial;
			detachedCol = detachedWheel.AddComponent<MeshCollider>();
			detachedCol.convex = true;
			detachedBody = detachedWheel.AddComponent<Rigidbody>();
			detachedBody.mass = mass;
			detachedWheel.SetActive(value: false);
		}
	}

	private void Update()
	{
		if ((bool)visualWheel && connected)
		{
			TiltWheel();
		}
	}

	private void FixedUpdate()
	{
		upDir = tr.up;
		if (getContact)
		{
			GetWheelContact();
		}
		else if (grounded)
		{
			contactPoint.point += rb.GetPointVelocity(tr.position) * Time.fixedDeltaTime;
		}
		compression = Mathf.Clamp01(contactPoint.distance / hoverDistance);
		if (grounded && doFloat && connected)
		{
			ApplyFloat();
			ApplyFloatDrive();
		}
	}

	private void GetWheelContact()
	{
		RaycastHit raycastHit = default(RaycastHit);
		Vector3 pointVelocity = rb.GetPointVelocity(tr.position);
		RaycastHit[] array = Physics.RaycastAll(tr.position, -upDir, hoverDistance, GlobalControl.wheelCastMaskStatic);
		bool flag = false;
		float num = float.PositiveInfinity;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit2 = array2[i];
			if (!raycastHit2.transform.IsChildOf(vp.tr) && raycastHit2.distance < num)
			{
				raycastHit = raycastHit2;
				num = raycastHit2.distance;
				flag = true;
			}
		}
		if (flag)
		{
			if (!raycastHit.collider.transform.IsChildOf(vp.tr))
			{
				grounded = true;
				contactPoint.distance = raycastHit.distance;
				contactPoint.point = raycastHit.point + pointVelocity * Time.fixedDeltaTime;
				contactPoint.grounded = true;
				contactPoint.normal = raycastHit.normal;
				contactPoint.relativeVelocity = tr.InverseTransformDirection(pointVelocity);
				contactPoint.col = raycastHit.collider;
			}
		}
		else
		{
			grounded = false;
			contactPoint.distance = hoverDistance;
			contactPoint.point = Vector3.zero;
			contactPoint.grounded = false;
			contactPoint.normal = upDir;
			contactPoint.relativeVelocity = Vector3.zero;
			contactPoint.col = null;
		}
	}

	private void ApplyFloat()
	{
		if (grounded)
		{
			float z = vp.norm.InverseTransformDirection(rb.GetPointVelocity(tr.position)).z;
			rb.AddForceAtPosition(upDir * floatForce * (Mathf.Pow(floatForceCurve.Evaluate(1f - compression), Mathf.Max(1f, floatExponent)) - floatDampening * Mathf.Clamp(z, -1f, 1f)), tr.position, ForceMode.Acceleration);
			if (contactPoint.distance < bufferDistance)
			{
				rb.AddForceAtPosition(-upDir * bufferFloatForce * floatForceCurve.Evaluate(contactPoint.distance / bufferDistance) * Mathf.Clamp(z, -1f, 0f), tr.position, ForceMode.Acceleration);
			}
		}
	}

	private void ApplyFloatDrive()
	{
		float num = ((!(vp.localVelocity.z > 0f)) ? Mathf.Clamp01(vp.accelInput) : vp.brakeInput) * brakeForce + vp.ebrakeInput * ebrakeForce;
		rb.AddForceAtPosition(tr.TransformDirection((Mathf.Clamp(targetSpeed, -1f, 1f) * targetForce - num * Mathf.Max(5f, Mathf.Abs(contactPoint.relativeVelocity.x)) * Mathf.Sign(contactPoint.relativeVelocity.x) * flippedSideFactor) * flippedSideFactor, 0f, (0f - steerRate) * steerFactor * flippedSideFactor - contactPoint.relativeVelocity.z * sideFriction) * (1f - compression), tr.position, ForceMode.Acceleration);
	}

	private void TiltWheel()
	{
		float num = Mathf.Clamp((0f - steerRate) * steerFactor * flippedSideFactor - Mathf.Clamp(contactPoint.relativeVelocity.z * 0.1f, -1f, 1f) * sideFriction, -1f, 1f);
		float num2 = ((!(vp.localVelocity.z > 0f)) ? Mathf.Clamp01(vp.accelInput) : vp.brakeInput) * brakeForce + vp.ebrakeInput * ebrakeForce;
		float num3 = Mathf.Clamp((Mathf.Clamp(targetSpeed, -1f, 1f) * targetForce - num2 * Mathf.Clamp(contactPoint.relativeVelocity.x * 0.1f, -1f, 1f) * flippedSideFactor) * flippedSideFactor, -1f, 1f);
		Transform transform = visualWheel;
		Quaternion localRotation = visualWheel.localRotation;
		Vector3 vector = new Vector3((0f - num3) * visualTiltAmount, -1f + Mathf.Abs(F.MaxAbs(num, num3)) * visualTiltAmount, (0f - num) * visualTiltAmount);
		transform.localRotation = Quaternion.Lerp(localRotation, Quaternion.LookRotation(vector.normalized, Vector3.forward), visualTiltRate * Time.deltaTime);
	}

	public void Detach()
	{
		if (connected && canDetach)
		{
			connected = false;
			detachedWheel.SetActive(value: true);
			detachedWheel.transform.position = visualWheel.position;
			detachedWheel.transform.rotation = visualWheel.rotation;
			detachedCol.sharedMaterial = detachedWheelMaterial;
			detachedCol.sharedMesh = ((!wheelMeshLoose) ? detachFilter.sharedMesh : wheelMeshLoose);
			rb.mass -= mass;
			detachedBody.velocity = rb.GetPointVelocity(visualWheel.position);
			detachedBody.angularVelocity = rb.angularVelocity;
			visualWheel.gameObject.SetActive(value: false);
		}
	}

	public void Reattach()
	{
		if (!connected)
		{
			connected = true;
			detachedWheel.SetActive(value: false);
			rb.mass += mass;
			visualWheel.gameObject.SetActive(value: true);
		}
	}

	private void OnDrawGizmosSelected()
	{
		tr = base.transform;
		Gizmos.color = Color.white;
		Gizmos.DrawRay(tr.position, -tr.up * hoverDistance);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(tr.position, -tr.up * bufferDistance);
	}

	private void OnDestroy()
	{
		if ((bool)detachedWheel)
		{
			UnityEngine.Object.Destroy(detachedWheel);
		}
	}
}
