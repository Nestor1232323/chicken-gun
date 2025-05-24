using System;
using UnityEngine;

[Serializable]
public class PartJoint
{
	public Vector3 hingeAnchor;

	public Vector3 hingeAxis = Vector3.right;

	public bool useLimits;

	public float minLimit;

	public float maxLimit;

	public float bounciness;

	public bool useSpring;

	public float springTargetPosition;

	public float springForce;

	public float springDamper;
}
