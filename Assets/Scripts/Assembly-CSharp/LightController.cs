using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Effects/Light Controller", 2)]
public class LightController : MonoBehaviour
{
	private VehicleParent vp;

	public bool headlightsOn;

	public bool highBeams;

	public bool brakelightsOn;

	public bool rightBlinkersOn;

	public bool leftBlinkersOn;

	public float blinkerInterval = 0.3f;

	private bool blinkerIntervalOn;

	private float blinkerSwitchTime;

	public bool reverseLightsOn;

	public Transmission transmission;

	private GearboxTransmission gearTrans;

	private ContinuousTransmission conTrans;

	public VehicleLight[] headlights;

	public VehicleLight[] brakeLights;

	public VehicleLight[] RightBlinkers;

	public VehicleLight[] LeftBlinkers;

	public VehicleLight[] ReverseLights;

	private void Start()
	{
		vp = GetComponent<VehicleParent>();
		if ((bool)transmission)
		{
			if (transmission is GearboxTransmission)
			{
				gearTrans = transmission as GearboxTransmission;
			}
			else if (transmission is ContinuousTransmission)
			{
				conTrans = transmission as ContinuousTransmission;
			}
		}
	}

	private void Update()
	{
		if (leftBlinkersOn || rightBlinkersOn)
		{
			if (blinkerSwitchTime == 0f)
			{
				blinkerIntervalOn = !blinkerIntervalOn;
				blinkerSwitchTime = blinkerInterval;
			}
			else
			{
				blinkerSwitchTime = Mathf.Max(0f, blinkerSwitchTime - Time.deltaTime);
			}
		}
		else
		{
			blinkerIntervalOn = false;
			blinkerSwitchTime = 0f;
		}
		if ((bool)gearTrans)
		{
			reverseLightsOn = gearTrans.curGearRatio < 0f;
		}
		else if ((bool)conTrans)
		{
			reverseLightsOn = conTrans.reversing;
		}
		if (vp.accelAxisIsBrake)
		{
			brakelightsOn = vp.accelInput != 0f && Mathf.Sign(vp.accelInput) != Mathf.Sign(vp.localVelocity.z) && Mathf.Abs(vp.localVelocity.z) > 1f;
		}
		else if (!vp.brakeIsReverse)
		{
			brakelightsOn = (vp.burnout > 0f && vp.brakeInput > 0f) || vp.brakeInput > 0f;
		}
		else
		{
			brakelightsOn = (vp.burnout > 0f && vp.brakeInput > 0f) || (vp.brakeInput > 0f && vp.localVelocity.z > 1f) || (vp.accelInput > 0f && vp.localVelocity.z < -1f);
		}
		SetLights(headlights, highBeams, headlightsOn);
		SetLights(brakeLights, headlightsOn || highBeams, brakelightsOn);
		SetLights(RightBlinkers, rightBlinkersOn && blinkerIntervalOn);
		SetLights(LeftBlinkers, leftBlinkersOn && blinkerIntervalOn);
		SetLights(ReverseLights, reverseLightsOn);
	}

	private void SetLights(VehicleLight[] lights, bool condition)
	{
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].on = condition;
		}
	}

	private void SetLights(VehicleLight[] lights, bool condition, bool halfCondition)
	{
		foreach (VehicleLight obj in lights)
		{
			obj.on = condition;
			obj.halfOn = halfCondition;
		}
	}
}
