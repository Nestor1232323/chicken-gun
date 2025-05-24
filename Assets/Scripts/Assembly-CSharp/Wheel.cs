using System;
using UnityEngine;

[RequireComponent(typeof(DriveForce))]
[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Drivetrain/Wheel", 1)]
public class Wheel : MonoBehaviour
{
	public enum SlipDependenceMode
	{
		dependent = 0,
		forward = 1,
		sideways = 2,
		independent = 3
	}

	[NonSerialized]
	public Transform tr;

	private Rigidbody rb;

	[NonSerialized]
	public VehicleParent vp;

	[NonSerialized]
	public Suspension suspensionParent;

	[NonSerialized]
	public Transform rim;

	private Transform tire;

	private Vector3 localVel;

	[Tooltip("Generate a sphere collider to represent the wheel for side collisions")]
	public bool generateHardCollider = true;

	private SphereCollider sphereCol;

	private Transform sphereColTr;

	[Header("Rotation")]
	[Tooltip("Bias for feedback RPM lerp between target RPM and raw RPM")]
	[Range(0f, 1f)]
	public float feedbackRpmBias;

	[Tooltip("Curve for setting final RPM of wheel based on driving torque/brake force, x-axis = torque/brake force, y-axis = lerp between raw RPM and target RPM")]
	public AnimationCurve rpmBiasCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Tooltip("As the RPM of the wheel approaches this value, the RPM bias curve is interpolated with the default linear curve")]
	public float rpmBiasCurveLimit = float.PositiveInfinity;

	[Range(0f, 10f)]
	public float axleFriction;

	[Header("Friction")]
	[Range(0f, 1f)]
	public float frictionSmoothness = 0.5f;

	public float forwardFriction = 1f;

	public float sidewaysFriction = 1f;

	public float forwardRimFriction = 0.5f;

	public float sidewaysRimFriction = 0.5f;

	public float forwardCurveStretch = 1f;

	public float sidewaysCurveStretch = 1f;

	private Vector3 frictionForce = Vector3.zero;

	[Tooltip("X-axis = slip, y-axis = friction")]
	public AnimationCurve forwardFrictionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Tooltip("X-axis = slip, y-axis = friction")]
	public AnimationCurve sidewaysFrictionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[NonSerialized]
	public float forwardSlip;

	[NonSerialized]
	public float sidewaysSlip;

	public SlipDependenceMode slipDependence = SlipDependenceMode.sideways;

	[Range(0f, 2f)]
	public float forwardSlipDependence = 2f;

	[Range(0f, 2f)]
	public float sidewaysSlipDependence = 2f;

	[Tooltip("Adjusts how much friction the wheel has based on the normal of the ground surface. X-axis = normal dot product, y-axis = friction multiplier")]
	public AnimationCurve normalFrictionCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	[Header("Size")]
	public float tireRadius;

	public float rimRadius;

	public float tireWidth;

	public float rimWidth;

	[NonSerialized]
	public float setTireWidth;

	[NonSerialized]
	public float tireWidthPrev;

	[NonSerialized]
	public float setTireRadius;

	[NonSerialized]
	public float tireRadiusPrev;

	[NonSerialized]
	public float setRimWidth;

	[NonSerialized]
	public float rimWidthPrev;

	[NonSerialized]
	public float setRimRadius;

	[NonSerialized]
	public float rimRadiusPrev;

	[NonSerialized]
	public float actualRadius;

	[Header("Tire")]
	[Range(0f, 1f)]
	public float tirePressure = 1f;

	[NonSerialized]
	public float setTirePressure;

	[NonSerialized]
	public float tirePressurePrev;

	private float initialTirePressure;

	public bool popped;

	[NonSerialized]
	public bool setPopped;

	[NonSerialized]
	public bool poppedPrev;

	public bool canPop;

	[Tooltip("Requires deform shader")]
	public float deformAmount;

	private Material rimMat;

	private Material tireMat;

	private float airLeakTime = -1f;

	[Range(0f, 1f)]
	public float rimGlow;

	private float glowAmount;

	private Color glowColor;

	[NonSerialized]
	public bool updatedSize;

	[NonSerialized]
	public bool updatedPopped;

	private float currentRPM;

	[NonSerialized]
	public DriveForce targetDrive;

	[NonSerialized]
	public float rawRPM;

	[NonSerialized]
	public WheelContact contactPoint = new WheelContact();

	[NonSerialized]
	public bool getContact = true;

	[NonSerialized]
	public bool grounded;

	private float airTime;

	[NonSerialized]
	public float travelDist;

	private Vector3 upDir;

	private float circumference;

	[NonSerialized]
	public Vector3 contactVelocity;

	private float actualEbrake;

	private float actualTargetRPM;

	private float actualTorque;

	[NonSerialized]
	public Vector3 forceApplicationPoint;

	[Tooltip("Apply friction forces at ground point")]
	public bool applyForceAtGroundContact;

	[Header("Audio")]
	public AudioSource impactSnd;

	public AudioClip[] tireHitClips;

	public AudioClip rimHitClip;

	public AudioClip tireAirClip;

	public AudioClip tirePopClip;

	[Header("Damage")]
	public float detachForce = float.PositiveInfinity;

	[NonSerialized]
	public float damage;

	public float mass = 0.05f;

	[NonSerialized]
	public bool canDetach;

	[NonSerialized]
	public bool connected = true;

	public Mesh tireMeshLoose;

	public Mesh rimMeshLoose;

	private GameObject detachedWheel;

	private GameObject detachedTire;

	private MeshCollider detachedCol;

	private Rigidbody detachedBody;

	private MeshFilter detachFilter;

	private MeshFilter detachTireFilter;

	public PhysicMaterial detachedTireMaterial;

	public PhysicMaterial detachedRimMaterial;

	private void Start()
	{
		tr = base.transform;
		rb = (Rigidbody)F.GetTopmostParentComponent<Rigidbody>(tr);
		vp = (VehicleParent)F.GetTopmostParentComponent<VehicleParent>(tr);
		suspensionParent = tr.parent.GetComponent<Suspension>();
		travelDist = suspensionParent.targetCompression;
		canDetach = detachForce < float.PositiveInfinity && Application.isPlaying;
		initialTirePressure = tirePressure;
		if (tr.childCount > 0)
		{
			rim = tr.GetChild(0);
			if (rimGlow > 0f && Application.isPlaying)
			{
				rimMat = new Material(rim.GetComponent<MeshRenderer>().sharedMaterial);
				rimMat.EnableKeyword("_EMISSION");
				rim.GetComponent<MeshRenderer>().material = rimMat;
			}
			if (canDetach)
			{
				detachedWheel = new GameObject(vp.transform.name + "'s Detached Wheel");
				detachedWheel.layer = LayerMask.NameToLayer("Detachable Part");
				detachFilter = detachedWheel.AddComponent<MeshFilter>();
				detachFilter.sharedMesh = rim.GetComponent<MeshFilter>().sharedMesh;
				detachedWheel.AddComponent<MeshRenderer>().sharedMaterial = rim.GetComponent<MeshRenderer>().sharedMaterial;
				detachedCol = detachedWheel.AddComponent<MeshCollider>();
				detachedCol.convex = true;
				detachedBody = detachedWheel.AddComponent<Rigidbody>();
				detachedBody.mass = mass;
			}
			if (rim.childCount > 0)
			{
				tire = rim.GetChild(0);
				if (deformAmount > 0f && Application.isPlaying)
				{
					tireMat = new Material(tire.GetComponent<MeshRenderer>().sharedMaterial);
					tire.GetComponent<MeshRenderer>().material = tireMat;
				}
				if (canDetach)
				{
					detachedTire = new GameObject("Detached Tire");
					detachedTire.transform.parent = detachedWheel.transform;
					detachedTire.transform.localPosition = Vector3.zero;
					detachedTire.transform.localRotation = Quaternion.identity;
					detachTireFilter = detachedTire.AddComponent<MeshFilter>();
					detachTireFilter.sharedMesh = tire.GetComponent<MeshFilter>().sharedMesh;
					detachedTire.AddComponent<MeshRenderer>().sharedMaterial = ((!tireMat) ? tire.GetComponent<MeshRenderer>().sharedMaterial : tireMat);
				}
			}
			if (Application.isPlaying)
			{
				if (generateHardCollider)
				{
					GameObject gameObject = new GameObject("Rim Collider");
					gameObject.layer = GlobalControl.ignoreWheelCastLayer;
					sphereColTr = gameObject.transform;
					sphereCol = gameObject.AddComponent<SphereCollider>();
					sphereColTr.parent = tr;
					sphereColTr.localPosition = Vector3.zero;
					sphereColTr.localRotation = Quaternion.identity;
					sphereCol.radius = Mathf.Min(rimWidth * 0.5f, rimRadius * 0.5f);
					sphereCol.material = GlobalControl.frictionlessMatStatic;
				}
				if (canDetach)
				{
					detachedWheel.SetActive(value: false);
				}
			}
		}
		targetDrive = GetComponent<DriveForce>();
		currentRPM = 0f;
	}

	private void FixedUpdate()
	{
		upDir = tr.up;
		actualRadius = ((!popped) ? Mathf.Lerp(rimRadius, tireRadius, tirePressure) : rimRadius);
		circumference = (float)Math.PI * actualRadius * 2f;
		localVel = rb.GetPointVelocity(forceApplicationPoint);
		actualEbrake = ((!suspensionParent.ebrakeEnabled) ? 0f : suspensionParent.ebrakeForce);
		actualTargetRPM = targetDrive.rpm * (float)((!suspensionParent.driveInverted) ? 1 : (-1));
		actualTorque = ((!suspensionParent.driveEnabled) ? 0f : Mathf.Lerp(targetDrive.torque, Mathf.Abs(vp.accelInput), vp.burnout));
		if (getContact)
		{
			GetWheelContact();
		}
		else if (grounded)
		{
			contactPoint.point += localVel * Time.fixedDeltaTime;
		}
		airTime = ((!grounded) ? (airTime + Time.fixedDeltaTime) : 0f);
		forceApplicationPoint = ((!applyForceAtGroundContact) ? tr.position : contactPoint.point);
		if (connected)
		{
			GetRawRPM();
			ApplyDrive();
		}
		else
		{
			rawRPM = 0f;
			currentRPM = 0f;
			targetDrive.feedbackRPM = 0f;
		}
		travelDist = ((!(suspensionParent.compression < travelDist) && !grounded) ? Mathf.Lerp(travelDist, suspensionParent.compression, suspensionParent.extendSpeed * Time.fixedDeltaTime) : suspensionParent.compression);
		PositionWheel();
		if (!connected)
		{
			return;
		}
		if (generateHardCollider)
		{
			setRimWidth = rimWidth;
			setRimRadius = rimRadius;
			setTireWidth = tireWidth;
			setTireRadius = tireRadius;
			setTirePressure = tirePressure;
			if (rimWidthPrev != setRimWidth || rimRadiusPrev != setRimRadius)
			{
				sphereCol.radius = Mathf.Min(rimWidth * 0.5f, rimRadius * 0.5f);
				updatedSize = true;
			}
			else if (tireWidthPrev != setTireWidth || tireRadiusPrev != setTireRadius || tirePressurePrev != setTirePressure)
			{
				updatedSize = true;
			}
			else
			{
				updatedSize = false;
			}
			rimWidthPrev = setRimWidth;
			rimRadiusPrev = setRimRadius;
			tireWidthPrev = setTireWidth;
			tireRadiusPrev = setTireRadius;
			tirePressurePrev = setTirePressure;
		}
		GetSlip();
		ApplyFriction();
		if (vp.burnout > 0f && targetDrive.rpm != 0f && actualEbrake * vp.ebrakeInput == 0f && connected && grounded)
		{
			rb.AddForceAtPosition(suspensionParent.forwardDir * (0f - suspensionParent.flippedSideFactor) * (vp.steerInput * vp.burnoutSpin * currentRPM * Mathf.Min(0.1f, targetDrive.torque) * 0.001f) * vp.burnout * ((!popped) ? 1f : 0.5f) * contactPoint.surfaceFriction, suspensionParent.tr.position, ForceMode.Acceleration);
		}
		setPopped = popped;
		if (poppedPrev != setPopped)
		{
			if ((bool)tire)
			{
				tire.gameObject.SetActive(!popped);
			}
			updatedPopped = true;
		}
		else
		{
			updatedPopped = false;
		}
		poppedPrev = setPopped;
		if (!(airLeakTime >= 0f))
		{
			return;
		}
		tirePressure = Mathf.Clamp01(tirePressure - Time.fixedDeltaTime * 0.5f);
		if (!grounded)
		{
			return;
		}
		airLeakTime += Mathf.Max(Mathf.Abs(currentRPM) * 0.001f, localVel.magnitude * 0.1f) * Time.timeScale * TimeMaster.inverseFixedTimeFactor;
		if (airLeakTime > 1000f && tirePressure == 0f)
		{
			popped = true;
			airLeakTime = -1f;
			if ((bool)impactSnd && (bool)tirePopClip)
			{
				impactSnd.PlayOneShot(tirePopClip);
				impactSnd.pitch = 1f;
			}
		}
	}

	private void Update()
	{
		RotateWheel();
		if (!Application.isPlaying)
		{
			PositionWheel();
			return;
		}
		if (deformAmount > 0f && (bool)tireMat && connected && tireMat.HasProperty("_DeformNormal"))
		{
			Vector3 vector = ((!grounded) ? Vector3.zero : (contactPoint.normal * Mathf.Max((0f - suspensionParent.penetration) * (1f - suspensionParent.compression) * 10f, 1f - tirePressure) * deformAmount));
			tireMat.SetVector("_DeformNormal", new Vector4(vector.x, vector.y, vector.z, 0f));
		}
		if ((bool)rimMat && rimMat.HasProperty("_EmissionColor"))
		{
			float num = ((!connected || !GroundSurfaceMaster.surfaceTypesStatic[contactPoint.surfaceType].leaveSparks) ? 0f : Mathf.Abs(F.MaxAbs(forwardSlip, sidewaysSlip)));
			glowAmount = ((!popped) ? 0f : Mathf.Lerp(glowAmount, num, ((!(num > glowAmount)) ? 0.2f : 2f) * Time.deltaTime));
			glowColor = new Color(glowAmount, glowAmount * 0.5f, 0f);
			rimMat.SetColor("_EmissionColor", (!popped) ? Color.black : Color.Lerp(Color.black, glowColor, glowAmount * rimGlow));
		}
	}

	private void GetWheelContact()
	{
		float maxDistance = Mathf.Max(suspensionParent.suspensionDistance * Mathf.Max(0.001f, suspensionParent.targetCompression) + actualRadius, 0.001f);
		RaycastHit[] array = Physics.RaycastAll(suspensionParent.maxCompressPoint, suspensionParent.springDirection, maxDistance, GlobalControl.wheelCastMaskStatic);
		int num = 0;
		bool flag = false;
		float num2 = float.PositiveInfinity;
		if (connected)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].transform.IsChildOf(vp.tr) && array[i].distance < num2)
				{
					num = i;
					num2 = array[i].distance;
					flag = true;
				}
			}
		}
		else
		{
			flag = false;
		}
		if (flag)
		{
			RaycastHit raycastHit = array[num];
			if (!grounded && (bool)impactSnd && ((tireHitClips.Length != 0 && !popped) || ((bool)rimHitClip && popped)))
			{
				impactSnd.PlayOneShot((!popped) ? tireHitClips[Mathf.RoundToInt(UnityEngine.Random.Range(0, tireHitClips.Length - 1))] : rimHitClip, Mathf.Clamp01(airTime * airTime));
				impactSnd.pitch = Mathf.Clamp(airTime * 0.2f + 0.8f, 0.8f, 1f);
			}
			grounded = true;
			contactPoint.distance = raycastHit.distance - actualRadius;
			contactPoint.point = raycastHit.point + localVel * Time.fixedDeltaTime;
			contactPoint.grounded = true;
			contactPoint.normal = raycastHit.normal;
			contactPoint.relativeVelocity = tr.InverseTransformDirection(localVel);
			contactPoint.col = raycastHit.collider;
			if ((bool)raycastHit.collider.attachedRigidbody)
			{
				contactVelocity = raycastHit.collider.attachedRigidbody.GetPointVelocity(contactPoint.point);
				contactPoint.relativeVelocity -= tr.InverseTransformDirection(contactVelocity);
			}
			else
			{
				contactVelocity = Vector3.zero;
			}
			GroundSurfaceInstance component = raycastHit.collider.GetComponent<GroundSurfaceInstance>();
			TerrainSurface component2 = raycastHit.collider.GetComponent<TerrainSurface>();
			if ((bool)component)
			{
				contactPoint.surfaceFriction = component.friction;
				contactPoint.surfaceType = component.surfaceType;
			}
			else if ((bool)component2)
			{
				contactPoint.surfaceType = component2.GetDominantSurfaceTypeAtPoint(contactPoint.point);
				contactPoint.surfaceFriction = component2.GetFriction(contactPoint.surfaceType);
			}
			else
			{
				contactPoint.surfaceFriction = raycastHit.collider.material.dynamicFriction * 2f;
				contactPoint.surfaceType = 0;
			}
			if (contactPoint.col.CompareTag("Pop Tire") && canPop && airLeakTime == -1f && !popped)
			{
				Deflate();
			}
		}
		else
		{
			grounded = false;
			contactPoint.distance = suspensionParent.suspensionDistance;
			contactPoint.point = Vector3.zero;
			contactPoint.grounded = false;
			contactPoint.normal = upDir;
			contactPoint.relativeVelocity = Vector3.zero;
			contactPoint.col = null;
			contactVelocity = Vector3.zero;
			contactPoint.surfaceFriction = 0f;
			contactPoint.surfaceType = 0;
		}
	}

	private void GetRawRPM()
	{
		if (grounded)
		{
			rawRPM = contactPoint.relativeVelocity.x / circumference * ((float)Math.PI * 100f) * (0f - suspensionParent.flippedSideFactor);
		}
		else
		{
			rawRPM = Mathf.Lerp(rawRPM, actualTargetRPM, (actualTorque + suspensionParent.brakeForce * vp.brakeInput + actualEbrake * vp.ebrakeInput) * Time.timeScale);
		}
	}

	private void GetSlip()
	{
		if (grounded)
		{
			sidewaysSlip = contactPoint.relativeVelocity.z * 0.1f / sidewaysCurveStretch;
			forwardSlip = 0.01f * (rawRPM - currentRPM) / forwardCurveStretch;
		}
		else
		{
			sidewaysSlip = 0f;
			forwardSlip = 0f;
		}
	}

	private void ApplyFriction()
	{
		if (grounded)
		{
			float f = ((slipDependence != SlipDependenceMode.dependent && slipDependence != SlipDependenceMode.forward) ? forwardSlip : (forwardSlip - sidewaysSlip));
			float f2 = ((slipDependence != SlipDependenceMode.dependent && slipDependence != SlipDependenceMode.sideways) ? sidewaysSlip : (sidewaysSlip - forwardSlip));
			float num = Mathf.Clamp01(forwardSlipDependence - Mathf.Clamp01(Mathf.Abs(sidewaysSlip)));
			float num2 = Mathf.Clamp01(sidewaysSlipDependence - Mathf.Clamp01(Mathf.Abs(forwardSlip)));
			frictionForce = Vector3.Lerp(frictionForce, tr.TransformDirection(forwardFrictionCurve.Evaluate(Mathf.Abs(f)) * (float)(-Math.Sign(forwardSlip)) * ((!popped) ? forwardFriction : forwardRimFriction) * num * (0f - suspensionParent.flippedSideFactor), 0f, sidewaysFrictionCurve.Evaluate(Mathf.Abs(f2)) * (float)(-Math.Sign(sidewaysSlip)) * ((!popped) ? sidewaysFriction : sidewaysRimFriction) * num2 * normalFrictionCurve.Evaluate(Mathf.Clamp01(Vector3.Dot(contactPoint.normal, GlobalControl.worldUpDir))) * ((!(vp.burnout > 0f) || Mathf.Abs(targetDrive.rpm) == 0f || actualEbrake * vp.ebrakeInput != 0f || !grounded) ? 1f : ((1f - vp.burnout) * (1f - Mathf.Abs(vp.accelInput))))) * (0.5f + (1f - suspensionParent.compression) * 0.5f * Mathf.Clamp01(Mathf.Abs(suspensionParent.tr.InverseTransformDirection(localVel).z) * 10f)) * contactPoint.surfaceFriction, 1f - frictionSmoothness);
			rb.AddForceAtPosition(frictionForce, forceApplicationPoint, ForceMode.Acceleration);
			if ((bool)contactPoint.col.attachedRigidbody)
			{
				contactPoint.col.attachedRigidbody.AddForceAtPosition(-frictionForce, contactPoint.point, ForceMode.Acceleration);
			}
		}
	}

	private void ApplyDrive()
	{
		float num = 0f;
		float num2 = ((!suspensionParent.skidSteerBrake) ? vp.localVelocity.z : vp.localAngularVel.y);
		if (vp.brakeIsReverse)
		{
			if (num2 > 0f)
			{
				num = suspensionParent.brakeForce * vp.brakeInput;
			}
			else if (num2 <= 0f)
			{
				num = suspensionParent.brakeForce * Mathf.Clamp01(vp.accelInput);
			}
		}
		else
		{
			num = suspensionParent.brakeForce * vp.brakeInput;
		}
		num += axleFriction * 0.1f * (float)(Mathf.Approximately(actualTorque, 0f) ? 1 : 0);
		if (targetDrive.rpm != 0f)
		{
			num *= 1f - vp.burnout;
		}
		if (!suspensionParent.jammed && connected)
		{
			bool flag = ((!Mathf.Approximately(actualTorque, 0f) || !(Mathf.Abs(actualTargetRPM) < 0.01f)) && !Mathf.Approximately(actualTargetRPM, 0f)) || num + actualEbrake * vp.ebrakeInput > 0f;
			currentRPM = Mathf.Lerp(rawRPM, Mathf.Lerp(Mathf.Lerp(rawRPM, actualTargetRPM, (!flag) ? actualTorque : EvaluateTorque(actualTorque)), 0f, Mathf.Max(num, actualEbrake * vp.ebrakeInput)), (!flag) ? (actualTorque + num + actualEbrake * vp.ebrakeInput) : EvaluateTorque(actualTorque + num + actualEbrake * vp.ebrakeInput));
			targetDrive.feedbackRPM = Mathf.Lerp(currentRPM, rawRPM, feedbackRpmBias);
		}
		else
		{
			currentRPM = 0f;
			targetDrive.feedbackRPM = 0f;
		}
	}

	private float EvaluateTorque(float t)
	{
		return Mathf.Lerp(rpmBiasCurve.Evaluate(t), t, rawRPM / (rpmBiasCurveLimit * Mathf.Sign(actualTargetRPM)));
	}

	private void PositionWheel()
	{
		if ((bool)suspensionParent)
		{
			rim.position = suspensionParent.maxCompressPoint + suspensionParent.springDirection * suspensionParent.suspensionDistance * ((!Application.isPlaying) ? suspensionParent.targetCompression : travelDist) + suspensionParent.upDir * Mathf.Pow(Mathf.Max(Mathf.Abs(Mathf.Sin(suspensionParent.sideAngle * ((float)Math.PI / 180f))), Mathf.Abs(Mathf.Sin(suspensionParent.casterAngle * ((float)Math.PI / 180f)))), 2f) * actualRadius + suspensionParent.pivotOffset * suspensionParent.tr.TransformDirection(Mathf.Sin(tr.localEulerAngles.y * ((float)Math.PI / 180f)), 0f, Mathf.Cos(tr.localEulerAngles.y * ((float)Math.PI / 180f))) - suspensionParent.pivotOffset * ((!Application.isPlaying) ? suspensionParent.tr.forward : suspensionParent.forwardDir);
		}
		if (Application.isPlaying && generateHardCollider)
		{
			_ = connected;
		}
	}

	private void RotateWheel()
	{
		if ((bool)tr && (bool)suspensionParent)
		{
			float num = ((Mathf.Sign(suspensionParent.steerAngle) != suspensionParent.flippedSideFactor) ? (1f - suspensionParent.ackermannFactor) : (1f + suspensionParent.ackermannFactor));
			tr.localEulerAngles = new Vector3(suspensionParent.camberAngle + suspensionParent.casterAngle * suspensionParent.steerAngle * suspensionParent.flippedSideFactor, (0f - suspensionParent.toeAngle) * suspensionParent.flippedSideFactor + suspensionParent.steerDegrees * num, 0f);
		}
		if (Application.isPlaying)
		{
			rim.Rotate(Vector3.forward, currentRPM * suspensionParent.flippedSideFactor * Time.deltaTime);
			if (damage > 0f)
			{
				rim.localEulerAngles = new Vector3(Mathf.Sin((0f - rim.localEulerAngles.z) * ((float)Math.PI / 180f)) * Mathf.Clamp(damage, 0f, 10f), Mathf.Cos((0f - rim.localEulerAngles.z) * ((float)Math.PI / 180f)) * Mathf.Clamp(damage, 0f, 10f), rim.localEulerAngles.z);
			}
			else if (rim.localEulerAngles.x != 0f || rim.localEulerAngles.y != 0f)
			{
				rim.localEulerAngles = new Vector3(0f, 0f, rim.localEulerAngles.z);
			}
		}
	}

	public void Deflate()
	{
		airLeakTime = 0f;
		if ((bool)impactSnd && (bool)tireAirClip)
		{
			impactSnd.PlayOneShot(tireAirClip);
			impactSnd.pitch = 1f;
		}
	}

	public void FixTire()
	{
		popped = false;
		tirePressure = initialTirePressure;
		airLeakTime = -1f;
	}

	public void Detach()
	{
		if (connected && canDetach)
		{
			connected = false;
			detachedWheel.SetActive(value: true);
			detachedWheel.transform.position = rim.position;
			detachedWheel.transform.rotation = rim.rotation;
			detachedCol.sharedMaterial = ((!popped) ? detachedTireMaterial : detachedRimMaterial);
			if ((bool)tire)
			{
				detachedTire.SetActive(!popped);
				detachedCol.sharedMesh = ((!(airLeakTime >= 0f) && !popped) ? ((!tireMeshLoose) ? detachTireFilter.sharedMesh : tireMeshLoose) : ((!rimMeshLoose) ? detachFilter.sharedMesh : rimMeshLoose));
			}
			else
			{
				detachedCol.sharedMesh = ((!rimMeshLoose) ? detachFilter.sharedMesh : rimMeshLoose);
			}
			rb.mass -= mass;
			detachedBody.velocity = rb.GetPointVelocity(rim.position);
			detachedBody.angularVelocity = rb.angularVelocity;
			rim.gameObject.SetActive(value: false);
			if ((bool)sphereColTr)
			{
				sphereColTr.gameObject.SetActive(value: false);
			}
		}
	}

	public void GetWheelDimensions(float radiusMargin, float widthMargin)
	{
		Mesh mesh = null;
		Mesh mesh2 = null;
		Transform child = base.transform;
		if (base.transform.childCount <= 0)
		{
			return;
		}
		if ((bool)base.transform.GetChild(0).GetComponent<MeshFilter>())
		{
			mesh = base.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
			child = base.transform.GetChild(0);
		}
		if (base.transform.GetChild(0).childCount > 0 && (bool)base.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>())
		{
			mesh2 = base.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>()
				.sharedMesh;
		}
		Mesh mesh3 = ((!mesh2) ? mesh : mesh2);
		if ((bool)mesh3)
		{
			float num = 0f;
			float num2 = 0f;
			Vector3[] vertices = mesh3.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vector = vertices[i];
				if (new Vector2(vector.x * child.localScale.x, vector.y * child.localScale.y).magnitude > num2)
				{
					num2 = new Vector2(vector.x * child.localScale.x, vector.y * child.localScale.y).magnitude;
				}
				if (Mathf.Abs(vector.z * child.localScale.z) > num)
				{
					num = Mathf.Abs(vector.z * child.localScale.z);
				}
			}
			tireRadius = num2 + radiusMargin;
			tireWidth = num + widthMargin;
			if ((bool)mesh2 && (bool)mesh)
			{
				num = 0f;
				num2 = 0f;
				Vector3[] vertices2 = mesh.vertices;
				for (int j = 0; j < vertices2.Length; j++)
				{
					Vector3 vector2 = vertices2[j];
					if (new Vector2(vector2.x * child.localScale.x, vector2.y * child.localScale.y).magnitude > num2)
					{
						num2 = new Vector2(vector2.x * child.localScale.x, vector2.y * child.localScale.y).magnitude;
					}
					if (Mathf.Abs(vector2.z * child.localScale.z) > num)
					{
						num = Mathf.Abs(vector2.z * child.localScale.z);
					}
				}
				rimRadius = num2 + radiusMargin;
				rimWidth = num + widthMargin;
			}
			else
			{
				rimRadius = num2 * 0.5f + radiusMargin;
				rimWidth = num * 0.5f + widthMargin;
			}
		}
		else
		{
			Debug.LogError("No rim or tire meshes found for getting wheel dimensions.", this);
		}
	}

	public void Reattach()
	{
		if (!connected)
		{
			connected = true;
			detachedWheel.SetActive(value: false);
			rb.mass += mass;
			rim.gameObject.SetActive(value: true);
			if ((bool)sphereColTr)
			{
				sphereColTr.gameObject.SetActive(value: true);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		tr = base.transform;
		if (tr.childCount > 0)
		{
			rim = tr.GetChild(0);
			if (rim.childCount > 0)
			{
				tire = rim.GetChild(0);
			}
		}
		float radius = Mathf.Lerp(rimRadius, tireRadius, tirePressure);
		if (tirePressure < 1f && tirePressure > 0f)
		{
			Gizmos.color = new Color(1f, 1f, 0f, (!popped) ? 1f : 0.5f);
			GizmosExtra.DrawWireCylinder(rim.position, rim.forward, radius, tireWidth * 2f);
		}
		Gizmos.color = Color.white;
		GizmosExtra.DrawWireCylinder(rim.position, rim.forward, tireRadius, tireWidth * 2f);
		Gizmos.color = ((tirePressure != 0f && !popped) ? Color.cyan : Color.green);
		GizmosExtra.DrawWireCylinder(rim.position, rim.forward, rimRadius, rimWidth * 2f);
		Gizmos.color = new Color(1f, 1f, 1f, (!(tirePressure < 1f)) ? 1f : 0.5f);
		GizmosExtra.DrawWireCylinder(rim.position, rim.forward, tireRadius, tireWidth * 2f);
		Gizmos.color = ((tirePressure != 0f && !popped) ? Color.cyan : Color.green);
		GizmosExtra.DrawWireCylinder(rim.position, rim.forward, rimRadius, rimWidth * 2f);
	}

	private void OnDestroy()
	{
		if (Application.isPlaying && (bool)detachedWheel)
		{
			UnityEngine.Object.Destroy(detachedWheel);
		}
	}
}
