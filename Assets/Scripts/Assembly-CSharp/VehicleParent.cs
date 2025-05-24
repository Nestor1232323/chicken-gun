using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Vehicle Controllers/Vehicle Parent", 0)]
public class VehicleParent : MonoBehaviour
{
	[NonSerialized]
	public Rigidbody rb;

	[NonSerialized]
	public Transform tr;

	[NonSerialized]
	public Transform norm;

	[NonSerialized]
	public float accelInput;

	[NonSerialized]
	public float brakeInput;

	[NonSerialized]
	public float steerInput;

	[NonSerialized]
	public float ebrakeInput;

	[NonSerialized]
	public bool boostButton;

	[NonSerialized]
	public bool upshiftPressed;

	[NonSerialized]
	public bool downshiftPressed;

	[NonSerialized]
	public float upshiftHold;

	[NonSerialized]
	public float downshiftHold;

	[NonSerialized]
	public float pitchInput;

	[NonSerialized]
	public float yawInput;

	[NonSerialized]
	public float rollInput;

	[Tooltip("Accel axis is used for brake input")]
	public bool accelAxisIsBrake;

	[Tooltip("Brake input will act as reverse input")]
	public bool brakeIsReverse;

	[Tooltip("Automatically hold ebrake if it's pressed while parked")]
	public bool holdEbrakePark;

	public float burnoutThreshold = 0.9f;

	[NonSerialized]
	public float burnout;

	public float burnoutSpin = 5f;

	[Range(0f, 0.9f)]
	public float burnoutSmoothness = 0.5f;

	public Motor engine;

	private bool stopUpshift;

	private bool stopDownShift;

	[NonSerialized]
	public Vector3 localVelocity;

	[NonSerialized]
	public Vector3 localAngularVel;

	[NonSerialized]
	public Vector3 forwardDir;

	[NonSerialized]
	public Vector3 rightDir;

	[NonSerialized]
	public Vector3 upDir;

	[NonSerialized]
	public float forwardDot;

	[NonSerialized]
	public float rightDot;

	[NonSerialized]
	public float upDot;

	[NonSerialized]
	public float velMag;

	[NonSerialized]
	public float sqrVelMag;

	[NonSerialized]
	public bool reversing;

	public Wheel[] wheels;

	public HoverWheel[] hoverWheels;

	public WheelCheckGroup[] wheelGroups;

	private bool wheelLoopDone;

	public bool hover;

	[NonSerialized]
	public int groundedWheels;

	[NonSerialized]
	public Vector3 wheelNormalAverage;

	private Vector3 wheelContactsVelocity;

	[Tooltip("Lower center of mass by suspension height")]
	public bool suspensionCenterOfMass;

	public Vector3 centerOfMassOffset;

	[Tooltip("Tow vehicle to instantiate")]
	public GameObject towVehicle;

	private GameObject newTow;

	[NonSerialized]
	public VehicleParent inputInherit;

	[NonSerialized]
	public bool crashing;

	[Header("Crashing")]
	public bool canCrash = true;

	public AudioSource crashSnd;

	public AudioClip[] crashClips;

	[NonSerialized]
	public bool playCrashSounds = true;

	public ParticleSystem sparks;

	[NonSerialized]
	public bool playCrashSparks = true;

	[Header("Camera")]
	public float cameraDistanceChange;

	public float cameraHeightChange;

	private void Start()
	{
		tr = base.transform;
		rb = GetComponent<Rigidbody>();
		GameObject gameObject = new GameObject(tr.name + "'s Normal Orientation");
		norm = gameObject.transform;
		SetCenterOfMass();
		if ((bool)towVehicle)
		{
			newTow = UnityEngine.Object.Instantiate(towVehicle, Vector3.zero, tr.rotation);
			newTow.SetActive(value: false);
			newTow.transform.position = tr.TransformPoint(newTow.GetComponent<Joint>().connectedAnchor - newTow.GetComponent<Joint>().anchor);
			newTow.GetComponent<Joint>().connectedBody = rb;
			newTow.SetActive(value: true);
			newTow.GetComponent<VehicleParent>().inputInherit = this;
		}
		if ((bool)sparks)
		{
			sparks.transform.parent = null;
		}
		if (wheelGroups.Length != 0)
		{
			StartCoroutine(WheelCheckLoop());
		}
	}

