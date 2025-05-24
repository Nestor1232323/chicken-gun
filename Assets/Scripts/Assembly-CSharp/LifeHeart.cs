using System.Collections;
using UnityEngine;

public class LifeHeart : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(WakeUp());
	}

	private void Update()
	{
	}

	private IEnumerator WakeUp()
	{
		float scaleX = base.transform.localScale.x;
		Vector3 zero = Vector3.zero;
		base.transform.localScale = zero;
		float T = 2f;
		for (float t = 0f; t < T; t += Time.deltaTime)
		{
			zero.x = t / T * scaleX;
			zero.y = t / T * scaleX;
			zero.z = t / T * scaleX;
			base.transform.localScale = zero;
			yield return null;
		}
	}
}
