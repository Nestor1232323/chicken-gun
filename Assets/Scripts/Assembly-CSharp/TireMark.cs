using System;
using UnityEngine;

public class TireMark : MonoBehaviour
{
	[NonSerialized]
	public float fadeTime = -1f;

	private bool fading;

	private float alpha = 1f;

	[NonSerialized]
	public Mesh mesh;

	[NonSerialized]
	public Color[] colors;

	private void Update()
	{
		if (fading)
		{
			if (alpha <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			alpha -= Time.deltaTime;
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i].a -= Time.deltaTime;
			}
			mesh.colors = colors;
		}
		else if (fadeTime > 0f)
		{
			fadeTime = Mathf.Max(0f, fadeTime - Time.deltaTime);
		}
		else if (fadeTime == 0f)
		{
			fading = true;
		}
	}
}
