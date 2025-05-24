using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Effects/Tire Screech Audio", 1)]
public class TireScreech : MonoBehaviour
{
	private AudioSource snd;

	private VehicleParent vp;

	private Wheel[] wheels;

	private float slipThreshold;

	private GroundSurface surfaceType;

	private void Start()
	{
		snd = GetComponent<AudioSource>();
		vp = (VehicleParent)F.GetTopmostParentComponent<VehicleParent>(base.transform);
		wheels = new Wheel[vp.wheels.Length];
		for (int i = 0; i < vp.wheels.Length; i++)
		{
			wheels[i] = vp.wheels[i];
			if ((bool)vp.wheels[i].GetComponent<TireMarkCreate>())
			{
				float num = vp.wheels[i].GetComponent<TireMarkCreate>().slipThreshold;
				slipThreshold = ((i != 0) ? ((slipThreshold + num) * 0.5f) : num);
			}
		}
	}

	private void Update()
	{
		float num = 0f;
		bool flag = true;
		bool flag2 = true;
		float num2 = 0f;
		for (int i = 0; i < vp.wheels.Length; i++)
		{
			if (!wheels[i].connected)
			{
				continue;
			}
			if (Mathf.Abs(F.MaxAbs(wheels[i].sidewaysSlip, wheels[i].forwardSlip, num2)) - slipThreshold > 0f)
			{
				if (wheels[i].popped)
				{
					flag2 = false;
				}
				else
				{
					flag = false;
				}
			}
			if (wheels[i].grounded)
			{
				surfaceType = GroundSurfaceMaster.surfaceTypesStatic[wheels[i].contactPoint.surfaceType];
				if (surfaceType.alwaysScrape)
				{
					num2 = slipThreshold + Mathf.Min(0.5f, Mathf.Abs(wheels[i].rawRPM * 0.001f));
				}
			}
			num = Mathf.Max(num, Mathf.Pow(Mathf.Clamp01(Mathf.Abs(F.MaxAbs(wheels[i].sidewaysSlip, wheels[i].forwardSlip, num2)) - slipThreshold), 2f));
		}
		if (surfaceType != null)
		{
			snd.clip = (flag ? surfaceType.rimSnd : ((!flag2) ? surfaceType.tireRimSnd : surfaceType.tireSnd));
		}
		if (num > 0f)
		{
			if (!snd.isPlaying)
			{
				snd.Play();
				snd.volume = 0f;
			}
			else
			{
				snd.volume = Mathf.Lerp(snd.volume, num * ((float)vp.groundedWheels * 1f / ((float)wheels.Length * 1f)), 2f * Time.deltaTime);
				snd.pitch = Mathf.Lerp(snd.pitch, 0.5f + num * 0.9f, 2f * Time.deltaTime);
			}
		}
		else if (snd.isPlaying)
		{
			snd.volume = 0f;
			snd.Stop();
		}
	}
}
