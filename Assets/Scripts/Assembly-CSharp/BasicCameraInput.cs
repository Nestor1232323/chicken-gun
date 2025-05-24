using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Camera/Basic Camera Input", 1)]
public class BasicCameraInput : MonoBehaviour
{
	private CameraControl cam;

	public string xInputAxis;

	public string yInputAxis;

	private void Start()
	{
		cam = GetComponent<CameraControl>();
	}

	private void FixedUpdate()
	{
		if ((bool)cam && !string.IsNullOrEmpty(xInputAxis) && !string.IsNullOrEmpty(yInputAxis))
		{
			cam.SetInput(Input.GetAxis(xInputAxis), Input.GetAxis(yInputAxis));
		}
	}
}
