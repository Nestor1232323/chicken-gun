using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Damage/Shatter Part", 2)]
public class ShatterPart : MonoBehaviour
{
	[NonSerialized]
	public Renderer rend;

	[NonSerialized]
	public bool shattered;

	public float breakForce = 5f;

	[Tooltip("Transform used for maintaining seams when deformed after shattering")]
	public Transform seamKeeper;

	[NonSerialized]
	public Material initialMat;

	public Material brokenMaterial;

	public ParticleSystem shatterParticles;

	public AudioSource shatterSnd;

	private void Start()
	{
		rend = GetComponent<Renderer>();
		if ((bool)rend)
		{
			initialMat = rend.sharedMaterial;
		}
	}

	public void Shatter()
	{
		if (!shattered)
		{
			shattered = true;
			if ((bool)shatterParticles)
			{
				shatterParticles.Play();
			}
			if ((bool)brokenMaterial)
			{
				rend.sharedMaterial = brokenMaterial;
			}
			else
			{
				rend.enabled = false;
			}
			if ((bool)shatterSnd)
			{
				shatterSnd.Play();
			}
		}
	}
}
