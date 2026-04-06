using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Effects/Vehicle Light", 3)]
public class VehicleLight : MonoBehaviour
{
	private Renderer rend;

	private ShatterPart shatter;

	public bool on;

	[Tooltip("Example: a brake light would be half on when the night lights are on, and fully on when the brakes are pressed")]
	public bool halfOn;

	public Light targetLight;

	[Tooltip("A light shared with another vehicle light, will turn off if one of the lights break, then the unbroken light will turn on its target light")]
	public Light sharedLight;

	[Tooltip("Vehicle light that the shared light is shared with")]
	public VehicleLight sharer;

	public Material onMaterial;

	private Material offMaterial;

	[NonSerialized]
	public bool shattered;

	private void Start()
	{
		rend = GetComponent<Renderer>();
		if ((bool)rend)
		{
			offMaterial = rend.sharedMaterial;
		}
		shatter = GetComponent<ShatterPart>();
	}

	private void Update()
	{
		if ((bool)shatter)
		{
			shattered = shatter.shattered;
		}
		if ((bool)sharedLight && (bool)sharer)
		{
			sharedLight.enabled = on && sharer.on && !shattered && !sharer.shattered;
		}
		if ((bool)targetLight && (bool)sharedLight && (bool)sharer)
		{
			targetLight.enabled = !shattered && on && !sharedLight.enabled;
		}
		if (!rend)
		{
			return;
		}
		if (shattered)
		{
			if ((bool)shatter.brokenMaterial)
			{
				rend.sharedMaterial = shatter.brokenMaterial;
			}
			else
			{
				rend.sharedMaterial = ((!on && !halfOn) ? offMaterial : onMaterial);
			}
		}
		else
		{
			rend.sharedMaterial = ((!on && !halfOn) ? offMaterial : onMaterial);
		}
	}
}
