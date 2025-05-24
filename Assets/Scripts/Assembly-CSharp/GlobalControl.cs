using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Scene Controllers/Global Control", 0)]
public class GlobalControl : MonoBehaviour
{
	[Tooltip("Reload the scene with the 'Restart' button in the input manager")]
	public bool quickRestart = true;

	[Tooltip("Mask for what the wheels collide with")]
	public LayerMask wheelCastMask;

	public static LayerMask wheelCastMaskStatic;

	[Tooltip("Mask for objects which vehicles check against if they are rolled over")]
	public LayerMask groundMask;

	public static LayerMask groundMaskStatic;

	[Tooltip("Mask for objects that cause damage to vehicles")]
	public LayerMask damageMask;

	public static LayerMask damageMaskStatic;

	public static int ignoreWheelCastLayer;

	[Tooltip("Frictionless physic material")]
	public PhysicMaterial frictionlessMat;

	public static PhysicMaterial frictionlessMatStatic;

	public static Vector3 worldUpDir;

	[Tooltip("Maximum segments per tire mark")]
	public int tireMarkLength;

	public static int tireMarkLengthStatic;

	[Tooltip("Gap between tire mark segments")]
	public float tireMarkGap;

	public static float tireMarkGapStatic;

	[Tooltip("Tire mark height above ground")]
	public float tireMarkHeight;

	public static float tireMarkHeightStatic;

	[Tooltip("Lifetime of tire marks")]
	public float tireFadeTime;

	public static float tireFadeTimeStatic;

	private void Start()
	{
		wheelCastMaskStatic = wheelCastMask;
		groundMaskStatic = groundMask;
		damageMaskStatic = damageMask;
		ignoreWheelCastLayer = LayerMask.NameToLayer("Ignore Wheel Cast");
		frictionlessMatStatic = frictionlessMat;
		tireMarkLengthStatic = Mathf.Max(tireMarkLength, 2);
		tireMarkGapStatic = tireMarkGap;
		tireMarkHeightStatic = tireMarkHeight;
		tireFadeTimeStatic = tireFadeTime;
	}

	private void FixedUpdate()
	{
		worldUpDir = ((Physics.gravity.sqrMagnitude != 0f) ? (-Physics.gravity.normalized) : Vector3.up);
	}
}
