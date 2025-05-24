using UnityEngine;

public class OpenChest : MonoBehaviour
{
	[Range(0f, 1f)]
	public float factor;

	private Quaternion closedAngle;

	private Quaternion openedAngle;

	public bool closing;

	public bool opening;

	public float speed = 0.5f;

	private int newAngle = 127;

	private void Start()
	{
		openedAngle = base.transform.rotation;
		closedAngle = Quaternion.Euler(base.transform.eulerAngles + Vector3.right * newAngle);
	}

	private void Update()
	{
		if (closing)
		{
			factor += speed * Time.deltaTime;
			if (factor > 1f)
			{
				factor = 1f;
			}
		}
		if (opening)
		{
			factor -= speed * Time.deltaTime;
			if (factor < 0f)
			{
				factor = 0f;
			}
		}
		base.transform.rotation = Quaternion.Lerp(openedAngle, closedAngle, factor);
	}

	private void Close()
	{
		closing = true;
		opening = false;
	}

	private void Open()
	{
		opening = true;
		closing = false;
	}
}
