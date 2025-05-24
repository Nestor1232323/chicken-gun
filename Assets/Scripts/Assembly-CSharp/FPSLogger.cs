using Photon;
using UnityEngine;

public class FPSLogger : Photon.MonoBehaviour
{
	public float FpsUpdateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeLeft;

	private int currnetFPS;

	private int minFPS = 1000;

	private int maxFPS;

	private void Start()
	{
		if (StorageController.instance != null)
		{
			base.enabled = StorageController.instance.isDevelopmentBuild;
		}
	}

	private void Update()
	{
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0f)
		{
			currnetFPS = (int)(accum * 1f / (float)frames);
			if (minFPS > currnetFPS)
			{
				minFPS = currnetFPS;
			}
			if (maxFPS < currnetFPS)
			{
				maxFPS = currnetFPS;
			}
			accum = 0f;
			frames = 0;
			timeLeft = FpsUpdateInterval;
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical(GUILayout.Width(230f));
		GUILayout.Space((float)Screen.height * 0.525f);
		GUIStyle gUIStyle = new GUIStyle("button");
		gUIStyle.fontSize = 45;
		GUILayout.Label("F: " + currnetFPS, gUIStyle);
		GUILayout.Label("P: " + PhotonNetwork.GetPing(), gUIStyle);
		GUILayout.EndVertical();
	}
}
