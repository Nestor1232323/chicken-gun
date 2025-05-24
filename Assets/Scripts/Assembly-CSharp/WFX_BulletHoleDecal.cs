using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WFX_BulletHoleDecal : MonoBehaviour
{
	private static Vector2[] quadUVs = new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 0f),
		new Vector2(1f, 1f)
	};

	public float lifetime = 10f;

	public float fadeoutpercent = 80f;

	public Vector2 frames;

	public bool randomRotation;

	public bool deactivate;

	private float life;

	private float fadeout;

	private Color color;

	private float orgAlpha;

	private void Awake()
	{
		color = GetComponent<Renderer>().material.GetColor("_TintColor");
		orgAlpha = color.a;
	}

	private void OnEnable()
	{
		int num = Random.Range(0, (int)(frames.x * frames.y));
		int num2 = (int)((float)num % frames.x);
		int num3 = (int)((float)num / frames.y);
		Vector2[] array = new Vector2[4];
		for (int i = 0; i < 4; i++)
		{
			array[i].x = (quadUVs[i].x + (float)num2) * (1f / frames.x);
			array[i].y = (quadUVs[i].y + (float)num3) * (1f / frames.y);
		}
		GetComponent<MeshFilter>().mesh.uv = array;
		if (randomRotation)
		{
			base.transform.Rotate(0f, 0f, Random.Range(0f, 360f), Space.Self);
		}
		life = lifetime;
		fadeout = life * (fadeoutpercent / 100f);
		color.a = orgAlpha;
		GetComponent<Renderer>().material.SetColor("_TintColor", color);
		StopAllCoroutines();
		StartCoroutine("holeUpdate");
	}

	private IEnumerator holeUpdate()
	{
		while (life > 0f)
		{
			life -= Time.deltaTime;
			if (life <= fadeout)
			{
				color.a = Mathf.Lerp(0f, orgAlpha, life / fadeout);
				GetComponent<Renderer>().material.SetColor("_TintColor", color);
			}
			yield return null;
		}
	}
}
