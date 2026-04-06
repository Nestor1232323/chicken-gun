using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private int frame;

	private double frameStartTime;

	private float fps;

	private bool bShow;

	private void Start()
	{
		frameStartTime = Time.realtimeSinceStartup;
		bShow = true;
	}

	private void Update()
	{
		frame++;
		if ((double)Time.realtimeSinceStartup - frameStartTime > 1.0)
		{
			fps = frame;
			frame = 0;
			frameStartTime = Time.realtimeSinceStartup;
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (bShow)
			{
				bShow = false;
			}
			else
			{
				bShow = true;
			}
		}
	}

	private void OnGUI()
	{
		if (bShow)
		{
			GUI.Label(new Rect(0f, 0f, 200f, 20f), "FPS : " + fps.ToString("f2"));
		}
	}
}
