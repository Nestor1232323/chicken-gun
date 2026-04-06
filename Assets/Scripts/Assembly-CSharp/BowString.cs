using UnityEngine;

public class BowString : MonoBehaviour
{
	[Range(0f, 1f)]
	public float factor;

	private Vector3 firstPosition;

	private Vector3 lastPosition;

	public bool stretching;

	public bool releasing;

	public float stretchSpeed = 0.5f;

	public float releaseSpeed = 0.5f;

	private void Start()
	{
		firstPosition = base.transform.localPosition;
		lastPosition = base.transform.localPosition + Vector3.up * 0.45f;
	}

	private void Update()
	{
		if (stretching)
		{
			factor += stretchSpeed * Time.deltaTime;
			if (factor > 1f)
			{
				factor = 1f;
			}
		}
		if (releasing)
		{
			factor -= releaseSpeed * Time.deltaTime;
			if (factor < 0f)
			{
				factor = 0f;
			}
		}
		base.transform.localPosition = Vector3.Lerp(firstPosition, lastPosition, factor);
	}

	private void Stretch()
	{
		stretching = true;
		releasing = false;
	}

	private void Release()
	{
		releasing = true;
		stretching = false;
	}
}
