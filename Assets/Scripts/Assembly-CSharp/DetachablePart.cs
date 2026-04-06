using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Damage/Detachable Part", 1)]
public class DetachablePart : MonoBehaviour
{
	private Transform tr;

	private Rigidbody rb;

	private Rigidbody parentBody;

	private Transform initialParent;

	private Vector3 initialLocalPos;

	private Quaternion initialLocalRot;

	[NonSerialized]
	public HingeJoint hinge;

	[NonSerialized]
	public bool detached;

	[NonSerialized]
	public Vector3 initialPos;

	public float mass = 0.1f;

	public float drag;

	public float angularDrag = 0.05f;

	public float looseForce = -1f;

	public float breakForce = 25f;

	[Tooltip("A hinge joint is randomly chosen from the list to use")]
	public PartJoint[] joints;

	private Vector3 initialAnchor;

	[NonSerialized]
	public Vector3 displacedAnchor;

	private void Start()
	{
		tr = base.transform;
		if ((bool)tr.parent)
		{
			initialParent = tr.parent;
			initialLocalPos = tr.localPosition;
			initialLocalRot = tr.localRotation;
		}
		parentBody = (Rigidbody)F.GetTopmostParentComponent<Rigidbody>(tr);
		initialPos = tr.localPosition;
	}

	private void Update()
	{
		if ((bool)hinge && (initialAnchor - displacedAnchor).sqrMagnitude > 0.1f)
		{
			UnityEngine.Object.Destroy(hinge);
		}
	}

	public void Detach(bool makeJoint)
	{
		if (detached)
		{
			return;
		}
		detached = true;
		tr.parent = null;
		rb = base.gameObject.AddComponent<Rigidbody>();
		rb.mass = mass;
		rb.drag = drag;
		rb.angularDrag = angularDrag;
		if ((bool)parentBody)
		{
			parentBody.mass -= mass;
			rb.velocity = parentBody.GetPointVelocity(tr.position);
			rb.angularVelocity = parentBody.angularVelocity;
			if (makeJoint && joints.Length > 0)
			{
				PartJoint partJoint = joints[UnityEngine.Random.Range(0, joints.Length)];
				initialAnchor = partJoint.hingeAnchor;
				displacedAnchor = initialAnchor;
				hinge = base.gameObject.AddComponent<HingeJoint>();
				hinge.autoConfigureConnectedAnchor = false;
				hinge.connectedBody = parentBody;
				hinge.anchor = partJoint.hingeAnchor;
				hinge.axis = partJoint.hingeAxis;
				hinge.connectedAnchor = initialPos + partJoint.hingeAnchor;
				hinge.enableCollision = false;
				hinge.useLimits = partJoint.useLimits;
				JointLimits limits = default(JointLimits);
				limits.min = partJoint.minLimit;
				limits.max = partJoint.maxLimit;
				limits.bounciness = partJoint.bounciness;
				hinge.limits = limits;
				hinge.useSpring = partJoint.useSpring;
				JointSpring spring = default(JointSpring);
				spring.targetPosition = partJoint.springTargetPosition;
				spring.spring = partJoint.springForce;
				spring.damper = partJoint.springDamper;
				hinge.spring = spring;
				hinge.breakForce = breakForce;
				hinge.breakTorque = breakForce;
			}
		}
	}

	public void Reattach()
	{
		if (detached)
		{
			detached = false;
			tr.parent = initialParent;
			tr.localPosition = initialLocalPos;
			tr.localRotation = initialLocalRot;
			if ((bool)parentBody)
			{
				parentBody.mass += mass;
			}
			if ((bool)hinge)
			{
				UnityEngine.Object.Destroy(hinge);
			}
			if ((bool)rb)
			{
				UnityEngine.Object.Destroy(rb);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!tr)
		{
			tr = base.transform;
		}
		if (looseForce >= 0f && joints.Length > 0)
		{
			Gizmos.color = Color.red;
			PartJoint[] array = joints;
			foreach (PartJoint partJoint in array)
			{
				Gizmos.DrawRay(tr.TransformPoint(partJoint.hingeAnchor), tr.TransformDirection(partJoint.hingeAxis).normalized * 0.2f);
				Gizmos.DrawWireSphere(tr.TransformPoint(partJoint.hingeAnchor), 0.02f);
			}
		}
	}
}
