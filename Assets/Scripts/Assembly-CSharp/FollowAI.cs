using System.Collections;
using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/AI/Follow AI", 0)]
public class FollowAI : MonoBehaviour
{
	private Transform tr;

	private Rigidbody rb;

	private VehicleParent vp;

	private VehicleAssist va;

	public Transform target;

	private Transform targetPrev;

	private Rigidbody targetBody;

	private Vector3 targetPoint;

	private bool targetVisible;

	private bool targetIsWaypoint;

	private VehicleWaypoint targetWaypoint;

	public float followDistance;

	private bool close;

	[Tooltip("Percentage of maximum speed to drive at")]
	[Range(0f, 1f)]
	public float speed = 1f;

	private float initialSpeed;

	private float prevSpeed;

	private float brakeTime;

	[Tooltip("Mask for which objects can block the view of the target")]
	public LayerMask viewBlockMask;

	private Vector3 dirToTarget;

	private float lookDot;

	private float steerDot;

	private float stoppedTime;

	private float reverseTime;

	[Tooltip("Time limit in seconds which the vehicle is stuck before attempting to reverse")]
	public float stopTimeReverse = 1f;

	[Tooltip("Duration in seconds the vehicle will reverse after getting stuck")]
	public float reverseAttemptTime = 1f;

	[Tooltip("How many times the vehicle will attempt reversing before resetting, -1 = no reset")]
	public int resetReverseCount = 1;

	private int reverseAttempts;

	[Tooltip("Seconds a vehicle will be rolled over before resetting, -1 = no reset")]
	public float rollResetTime = 3f;

	private float rolledOverTime;

	private void Start()
	{
		tr = base.transform;
		rb = GetComponent<Rigidbody>();
		vp = GetComponent<VehicleParent>();
		va = GetComponent<VehicleAssist>();
		initialSpeed = speed;
		InitializeTarget();
	}

	private void FixedUpdate()
	{
		if ((bool)target)
		{
			if (target != targetPrev)
			{
				InitializeTarget();
			}
			targetPrev = target;
			targetIsWaypoint = target.GetComponent<VehicleWaypoint>();
			targetVisible = !Physics.Linecast(tr.position, target.position, viewBlockMask);
			if (targetVisible || targetIsWaypoint)
			{
				targetPoint = ((!targetBody) ? target.position : (target.position + targetBody.velocity));
			}
			if (targetIsWaypoint)
			{
				if ((tr.position - target.position).sqrMagnitude <= targetWaypoint.radius * targetWaypoint.radius)
				{
					target = targetWaypoint.nextPoint.transform;
					targetWaypoint = targetWaypoint.nextPoint;
					prevSpeed = speed;
					speed = Mathf.Clamp01(targetWaypoint.speed * initialSpeed);
					brakeTime = prevSpeed / speed;
					if (brakeTime <= 1f)
					{
						brakeTime = 0f;
					}
				}
			}
			else
			{
				speed = initialSpeed;
			}
			brakeTime = Mathf.Max(0f, brakeTime - Time.fixedDeltaTime);
			close = (tr.position - target.position).sqrMagnitude <= Mathf.Pow(followDistance, 2f) && !targetIsWaypoint;
			dirToTarget = (targetPoint - tr.position).normalized;
			lookDot = Vector3.Dot(vp.forwardDir, dirToTarget);
			steerDot = Vector3.Dot(vp.rightDir, dirToTarget);
			stoppedTime = ((!(Mathf.Abs(vp.localVelocity.z) < 1f) || close || vp.groundedWheels <= 0) ? 0f : (stoppedTime + Time.fixedDeltaTime));
			if ((stoppedTime > stopTimeReverse || (vp.velMag < 1f && lookDot < 0f) || (vp.velMag <= 20f && (double)lookDot < -0.8)) && reverseTime == 0f)
			{
				reverseTime = reverseAttemptTime;
				reverseAttempts++;
			}
			if (reverseAttempts > resetReverseCount && resetReverseCount >= 0)
			{
				StartCoroutine(ReverseReset());
			}
			reverseTime = Mathf.Max(0f, reverseTime - Time.fixedDeltaTime);
			vp.SetAccel((close || (!(lookDot > 0f) && !(vp.localVelocity.z < 5f)) || vp.groundedWheels <= 0 || reverseTime != 0f) ? 0f : speed);
			vp.SetBrake((reverseTime != 0f || brakeTime != 0f || (close && vp.localVelocity.z > 0.1f)) ? ((!(brakeTime > 0f)) ? 1f : (brakeTime * 0.2f)) : ((!(lookDot < 0.5f) || !(lookDot > 0f) || !(vp.localVelocity.z > 10f)) ? 0f : (0.5f - lookDot)));
			vp.SetSteer((reverseTime != 0f) ? ((0f - Mathf.Sign(steerDot)) * (float)((!close) ? 1 : 0)) : (Mathf.Abs(Mathf.Pow(steerDot, ((tr.position - target.position).sqrMagnitude > 20f) ? 1 : 2)) * Mathf.Sign(steerDot)));
			vp.SetEbrake(((close && vp.localVelocity.z <= 0.1f) || (lookDot <= 0f && vp.velMag > 20f)) ? 1 : 0);
		}
		rolledOverTime = ((!va.rolledOver) ? 0f : (rolledOverTime + Time.fixedDeltaTime));
		if (rolledOverTime > rollResetTime && rollResetTime >= 0f)
		{
			StartCoroutine(ResetRotation());
		}
	}

	private IEnumerator ReverseReset()
	{
		reverseAttempts = 0;
		reverseTime = 0f;
		yield return new WaitForFixedUpdate();
		tr.position = targetPoint;
		tr.rotation = Quaternion.LookRotation((!targetIsWaypoint) ? Vector3.forward : (targetWaypoint.nextPoint.transform.position - targetPoint).normalized, GlobalControl.worldUpDir);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	private IEnumerator ResetRotation()
	{
		yield return new WaitForFixedUpdate();
		tr.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
		tr.Translate(Vector3.up, Space.World);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	public void InitializeTarget()
	{
		if ((bool)target)
		{
			targetBody = (Rigidbody)F.GetTopmostParentComponent<Rigidbody>(target);
			targetWaypoint = target.GetComponent<VehicleWaypoint>();
			if ((bool)targetWaypoint)
			{
				prevSpeed = targetWaypoint.speed;
			}
		}
	}
}