	private void Update()
	{
		if (stopUpshift)
		{
			upshiftPressed = false;
			stopUpshift = false;
		}
		if (stopDownShift)
		{
			downshiftPressed = false;
			stopDownShift = false;
		}
		if (upshiftPressed)
		{
			stopUpshift = true;
		}
		if (downshiftPressed)
		{
			stopDownShift = true;
		}
		if ((bool)inputInherit)
		{
			InheritInputOneShot();
		}
	}

	private void FixedUpdate()
	{
		if ((bool)inputInherit)
		{
			InheritInput();
		}
		if (wheelLoopDone && wheelGroups.Length != 0)
		{
			wheelLoopDone = false;
			StartCoroutine(WheelCheckLoop());
		}
		GetGroundedWheels();
		if (groundedWheels > 0)
		{
			crashing = false;
		}
		localVelocity = tr.InverseTransformDirection(rb.velocity - wheelContactsVelocity);
		localAngularVel = tr.InverseTransformDirection(rb.angularVelocity);
		velMag = rb.velocity.magnitude;
		sqrVelMag = rb.velocity.sqrMagnitude;
		forwardDir = tr.forward;
		rightDir = tr.right;
		upDir = tr.up;
		forwardDot = Vector3.Dot(forwardDir, GlobalControl.worldUpDir);
		rightDot = Vector3.Dot(rightDir, GlobalControl.worldUpDir);
		upDot = Vector3.Dot(upDir, GlobalControl.worldUpDir);
		norm.transform.position = tr.position;
		norm.transform.rotation = Quaternion.LookRotation((groundedWheels != 0) ? wheelNormalAverage : upDir, forwardDir);
		if (groundedWheels > 0 && !hover && !accelAxisIsBrake && burnoutThreshold >= 0f && accelInput > burnoutThreshold && brakeInput > burnoutThreshold)
		{
			burnout = Mathf.Lerp(burnout, (5f - Mathf.Min(5f, Mathf.Abs(localVelocity.z))) / 5f * Mathf.Abs(accelInput), Time.fixedDeltaTime * (1f - burnoutSmoothness) * 10f);
		}
		else if (burnout > 0.01f)
		{
			burnout = Mathf.Lerp(burnout, 0f, Time.fixedDeltaTime * (1f - burnoutSmoothness) * 10f);
		}
		else
		{
			burnout = 0f;
		}
		if ((bool)engine)
		{
			burnout *= engine.health;
		}
		if (brakeIsReverse && brakeInput > 0f && localVelocity.z < 1f && burnout == 0f)
		{
			reversing = true;
		}
		else if (localVelocity.z >= 0f || burnout > 0f)
		{
			reversing = false;
		}
	}

	public void SetAccel(float f)
	{
		f = Mathf.Clamp(f, -1f, 1f);
		accelInput = f;
	}

	public void SetBrake(float f)
	{
		brakeInput = ((!accelAxisIsBrake) ? Mathf.Clamp(f, -1f, 1f) : (0f - Mathf.Clamp(accelInput, -1f, 0f)));
	}

	public void SetSteer(float f)
	{
		steerInput = Mathf.Clamp(f, -1f, 1f);
	}

	public void SetEbrake(float f)
	{
		if ((f > 0f || ebrakeInput > 0f) && holdEbrakePark && velMag < 1f && accelInput == 0f && (brakeInput == 0f || !brakeIsReverse))
		{
			ebrakeInput = 1f;
		}
		else
		{
			ebrakeInput = Mathf.Clamp01(f);
		}
	}

	public void SetBoost(bool b)
	{
		boostButton = b;
	}

	public void SetPitch(float f)
	{
		pitchInput = Mathf.Clamp(f, -1f, 1f);
	}

	public void SetYaw(float f)
	{
		yawInput = Mathf.Clamp(f, -1f, 1f);
	}

	public void SetRoll(float f)
	{
		rollInput = Mathf.Clamp(f, -1f, 1f);
	}

	public void PressUpshift()
	{
		upshiftPressed = true;
	}

	public void PressDownshift()
	{
		downshiftPressed = true;
	}

	public void SetUpshift(float f)
	{
		upshiftHold = f;
	}

	public void SetDownshift(float f)
	{
		downshiftHold = f;
	}

	private void InheritInput()
	{
		accelInput = inputInherit.accelInput;
		brakeInput = inputInherit.brakeInput;
		steerInput = inputInherit.steerInput;
		ebrakeInput = inputInherit.ebrakeInput;
		pitchInput = inputInherit.pitchInput;
		yawInput = inputInherit.yawInput;
		rollInput = inputInherit.rollInput;
	}

	private void InheritInputOneShot()
	{
		upshiftPressed = inputInherit.upshiftPressed;
		downshiftPressed = inputInherit.downshiftPressed;
	}

