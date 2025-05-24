using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Damage/Vehicle Damage", 0)]
public class VehicleDamage : MonoBehaviour
{
	private Transform tr;

	private Rigidbody rb;

	private VehicleParent vp;

	[Range(0f, 1f)]
	public float strength = 1f;

	public float damageFactor = 1f;

	public float maxCollisionMagnitude = 100f;

	[Tooltip("Maximum collision points to use when deforming, has large effect on performance")]
	public int maxCollisionPoints = 2;

	[Tooltip("Collisions underneath this local y-position will be ignored")]
	public float collisionIgnoreHeight;

	[Tooltip("If true, grounded wheels will not be damaged, but can still be displaced")]
	public bool ignoreGroundedWheels;

	[Tooltip("Minimum time in seconds between collisions")]
	public float collisionTimeGap = 0.1f;

	private float hitTime;

	[Tooltip("Whether the edges of adjacent deforming parts should match")]
	public bool seamlessDeform;

	[Tooltip("Add some perlin noise to deformation")]
	public bool usePerlinNoise = true;

	[Tooltip("Recalculate normals of deformed meshes")]
	public bool calculateNormals = true;

	[Tooltip("Parts that are damaged")]
	public Transform[] damageParts;

	[Tooltip("Meshes that are deformed")]
	public MeshFilter[] deformMeshes;

	private bool[] damagedMeshes;

	private Mesh[] tempMeshes;

	private meshVerts[] meshVertices;

	[Tooltip("Mesh colliders that are deformed (Poor performance, must be convex)")]
	public MeshCollider[] deformColliders;

	private bool[] damagedCols;

	private Mesh[] tempCols;

	private meshVerts[] colVertices;

	[Tooltip("Parts that are displaced")]
	public Transform[] displaceParts;

	private Vector3[] initialPartPositions;

	private ContactPoint nullContact;

	private void Start()
	{
		tr = base.transform;
		rb = GetComponent<Rigidbody>();
		vp = GetComponent<VehicleParent>();
		vp.playCrashSounds = false;
		vp.playCrashSparks = false;
		tempMeshes = new Mesh[deformMeshes.Length];
		damagedMeshes = new bool[deformMeshes.Length];
		meshVertices = new meshVerts[deformMeshes.Length];
		for (int i = 0; i < deformMeshes.Length; i++)
		{
			tempMeshes[i] = deformMeshes[i].mesh;
			meshVertices[i] = new meshVerts();
			meshVertices[i].verts = deformMeshes[i].mesh.vertices;
			meshVertices[i].initialVerts = deformMeshes[i].mesh.vertices;
			damagedMeshes[i] = false;
		}
		tempCols = new Mesh[deformColliders.Length];
		damagedCols = new bool[deformColliders.Length];
		colVertices = new meshVerts[deformColliders.Length];
		for (int j = 0; j < deformColliders.Length; j++)
		{
			tempCols[j] = Object.Instantiate(deformColliders[j].sharedMesh);
			colVertices[j] = new meshVerts();
			colVertices[j].verts = deformColliders[j].sharedMesh.vertices;
			colVertices[j].initialVerts = deformColliders[j].sharedMesh.vertices;
			damagedCols[j] = false;
		}
		initialPartPositions = new Vector3[displaceParts.Length];
		for (int k = 0; k < displaceParts.Length; k++)
		{
			initialPartPositions[k] = displaceParts[k].localPosition;
		}
	}

	private void FixedUpdate()
	{
		hitTime = Mathf.Max(0f, hitTime - Time.fixedDeltaTime);
		damageFactor = Mathf.Max(0f, damageFactor);
	}

