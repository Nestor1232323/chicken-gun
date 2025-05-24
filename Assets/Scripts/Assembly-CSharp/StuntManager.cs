using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Stunt/Stunt Manager", 0)]
public class StuntManager : MonoBehaviour
{
	public float driftScoreRate;

	public static float driftScoreRateStatic;

	[Tooltip("Maximum time gap between connected drifts")]
	public float driftConnectDelay;

	public static float driftConnectDelayStatic;

	public float driftBoostAdd;

	public static float driftBoostAddStatic;

	public float jumpScoreRate;

	public static float jumpScoreRateStatic;

	public float jumpBoostAdd;

	public static float jumpBoostAddStatic;

	public Stunt[] stunts;

	public static Stunt[] stuntsStatic;

	private void Start()
	{
		driftScoreRateStatic = driftScoreRate;
		driftConnectDelayStatic = driftConnectDelay;
		driftBoostAddStatic = driftBoostAdd;
		jumpScoreRateStatic = jumpScoreRate;
		jumpBoostAddStatic = jumpBoostAdd;
		stuntsStatic = stunts;
	}
}
