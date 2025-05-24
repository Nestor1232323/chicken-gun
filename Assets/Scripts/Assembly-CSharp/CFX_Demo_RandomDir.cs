using UnityEngine;

public class CFX_Demo_RandomDir : MonoBehaviour
{
	public Vector3 min = new Vector3(0f, 0f, 0f);

	public Vector3 max = new Vector3(0f, 360f, 0f);

	private void Awake()
	{
		base.transform.eulerAngles = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
	}
}
