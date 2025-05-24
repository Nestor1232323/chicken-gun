using System;
using UnityEngine;

public abstract class Motor : MonoBehaviour
{
	protected VehicleParent vp;

	public bool ignition;

	public float power = 1f;

	[Tooltip("Throttle curve, x-axis = input, y-axis = output")]
	public AnimationCurve inputCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	protected float actualInput;

	protected AudioSource snd;

	[Header("Engine Audio")]
	public float minPitch;

	public float maxPitch;

	[NonSerialized]
	public float targetPitch;

	protected float pitchFactor;

	protected float airPitch;

	[Header("Nitrous Boost")]
	public bool canBoost = true;

	[NonSerialized]
	public bool boosting;

	public float boost = 1f;

	private bool boostReleased;

	private bool boostPrev;

	[Tooltip("X-axis = local z-velocity, y-axis = power")]
	public AnimationCurve boostPowerCurve = AnimationCurve.EaseInOut(0f, 0.1f, 50f, 0.2f);

	public float maxBoost = 1f;

	public float boostBurnRate = 0.01f;

	public AudioSource boostLoopSnd;

	private AudioSource boostSnd;

	public AudioClip boostStart;

	public AudioClip boostEnd;

	public ParticleSystem[] boostParticles;

	[Header("Damage")]
	[Range(0f, 1f)]
	public float strength = 1f;

	[NonSerialized]
	public float health = 1f;

	public float damagePitchWiggle;

	public ParticleSystem smoke;

	private float initialSmokeEmission;

	public virtual void Start()
	{
		vp = (VehicleParent)F.GetTopmostParentComponent<VehicleParent>(base.transform);
		snd = GetComponent<AudioSource>();
		if ((bool)snd)
		{
			snd.pitch = minPitch;
		}
		if ((bool)boostLoopSnd)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(boostLoopSnd.gameObject, boostLoopSnd.transform.position, boostLoopSnd.transform.rotation);
			boostSnd = gameObject.GetComponent<AudioSource>();
			boostSnd.transform.parent = boostLoopSnd.transform;
			boostSnd.transform.localPosition = Vector3.zero;
			boostSnd.transform.localRotation = Quaternion.identity;
			boostSnd.loop = false;
		}
		if ((bool)smoke)
		{
			initialSmokeEmission = smoke.emission.rateOverTime.constantMax;
		}
	}

	public virtual void FixedUpdate()
	{
		health = Mathf.Clamp01(health);
		boost = Mathf.Clamp((!boosting) ? boost : (boost - boostBurnRate * Time.timeScale * 0.05f * TimeMaster.inverseFixedTimeFactor), 0f, maxBoost);
		boostPrev = boosting;
		if (!canBoost || !ignition || !(health > 0f) || vp.crashing || !(boost > 0f))
		{
			goto IL_01f1;
		}
		bool flag;
		if (vp.hover)
		{
			if (vp.accelInput == 0f)
			{
				flag = Mathf.Abs(vp.localVelocity.z) > 1f;
				goto IL_0116;
			}
		}
		else if (!(vp.accelInput > 0f))
		{
			flag = vp.localVelocity.z > 1f;
			goto IL_0116;
		}
		goto IL_01fd;
		IL_0116:
		if (!flag)
		{
			goto IL_01f1;
		}
		goto IL_01fd;
		IL_0121:
		if (!vp.boostButton)
		{
			boostReleased = true;
		}
		if ((bool)boostLoopSnd && (bool)boostSnd)
		{
			if (boosting && !boostLoopSnd.isPlaying)
			{
				boostLoopSnd.Play();
			}
			else if (!boosting && boostLoopSnd.isPlaying)
			{
				boostLoopSnd.Stop();
			}
			if (boosting && !boostPrev)
			{
				boostSnd.clip = boostStart;
				boostSnd.Play();
			}
			else if (!boosting && boostPrev)
			{
				boostSnd.clip = boostEnd;
				boostSnd.Play();
			}
		}
		return;
		IL_01f1:
		boosting = false;
		goto IL_0121;
		IL_01fd:
		if (((boostReleased && !boosting) || boosting) && vp.boostButton)
		{
			boosting = true;
			boostReleased = false;
		}
		else
		{
			boosting = false;
		}
		goto IL_0121;
	}

	public virtual void Update()
	{
		if (!ignition)
		{
			targetPitch = 0f;
		}
		if ((bool)snd)
		{
			if (ignition && health > 0f)
			{
				snd.enabled = true;
				snd.pitch = Mathf.Lerp(snd.pitch, Mathf.Lerp(minPitch, maxPitch, targetPitch), 20f * Time.deltaTime) + Mathf.Sin(Time.time * 200f * (1f - health)) * (1f - health) * 0.1f * damagePitchWiggle;
				snd.volume = Mathf.Lerp(snd.volume, 0.3f + targetPitch * 0.7f, 20f * Time.deltaTime);
			}
			else
			{
				snd.enabled = false;
			}
		}
		if (boostParticles.Length != 0)
		{
			ParticleSystem[] array = boostParticles;
			foreach (ParticleSystem particleSystem in array)
			{
				if (boosting && particleSystem.isStopped)
				{
					particleSystem.Play();
				}
				else if (!boosting && particleSystem.isPlaying)
				{
					particleSystem.Stop();
				}
			}
		}
		if ((bool)smoke)
		{
			ParticleSystem.EmissionModule emission = smoke.emission;
			emission.rateOverTime = new ParticleSystem.MinMaxCurve((!(health < 0.7f)) ? 0f : (initialSmokeEmission * (1f - health)));
		}
	}
}
