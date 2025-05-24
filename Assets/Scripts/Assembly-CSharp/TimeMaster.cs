using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Scene Controllers/Time Master", 1)]
public class TimeMaster : MonoBehaviour
{
	private float initialFixedTime;

	[Tooltip("Master audio mixer")]
	public AudioMixer masterMixer;

	public bool destroyOnLoad;

	public static float fixedTimeFactor;

	public static float inverseFixedTimeFactor;

	private void Awake()
	{
		initialFixedTime = Time.fixedDeltaTime;
		if (!destroyOnLoad)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void Update()
	{
		if ((bool)masterMixer)
		{
			masterMixer.SetFloat("MasterPitch", Time.timeScale);
		}
	}

	private void FixedUpdate()
	{
		Time.fixedDeltaTime = Time.timeScale * initialFixedTime;
		fixedTimeFactor = 0.01f / initialFixedTime;
		inverseFixedTimeFactor = 1f / fixedTimeFactor;
	}
}
