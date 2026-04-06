using UnityEngine;

public class BillboardLaub : MonoBehaviour
{
	private Transform mainCamTransform;

	private Transform cachedTransform;

	private void Start()
	{
		mainCamTransform = Camera.main.transform;
		cachedTransform = base.transform;
	}

	private void Update()
	{
		if (mainCamTransform.InverseTransformPoint(cachedTransform.position).z >= 0f)
		{
			Vector3 vector = mainCamTransform.position - cachedTransform.position;
			vector.x = (vector.z = 0f);
			cachedTransform.LookAt(mainCamTransform.position - vector);
			GetComponent<Renderer>().enabled = true;
		}
		else
		{
			GetComponent<Renderer>().enabled = false;
		}
	}
}
