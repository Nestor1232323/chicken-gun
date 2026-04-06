using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Hover/Hover Motor", 0)]
public class HoverMotor : Motor
{
	[Header("Performance")]
	[Tooltip("Curve which calculates the driving force based on the speed of the vehicle, x-axis = speed, y-axis = force")]
	public AnimationCurve forceCurve = AnimationCurve.EaseInOut(0f, 1f, 50f, 0f);

	public HoverWheel[] wheels;

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		float f = ((!vp.brakeIsReverse) ? vp.accelInput : (vp.accelInput - vp.brakeInput));
		actualInput = inputCurve.Evaluate(Mathf.Abs(f)) * Mathf.Sign(f);
		HoverWheel[] array = wheels;
		foreach (HoverWheel hoverWheel in array)
		{
			if (ignition)
			{
				float num = boostPowerCurve.Evaluate(Mathf.Abs(vp.localVelocity.z));
				hoverWheel.targetSpeed = actualInput * forceCurve.keys[forceCurve.keys.Length - 1].time * ((!boosting) ? 1f : (1f + num));
				hoverWheel.targetForce = Mathf.Abs(actualInput) * forceCurve.Evaluate(Mathf.Abs(vp.localVelocity.z) - ((!boosting) ? 0f : num)) * power * ((!boosting) ? 1f : (1f + num)) * health;
			}
			else
			{
				hoverWheel.targetSpeed = 0f;
				hoverWheel.targetForce = 0f;
			}
			hoverWheel.doFloat = ignition && health > 0f;
		}
	}

	public override void Update()
	{
		if ((bool)snd && ignition)
		{
			targetPitch = Mathf.Max(Mathf.Abs(actualInput), Mathf.Abs(vp.steerInput) * 0.5f) * (1f - forceCurve.Evaluate(Mathf.Abs(vp.localVelocity.z)));
		}
		base.Update();
	}
}