	private void SetCenterOfMass()
	{
		float num = 0f;
		if (suspensionCenterOfMass)
		{
			if (hover)
			{
				for (int i = 0; i < hoverWheels.Length; i++)
				{
					num = ((i != 0) ? ((num + hoverWheels[i].hoverDistance) * 0.5f) : hoverWheels[i].hoverDistance);
				}
			}
			else
			{
				for (int j = 0; j < wheels.Length; j++)
				{
					float suspensionDistance = wheels[j].transform.parent.GetComponent<Suspension>().suspensionDistance;
					num = ((j != 0) ? ((num + suspensionDistance) * 0.5f) : suspensionDistance);
				}
			}
		}
		rb.centerOfMass = centerOfMassOffset + new Vector3(0f, 0f - num, 0f);
		rb.inertiaTensor = rb.inertiaTensor;
	}

	private void GetGroundedWheels()
	{
		groundedWheels = 0;
		wheelContactsVelocity = Vector3.zero;
		if (hover)
		{
			for (int i = 0; i < hoverWheels.Length; i++)
			{
				if (hoverWheels[i].grounded)
				{
					wheelNormalAverage = ((i != 0) ? (wheelNormalAverage + hoverWheels[i].contactPoint.normal).normalized : hoverWheels[i].contactPoint.normal);
				}
				if (hoverWheels[i].grounded)
				{
					groundedWheels++;
				}
			}
			return;
		}
		for (int j = 0; j < wheels.Length; j++)
		{
			if (wheels[j].grounded)
			{
				wheelContactsVelocity = ((j != 0) ? ((wheelContactsVelocity + wheels[j].contactVelocity) * 0.5f) : wheels[j].contactVelocity);
				wheelNormalAverage = ((j != 0) ? (wheelNormalAverage + wheels[j].contactPoint.normal).normalized : wheels[j].contactPoint.normal);
			}
			if (wheels[j].grounded)
			{
				groundedWheels++;
			}
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.contacts.Length == 0 || groundedWheels != 0)
		{
			return;
		}
		ContactPoint[] contacts = col.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			if (contactPoint.thisCollider.CompareTag("Underside") || contactPoint.thisCollider.gameObject.layer == GlobalControl.ignoreWheelCastLayer || !(Vector3.Dot(contactPoint.normal, col.relativeVelocity.normalized) > 0.2f) || !(col.relativeVelocity.sqrMagnitude > 20f))
			{
				continue;
			}
			bool flag = true;
			if ((bool)newTow)
			{
				flag = !contactPoint.otherCollider.transform.IsChildOf(newTow.transform);
			}
			if (flag)
			{
				crashing = canCrash;
				if ((bool)crashSnd && crashClips.Length != 0 && playCrashSounds)
				{
					crashSnd.PlayOneShot(crashClips[UnityEngine.Random.Range(0, crashClips.Length)], Mathf.Clamp01(col.relativeVelocity.magnitude * 0.1f));
				}
				if ((bool)sparks && playCrashSparks)
				{
					sparks.transform.position = contactPoint.point;
					sparks.transform.rotation = Quaternion.LookRotation(col.relativeVelocity.normalized, contactPoint.normal);
					sparks.Play();
				}
			}
		}
	}

	private void OnCollisionStay(Collision col)
	{
		if (col.contacts.Length == 0 || groundedWheels != 0)
		{
			return;
		}
		ContactPoint[] contacts = col.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			if (!contactPoint.thisCollider.CompareTag("Underside") && contactPoint.thisCollider.gameObject.layer != GlobalControl.ignoreWheelCastLayer && col.relativeVelocity.sqrMagnitude < 5f)
			{
				bool flag = true;
				if ((bool)newTow)
				{
					flag = !contactPoint.otherCollider.transform.IsChildOf(newTow.transform);
				}
				if (flag)
				{
					crashing = canCrash;
				}
			}
		}
	}

	private void OnDestroy()
	{
		if ((bool)norm)
		{
			UnityEngine.Object.Destroy(norm.gameObject);
		}
		if ((bool)sparks)
		{
			UnityEngine.Object.Destroy(sparks.gameObject);
		}
	}

	private IEnumerator WheelCheckLoop()
	{
		for (int i = 0; i < wheelGroups.Length; i++)
		{
			wheelGroups[i].Activate();
			wheelGroups[(i != 0) ? (i - 1) : (wheelGroups.Length - 1)].Deactivate();
			yield return new WaitForFixedUpdate();
		}
		wheelLoopDone = true;
	}
}
