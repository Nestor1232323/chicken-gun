using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Stunt/Stunt Detector", 1)]
public class StuntDetect : MonoBehaviour
{
	private Transform tr;

	private Rigidbody rb;

	private VehicleParent vp;

	[NonSerialized]
	public float score;

	private List<Stunt> stunts = new List<Stunt>();

	private List<Stunt> doneStunts = new List<Stunt>();

	private bool drifting;

	private float driftDist;

	private float driftScore;

	private float endDriftTime;

	private float jumpDist;

	private float jumpTime;

	private Vector3 jumpStart;

	public bool detectDrift = true;

	public bool detectJump = true;

	public bool detectFlips = true;

	private string driftString;

	private string jumpString;

	private string flipString;

	[NonSerialized]
	public string stuntString;

	public Motor engine;

	private void Start()
	{
		tr = base.transform;
		rb = GetComponent<Rigidbody>();
		vp = GetComponent<VehicleParent>();
	}

	private void FixedUpdate()
	{
		if (detectDrift && !vp.crashing)
		{
			DetectDrift();
		}
		else
		{
			drifting = false;
			driftDist = 0f;
			driftScore = 0f;
			driftString = string.Empty;
		}
		if (detectJump && !vp.crashing)
		{
			DetectJump();
		}
		else
		{
			jumpTime = 0f;
			jumpDist = 0f;
			jumpString = string.Empty;
		}
		if (detectFlips && !vp.crashing)
		{
			DetectFlips();
		}
		else
		{
			stunts.Clear();
			flipString = string.Empty;
		}
		stuntString = ((!vp.crashing) ? (driftString + jumpString + ((!string.IsNullOrEmpty(flipString) && !string.IsNullOrEmpty(jumpString)) ? " + " : string.Empty) + flipString) : "Crashed");
	}

	private void DetectDrift()
	{
		endDriftTime = ((vp.groundedWheels <= 0) ? 0f : ((!(Mathf.Abs(vp.localVelocity.x) > 5f)) ? Mathf.Max(0f, endDriftTime - Time.timeScale * TimeMaster.inverseFixedTimeFactor) : StuntManager.driftConnectDelayStatic));
		drifting = endDriftTime > 0f;
		if (drifting)
		{
			driftScore += StuntManager.driftScoreRateStatic * Mathf.Abs(vp.localVelocity.x) * Time.timeScale * TimeMaster.inverseFixedTimeFactor;
			driftDist += vp.velMag * Time.fixedDeltaTime;
			driftString = "Drift: " + driftDist.ToString("n0") + " m";
			if ((bool)engine)
			{
				engine.boost += StuntManager.driftBoostAddStatic * Mathf.Abs(vp.localVelocity.x) * Time.timeScale * 0.0002f * TimeMaster.inverseFixedTimeFactor;
			}
		}
		else
		{
			score += driftScore;
			driftDist = 0f;
			driftScore = 0f;
			driftString = string.Empty;
		}
	}

	private void DetectJump()
	{
		if (vp.groundedWheels == 0)
		{
			jumpDist = Vector3.Distance(jumpStart, tr.position);
			jumpTime += Time.fixedDeltaTime;
			jumpString = "Jump: " + jumpDist.ToString("n0") + " m";
			if ((bool)engine)
			{
				engine.boost += StuntManager.jumpBoostAddStatic * Time.timeScale * 0.01f * TimeMaster.inverseFixedTimeFactor;
			}
			return;
		}
		score += (jumpDist + jumpTime) * StuntManager.jumpScoreRateStatic;
		if ((bool)engine)
		{
			engine.boost += (jumpDist + jumpTime) * StuntManager.jumpBoostAddStatic * Time.timeScale * 0.01f * TimeMaster.inverseFixedTimeFactor;
		}
		jumpStart = tr.position;
		jumpDist = 0f;
		jumpTime = 0f;
		jumpString = string.Empty;
	}

	private void DetectFlips()
	{
		if (vp.groundedWheels == 0)
		{
			Stunt[] stuntsStatic = StuntManager.stuntsStatic;
			foreach (Stunt stunt in stuntsStatic)
			{
				if (!(Vector3.Dot(vp.localAngularVel.normalized, stunt.rotationAxis) >= stunt.precision))
				{
					continue;
				}
				bool flag = false;
				foreach (Stunt stunt2 in stunts)
				{
					if (stunt.name == stunt2.name)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					stunts.Add(new Stunt(stunt));
				}
			}
			foreach (Stunt stunt3 in stunts)
			{
				if (Vector3.Dot(vp.localAngularVel.normalized, stunt3.rotationAxis) >= stunt3.precision)
				{
					stunt3.progress += rb.angularVelocity.magnitude * Time.fixedDeltaTime;
				}
				if (!(stunt3.progress * 57.29578f >= stunt3.angleThreshold))
				{
					continue;
				}
				bool flag2 = false;
				foreach (Stunt doneStunt in doneStunts)
				{
					if (doneStunt == stunt3)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					doneStunts.Add(stunt3);
				}
			}
			string empty = string.Empty;
			flipString = string.Empty;
			{
				foreach (Stunt doneStunt2 in doneStunts)
				{
					empty = ((!(doneStunt2.progress * 57.29578f >= doneStunt2.angleThreshold * 2f)) ? string.Empty : (" x" + Mathf.FloorToInt(doneStunt2.progress * 57.29578f / doneStunt2.angleThreshold)));
					flipString = ((!string.IsNullOrEmpty(flipString)) ? (flipString + " + " + doneStunt2.name + empty) : (doneStunt2.name + empty));
				}
				return;
			}
		}
		foreach (Stunt stunt4 in stunts)
		{
			score += stunt4.progress * 57.29578f * stunt4.scoreRate * (float)Mathf.FloorToInt(stunt4.progress * 57.29578f / stunt4.angleThreshold) * stunt4.multiplier;
			if ((bool)engine)
			{
				engine.boost += stunt4.progress * 57.29578f * stunt4.boostAdd * stunt4.multiplier * 0.01f;
			}
		}
		stunts.Clear();
		doneStunts.Clear();
		flipString = string.Empty;
	}
}