	private void OnCollisionEnter(Collision col)
	{
		if (hitTime != 0f || !(col.relativeVelocity.sqrMagnitude * damageFactor > 1f) || !(strength < 1f))
		{
			return;
		}
		Vector3 normalized = col.relativeVelocity.normalized;
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		hitTime = collisionTimeGap;
		ContactPoint[] contacts = col.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint colPoint = contacts[i];
			if (tr.InverseTransformPoint(colPoint.point).y > collisionIgnoreHeight && (int)GlobalControl.damageMaskStatic == ((int)GlobalControl.damageMaskStatic | (1 << colPoint.otherCollider.gameObject.layer)))
			{
				num++;
				if ((bool)vp.crashSnd && vp.crashClips.Length != 0 && !flag)
				{
					vp.crashSnd.PlayOneShot(vp.crashClips[Random.Range(0, vp.crashClips.Length)], Mathf.Clamp01(col.relativeVelocity.magnitude * 0.1f));
					flag = true;
				}
				if ((bool)vp.sparks && !flag2)
				{
					vp.sparks.transform.position = colPoint.point;
					vp.sparks.transform.rotation = Quaternion.LookRotation(normalized, colPoint.normal);
					vp.sparks.Play();
					flag2 = true;
				}
				DamageApplication(colPoint.point, col.relativeVelocity, maxCollisionMagnitude, colPoint.normal, colPoint, useContactPoint: true);
			}
			if (num >= maxCollisionPoints)
			{
				break;
			}
		}
		FinalizeDamage();
	}

	public void ApplyDamage(ContactPoint colPoint, Vector3 colVel)
	{
		DamageApplication(colPoint.point, colVel, float.PositiveInfinity, colPoint.normal, colPoint, useContactPoint: true);
		FinalizeDamage();
	}

	public void ApplyDamage(ContactPoint colPoint, Vector3 colVel, float damageForceLimit)
	{
		DamageApplication(colPoint.point, colVel, damageForceLimit, colPoint.normal, colPoint, useContactPoint: true);
		FinalizeDamage();
	}

	public void ApplyDamage(Vector3 damagePoint, Vector3 damageForce)
	{
		DamageApplication(damagePoint, damageForce, float.PositiveInfinity, damageForce.normalized, nullContact, useContactPoint: false);
		FinalizeDamage();
	}

	public void ApplyDamage(Vector3 damagePoint, Vector3 damageForce, float damageForceLimit)
	{
		DamageApplication(damagePoint, damageForce, damageForceLimit, damageForce.normalized, nullContact, useContactPoint: false);
		FinalizeDamage();
	}

	public void ApplyDamage(Vector3[] damagePoints, Vector3 damageForce)
	{
		foreach (Vector3 damagePoint in damagePoints)
		{
			DamageApplication(damagePoint, damageForce, float.PositiveInfinity, damageForce.normalized, nullContact, useContactPoint: false);
		}
		FinalizeDamage();
	}

	public void ApplyDamage(Vector3[] damagePoints, Vector3 damageForce, float damageForceLimit)
	{
		foreach (Vector3 damagePoint in damagePoints)
		{
			DamageApplication(damagePoint, damageForce, damageForceLimit, damageForce.normalized, nullContact, useContactPoint: false);
		}
		FinalizeDamage();
	}

	private void DamageApplication(Vector3 damagePoint, Vector3 damageForce, float damageForceLimit, Vector3 surfaceNormal, ContactPoint colPoint, bool useContactPoint)
	{
		float num = Mathf.Min(damageForce.magnitude, maxCollisionMagnitude) * (1f - strength) * damageFactor;
		float num2 = Mathf.Pow(Mathf.Sqrt(num) * 0.5f, 1.5f);
		Vector3 vector = Vector3.ClampMagnitude(damageForce, damageForceLimit);
		Vector3 normalized = damageForce.normalized;
		float num3 = 1f;
		Transform transform = null;
		if (useContactPoint)
		{
			damagePoint = colPoint.point;
			surfaceNormal = colPoint.normal;
			if ((bool)colPoint.otherCollider.attachedRigidbody)
			{
				num3 = Mathf.Clamp01(colPoint.otherCollider.attachedRigidbody.mass / rb.mass);
			}
		}
		float num4 = Mathf.Clamp01(Vector3.Dot(surfaceNormal, normalized)) * (Vector3.Dot((tr.position - damagePoint).normalized, normalized) + 1f) * 0.5f;
		for (int i = 0; i < damageParts.Length; i++)
		{
			Transform transform2 = damageParts[i];
			float num5 = num * num4 * num3 * Mathf.Min(num2 * 0.01f, num2 * 0.001f / Mathf.Pow(Vector3.Distance(transform2.position, damagePoint), num2));
			Motor component = transform2.GetComponent<Motor>();
			if ((bool)component)
			{
				component.health -= num5 * (1f - component.strength);
			}
			Transmission component2 = transform2.GetComponent<Transmission>();
			if ((bool)component2)
			{
				component2.health -= num5 * (1f - component2.strength);
			}
		}
		for (int j = 0; j < deformMeshes.Length; j++)
		{
			MeshFilter meshFilter = deformMeshes[j];
			Vector3 b = meshFilter.transform.InverseTransformPoint(damagePoint);
			Vector3 vector2 = meshFilter.transform.InverseTransformDirection(vector);
			Vector3 vector3 = Vector3.ClampMagnitude(vector2, num2);
			ShatterPart component3 = meshFilter.GetComponent<ShatterPart>();
			if ((bool)component3)
			{
				transform = component3.seamKeeper;
				if (Vector3.Distance(meshFilter.transform.position, damagePoint) < num * num4 * 0.1f * num3 && num * num4 * num3 > component3.breakForce)
				{
					component3.Shatter();
				}
			}
			if (!(vector2.sqrMagnitude > 0f) || !(strength < 1f))
			{
				continue;
			}
			for (int k = 0; k < meshVertices[j].verts.Length; k++)
			{
				float f = Vector3.Distance(meshVertices[j].verts[k], b);
				float num6 = num2 * 0.001f / Mathf.Pow(f, num2);
				if (num6 > 0.001f)
				{
					damagedMeshes[j] = true;
					if (transform == null || seamlessDeform)
					{
						Vector3 vector4 = ((!seamlessDeform) ? Vector3.Project(normalized, meshVertices[j].verts[k]) : Vector3.zero);
						meshVertices[j].verts[k] += (vector3 - vector4 * ((!usePerlinNoise) ? 1f : (1f + Mathf.PerlinNoise(meshVertices[j].verts[k].x * 100f, meshVertices[j].verts[k].y * 100f)))) * num4 * Mathf.Min(num2 * 0.01f, num6) * num3;
					}
					else
					{
						Vector3 onNormal = transform.InverseTransformPoint(meshFilter.transform.TransformPoint(meshVertices[j].verts[k]));
						meshVertices[j].verts[k] += (vector3 - Vector3.Project(normalized, onNormal) * ((!usePerlinNoise) ? 1f : (1f + Mathf.PerlinNoise(onNormal.x * 100f, onNormal.y * 100f)))) * num4 * Mathf.Min(num2 * 0.01f, num6) * num3;
					}
				}
			}
		}
		transform = null;
		for (int l = 0; l < deformColliders.Length; l++)
		{
			Vector3 b2 = deformColliders[l].transform.InverseTransformPoint(damagePoint);
			Vector3 vector5 = deformColliders[l].transform.InverseTransformDirection(vector);
			Vector3 vector6 = Vector3.ClampMagnitude(vector5, num2);
			if (!(vector5.sqrMagnitude > 0f) || !(strength < 1f))
			{
				continue;
			}
			for (int m = 0; m < colVertices[l].verts.Length; m++)
			{
				float f2 = Vector3.Distance(colVertices[l].verts[m], b2);
				float num7 = num2 * 0.001f / Mathf.Pow(f2, num2);
				if (num7 > 0.001f)
				{
					damagedCols[l] = true;
					colVertices[l].verts[m] += vector6 * num4 * Mathf.Min(num2 * 0.01f, num7) * num3;
				}
			}
		}
		for (int n = 0; n < displaceParts.Length; n++)
		{
			Transform transform3 = displaceParts[n];
			Vector3 vector7 = vector;
			Vector3 vector8 = Vector3.ClampMagnitude(vector7, num2);
			if (!(vector7.sqrMagnitude > 0f) || !(strength < 1f))
			{
				continue;
			}
			float f3 = Vector3.Distance(transform3.position, damagePoint);
			float num8 = num2 * 0.001f / Mathf.Pow(f3, num2);
			if (!(num8 > 0.001f))
			{
				continue;
			}
			transform3.position += vector8 * num4 * Mathf.Min(num2 * 0.01f, num8) * num3;
			if ((bool)transform3.GetComponent<DetachablePart>())
			{
				DetachablePart component4 = transform3.GetComponent<DetachablePart>();
				if (num * num4 * num3 > component4.looseForce && component4.looseForce >= 0f)
				{
					component4.initialPos = transform3.localPosition;
					component4.Detach(makeJoint: true);
				}
				else if (num * num4 * num3 > component4.breakForce)
				{
					component4.Detach(makeJoint: false);
				}
			}
			else if ((bool)transform3.parent.GetComponent<DetachablePart>())
			{
				DetachablePart component5 = transform3.parent.GetComponent<DetachablePart>();
				if (!component5.detached)
				{
					if (num * num4 * num3 > component5.looseForce && component5.looseForce >= 0f)
					{
						component5.initialPos = transform3.parent.localPosition;
						component5.Detach(makeJoint: true);
					}
					else if (num * num4 * num3 > component5.breakForce)
					{
						component5.Detach(makeJoint: false);
					}
				}
				else if ((bool)component5.hinge)
				{
					component5.displacedAnchor += transform3.parent.InverseTransformDirection(vector8 * num4 * Mathf.Min(num2 * 0.01f, num8) * num3);
				}
			}
			Suspension component6 = transform3.GetComponent<Suspension>();
			if ((bool)component6 && ((!component6.wheel.grounded && ignoreGroundedWheels) || !ignoreGroundedWheels))
			{
				transform3.RotateAround(component6.tr.TransformPoint(component6.damagePivot), Vector3.ProjectOnPlane(damagePoint - transform3.position, -vector7.normalized), num2 * num4 * num8 * 20f * num3);
				component6.wheel.damage += num2 * num4 * num8 * 10f * num3;
				if (num2 * num4 * num8 * 10f * num3 > component6.jamForce)
				{
					component6.jammed = true;
				}
				if (num2 * num4 * num8 * 10f * num3 > component6.wheel.detachForce)
				{
					component6.wheel.Detach();
				}
				foreach (SuspensionPart movingPart in component6.movingParts)
				{
					if ((bool)movingPart.connectObj && !movingPart.isHub && !movingPart.solidAxle && !movingPart.connectObj.GetComponent<SuspensionPart>())
					{
						movingPart.connectPoint += movingPart.connectObj.InverseTransformDirection(vector8 * num4 * Mathf.Min(num2 * 0.01f, num8) * num3);
					}
				}
			}
			HoverWheel component7 = transform3.GetComponent<HoverWheel>();
			if ((bool)component7 && ((!component7.grounded && ignoreGroundedWheels) || !ignoreGroundedWheels) && num2 * num4 * num8 * 10f * num3 > component7.detachForce)
			{
				component7.Detach();
			}
		}
	}

	private void FinalizeDamage()
	{
		for (int i = 0; i < deformMeshes.Length; i++)
		{
			if (damagedMeshes[i])
			{
				tempMeshes[i].vertices = meshVertices[i].verts;
				if (calculateNormals)
				{
					tempMeshes[i].RecalculateNormals();
				}
				tempMeshes[i].RecalculateBounds();
			}
			damagedMeshes[i] = false;
		}
		for (int j = 0; j < deformColliders.Length; j++)
		{
			if (damagedCols[j])
			{
				tempCols[j].vertices = colVertices[j].verts;
				deformColliders[j].sharedMesh = null;
				deformColliders[j].sharedMesh = tempCols[j];
			}
			damagedCols[j] = false;
		}
	}

	public void Repair()
	{
		for (int i = 0; i < damageParts.Length; i++)
		{
			if ((bool)damageParts[i].GetComponent<Motor>())
			{
				damageParts[i].GetComponent<Motor>().health = 1f;
			}
			if ((bool)damageParts[i].GetComponent<Transmission>())
			{
				damageParts[i].GetComponent<Transmission>().health = 1f;
			}
		}
		for (int j = 0; j < deformMeshes.Length; j++)
		{
			for (int k = 0; k < meshVertices[j].verts.Length; k++)
			{
				meshVertices[j].verts[k] = meshVertices[j].initialVerts[k];
			}
			tempMeshes[j].vertices = meshVertices[j].verts;
			tempMeshes[j].RecalculateNormals();
			tempMeshes[j].RecalculateBounds();
			ShatterPart component = deformMeshes[j].GetComponent<ShatterPart>();
			if ((bool)component)
			{
				component.shattered = false;
				if ((bool)component.brokenMaterial)
				{
					component.rend.sharedMaterial = component.initialMat;
				}
				else
				{
					component.rend.enabled = true;
				}
			}
		}
		for (int l = 0; l < deformColliders.Length; l++)
		{
			for (int m = 0; m < colVertices[l].verts.Length; m++)
			{
				colVertices[l].verts[m] = colVertices[l].initialVerts[m];
			}
			tempCols[l].vertices = colVertices[l].verts;
			deformColliders[l].sharedMesh = null;
			deformColliders[l].sharedMesh = tempCols[l];
		}
		for (int n = 0; n < displaceParts.Length; n++)
		{
			Transform transform = displaceParts[n];
			transform.localPosition = initialPartPositions[n];
			if ((bool)transform.GetComponent<DetachablePart>())
			{
				transform.GetComponent<DetachablePart>().Reattach();
			}
			else if ((bool)transform.parent.GetComponent<DetachablePart>())
			{
				transform.parent.GetComponent<DetachablePart>().Reattach();
			}
			Suspension component2 = transform.GetComponent<Suspension>();
			if (!component2)
			{
				continue;
			}
			transform.localRotation = component2.initialRotation;
			component2.jammed = false;
			foreach (SuspensionPart movingPart in component2.movingParts)
			{
				if ((bool)movingPart.connectObj && !movingPart.isHub && !movingPart.solidAxle && !movingPart.connectObj.GetComponent<SuspensionPart>())
				{
					movingPart.connectPoint = movingPart.initialConnectPoint;
				}
			}
		}
		Wheel[] wheels = vp.wheels;
		foreach (Wheel obj in wheels)
		{
			obj.Reattach();
			obj.FixTire();
			obj.damage = 0f;
		}
		HoverWheel[] hoverWheels = vp.hoverWheels;
		for (int num = 0; num < hoverWheels.Length; num++)
		{
			hoverWheels[num].Reattach();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.TransformPoint(Vector3.up * collisionIgnoreHeight);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(vector, base.transform.forward);
		Gizmos.DrawRay(vector, -base.transform.forward);
		Gizmos.DrawRay(vector, base.transform.right);
		Gizmos.DrawRay(vector, -base.transform.right);
	}

	private void OnDestroy()
	{
		Transform[] array = displaceParts;
		foreach (Transform transform in array)
		{
			if ((bool)transform)
			{
				if ((bool)transform.GetComponent<DetachablePart>() && transform.parent == null)
				{
					Object.Destroy(transform.gameObject);
				}
				else if ((bool)transform.parent.GetComponent<DetachablePart>() && transform.parent.parent == null)
				{
					Object.Destroy(transform.parent.gameObject);
				}
			}
		}
	}
}
